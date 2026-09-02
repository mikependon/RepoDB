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
using RepoDb.Connector.EnterpriseDb;
using RepoDb.Enumerations.EnterpriseDb;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.EnterpriseDb.BulkOperations;
using RepoDb.EnterpriseDb.BulkOperations.Extensions;

namespace RepoDb
{
    /// <summary>
    ///
    /// </summary>
    public static partial class EDBConnectionExtension
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
        private static int BulkUpdateBase<TEntity>(this EDBConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkUpdate,
            EDBTransaction transaction = null)
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

            var pseudoTableName = EDBText.GetPseudoTableNameForUpdate(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                EDBExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                EDBExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                EDBExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                result = EDBExecution.UpdateFromPseudoTable(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                EDBExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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
        private static int BulkUpdateBase(this EDBConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkUpdate,
            EDBTransaction transaction = null)
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

            var pseudoTableName = EDBText.GetPseudoTableNameForUpdate(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                EDBExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                EDBExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                EDBExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                result = EDBExecution.UpdateFromPseudoTable(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                EDBExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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
        private static int BulkUpdateBase(this EDBConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkUpdate,
            EDBTransaction transaction = null)
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
            var pseudoTableName = EDBText.GetPseudoTableNameForUpdate(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                EDBExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                EDBExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                EDBExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, reader, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                result = EDBExecution.UpdateFromPseudoTable(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                EDBExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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
        private static async Task<int> BulkUpdateBaseAsync<TEntity>(this EDBConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkUpdate,
            EDBTransaction transaction = null,
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

            var pseudoTableName = EDBText.GetPseudoTableNameForUpdate(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await EDBExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await EDBExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await EDBExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                result = await EDBExecution.UpdateFromPseudoTableAsync(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await EDBExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
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
        private static async Task<int> BulkUpdateBaseAsync(this EDBConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkUpdate,
            EDBTransaction transaction = null,
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

            var pseudoTableName = EDBText.GetPseudoTableNameForUpdate(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await EDBExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await EDBExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await EDBExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                result = await EDBExecution.UpdateFromPseudoTableAsync(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await EDBExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
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
        private static async Task<int> BulkUpdateBaseAsync(this EDBConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<EDBBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            EDBBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = EDBTraceKeys.EDBBulkUpdate,
            EDBTransaction transaction = null,
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
            var pseudoTableName = EDBText.GetPseudoTableNameForUpdate(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await EDBExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await EDBExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await EDBExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, reader, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                result = await EDBExecution.UpdateFromPseudoTableAsync(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await EDBExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
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
