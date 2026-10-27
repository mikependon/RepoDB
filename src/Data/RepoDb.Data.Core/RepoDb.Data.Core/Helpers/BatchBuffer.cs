#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Buffers the rows of a data reader in memory, a batch at a time: it counts the rows that it reads until the number of rows reaches the batch size, then it gives the batch as a
    /// data reader, so the bulk insert of the provider can send it right away. The buffer is cleared before the next batch is read, so only one batch is in memory at any time.
    /// If there are interceptors, every row is read as a <see cref="Row"/> and goes through them (see <see cref="RowInterception"/>) before it is buffered: the columns of the
    /// batch are the columns of the row that the last interceptor returns (the excluded ones left out), which are the same for every row.
    /// </summary>
    internal sealed class BatchBuffer : IDisposable
    {
        #region Private Variables

        private readonly IDataReader _source;
        private readonly int _batchSize;
        private readonly int[] _ordinals;
        private DataTable _table;
        private readonly object[] _values;
        private readonly object[] _row;
        private readonly ISet<string> _excludedColumns;
        private readonly IList<IDataInterceptor<Row>> _interceptors;
        private readonly List<Column> _columns;
        private string[] _shape;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="BatchBuffer"/> class.
        /// </summary>
        /// <param name="source">The data reader that has the rows. It is not disposed by the buffer.</param>
        /// <param name="batchSize">The number of rows of a batch.</param>
        /// <param name="excludedColumns">The names of the columns of the source (without the case) that are left out of the batches, or <c>null</c> to buffer all of them.</param>
        /// <param name="interceptors">The interceptors that every row goes through, in order, or <c>null</c> to buffer the rows as they are.</param>
        /// <exception cref="ArgumentNullException">The source is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The batch size is less than 1.</exception>
        /// <exception cref="ArgumentException">An interceptor is <c>null</c>.</exception>
        public BatchBuffer(IDataReader source,
            int batchSize,
            ISet<string> excludedColumns = null,
            IList<IDataInterceptor<Row>> interceptors = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (batchSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize), "The batch size must be at least 1.");
            }
            _source = source;
            _batchSize = batchSize;
            _ordinals = GetOrdinals(source, excludedColumns);
            _table = CreateTable(source, _ordinals);
            _values = new object[source.FieldCount];
            _row = new object[_ordinals.Length];
            _excludedColumns = excludedColumns;
            _interceptors = RowInterception.Normalize(interceptors);
            if (_interceptors != null)
            {
                _columns = RowInterception.GetColumns(source);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of rows that are buffered.
        /// </summary>
        public int Count => _table.Rows.Count;

        /// <summary>Gets the estimated bytes of the buffered values.</summary>
        public long CopiedBytes
        {
            get
            {
                long bytes = 0;
                foreach (DataRow row in _table.Rows)
                {
                    for (var i = 0; i < _table.Columns.Count; i++)
                    {
                        bytes += CopySize.OfValue(row[i]);
                    }
                }
                return bytes;
            }
        }

        /// <summary>
        /// Checks whether the batches have a column with the specified name (the case is ignored). With interceptors, the columns are known when the first row is buffered.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns><c>true</c> if the batches have the column; otherwise, <c>false</c>.</returns>
        public bool HasColumn(string name) =>
            _table.Columns.Contains(name);

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears the buffer, then buffers the next rows of the source until the number of rows reaches the batch size, or the source has no more rows.
        /// </summary>
        /// <returns><c>true</c> if at least a row is buffered; otherwise, <c>false</c> (the source has no more rows).</returns>
        public bool Fill()
        {
            Clear();
            while (_table.Rows.Count < _batchSize && _source.Read())
            {
                Add();
            }
            return _table.Rows.Count > 0;
        }

        /// <summary>
        /// Clears the buffer, then buffers the next rows of the source in an asynchronous way until the number of rows reaches the batch size, or the source has no more rows.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if at least a row is buffered; otherwise, <c>false</c> (the source has no more rows).</returns>
        public async Task<bool> FillAsync(CancellationToken cancellationToken = default)
        {
            Clear();
            while (_table.Rows.Count < _batchSize && await ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                Add();
            }
            return _table.Rows.Count > 0;
        }

        /// <summary>
        /// Creates the data reader of the rows that are buffered, to be passed to the bulk insert. It is to be disposed by the caller, before the buffer is filled again.
        /// </summary>
        /// <returns>The data reader of the buffered rows (with the columns of the source, even if no row is buffered).</returns>
        public IDataReader CreateReader() =>
            _table.CreateDataReader();

        /// <summary>
        /// Removes the buffered rows from the memory.
        /// </summary>
        public void Clear() =>
            _table.Clear();

        /// <summary>
        /// Disposes the buffer.
        /// </summary>
        public void Dispose() =>
            _table.Dispose();

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the table that buffers the rows, with the columns of the source (by their names and types).
        /// </summary>
        /// <param name="source">The data reader that has the rows.</param>
        /// <param name="ordinals">The positions of the columns of the source that are buffered.</param>
        /// <returns>The table, without rows.</returns>
        private static DataTable CreateTable(IDataReader source,
            int[] ordinals)
        {
            var table = new DataTable();
            foreach (var ordinal in ordinals)
            {
                table.Columns.Add(source.GetName(ordinal), source.GetFieldType(ordinal) ?? typeof(object));
            }
            return table;
        }

        /// <summary>
        /// Gets the positions of the columns of the source that are buffered: all of them, but the excluded ones.
        /// </summary>
        /// <param name="source">The data reader that has the rows.</param>
        /// <param name="excludedColumns">The names of the columns that are left out (without the case), or <c>null</c>.</param>
        /// <returns>The positions of the columns, in order.</returns>
        private static int[] GetOrdinals(IDataReader source,
            ISet<string> excludedColumns) =>
            Enumerable.Range(0, source.FieldCount)
                .Where(ordinal => excludedColumns == null || !excludedColumns.Contains(source.GetName(ordinal)))
                .ToArray();

        /// <summary>
        /// Adds the current row of the source to the buffer.
        /// </summary>
        private void Add()
        {
            if (_interceptors != null)
            {
                AddIntercepted();
                return;
            }
            _source.GetValues(_values);
            for (var i = 0; i < _ordinals.Length; i++)
            {
                _row[i] = _values[_ordinals[i]];
            }
            _table.Rows.Add(_row);
        }

        /// <summary>
        /// Adds the current row of the source to the buffer after it goes through the interceptors.
        /// </summary>
        /// <exception cref="InvalidOperationException">An interceptor returned <c>null</c>, or the row has other columns than the rows before it, or a value does not fit its column.</exception>
        private void AddIntercepted()
        {
            var row = RowInterception.Apply(RowInterception.Read(_source, _columns), _interceptors);
            var included = new List<int>(row.Count);
            for (var i = 0; i < row.Count; i++)
            {
                if (_excludedColumns == null || !_excludedColumns.Contains(row.Columns[i].Name))
                {
                    included.Add(i);
                }
            }
            if (_shape == null)
            {
                Reshape(row, included);
            }
            else if (!IsShape(row, included))
            {
                throw new InvalidOperationException("The data interceptors must give every row the same columns, in the same order.");
            }
            var values = new object[included.Count];
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = row[included[i]] ?? DBNull.Value;
            }
            try
            {
                _table.Rows.Add(values);
            }
            catch (Exception e) when (e is ArgumentException || e is InvalidCastException || e is FormatException)
            {
                throw new InvalidOperationException($"A value that a data interceptor gave does not fit the type of its column: {e.Message}", e);
            }
        }

        /// <summary>
        /// Gives the table the columns of the row that the interceptors returned (the first row of the copy).
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="included">The positions of the columns of the row that are buffered.</param>
        private void Reshape(Row row,
            List<int> included)
        {
            var table = new DataTable();
            foreach (var ordinal in included)
            {
                var column = row.Columns[ordinal];
                table.Columns.Add(column.Name, column.Type == null ? typeof(object) : Nullable.GetUnderlyingType(column.Type) ?? column.Type);
            }
            _table.Dispose();
            _table = table;
            _shape = included.Select(ordinal => row.Columns[ordinal].Name).ToArray();
        }

        /// <summary>
        /// Checks whether the row has the same columns as the first row of the copy.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="included">The positions of the columns of the row that are buffered.</param>
        /// <returns><c>true</c> if the columns are the same, in the same order; otherwise, <c>false</c>.</returns>
        private bool IsShape(Row row,
            List<int> included)
        {
            if (included.Count != _shape.Length)
            {
                return false;
            }
            for (var i = 0; i < _shape.Length; i++)
            {
                if (!string.Equals(row.Columns[included[i]].Name, _shape[i], StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Reads the next row of the source asynchronously, if the source supports it.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if there is another row; otherwise, <c>false</c>.</returns>
        private Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _source is System.Data.Common.DbDataReader reader
                ? reader.ReadAsync(cancellationToken)
                : Task.FromResult(_source.Read());
        }

        #endregion
    }
}
