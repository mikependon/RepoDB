#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Interfaces;

namespace RepoDb.Data.Core.UnitTests.Fake.BulkOperations
{
    /// <summary>
    /// A call of one of the fake <c>BulkInsert</c> extension methods: the arguments that it received, and the number of rows that it read.
    /// </summary>
    public sealed class FakeBulkCall
    {
        public string Method { get; set; }

        public string TableName { get; set; }

        public int? BatchSize { get; set; }

        public int? Timeout { get; set; }

        public IDbTransaction Transaction { get; set; }

        public object Trace { get; set; }

        public string TraceKey { get; set; }

        public int Retries { get; set; }

        public long Skipped { get; set; }

        public CancellationToken CancellationToken { get; set; }

        public int RowCount { get; set; }
    }

    /// <summary>
    /// A type that is not a trace: a parameter of this type named <c>trace</c> never receives the trace that is passed by the caller.
    /// </summary>
    public sealed class FakeNotATrace
    {
    }

    /// <summary>
    /// A connection that has a fake <c>BulkInsert</c> and <c>BulkInsertAsync</c> (with the optional arguments of a real provider).
    /// </summary>
    public sealed class FakeBulkDbConnection : FakeBulkConnectionBase
    {
    }

    /// <summary>
    /// A connection whose <c>BulkInsert</c> arguments are of other types, to check that the arguments of another type are not passed.
    /// </summary>
    public sealed class FakeBulkTypedDbConnection : FakeBulkConnectionBase
    {
    }

    /// <summary>
    /// A connection whose <c>BulkInsert</c> throws.
    /// </summary>
    public sealed class FakeBulkThrowingDbConnection : FakeBulkConnectionBase
    {
    }

    /// <summary>
    /// A connection whose only <c>BulkInsert</c> methods are not valid bulk insert extension methods.
    /// </summary>
    public sealed class FakeBulkDecoyDbConnection : FakeBulkConnectionBase
    {
    }

    /// <summary>
    /// A connection that has a <c>BulkInsert</c> for its base type only.
    /// </summary>
    public sealed class FakeBulkDerivedDbConnection : FakeBulkBaseDbConnection
    {
    }

    /// <summary>
    /// A connection that has a <c>BulkInsert</c> for its own type and another one for its base type (the one of its own type is preferred).
    /// </summary>
    public sealed class FakeBulkPreferredDbConnection : FakeBulkBaseDbConnection
    {
    }

    /// <summary>
    /// A connection that has a <c>BulkInsert</c> without a transaction, a trace and a trace key.
    /// </summary>
    public sealed class FakeBulkMinimalDbConnection : FakeBulkConnectionBase
    {
    }

    /// <summary>
    /// A connection that has the fake <c>BulkInsert</c> methods of the base type only.
    /// </summary>
    public abstract class FakeBulkBaseDbConnection : FakeBulkConnectionBase
    {
    }

    /// <summary>
    /// The base class of the fake connections: it has no behavior.
    /// </summary>
    public abstract class FakeBulkConnectionBase : DbConnection
    {
        public override string ConnectionString { get; set; }

        public override string Database { get; }

        public override string DataSource { get; }

        public override string ServerVersion { get; }

        public override ConnectionState State { get; }

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Close()
        {
        }

        public override void Open()
        {
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) =>
            null;

        protected override DbCommand CreateDbCommand() =>
            null;
    }

    /// <summary>
    /// The fake <c>BulkInsert</c> extension methods, and the calls that they received.
    /// </summary>
    public static class FakeBulkOperations
    {
        private static readonly ConcurrentQueue<FakeBulkCall> _calls = new ConcurrentQueue<FakeBulkCall>();

        /// <summary>
        /// Gets the calls that the fake methods received, in order.
        /// </summary>
        public static FakeBulkCall[] Calls => _calls.ToArray();

        /// <summary>
        /// Removes the calls that were received.
        /// </summary>
        public static void Reset()
        {
            while (_calls.TryDequeue(out _))
            {
            }
        }

        private static int Read(IDataReader reader)
        {
            var count = 0;
            while (reader.Read())
            {
                count++;
            }
            return count;
        }

