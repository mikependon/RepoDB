using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Common;
using RepoDb.Enumerations.ClickHouse;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.ClickHouse.BulkOperations;
using RepoDb.ClickHouse.BulkOperations.Extensions;
using ClickHouse.Driver.ADO;

namespace RepoDb
{
    /// <summary>
    ///
    /// </summary>
    public static partial class ClickHouseConnectionExtension
    {
        #region Sync

        #region BulkUpdateBase<TEntity>

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
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkUpdateBase<TEntity>(this ClickHouseConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<ClickHouseBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ClickHouseBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = ClickHouseTraceKeys.ClickHouseBulkUpdate,
            DbTransaction transaction = null)
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

            var pseudoTableName = ClickHouseText.GetPseudoTableNameForUpdate(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                ClickHouseExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                ClickHouseExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                result = WriteToServerInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                ClickHouseExecution.UpdateFromPseudoTable(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                ClickHouseExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkUpdateBase<DataTable>

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
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkUpdateBase(this ClickHouseConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<ClickHouseBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ClickHouseBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = ClickHouseTraceKeys.ClickHouseBulkUpdate,
            DbTransaction transaction = null)
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

            var pseudoTableName = ClickHouseText.GetPseudoTableNameForUpdate(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                ClickHouseExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                ClickHouseExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                result = WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                ClickHouseExecution.UpdateFromPseudoTable(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                ClickHouseExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkUpdateBase<DbDataReader>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkUpdateBase(this ClickHouseConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<ClickHouseBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ClickHouseBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = ClickHouseTraceKeys.ClickHouseBulkUpdate,
            DbTransaction transaction = null)
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
            var pseudoTableName = ClickHouseText.GetPseudoTableNameForUpdate(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                ClickHouseExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                ClickHouseExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                result = WriteToServerInternal(connection, pseudoTableName, reader, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                ClickHouseExecution.UpdateFromPseudoTable(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                ClickHouseExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkUpdateBaseAsync<TEntity>(this ClickHouseConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<ClickHouseBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ClickHouseBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = ClickHouseTraceKeys.ClickHouseBulkUpdate,
            DbTransaction transaction = null,
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

            var pseudoTableName = ClickHouseText.GetPseudoTableNameForUpdate(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await ClickHouseExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await ClickHouseExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                result = await WriteToServerAsyncInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                await ClickHouseExecution.UpdateFromPseudoTableAsync(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await ClickHouseExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #region BulkUpdateBaseAsync<DataTable>

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
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkUpdateBaseAsync(this ClickHouseConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<ClickHouseBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ClickHouseBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = ClickHouseTraceKeys.ClickHouseBulkUpdate,
            DbTransaction transaction = null,
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

            var pseudoTableName = ClickHouseText.GetPseudoTableNameForUpdate(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await ClickHouseExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await ClickHouseExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                result = await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                await ClickHouseExecution.UpdateFromPseudoTableAsync(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await ClickHouseExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #region BulkUpdateBaseAsync<DbDataReader>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
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
        private static async Task<int> BulkUpdateBaseAsync(this ClickHouseConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<ClickHouseBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ClickHouseBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = ClickHouseTraceKeys.ClickHouseBulkUpdate,
            DbTransaction transaction = null,
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
            var pseudoTableName = ClickHouseText.GetPseudoTableNameForUpdate(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await ClickHouseExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await ClickHouseExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                result = await WriteToServerAsyncInternal(connection, pseudoTableName, reader, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                await ClickHouseExecution.UpdateFromPseudoTableAsync(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await ClickHouseExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
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
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <returns></returns>
        private static bool HasUpdateableFields(IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers) =>
            fields.Any(field =>
                qualifiers.Any(qualifier => string.Equals(qualifier.Name, field.Name, StringComparison.OrdinalIgnoreCase)) == false);

        #endregion
    }
}
