#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Enumerations.AuroraDb;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.AuroraDb.PostgreSql.BulkOperations;
using RepoDb.AuroraDb.PostgreSql.BulkOperations.Extensions;

namespace RepoDb
{
    public static partial class AuroraDbConnectionExtension
    {
        #region Sync

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKeys"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkDeleteByKeyBase<TPrimaryKey>(this AuroraDbConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            AuroraDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = AuroraDbTraceKeys.AuroraDbBulkDeleteByKey,
            AuroraDbTransaction transaction = null)
        {
            var primaryKeyList = primaryKeys?.Select(primaryKey => (object)primaryKey).AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, primaryKeyList?.Count);

            return BulkDeleteBaseViaKeyValues(connection,
                tableName,
                primaryKeyList,
                bulkCopyTimeout,
                batchSize,
                pseudoTableType,
                trace,
                traceKey,
                transaction);
        }

        #endregion

        #region Async

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKeys"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkDeleteByKeyBaseAsync<TPrimaryKey>(this AuroraDbConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            AuroraDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = AuroraDbTraceKeys.AuroraDbBulkDeleteByKey,
            AuroraDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var primaryKeyList = primaryKeys?.Select(primaryKey => (object)primaryKey).AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, primaryKeyList?.Count);

            return await BulkDeleteBaseViaKeyValuesAsync(connection,
                tableName,
                primaryKeyList,
                bulkCopyTimeout,
                batchSize,
                pseudoTableType,
                trace,
                traceKey,
                transaction,
                cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="keyValues"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkDeleteBaseViaKeyValues(AuroraDbConnection connection,
            string tableName,
            IEnumerable<object> keyValues,
            int? bulkCopyTimeout,
            int? batchSize,
            AuroraDbBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            AuroraDbTransaction transaction)
        {
            var pseudoTableName = AuroraDbText.GetPseudoTableNameForDeleteByKey(tableName, pseudoTableType, connection.GetDbSetting());
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierField = GetQualifierFields(tableName, dbFields).First();

            using var command = CreateTraceCommand(connection, $"BULK DELETE BY KEY FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process - the pseudo table only ever needs the one qualifier column
                AuroraDbExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, qualifierField, trace, traceKey, transaction);
                AuroraDbExecution.CreatePseudoTableIndex(connection, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction);
                AuroraDbExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);

                using var dataTable = CreateKeyValuesDataTable(qualifierField, keyValues);
                var mappings = new[] { new AuroraDbBulkInsertMapItem(qualifierField.Name, qualifierField.Name) };
                WriteToServerInternal(connection, pseudoTableName, dataTable, null, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                result = AuroraDbExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                AuroraDbExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="keyValues"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkDeleteBaseViaKeyValuesAsync(AuroraDbConnection connection,
            string tableName,
            IEnumerable<object> keyValues,
            int? bulkCopyTimeout,
            int? batchSize,
            AuroraDbBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            AuroraDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            var pseudoTableName = AuroraDbText.GetPseudoTableNameForDeleteByKey(tableName, pseudoTableType, connection.GetDbSetting());
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierField = GetQualifierFields(tableName, dbFields).First();

            using var command = CreateTraceCommand(connection, $"BULK DELETE BY KEY FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

            int result;

            try
            {
                // Bulk and post process - the pseudo table only ever needs the one qualifier column
                await AuroraDbExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, qualifierField, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                await AuroraDbExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                await AuroraDbExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);

                using var dataTable = CreateKeyValuesDataTable(qualifierField, keyValues);
                var mappings = new[] { new AuroraDbBulkInsertMapItem(qualifierField.Name, qualifierField.Name) };
                await WriteToServerAsyncInternal(connection, pseudoTableName, dataTable, null, mappings, bulkCopyTimeout, batchSize, cancellationToken).ConfigureAwait(false);

                // Execute and return
                result = await AuroraDbExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Drop the pseudo table
                await AuroraDbExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken).ConfigureAwait(false);

            return result;
        }

        #endregion
    }
}
