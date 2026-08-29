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

        #region BulkUpdateBase<TEntity>

        /// <summary>
        /// Updates existing rows on <paramref name="tableName"/> via a staging (pseudo) table: the pseudo
        /// table is (re)used and cleared, the entities are bulk-written into it, and a single <c>MERGE</c>
        /// statement (with only a <c>WHEN MATCHED THEN UPDATE</c> branch - see
        /// <see cref="OracleText.GetUpdateFromPseudoTableSql"/>) updates every row it matches. Unlike
        /// <c>BulkMerge</c>, there is no identity-returning variant - a plain update never creates rows.
        /// This is the "actual base execution" - the single <see cref="Tracer.InvokeBeforeExecution"/>/
        /// <see cref="Tracer.InvokeAfterExecution"/> pair for the whole <c>BulkUpdate</c> call wraps the
        /// entire create/truncate/write/update/drop sequence below (when there's anything to update).
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers">The field(s) to match an existing row on. Defaults to the primary/identity key when not provided.</param>
        /// <param name="mappings">
        /// The explicit source-to-destination column mapping. When provided, only the destination columns named here
        /// (plus, always, the qualifier column(s)) are staged and updated - see the same caveat documented on
        /// <see cref="BulkMergeBaseNoReturnIdentity{TEntity}"/> regarding leaving a qualifier column out of the mapping.
        /// </param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction">
        /// The transaction under which the staging-table DDL and the final <c>MERGE</c> statement run. As with
        /// <c>BulkMerge</c>, the bulk-write step in between (<see cref="OracleBulkCopy"/>) is transaction-agnostic
        /// and always commits immediately regardless of this parameter.
        /// </param>
        /// <returns>The number of rows updated.</returns>
        /// <exception cref="PrimaryFieldNotFoundException">No <paramref name="qualifiers"/> were given, and the table has neither a primary nor an identity key.</exception>
        /// <exception cref="MissingFieldsException">The resulting staged-field list is empty.</exception>
        private static int BulkUpdateBase<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            OracleBulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkUpdate,
            OracleTransaction transaction = null)
            where TEntity : class
        {
            // Identify the columns
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);
            var stagingFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);

            // Skip if there's nothing to update
            if (!HasUpdateableFields(stagingFields, qualifierFields))
            {
                return 0;
            }

            var pseudoTableName = OracleText.GetPseudoTableNameForUpdate(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                OracleExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                OracleExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                OracleExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, entityList, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);

                // Execute and return
                result = OracleExecution.UpdateFromPseudoTable(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                OracleExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkUpdateBase<DataTable>

        /// <summary>
        /// Updates the rows of <paramref name="tableName"/> that are matched by <paramref name="table"/>,
        /// following the same steps as the <c>TEntity</c> overload - see
        /// <see cref="BulkUpdateBase{TEntity}"/> for the detailed remarks (identical caveats around
        /// <paramref name="mappings"/> and <paramref name="transaction"/> apply here).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns>The number of rows updated.</returns>
        private static int BulkUpdateBase(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            OracleBulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkUpdate,
            OracleTransaction transaction = null)
        {
            // Identify the columns
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);
            var stagingFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);

            // Skip if there's nothing to update
            if (!HasUpdateableFields(stagingFields, qualifierFields))
            {
                return 0;
            }

            var pseudoTableName = OracleText.GetPseudoTableNameForUpdate(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                OracleExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                OracleExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                OracleExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);

                // Execute and return
                result = OracleExecution.UpdateFromPseudoTable(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                OracleExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkUpdateBase<DbDataReader>

        /// <summary>
        /// Updates existing rows on <paramref name="tableName"/> by streaming <paramref name="reader"/>
        /// straight into a staging (pseudo) table - see <see cref="BulkUpdateBase{TEntity}"/> for the
        /// detailed staging-table steps (identical caveats around <paramref name="mappings"/> and
        /// <paramref name="transaction"/> apply here). As with the other overloads, there is no
        /// identity-returning variant - a plain update never creates rows - so this is the "actual base
        /// execution" directly.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="qualifiers">The field(s) to match an existing row on. Defaults to the primary/identity key when not provided.</param>
        /// <param name="mappings">
        /// The explicit source-to-destination column mapping. When provided, only the destination columns named here
        /// (plus, always, the qualifier column(s)) are staged and updated - see the same caveat documented on
        /// <see cref="BulkMergeBaseNoReturnIdentity{TEntity}"/> regarding leaving a qualifier column out of the mapping.
        /// </param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction">
        /// The transaction under which the staging-table DDL and the final <c>MERGE</c> statement run. As with
        /// <c>BulkMerge</c>, the bulk-write step in between (<see cref="OracleBulkCopy"/>) is transaction-agnostic
        /// and always commits immediately regardless of this parameter.
        /// </param>
        /// <returns>The number of rows updated.</returns>
        /// <exception cref="PrimaryFieldNotFoundException">No <paramref name="qualifiers"/> were given, and the table has neither a primary nor an identity key.</exception>
        /// <exception cref="MissingFieldsException">The resulting staged-field list is empty.</exception>
        private static int BulkUpdateBase(this OracleConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            OracleBulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkUpdate,
            OracleTransaction transaction = null)
        {
            // Identify the columns - row count is unknown for a streaming reader (see the remarks on the
            // DbDataReader BulkMerge overload); Auto-resolution is currently a no-op regardless.
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);
            var stagingFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);

            // Skip if there's nothing to update
            if (!HasUpdateableFields(stagingFields, qualifierFields))
            {
                return 0;
            }

            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);
            var pseudoTableName = OracleText.GetPseudoTableNameForUpdate(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                OracleExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                OracleExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                OracleExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, reader, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);

                // Execute and return
                result = OracleExecution.UpdateFromPseudoTable(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                OracleExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #endregion

        #region Async

        #region BulkUpdateBaseAsync<TEntity>

        /// <summary>
        /// Asynchronous counterpart of <see cref="BulkUpdateBase{TEntity}"/> - see its remarks for the
        /// detailed behavior and caveats (identical here).
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The number of rows updated.</returns>
        private static async Task<int> BulkUpdateBaseAsync<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            OracleBulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkUpdate,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Identify the columns
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);
            var stagingFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);

            // Skip if there's nothing to update
            if (!HasUpdateableFields(stagingFields, qualifierFields))
            {
                return 0;
            }

            var pseudoTableName = OracleText.GetPseudoTableNameForUpdate(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await OracleExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await OracleExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await OracleExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityList, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                result = await OracleExecution.UpdateFromPseudoTableAsync(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await OracleExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #region BulkUpdateBaseAsync<DataTable>

        /// <summary>
        /// Asynchronous counterpart of the <c>DataTable</c> <see cref="BulkUpdateBase(OracleConnection, string, DataTable, IEnumerable{Field}, DataRowState?, IEnumerable{OracleBulkInsertMapItem}, OracleBulkCopyOptions, int?, int?, OracleBulkImportPseudoTableType, ITrace, string, OracleTransaction)"/> -
        /// see its remarks for the detailed behavior and caveats (identical here).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The number of rows updated.</returns>
        private static async Task<int> BulkUpdateBaseAsync(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            OracleBulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkUpdate,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Identify the columns
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);
            var stagingFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);

            // Skip if there's nothing to update
            if (!HasUpdateableFields(stagingFields, qualifierFields))
            {
                return 0;
            }

            var pseudoTableName = OracleText.GetPseudoTableNameForUpdate(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await OracleExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await OracleExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await OracleExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                result = await OracleExecution.UpdateFromPseudoTableAsync(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await OracleExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #region BulkUpdateBaseAsync<DbDataReader>

        /// <summary>
        /// Asynchronous counterpart of <see cref="BulkUpdateBase(OracleConnection, string, DbDataReader, IEnumerable{Field}, IEnumerable{OracleBulkInsertMapItem}, OracleBulkCopyOptions, int?, int?, OracleBulkImportPseudoTableType, ITrace, string, OracleTransaction)"/> -
        /// see its remarks for the detailed behavior and caveats (identical here).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The number of rows updated.</returns>
        private static async Task<int> BulkUpdateBaseAsync(this OracleConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            OracleBulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkUpdate,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Identify the columns
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);
            var stagingFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);

            // Skip if there's nothing to update
            if (!HasUpdateableFields(stagingFields, qualifierFields))
            {
                return 0;
            }

            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);
            var pseudoTableName = OracleText.GetPseudoTableNameForUpdate(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await OracleExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await OracleExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await OracleExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, reader, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                result = await OracleExecution.UpdateFromPseudoTableAsync(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await OracleExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
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
        /// Returns whether at least one of <paramref name="fields"/> is not also a qualifier - i.e.
        /// whether a <c>BulkUpdate</c> against these fields would actually have anything to <c>SET</c>.
        /// Qualifiers-only field lists are unusual (the table's only columns are all part of the match
        /// key) but not an error - they simply mean there is nothing to update.
        /// </summary>
        private static bool HasUpdateableFields(IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers) =>
            fields.Any(field =>
                qualifiers.Any(qualifier => string.Equals(qualifier.Name, field.Name, StringComparison.OrdinalIgnoreCase)) == false);

        #endregion
    }
}
