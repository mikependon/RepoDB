#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.DbSettings;
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

        public object Options { get; set; }

        public object IdentityBehavior { get; set; }

        public string ColumnMappingsBehavior { get; set; }

        public int RowCount { get; set; }

        public string[] Qualifiers { get; set; }
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
    /// A connection that has a fake <c>BulkInsert</c>, <c>BulkMerge</c> (and their asynchronous ones), the base of the connections that are the destination of the copy of the data.
    /// Its database and data source are fixed, and the test project maps its setting, helper and statement builder.
    /// </summary>
    public abstract class FakeBulkMappedDbConnection : FakeBulkConnectionBase
    {
        public override string Database => "FakeDatabase";

        public override string DataSource => "FakeDataSource";

        public override ConnectionState State => ConnectionState.Open;
    }

    /// <summary>
    /// A destination connection whose table has a primary key (that is the identity too).
    /// </summary>
    public sealed class FakeBulkKeyedDbConnection : FakeBulkMappedDbConnection
    {
    }

    /// <summary>
    /// A destination connection whose table has an identity column, and no primary key.
    /// </summary>
    public sealed class FakeBulkIdentityOnlyDbConnection : FakeBulkMappedDbConnection
    {
    }

    /// <summary>
    /// A destination connection whose table has neither a primary key nor an identity column.
    /// </summary>
    public sealed class FakeBulkKeylessDbConnection : FakeBulkMappedDbConnection
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
    /// The options of the fake bulk insert that have the <c>KeepIdentity</c> option, as the <c>SqlBulkCopyOptions</c> of SQL Server.
    /// </summary>
    [Flags]
    public enum FakeBulkCopyOptions
    {
        Default = 0,
        CheckConstraints = 1,
        KeepIdentity = 2,
        TableLock = 4
    }

    /// <summary>
    /// The options of the fake bulk insert that do not have the <c>KeepIdentity</c> option.
    /// </summary>
    [Flags]
    public enum FakeBulkOtherOptions
    {
        Default = 0,
        Fast = 1
    }

    /// <summary>
    /// A connection whose <c>BulkInsert</c> has an <c>options</c> argument that has the <c>KeepIdentity</c> option.
    /// </summary>
    public sealed class FakeBulkOptionsDbConnection : FakeBulkConnectionBase
    {
    }

    /// <summary>
    /// A connection whose <c>BulkInsert</c> has an <c>options</c> argument that has the <c>KeepIdentity</c> option, and a default value other than the default of the type.
    /// </summary>
    public sealed class FakeBulkDefaultOptionsDbConnection : FakeBulkConnectionBase
    {
    }

    /// <summary>
    /// A connection whose <c>BulkInsert</c> has a nullable <c>options</c> argument that has the <c>KeepIdentity</c> option.
    /// </summary>
    public sealed class FakeBulkNullableOptionsDbConnection : FakeBulkConnectionBase
    {
    }

    /// <summary>
    /// A connection whose <c>BulkInsert</c> has an <c>options</c> argument that does not have the <c>KeepIdentity</c> option.
    /// </summary>
    public sealed class FakeBulkOtherOptionsDbConnection : FakeBulkConnectionBase
    {
    }

    /// <summary>
    /// A connection whose <c>BulkInsert</c> has a <c>bulkCopyOptions</c> argument that has the <c>KeepIdentity</c> option (as the providers that are not SQL Server).
    /// </summary>
    public sealed class FakeBulkCopyOptionsDbConnection : FakeBulkConnectionBase
    {
    }

    /// <summary>
    /// The identity behavior of the fake bulk insert, as the <c>identityBehavior</c> of the bulk operations of the providers.
    /// </summary>
    public enum FakeBulkImportIdentityBehavior : short
    {
        KeepIdentity,
        ReturnIdentity
    }

    /// <summary>
    /// The identity behavior of the fake bulk insert that does not have the <c>KeepIdentity</c> value.
    /// </summary>
    public enum FakeBulkOtherIdentityBehavior : short
    {
        Generate,
        Return
    }

    /// <summary>
    /// A connection whose <c>BulkInsert</c> has a nullable <c>identityBehavior</c> argument that has the <c>KeepIdentity</c> value.
    /// </summary>
    public sealed class FakeBulkIdentityBehaviorDbConnection : FakeBulkConnectionBase
    {
    }

    /// <summary>
    /// A connection whose <c>BulkInsert</c> has an <c>identityBehavior</c> argument that does not have the <c>KeepIdentity</c> value.
    /// </summary>
    public sealed class FakeBulkOtherIdentityBehaviorDbConnection : FakeBulkConnectionBase
    {
    }

    /// <summary>
    /// A connection whose <c>BulkInsert</c> has an <c>options</c> argument that is not an enumeration.
    /// </summary>
    public sealed class FakeBulkTextOptionsDbConnection : FakeBulkConnectionBase
    {
    }

    /// <summary>
    /// The column mappings behavior of the fake bulk operations (the same names and values as the behavior of the data copy).
    /// </summary>
    public enum FakeBulkColumnMappingsBehavior : short
    {
        Automatic,
        Strict,
        StrictBypass
    }

    /// <summary>
    /// A column mappings behavior that does not define all the values of the behavior of the data copy.
    /// </summary>
    public enum FakeBulkLimitedColumnMappingsBehavior : short
    {
        Automatic,
        Strict
    }

    /// <summary>
    /// The setting of the fake bulk operations, as the <c>SqlServerBulkOperationsDbSetting</c>: the behavior is read from it by the bulk insert.
    /// </summary>
    public sealed class FakeBulkDbSetting : BaseDbSetting
    {
        public FakeBulkColumnMappingsBehavior BulkColumnMappingsBehavior { get; set; }
    }

    /// <summary>
    /// A setting with a behavior that does not define all the values of the behavior of the data copy.
    /// </summary>
    public sealed class FakeBulkLimitedDbSetting : BaseDbSetting
    {
        public FakeBulkLimitedColumnMappingsBehavior BulkColumnMappingsBehavior { get; set; }
    }

    /// <summary>
    /// A connection whose <c>BulkInsert</c> reads the column mappings behavior from the setting of the connection (<see cref="FakeBulkDbSetting"/>).
    /// </summary>
    public sealed class FakeBulkSettingDbConnection : FakeBulkConnectionBase
    {
    }

    /// <summary>
    /// A connection like <see cref="FakeBulkSettingDbConnection"/> whose <c>BulkInsert</c> throws.
    /// </summary>
    public sealed class FakeBulkSettingThrowingDbConnection : FakeBulkConnectionBase
    {
    }

    /// <summary>
    /// A connection whose setting (<see cref="FakeBulkLimitedDbSetting"/>) has a behavior that does not define all the values.
    /// </summary>
    public sealed class FakeBulkLimitedSettingDbConnection : FakeBulkConnectionBase
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
        public int ScalarExecutions { get; set; }

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
            new FakeBulkCommand { Connection = this };
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
        /// Gets the statements that the fake commands of the bulk connections executed, in order.
        /// </summary>
        public static FakeBulkStatement[] Statements => FakeBulkCommand.Statements;

        /// <summary>
        /// Removes the calls that were received.
        /// </summary>
        public static void Reset()
        {
            while (_calls.TryDequeue(out _))
            {
            }
            FakeBulkCommand.Reset();
        }

        /// <summary>
        /// Gets the names of the columns of the reader that was read by the last bulk insert.
        /// </summary>
        public static string[] LastColumns { get; private set; } = new string[0];

        private static int Read(IDataReader reader)
        {
            LastColumns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToArray();
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

        // The ones of the destination connections of the copy of the data
        public static int BulkInsert(this FakeBulkMappedDbConnection connection,
            string tableName,
            IDataReader reader,
            int? batchSize = null,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            string traceKey = "FakeTraceKey")
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
                RowCount = count
            });
            return count;
        }

        public static async Task<int> BulkInsertAsync(this FakeBulkMappedDbConnection connection,
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

        public static int BulkMerge(this FakeBulkMappedDbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            int? batchSize = null,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            string traceKey = "FakeTraceKey")
        {
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall
            {
                Method = nameof(BulkMerge),
                TableName = tableName,
                BatchSize = batchSize,
                Timeout = bulkCopyTimeout,
                Transaction = transaction,
                Trace = trace,
                TraceKey = traceKey,
                RowCount = count,
                Qualifiers = qualifiers?.Select(field => field.Name).ToArray()
            });
            return count;
        }

        public static async Task<int> BulkMergeAsync(this FakeBulkMappedDbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
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
                Method = nameof(BulkMergeAsync),
                TableName = tableName,
                BatchSize = batchSize,
                Timeout = commandTimeout,
                Transaction = transaction,
                Trace = trace,
                TraceKey = traceKey,
                CancellationToken = cancellationToken,
                RowCount = count,
                Qualifiers = qualifiers?.Select(field => field.Name).ToArray()
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

        // The options (the ones that have the KeepIdentity option)
        public static int BulkInsert(this FakeBulkOptionsDbConnection connection,
            string tableName,
            IDataReader reader,
            FakeBulkCopyOptions options = default)
        {
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsert (options)", TableName = tableName, Options = options, RowCount = count });
            return count;
        }

        public static async Task<int> BulkInsertAsync(this FakeBulkOptionsDbConnection connection,
            string tableName,
            IDataReader reader,
            FakeBulkCopyOptions options = default,
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsertAsync (options)", TableName = tableName, Options = options, RowCount = count });
            return count;
        }

        public static int BulkInsert(this FakeBulkDefaultOptionsDbConnection connection,
            string tableName,
            IDataReader reader,
            FakeBulkCopyOptions options = FakeBulkCopyOptions.TableLock)
        {
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsert (default options)", TableName = tableName, Options = options, RowCount = count });
            return count;
        }

        public static int BulkInsert(this FakeBulkNullableOptionsDbConnection connection,
            string tableName,
            IDataReader reader,
            FakeBulkCopyOptions? options = null)
        {
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsert (nullable options)", TableName = tableName, Options = options, RowCount = count });
            return count;
        }

        public static int BulkInsert(this FakeBulkOtherOptionsDbConnection connection,
            string tableName,
            IDataReader reader,
            FakeBulkOtherOptions options = FakeBulkOtherOptions.Fast)
        {
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsert (other options)", TableName = tableName, Options = options, RowCount = count });
            return count;
        }

        public static int BulkInsert(this FakeBulkCopyOptionsDbConnection connection,
            string tableName,
            IDataReader reader,
            FakeBulkCopyOptions bulkCopyOptions = default)
        {
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsert (bulk copy options)", TableName = tableName, Options = bulkCopyOptions, RowCount = count });
            return count;
        }

        public static int BulkInsert(this FakeBulkIdentityBehaviorDbConnection connection,
            string tableName,
            IDataReader reader,
            FakeBulkImportIdentityBehavior? identityBehavior = null)
        {
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsert (identity behavior)", TableName = tableName, IdentityBehavior = identityBehavior, RowCount = count });
            return count;
        }

        public static async Task<int> BulkInsertAsync(this FakeBulkIdentityBehaviorDbConnection connection,
            string tableName,
            IDataReader reader,
            FakeBulkImportIdentityBehavior? identityBehavior = null,
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsertAsync (identity behavior)", TableName = tableName, IdentityBehavior = identityBehavior, RowCount = count });
            return count;
        }

        public static int BulkInsert(this FakeBulkOtherIdentityBehaviorDbConnection connection,
            string tableName,
            IDataReader reader,
            FakeBulkOtherIdentityBehavior identityBehavior = FakeBulkOtherIdentityBehavior.Return)
        {
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsert (other identity behavior)", TableName = tableName, IdentityBehavior = identityBehavior, RowCount = count });
            return count;
        }

        public static int BulkInsert(this FakeBulkTextOptionsDbConnection connection,
            string tableName,
            IDataReader reader,
            string options = "text")
        {
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsert (text options)", TableName = tableName, Options = options, RowCount = count });
            return count;
        }

        // The column mappings behavior (read from the setting of the connection, as the bulk operations of a provider do)
        public static int BulkInsert(this FakeBulkSettingDbConnection connection,
            string tableName,
            IDataReader reader)
        {
            var behavior = ((FakeBulkDbSetting)DbSettingMapper.Get(connection)).BulkColumnMappingsBehavior.ToString();
            System.Threading.Thread.Sleep(2);
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsert (setting)", TableName = tableName, ColumnMappingsBehavior = behavior, RowCount = count });
            return count;
        }

        public static async Task<int> BulkInsertAsync(this FakeBulkSettingDbConnection connection,
            string tableName,
            IDataReader reader,
            CancellationToken cancellationToken = default)
        {
            var behavior = ((FakeBulkDbSetting)DbSettingMapper.Get(connection)).BulkColumnMappingsBehavior.ToString();
            await Task.Delay(2, cancellationToken);
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsertAsync (setting)", TableName = tableName, ColumnMappingsBehavior = behavior, RowCount = count });
            return count;
        }

        public static int BulkInsert(this FakeBulkSettingThrowingDbConnection connection,
            string tableName,
            IDataReader reader) =>
            throw new InvalidOperationException("The fake bulk insert failed.");

        public static int BulkInsert(this FakeBulkLimitedSettingDbConnection connection,
            string tableName,
            IDataReader reader)
        {
            var count = Read(reader);
            _calls.Enqueue(new FakeBulkCall { Method = "BulkInsert (limited setting)", TableName = tableName, RowCount = count });
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
