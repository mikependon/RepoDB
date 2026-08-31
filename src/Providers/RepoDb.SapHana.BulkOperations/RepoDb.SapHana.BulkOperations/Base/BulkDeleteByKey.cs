#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sap.Data.Hana;
using RepoDb.Enumerations.SapHana;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.SapHana.BulkOperations;
using RepoDb.SapHana.BulkOperations.Extensions;

namespace RepoDb
{
    public static partial class SapHanaConnectionExtension
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
        private static int BulkDeleteByKeyBase<TPrimaryKey>(this HanaConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SapHanaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SapHanaTraceKeys.SapHanaBulkDeleteByKey,
            HanaTransaction transaction = null)
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
        private static async Task<int> BulkDeleteByKeyBaseAsync<TPrimaryKey>(this HanaConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SapHanaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SapHanaTraceKeys.SapHanaBulkDeleteByKey,
            HanaTransaction transaction = null,
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
                cancellationToken);
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
        private static int BulkDeleteBaseViaKeyValues(HanaConnection connection,
            string tableName,
            IEnumerable<object> keyValues,
            int? bulkCopyTimeout,
            int? batchSize,
            SapHanaBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            HanaTransaction transaction)
        {
            var pseudoTableName = SapHanaText.GetPseudoTableNameForDeleteByKey(tableName, pseudoTableType, connection.GetDbSetting());
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
                SapHanaExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, qualifierField, trace, traceKey, transaction);
                SapHanaExecution.CreatePseudoTableIndex(connection, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction);
                SapHanaExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);

                using var dataTable = CreateKeyValuesDataTable(qualifierField, keyValues);
                var mappings = new[] { new SapHanaBulkInsertMapItem(qualifierField.Name, qualifierField.Name) };
                WriteToServerInternal(connection, pseudoTableName, dataTable, null, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                result = SapHanaExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                SapHanaExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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
        private static async Task<int> BulkDeleteBaseViaKeyValuesAsync(HanaConnection connection,
            string tableName,
            IEnumerable<object> keyValues,
            int? bulkCopyTimeout,
            int? batchSize,
            SapHanaBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            HanaTransaction transaction,
            CancellationToken cancellationToken)
        {
            var pseudoTableName = SapHanaText.GetPseudoTableNameForDeleteByKey(tableName, pseudoTableType, connection.GetDbSetting());
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierField = GetQualifierFields(tableName, dbFields).First();

            using var command = CreateTraceCommand(connection, $"BULK DELETE BY KEY FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process - the pseudo table only ever needs the one qualifier column
                await SapHanaExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, qualifierField, trace, traceKey, transaction, cancellationToken);
                await SapHanaExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction, cancellationToken);
                await SapHanaExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);

                using var dataTable = CreateKeyValuesDataTable(qualifierField, keyValues);
                var mappings = new[] { new SapHanaBulkInsertMapItem(qualifierField.Name, qualifierField.Name) };
                await WriteToServerAsyncInternal(connection, pseudoTableName, dataTable, null, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                result = await SapHanaExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await SapHanaExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion
    }
}
