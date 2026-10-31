#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.UnitTests.CustomObjects
{
    /// <summary>
    /// Owns the provider, observers, and resources of one trace test without making assertions.
    /// </summary>
    public sealed class TraceTestExecution : IDisposable
    {
        #region Private Variables

        private readonly ConversionType _conversionType = GlobalConfiguration.Options.ConversionType;
        private readonly bool _useGlobalTraces = GlobalConfiguration.Options.UseRegisteredGlobalTraces;
        private readonly TimeSpan? _warningThreshold = GlobalConfiguration.Options.TraceWarningThreshold;
        private readonly IList<ITrace> _globalTraces = new List<ITrace>();

        #endregion

        #region Constructors

        /// <summary>
        /// Prepares a provider with one result row, an activity, and independent observers.
        /// </summary>
        /// <param name="operation">The operation whose result shape is required.</param>
        public TraceTestExecution(string operation = null)
        {
            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = false;
            DbSettingMapper.Add<CustomTracedDbConnection>(new CustomDbSetting(), true);
            DbHelperMapper.Add<CustomTracedDbConnection>(new CustomDbHelper(), true);
            StatementBuilderMapper.Add<CustomTracedDbConnection>(new CustomStatementBuilder(), true);
            Table.Columns.Add("Id", typeof(int));
            if (string.Equals(operation, "InsertAllBatch", StringComparison.Ordinal) ||
                string.Equals(operation, "MergeAllBatch", StringComparison.Ordinal))
            {
                Table.Columns.Add("Position", typeof(int));
                Table.Rows.Add(1, 0);
                Table.Rows.Add(2, 1);
            }
            else
            {
                Table.Columns.Add("Name", typeof(string));
                Table.Rows.Add(1, "Trace");
            }
            Connection.ReaderFactory = () => Table.CreateDataReader();
            Activity = new Activity("TraceTest").SetIdFormat(ActivityIdFormat.W3C).Start();
        }

        #endregion

        #region Properties

        /// <summary>Gets the activity surrounding the tested operation.</summary>
        public Activity Activity { get; }

        /// <summary>Gets the caller's cancellation source.</summary>
        public CancellationTokenSource Cancellation { get; } = new CancellationTokenSource();

        /// <summary>Gets the connection executing the tested operation.</summary>
        public CustomTracedDbConnection Connection { get; private set; } = new CustomTracedDbConnection
        {
            ScalarValue = 1,
            RowsAffected = 1
        };

        /// <summary>Gets the exception received by the caller, if execution failed.</summary>
        public Exception Error { get; private set; }

        /// <summary>Gets the error used to simulate a failing execution.</summary>
        public Exception Failure { get; } = new InvalidOperationException("Execution failed.");

        /// <summary>Gets the first independently configurable global observer.</summary>
        public RecordingTrace First { get; } = new RecordingTrace();

        /// <summary>Gets the error used to simulate a failing completion observer.</summary>
        public Exception ObserverFailure { get; } = new NotSupportedException("Observer failed.");

        /// <summary>Gets the result returned to the caller.</summary>
        public object Result { get; private set; }

        /// <summary>Gets the second independently configurable global observer.</summary>
        public RecordingTrace Second { get; } = new RecordingTrace();

        /// <summary>Gets the rows supplied to query and batched identity operations.</summary>
        public DataTable Table { get; } = new DataTable();

        /// <summary>Gets the explicitly supplied command observer.</summary>
        public RecordingTrace Trace { get; } = new RecordingTrace();

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers an observer for this test and removes it during cleanup.
        /// </summary>
        /// <param name="trace">The observer to register.</param>
        public void RegisterGlobal(ITrace trace)
        {
            GlobalTraceRegistration.Register(trace);
            _globalTraces.Add(trace);
        }

        /// <summary>
        /// Executes a public connection operation and records its result or error.
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The execution task.</returns>
        public Task ExecuteOperationAsync(string operation, bool asynchronous) =>
            CaptureAsync(() => OperationTraceExecutor.ExecuteAsync(Connection, Trace, operation,
                asynchronous, Cancellation.Token));

        /// <summary>
        /// Executes a raw command and records its result or error.
        /// </summary>
        /// <param name="operation">The raw execution API to use.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <param name="useLocalTrace">Whether to supply the local observer.</param>
        /// <returns>The execution task.</returns>
        public Task ExecuteCommandAsync(string operation, bool asynchronous, bool useLocalTrace = true) =>
            CaptureAsync(() => ExecuteCommandCoreAsync(operation, asynchronous, useLocalTrace ? Trace : null));

        /// <summary>
        /// Executes through a repository and records the result or error received by its caller.
        /// </summary>
        /// <param name="operation">The repository operation to execute.</param>
        /// <param name="entityRepository">Whether to use the entity repository.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The execution task.</returns>
        public async Task ExecuteRepositoryAsync(string operation, bool entityRepository, bool asynchronous)
        {
            using var typed = new CustomTracedRepository(Trace);
            using var repository = new DbRepository<CustomTracedDbConnection>("ConnectionString",
                null, ConnectionPersistency.Instance, trace: Trace);
            var connection = entityRepository ? typed.CreateConnection() : repository.CreateConnection();
            connection.ScalarValue = Connection.ScalarValue;
            connection.RowsAffected = Connection.RowsAffected;
            connection.Failure = Connection.Failure;
            connection.ReaderFactory = Connection.ReaderFactory;
            Connection.Dispose();
            Connection = connection;
            await CaptureAsync(() => ExecuteRepositoryCoreAsync(typed, repository, operation,
                entityRepository, asynchronous)).ConfigureAwait(false);
        }

        /// <summary>
        /// Releases resources and restores the global configuration modified by this test.
        /// </summary>
        public void Dispose()
        {
            foreach (var trace in _globalTraces)
            {
                GlobalTraceRegistration.GetTracers().Remove(trace);
            }
            GlobalConfiguration.Options.ConversionType = _conversionType;
            GlobalConfiguration.Options.UseRegisteredGlobalTraces = _useGlobalTraces;
            GlobalConfiguration.Options.TraceWarningThreshold = _warningThreshold;
            DbSettingMapper.Add<CustomTracedDbConnection>(new CustomDbSetting(), true);
            Activity.Dispose();
            Cancellation.Dispose();
            Connection.Dispose();
            Table.Dispose();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Captures the operation outcome so each test can assert its own contract.
        /// </summary>
        /// <param name="execute">The public API call to execute.</param>
        /// <returns>The execution task.</returns>
        private async Task CaptureAsync(Func<Task<object>> execute)
        {
            try
            {
                Result = await execute().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Error = exception;
            }
        }

        /// <summary>
        /// Dispatches a raw command without performing assertions.
        /// </summary>
        /// <param name="operation">The raw execution API to use.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <param name="trace">The explicit command observer, if any.</param>
        /// <returns>The command result.</returns>
        private async Task<object> ExecuteCommandCoreAsync(string operation, bool asynchronous, ITrace trace)
        {
            const string statement = "SELECT [Id] FROM [Table]";
            switch (operation)
            {
                case "NonQuery":
                    return asynchronous
                        ? await Connection.ExecuteNonQueryAsync(statement, trace: trace, cancellationToken: Cancellation.Token).ConfigureAwait(false)
                        : Connection.ExecuteNonQuery(statement, trace: trace);
                case "Scalar":
                    return asynchronous
                        ? await Connection.ExecuteScalarAsync<int>(statement, trace: trace, cancellationToken: Cancellation.Token).ConfigureAwait(false)
                        : Connection.ExecuteScalar<int>(statement, trace: trace);
                case "Reader":
                    using (var reader = asynchronous
                        ? await Connection.ExecuteReaderAsync(statement, trace: trace, cancellationToken: Cancellation.Token).ConfigureAwait(false)
                        : Connection.ExecuteReader(statement, trace: trace))
                    {
                        return null;
                    }
                case "Multiple":
                    using (var extractor = asynchronous
                        ? await Connection.ExecuteQueryMultipleAsync(statement, trace: trace, cancellationToken: Cancellation.Token).ConfigureAwait(false)
                        : Connection.ExecuteQueryMultiple(statement, trace: trace))
                    {
                        return null;
                    }
                default:
                    return await ExecuteQueryCoreAsync(operation, asynchronous, trace).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Dispatches a raw query without performing assertions.
        /// </summary>
        /// <param name="operation">The query API to use.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <param name="trace">The explicit command observer.</param>
        /// <returns>The query result.</returns>
        private async Task<object> ExecuteQueryCoreAsync(string operation, bool asynchronous, ITrace trace)
        {
            const string statement = "SELECT [Id] FROM [Table]";
            switch (operation)
            {
                case "Query":
                    return asynchronous
                        ? await Connection.ExecuteQueryAsync(statement, trace: trace, cancellationToken: Cancellation.Token).ConfigureAwait(false)
                        : Connection.ExecuteQuery(statement, trace: trace);
                case "QueryTyped":
                    return asynchronous
                        ? await Connection.ExecuteQueryAsync<OperationTraceEntity>(statement, trace: trace, cancellationToken: Cancellation.Token).ConfigureAwait(false)
                        : Connection.ExecuteQuery<OperationTraceEntity>(statement, trace: trace);
                case "First":
                    return asynchronous
                        ? await Connection.ExecuteQueryFirstAsync(statement, trace: trace, cancellationToken: Cancellation.Token).ConfigureAwait(false)
                        : Connection.ExecuteQueryFirst(statement, trace: trace);
                case "FirstTyped":
                    return asynchronous
                        ? await Connection.ExecuteQueryFirstAsync<OperationTraceEntity>(statement, trace: trace, cancellationToken: Cancellation.Token).ConfigureAwait(false)
                        : Connection.ExecuteQueryFirst<OperationTraceEntity>(statement, trace: trace);
                case "Single":
                    return asynchronous
                        ? await Connection.ExecuteQuerySingleAsync(statement, trace: trace, cancellationToken: Cancellation.Token).ConfigureAwait(false)
                        : Connection.ExecuteQuerySingle(statement, trace: trace);
                case "SingleTyped":
                    return asynchronous
                        ? await Connection.ExecuteQuerySingleAsync<OperationTraceEntity>(statement, trace: trace, cancellationToken: Cancellation.Token).ConfigureAwait(false)
                        : Connection.ExecuteQuerySingle<OperationTraceEntity>(statement, trace: trace);
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation));
            }
        }

        /// <summary>
        /// Dispatches through the public repository APIs without bypassing their trace configuration.
        /// </summary>
        /// <param name="typed">The entity repository.</param>
        /// <param name="repository">The shared repository.</param>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="entityRepository">Whether to use the entity repository.</param>
        /// <param name="asynchronous">Whether to use the asynchronous API.</param>
        /// <returns>The repository result.</returns>
        private async Task<object> ExecuteRepositoryCoreAsync(CustomTracedRepository typed,
            DbRepository<CustomTracedDbConnection> repository, string operation,
            bool entityRepository, bool asynchronous)
        {
            var entity = new OperationTraceEntity { Id = 1, Name = "Trace" };
            switch (operation)
            {
                case "CountAll":
                    if (entityRepository)
                    {
                        return asynchronous
                            ? await typed.CountAllAsync(cancellationToken: Cancellation.Token).ConfigureAwait(false)
                            : typed.CountAll();
                    }
                    return asynchronous
                        ? await repository.CountAllAsync<OperationTraceEntity>(cancellationToken: Cancellation.Token).ConfigureAwait(false)
                        : repository.CountAll<OperationTraceEntity>();
                case "Insert":
                    if (entityRepository)
                    {
                        return asynchronous
                            ? await typed.InsertAsync(entity, cancellationToken: Cancellation.Token).ConfigureAwait(false)
                            : typed.Insert(entity);
                    }
                    return asynchronous
                        ? await repository.InsertAsync(entity, cancellationToken: Cancellation.Token).ConfigureAwait(false)
                        : repository.Insert(entity);
                case "QueryAll":
                    if (entityRepository)
                    {
                        return asynchronous
                            ? await typed.QueryAllAsync(cancellationToken: Cancellation.Token).ConfigureAwait(false)
                            : typed.QueryAll();
                    }
                    return asynchronous
                        ? await repository.QueryAllAsync<OperationTraceEntity>(cancellationToken: Cancellation.Token).ConfigureAwait(false)
                        : repository.QueryAll<OperationTraceEntity>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation));
            }
        }

        #endregion
    }
}
