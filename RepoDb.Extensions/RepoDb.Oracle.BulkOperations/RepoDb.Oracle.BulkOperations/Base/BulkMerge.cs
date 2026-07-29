using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations.Oracle;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Oracle.BulkOperations;
using RepoDb.Oracle.BulkOperations.Extensions;

namespace RepoDb
{
    /// <summary>
    ///
    /// </summary>
    public static partial class OracleConnectionExtension
    {
        #region Sync

        #region BulkMergeBase<TEntity>

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static int BulkMergeBase<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkMerge,
            OracleTransaction transaction = null)
            where TEntity : class
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == OracleBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return connection.BulkMergeBaseForReturnIdentity(tableName,
                    entities,
                    qualifiers,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction);
            }
            else
            {
                return connection.BulkMergeBaseNoReturnIdentity(tableName,
                    entities,
                    qualifiers,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">
        /// Not implemented yet. This pass only covers the <see cref="OracleBulkImportIdentityBehavior.ReturnIdentity"/> == <c>false</c>
        /// path - see <see cref="BulkMergeBaseNoReturnIdentity{TEntity}"/>.
        /// </exception>
        private static int BulkMergeBaseForReturnIdentity<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkMerge,
            OracleTransaction transaction = null)
            where TEntity : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Upserts <paramref name="entities"/> into <paramref name="tableName"/> via a staging (pseudo)
        /// table: the pseudo table is (re)used and cleared, the entities are bulk-written into it, and a
        /// single <c>MERGE</c> statement upserts every staged row into the real table. This is the "actual
        /// base execution" - the single <see cref="Tracer.InvokeBeforeExecution"/>/
        /// <see cref="Tracer.InvokeAfterExecution"/> pair for the whole <c>BulkMerge</c> call wraps the
        /// entire create/truncate/write/merge/drop sequence below.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers">The field(s) to match an existing row on. Defaults to the primary/identity key when not provided.</param>
        /// <param name="mappings">
        /// The explicit source-to-destination column mapping. When provided, only the destination columns named here
        /// (plus, always, the qualifier column(s)) are staged and merged - if a qualifier column is intentionally
        /// left out of <paramref name="mappings"/>, that column will never be populated on the staging table and
        /// every row will be treated as new (insert-only) rather than matched for update.
        /// </param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction">
        /// The transaction under which the staging-table DDL and the final <c>MERGE</c> statement run. Note that
        /// the bulk-write step in between (<see cref="OracleBulkCopy"/>) is transaction-agnostic - ODP.NET does not
        /// support enlisting a bulk-copy operation into a transaction, so that specific step always commits
        /// immediately regardless of this parameter.
        /// </param>
        /// <returns>The number of rows affected by the <c>MERGE</c>.</returns>
        private static int BulkMergeBaseNoReturnIdentity<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkMerge,
            OracleTransaction transaction = null)
            where TEntity : class
        {
            // Identify the columns
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var pseudoTableName = OracleText.GetPseudoTableNameForMerge(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                OracleExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, transaction: transaction);
                OracleExecution.TruncatePseudoTable(connection, pseudoTableName, transaction);
                WriteToServerInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                result = OracleExecution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, transaction);
            }
            finally
            {
                // Drop the pseudo table
                OracleExecution.DropPseudoTable(connection, pseudoTableName, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkMergeBase<DataTable>

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBase(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkMerge,
            OracleTransaction transaction = null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == OracleBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return connection.BulkMergeBaseForReturnIdentity(tableName,
                    table,
                    qualifiers,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction);
            }
            else
            {
                return connection.BulkMergeBaseNoReturnIdentity(tableName,
                    table,
                    qualifiers,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">
        /// Not implemented yet. This pass only covers the <see cref="OracleBulkImportIdentityBehavior.ReturnIdentity"/> == <c>false</c>
        /// path - see <see cref="BulkMergeBaseNoReturnIdentity(OracleConnection, string, DataTable, IEnumerable{Field}, DataRowState?, IEnumerable{OracleBulkInsertMapItem}, int?, int?, OracleBulkImportPseudoTableType, ITrace, string, OracleTransaction)"/>.
        /// </exception>
        private static int BulkMergeBaseForReturnIdentity(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkMerge,
            OracleTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Upserts the rows of <paramref name="table"/> into <paramref name="tableName"/> via a staging
        /// (pseudo) table, following the same steps as the <c>TEntity</c> overload - see
        /// <see cref="BulkMergeBaseNoReturnIdentity{TEntity}"/> for the detailed remarks (identical
        /// caveats around <paramref name="mappings"/> and <paramref name="transaction"/> apply here).
        /// This is the "actual base execution" that the <see cref="Tracer"/> Before/After pair wraps.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns>The number of rows affected by the <c>MERGE</c>.</returns>
        private static int BulkMergeBaseNoReturnIdentity(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkMerge,
            OracleTransaction transaction = null)
        {
            // Identify the columns
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = OracleText.GetPseudoTableNameForMerge(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                OracleExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, transaction: transaction);
                OracleExecution.TruncatePseudoTable(connection, pseudoTableName, transaction);
                WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                result = OracleExecution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, transaction);
            }
            finally
            {
                // Drop the pseudo table
                OracleExecution.DropPseudoTable(connection, pseudoTableName, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkMergeBase<DbDataReader>

        /// <summary>
        /// Upserts <paramref name="dataReader"/> into <paramref name="tableName"/> via a staging (pseudo)
        /// table, streaming straight from the reader into it - see <see cref="BulkMergeBaseNoReturnIdentity{TEntity}"/>
        /// for the detailed staging-table steps (identical caveats around <paramref name="mappings"/> and
        /// <paramref name="transaction"/> apply here). Unlike the <c>TEntity</c>/<see cref="DataTable"/>
        /// overloads, there is no return-identity branch - a forward-only, single-pass reader cannot be
        /// rewound to retry/reconcile identity values - so this is both the "outer" call and the "actual
        /// base execution" in one, and there is no <c>identityBehavior</c> parameter at all.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dataReader"></param>
        /// <param name="qualifiers">The field(s) to match an existing row on. Defaults to the primary/identity key when not provided.</param>
        /// <param name="mappings">
        /// The explicit source-to-destination column mapping. When provided, only the destination columns named here
        /// (plus, always, the qualifier column(s)) are staged and merged - see the same caveat documented on
        /// <see cref="BulkMergeBaseNoReturnIdentity{TEntity}"/> regarding leaving a qualifier column out of the mapping.
        /// </param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction">
        /// The transaction under which the staging-table DDL and the final <c>MERGE</c> statement run. Note that
        /// the bulk-write step in between (<see cref="OracleBulkCopy"/>) is transaction-agnostic - ODP.NET does not
        /// support enlisting a bulk-copy operation into a transaction, so that specific step always commits
        /// immediately regardless of this parameter.
        /// </param>
        /// <returns>The number of rows affected by the <c>MERGE</c>.</returns>
        private static int BulkMergeBase(this OracleConnection connection,
            string tableName,
            DbDataReader dataReader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkMerge,
            OracleTransaction transaction = null)
        {
            // Identify the columns - row count is unknown for a streaming reader, so Auto-resolution (see
            // ResolvePseudoTableType's remarks) is passed a null hint; it is currently a no-op regardless.
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);
            var pseudoTableName = OracleText.GetPseudoTableNameForMerge(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                OracleExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, transaction: transaction);
                OracleExecution.TruncatePseudoTable(connection, pseudoTableName, transaction);
                WriteToServerInternal(connection, pseudoTableName, dataReader, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                result = OracleExecution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, transaction);
            }
            finally
            {
                // Drop the pseudo table
                OracleExecution.DropPseudoTable(connection, pseudoTableName, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #endregion

        #region Async

        #region BulkMergeBaseAsync<TEntity>

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseAsync<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkMerge,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == OracleBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return await connection.BulkMergeBaseForReturnIdentityAsync(tableName,
                    entities,
                    qualifiers,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }
            else
            {
                return await connection.BulkMergeBaseNoReturnIdentityAsync(tableName,
                    entities,
                    qualifiers,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">
        /// Not implemented yet. This pass only covers the <see cref="OracleBulkImportIdentityBehavior.ReturnIdentity"/> == <c>false</c>
        /// path - see <see cref="BulkMergeBaseNoReturnIdentityAsync{TEntity}"/>.
        /// </exception>
        private static async Task<int> BulkMergeBaseForReturnIdentityAsync<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkMerge,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="BulkMergeBaseNoReturnIdentity{TEntity}"/> - see its remarks
        /// for the detailed behavior and caveats (identical here).
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseNoReturnIdentityAsync<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            IEnumerable<OracleBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            int? batchSize,
            OracleBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            OracleTransaction transaction,
            CancellationToken cancellationToken)
            where TEntity : class
        {
            // Identify the columns
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var pseudoTableName = OracleText.GetPseudoTableNameForMerge(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await OracleExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, transaction: transaction, cancellationToken: cancellationToken);
                await OracleExecution.TruncatePseudoTableAsync(connection, pseudoTableName, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                result = await OracleExecution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await OracleExecution.DropPseudoTableAsync(connection, pseudoTableName, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #region BulkMergeBaseAsync<DataTable>

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static async Task<int> BulkMergeBaseAsync(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkMerge,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == OracleBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return await connection.BulkMergeBaseForReturnIdentityAsync(tableName,
                    table,
                    qualifiers,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }
            else
            {
                return await connection.BulkMergeBaseNoReturnIdentityAsync(tableName,
                    table,
                    qualifiers,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">
        /// Not implemented yet. This pass only covers the <see cref="OracleBulkImportIdentityBehavior.ReturnIdentity"/> == <c>false</c>
        /// path - see <see cref="BulkMergeBaseNoReturnIdentityAsync(OracleConnection, string, DataTable, IEnumerable{Field}, DataRowState?, IEnumerable{OracleBulkInsertMapItem}, int?, int?, OracleBulkImportPseudoTableType, ITrace, string, OracleTransaction, CancellationToken)"/>.
        /// </exception>
        private static async Task<int> BulkMergeBaseForReturnIdentityAsync(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkMerge,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronous counterpart of the <c>DataTable</c> <see cref="BulkMergeBaseNoReturnIdentity(OracleConnection, string, DataTable, IEnumerable{Field}, DataRowState?, IEnumerable{OracleBulkInsertMapItem}, int?, int?, OracleBulkImportPseudoTableType, ITrace, string, OracleTransaction)"/> -
        /// see its remarks for the detailed behavior and caveats (identical here).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseNoReturnIdentityAsync(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkMerge,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Identify the columns
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = OracleText.GetPseudoTableNameForMerge(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await OracleExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, transaction: transaction, cancellationToken: cancellationToken);
                await OracleExecution.TruncatePseudoTableAsync(connection, pseudoTableName, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                result = await OracleExecution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await OracleExecution.DropPseudoTableAsync(connection, pseudoTableName, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #region BulkMergeBaseAsync<DbDataReader>

        /// <summary>
        /// Asynchronous counterpart of <see cref="BulkMergeBase(OracleConnection, string, DbDataReader, IEnumerable{Field}, IEnumerable{OracleBulkInsertMapItem}, int?, int?, OracleBulkImportPseudoTableType, ITrace, string, OracleTransaction)"/> -
        /// see its remarks for the detailed behavior and caveats (identical here).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dataReader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseAsync(this OracleConnection connection,
            string tableName,
            DbDataReader dataReader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkMerge,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Identify the columns
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);
            var pseudoTableName = OracleText.GetPseudoTableNameForMerge(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await OracleExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, transaction: transaction, cancellationToken: cancellationToken);
                await OracleExecution.TruncatePseudoTableAsync(connection, pseudoTableName, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, dataReader, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                result = await OracleExecution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await OracleExecution.DropPseudoTableAsync(connection, pseudoTableName, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// Resolves the field(s) used to match an existing row during the <c>MERGE</c> (the <c>ON</c> clause).
        /// Falls back to the table's primary key, then its identity key, when <paramref name="qualifiers"/>
        /// is not provided.
        /// </summary>
        /// <exception cref="PrimaryFieldNotFoundException">
        /// No <paramref name="qualifiers"/> were given, and the table has neither a primary nor an identity key.
        /// </exception>
        private static IEnumerable<Field> GetQualifierFields(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<Field> qualifiers)
        {
            if (qualifiers?.Any() == true)
            {
                return qualifiers;
            }

            var primaryOrIdentity = dbFields?.GetPrimary() ?? dbFields?.GetIdentity();

            if (primaryOrIdentity == null)
            {
                throw new PrimaryFieldNotFoundException(
                    $"No primary or identity key found for table '{tableName}'. Provide explicit 'qualifiers' instead.");
            }

            return new[] { primaryOrIdentity.AsField() };
        }

        /// <summary>
        /// Resolves the full set of fields to stage and merge (both inserted and, where not a qualifier,
        /// updated). When <paramref name="mappings"/> is provided, only its destination columns - plus,
        /// always, <paramref name="qualifierFields"/> (needed for the <c>ON</c> clause regardless of
        /// whether they were explicitly mapped) - are kept.
        /// </summary>
        /// <exception cref="MissingFieldsException">The resulting field list is empty.</exception>
        private static IEnumerable<Field> GetMergeFields(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<OracleBulkInsertMapItem> mappings,
            IEnumerable<Field> qualifierFields)
        {
            var fields = dbFields?.GetAsFields();

            if (mappings?.Any() == true)
            {
                fields = fields?.Where(field =>
                    mappings.Any(mapping => string.Equals(mapping.DestinationColumn, field.Name, StringComparison.OrdinalIgnoreCase)) ||
                    qualifierFields.Any(qualifier => string.Equals(qualifier.Name, field.Name, StringComparison.OrdinalIgnoreCase)));
            }

            if (fields?.Any() != true)
            {
                throw new MissingFieldsException($"There are no field(s) found for table '{tableName}' for this operation.");
            }

            return fields;
        }

        #endregion
    }
}
