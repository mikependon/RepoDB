#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace RepoDb.Data.Core.UnitTests.Fake.BulkOperations
{
    /// <summary>
    /// A statement that the fake command of a bulk connection has executed: its text, and the values of its parameters at the time that it was executed.
    /// </summary>
    public sealed class FakeBulkStatement
    {
        /// <summary>
        /// Gets or sets the text of the statement.
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// Gets or sets the command timeout of the statement, in seconds.
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// Gets or sets the values of the parameters of the statement.
        /// </summary>
        public object[] Values { get; set; }
    }

    /// <summary>
    /// The command of a fake bulk connection: it records the statements that it executes (see <see cref="FakeBulkOperations.Statements"/>), and every statement affects one row.
    /// </summary>
    public sealed class FakeBulkCommand : DbCommand
    {
        private static readonly ConcurrentQueue<FakeBulkStatement> _statements = new ConcurrentQueue<FakeBulkStatement>();

        /// <summary>
        /// Gets the statements that were executed by the fake commands, in order.
        /// </summary>
        public static FakeBulkStatement[] Statements => _statements.ToArray();

        /// <summary>
        /// Removes the statements that were recorded.
        /// </summary>
        public static void Reset()
        {
            while (_statements.TryDequeue(out _))
            {
            }
        }

        /// <inheritdoc />
        public override string CommandText { get; set; }

        /// <inheritdoc />
        public override int CommandTimeout { get; set; }

        /// <inheritdoc />
        public override CommandType CommandType { get; set; }

        /// <inheritdoc />
        public override bool DesignTimeVisible { get; set; }

        /// <inheritdoc />
        public override UpdateRowSource UpdatedRowSource { get; set; }

        /// <inheritdoc />
        protected override DbConnection DbConnection { get; set; }

        /// <inheritdoc />
        protected override DbParameterCollection DbParameterCollection { get; } = new FakeBulkParameterCollection();

        /// <inheritdoc />
        protected override DbTransaction DbTransaction { get; set; }

        /// <inheritdoc />
        public override void Cancel()
        {
        }

        /// <inheritdoc />
        public override int ExecuteNonQuery()
        {
            _statements.Enqueue(new FakeBulkStatement
            {
                CommandText = CommandText,
                CommandTimeout = CommandTimeout,
                Values = DbParameterCollection.Cast<DbParameter>().Select(p => p.Value).ToArray()
            });
            return 1;
        }

        /// <inheritdoc />
        public override object ExecuteScalar() =>
            null;

        /// <inheritdoc />
        public override void Prepare()
        {
        }

        /// <inheritdoc />
        protected override DbParameter CreateDbParameter() =>
            new FakeBulkParameter();

        /// <inheritdoc />
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) =>
            throw new NotSupportedException();
    }

    /// <summary>
    /// The parameter of a fake bulk command.
    /// </summary>
    public sealed class FakeBulkParameter : DbParameter
    {
        /// <inheritdoc />
        public override DbType DbType { get; set; }

        /// <inheritdoc />
        public override ParameterDirection Direction { get; set; }

        /// <inheritdoc />
        public override bool IsNullable { get; set; }

        /// <inheritdoc />
        public override string ParameterName { get; set; }

        /// <inheritdoc />
        public override int Size { get; set; }

        /// <inheritdoc />
        public override string SourceColumn { get; set; }

        /// <inheritdoc />
        public override bool SourceColumnNullMapping { get; set; }

        /// <inheritdoc />
        public override object Value { get; set; }

        /// <inheritdoc />
        public override void ResetDbType()
        {
        }
    }

    /// <summary>
    /// The parameters of a fake bulk command.
    /// </summary>
    public sealed class FakeBulkParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> _items = new List<DbParameter>();

        /// <inheritdoc />
        public override int Count => _items.Count;

        /// <inheritdoc />
        public override object SyncRoot => ((ICollection)_items).SyncRoot;

        /// <inheritdoc />
        public override int Add(object value)
        {
            _items.Add((DbParameter)value);
            return _items.Count - 1;
        }

        /// <inheritdoc />
        public override void AddRange(Array values)
        {
            foreach (var value in values)
            {
                Add(value);
            }
        }

        /// <inheritdoc />
        public override void Clear() =>
            _items.Clear();

        /// <inheritdoc />
        public override bool Contains(object value) =>
            _items.Contains((DbParameter)value);

        /// <inheritdoc />
        public override bool Contains(string value) =>
            IndexOf(value) >= 0;

        /// <inheritdoc />
        public override void CopyTo(Array array,
            int index) =>
            ((ICollection)_items).CopyTo(array, index);

        /// <inheritdoc />
        public override IEnumerator GetEnumerator() =>
            _items.GetEnumerator();

        /// <inheritdoc />
        public override int IndexOf(object value) =>
            _items.IndexOf((DbParameter)value);

        /// <inheritdoc />
        public override int IndexOf(string parameterName) =>
            _items.FindIndex(p => string.Equals(p.ParameterName, parameterName, StringComparison.Ordinal));

        /// <inheritdoc />
        public override void Insert(int index,
            object value) =>
            _items.Insert(index, (DbParameter)value);

        /// <inheritdoc />
        public override void Remove(object value) =>
            _items.Remove((DbParameter)value);

        /// <inheritdoc />
        public override void RemoveAt(int index) =>
            _items.RemoveAt(index);

        /// <inheritdoc />
        public override void RemoveAt(string parameterName) =>
            _items.RemoveAt(IndexOf(parameterName));

        /// <inheritdoc />
        protected override DbParameter GetParameter(int index) =>
            _items[index];

        /// <inheritdoc />
        protected override DbParameter GetParameter(string parameterName) =>
            _items[IndexOf(parameterName)];

        /// <inheritdoc />
        protected override void SetParameter(int index,
            DbParameter value) =>
            _items[index] = value;

        /// <inheritdoc />
        protected override void SetParameter(string parameterName,
            DbParameter value) =>
            _items[IndexOf(parameterName)] = value;
    }
}
