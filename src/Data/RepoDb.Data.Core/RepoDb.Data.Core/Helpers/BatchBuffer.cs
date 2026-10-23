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

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Buffers the rows of a data reader in memory, a batch at a time: it counts the rows that it reads until the number of rows reaches the batch size, then it gives the batch as a
    /// data reader, so the bulk insert of the provider can send it right away. The buffer is cleared before the next batch is read, so only one batch is in memory at any time.
    /// </summary>
    internal sealed class BatchBuffer : IDisposable
    {
        #region Private Variables

        private readonly IDataReader _source;
        private readonly int _batchSize;
        private readonly int[] _ordinals;
        private readonly DataTable _table;
        private readonly object[] _values;
        private readonly object[] _row;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="BatchBuffer"/> class.
        /// </summary>
        /// <param name="source">The data reader that has the rows. It is not disposed by the buffer.</param>
        /// <param name="batchSize">The number of rows of a batch.</param>
        /// <param name="excludedColumns">The names of the columns of the source (without the case) that are left out of the batches, or <c>null</c> to buffer all of them.</param>
        /// <exception cref="ArgumentNullException">The source is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The batch size is less than 1.</exception>
        public BatchBuffer(IDataReader source,
            int batchSize,
            ISet<string> excludedColumns = null)
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
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of rows that are buffered.
        /// </summary>
        public int Count => _table.Rows.Count;

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
            _source.GetValues(_values);
            for (var i = 0; i < _ordinals.Length; i++)
            {
                _row[i] = _values[_ordinals[i]];
            }
            _table.Rows.Add(_row);
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
