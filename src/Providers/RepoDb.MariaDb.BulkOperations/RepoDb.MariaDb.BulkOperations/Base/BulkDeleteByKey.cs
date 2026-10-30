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
using RepoDb.Connector.MariaDb;
using RepoDb.Enumerations.MariaDb;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.MariaDb.BulkOperations;
using RepoDb.MariaDb.BulkOperations.Extensions;

namespace RepoDb
{
    public static partial class MariaDbConnectionExtension
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
        private static int BulkDeleteByKeyBase<TPrimaryKey>(this MariaDbConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MariaDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkDeleteByKey,
            MariaDbTransaction transaction = null)
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
        private static async Task<int> BulkDeleteByKeyBaseAsync<TPrimaryKey>(this MariaDbConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MariaDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkDeleteByKey,
            MariaDbTransaction transaction = null,
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
        private static int BulkDeleteBaseViaKeyValues(MariaDbConnection connection,
            string tableName,
            IEnumerable<object> keyValues,
            int? bulkCopyTimeout,
            int? batchSize,
            MariaDbBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            MariaDbTransaction transaction)
        {
            TraceResult traceResult = null;
            int result = default;
            Exception executionError = null;
            try
            {
                var pseudoTableName = MariaDbText.GetPseudoTableNameForDeleteByKey(tableName, pseudoTableType, connection.GetDbSetting());
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
                    MariaDbExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, qualifierField, trace, traceKey, transaction);
                    MariaDbExecution.CreatePseudoTableIndex(connection, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction);
                    MariaDbExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);

                    using var dataTable = CreateKeyValuesDataTable(qualifierField, keyValues);
                    var mappings = new[] { new MariaDbBulkInsertMapItem(qualifierField.Name, qualifierField.Name) };
                    WriteToServerInternal(connection, pseudoTableName, dataTable, null, mappings, bulkCopyTimeout, batchSize);

                    // Execute and return
                    result = MariaDbExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction);
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
                        MariaDbExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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
        private static async Task<int> BulkDeleteBaseViaKeyValuesAsync(MariaDbConnection connection,
            string tableName,
            IEnumerable<object> keyValues,
            int? bulkCopyTimeout,
            int? batchSize,
            MariaDbBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            MariaDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            TraceResult traceResult = null;
            int result = default;
            Exception executionError = null;
            try
            {
                var pseudoTableName = MariaDbText.GetPseudoTableNameForDeleteByKey(tableName, pseudoTableType, connection.GetDbSetting());
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
                    await MariaDbExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, qualifierField, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                    await MariaDbExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                    await MariaDbExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);

                    using var dataTable = CreateKeyValuesDataTable(qualifierField, keyValues);
                    var mappings = new[] { new MariaDbBulkInsertMapItem(qualifierField.Name, qualifierField.Name) };
                    await WriteToServerAsyncInternal(connection, pseudoTableName, dataTable, null, mappings, bulkCopyTimeout, batchSize, cancellationToken).ConfigureAwait(false);

                    // Execute and return
                    result = await MariaDbExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
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
                        await MariaDbExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, (executionError == null ? cancellationToken : CancellationToken.None)).ConfigureAwait(false);
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