        // The one with the optional arguments that a real provider has (the timeout is named 'bulkCopyTimeout')
        public static int BulkInsert(this FakeBulkDbConnection connection,
            string tableName,
            IDataReader reader,
            [Optional] long skipped,
            int? batchSize = null,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            string traceKey = "FakeTraceKey",
            int retries = 3)
        {
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall
            {
                Method = nameof(BulkInsert),
                TableName = tableName,
                BatchSize = batchSize,
                Timeout = bulkCopyTimeout,
                Transaction = transaction,
                Trace = trace,
                TraceKey = traceKey,
                Retries = retries,
                Skipped = skipped,
                RowCount = count
            });
            return count;
        }

        // The asynchronous one (the timeout is named 'commandTimeout')
        public static async Task<int> BulkInsertAsync(this FakeBulkDbConnection connection,
            string tableName,
            IDataReader reader,
            int? batchSize = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            string traceKey = "FakeTraceKey",
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall
            {
                Method = nameof(BulkInsertAsync),
                TableName = tableName,
                BatchSize = batchSize,
                Timeout = commandTimeout,
                Transaction = transaction,
                Trace = trace,
                TraceKey = traceKey,
                CancellationToken = cancellationToken,
                RowCount = count
            });
            return count;
        }

        // The arguments are of other types: only the ones of the same type are passed
        public static int BulkInsert(this FakeBulkTypedDbConnection connection,
            string tableName,
            IDataReader reader,
            DbTransaction transaction = null,
            FakeNotATrace trace = null)
        {
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall
            {
                Method = "BulkInsert (typed)",
                TableName = tableName,
                Transaction = transaction,
                Trace = trace,
                RowCount = count
            });
            return count;
        }

        // Throws
        public static int BulkInsert(this FakeBulkThrowingDbConnection connection,
            string tableName,
            IDataReader reader) =>
            throw new InvalidOperationException("The fake bulk insert failed.");

        public static Task<int> BulkInsertAsync(this FakeBulkThrowingDbConnection connection,
            string tableName,
            IDataReader reader,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("The fake bulk insert failed.");

        // The minimal one (only the required arguments, and the cancellation token)
        public static int BulkInsert(this FakeBulkMinimalDbConnection connection,
            string tableName,
            IDataReader reader)
        {
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsert (minimal)", TableName = tableName, RowCount = count });
            return count;
        }

        public static async Task<int> BulkInsertAsync(this FakeBulkMinimalDbConnection connection,
            string tableName,
            IDataReader reader,
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsertAsync (minimal)", TableName = tableName, RowCount = count, CancellationToken = cancellationToken });
            return count;
        }

        // The ones for the base type
        public static int BulkInsert(this FakeBulkBaseDbConnection connection,
            string tableName,
            IDataReader reader)
        {
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsert (base)", TableName = tableName, RowCount = count });
            return count;
        }

        // The one for the type that is preferred over the one of its base type
        public static int BulkInsert(this FakeBulkPreferredDbConnection connection,
            string tableName,
            IDataReader reader)
        {
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsert (preferred)", TableName = tableName, RowCount = count });
            return count;
        }

        // The decoys: none of them is a valid bulk insert extension method of the decoy connection

        // Not an extension method
        public static int BulkInsert(FakeBulkDecoyDbConnection connection,
            string tableName,
            IDataReader reader) =>
            0;

        // Generic
        public static int BulkInsert<T>(this FakeBulkDecoyDbConnection connection,
            string tableName,
            IDataReader reader) =>
            0;

        // The second argument is not a string
        public static int BulkInsert(this FakeBulkDecoyDbConnection connection,
            int tableName,
            IDataReader reader) =>
            0;

        // The third argument is not a reader
        public static int BulkInsert(this FakeBulkDecoyDbConnection connection,
            string tableName,
            string reader) =>
            0;

        // A required argument after the reader
        public static int BulkInsert(this FakeBulkDecoyDbConnection connection,
            string tableName,
            IDataReader reader,
            int required) =>
            0;

        // Another name
        public static int BulkInsertAll(this FakeBulkDecoyDbConnection connection,
            string tableName,
            IDataReader reader) =>
            0;

        // Too few arguments
        public static int BulkInsert(this FakeBulkDecoyDbConnection connection,
            string tableName) =>
            0;
    }
}
