#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Connector.CockroachDb;
using RepoDb.Enumerations.CockroachDb;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.CockroachDb.BulkOperations;
using RepoDb.CockroachDb.BulkOperations.Extensions;

namespace RepoDb
{
    public static partial class CockroachDbConnectionExtension
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
        private static int BulkDeleteByKeyBase<TPrimaryKey>(this CockroachDbConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkDeleteByKey,
            CockroachDbTransaction transaction = null)
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
        private static async Task<int> BulkDeleteByKeyBaseAsync<TPrimaryKey>(this CockroachDbConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            CockroachDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = CockroachDbTraceKeys.CockroachDbBulkDeleteByKey,
            CockroachDbTransaction transaction = null,
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
        private static int BulkDeleteBaseViaKeyValues(CockroachDbConnection connection,
            string tableName,
            IEnumerable<object> keyValues,
            int? bulkCopyTimeout,
            int? batchSize,
            CockroachDbBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            CockroachDbTransaction transaction)
        {
            TraceResult traceResult = null;
            int result = default;
            Exception executionError = null;
            try
            {
                var pseudoTableName = CockroachDbText.GetPseudoTableNameForDeleteByKey(tableName, pseudoTableType, connection.GetDbSetting());
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierField = GetQualifierFields(tableName, dbFields).First();

                using var command = CreateTraceCommand(connection, $"BULK DELETE BY KEY FROM {tableName}", bulkCopyTimeout, transaction);

                // Before Execution
                traceResult = Tracer
                    .InvokeBeforeExecution(traceKey, trace, command);

                if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                {
                    return default;
                }

                try
                {
                    // Bulk and post process - the pseudo table only ever needs the one qualifier column
                    CockroachDbExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, qualifierField, trace, traceKey, transaction);
                    CockroachDbExecution.CreatePseudoTableIndex(connection, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction);
                    CockroachDbExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);

                    using var dataTable = CreateKeyValuesDataTable(qualifierField, keyValues);
                    var mappings = new[] { new CockroachDbBulkInsertMapItem(qualifierField.Name, qualifierField.Name) };
                    WriteToServerInternal(connection, pseudoTableName, dataTable, null, mappings, bulkCopyTimeout, batchSize);

                    // Execute and return
                    result = CockroachDbExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction);
                }
                catch (Exception exception)
                {
                    executionError = exception;
                    throw;
                }
                finally
                {
                    Tracer.Cleanup(() =>
                    {
                        // Drop the pseudo table
                        CockroachDbExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                    },
                        executionError);
                }

                return result;
            }
            catch (Exception exception)
            {
                executionError ??= exception;
                throw;
            }
            finally
            {
                Tracer.InvokeAfterExecution(traceResult,
                    trace,
                    result,
                    executionError);
            }
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
        private static async Task<int> BulkDeleteBaseViaKeyValuesAsync(CockroachDbConnection connection,
            string tableName,
            IEnumerable<object> keyValues,
            int? bulkCopyTimeout,
            int? batchSize,
            CockroachDbBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            CockroachDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            TraceResult traceResult = null;
            int result = default;
            Exception executionError = null;
            try
            {
                var pseudoTableName = CockroachDbText.GetPseudoTableNameForDeleteByKey(tableName, pseudoTableType, connection.GetDbSetting());
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierField = GetQualifierFields(tableName, dbFields).First();

                using var command = CreateTraceCommand(connection, $"BULK DELETE BY KEY FROM {tableName}", bulkCopyTimeout, transaction);

                // Before Execution
                traceResult = await Tracer
                    .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

                if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                {
                    return default;
                }

                try
                {
                    // Bulk and post process - the pseudo table only ever needs the one qualifier column
                    await CockroachDbExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, qualifierField, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                    await CockroachDbExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                    await CockroachDbExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);

                    using var dataTable = CreateKeyValuesDataTable(qualifierField, keyValues);
                    var mappings = new[] { new CockroachDbBulkInsertMapItem(qualifierField.Name, qualifierField.Name) };
                    await WriteToServerAsyncInternal(connection, pseudoTableName, dataTable, null, mappings, bulkCopyTimeout, batchSize, cancellationToken).ConfigureAwait(false);

                    // Execute and return
                    result = await CockroachDbExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    executionError = exception;
                    throw;
                }
                finally
                {
                    await Tracer.CleanupAsync(async () =>
                    {
                        // Drop the pseudo table
                        await CockroachDbExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, (executionError == null ? cancellationToken : CancellationToken.None)).ConfigureAwait(false);
                    },
                        executionError).ConfigureAwait(false);
                }

                return result;
            }
            catch (Exception exception)
            {
                executionError ??= exception;
                throw;
            }
            finally
            {
                await Tracer.InvokeAfterExecutionAsync(traceResult,
                    trace,
                    result,
                    cancellationToken,
                    executionError).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
