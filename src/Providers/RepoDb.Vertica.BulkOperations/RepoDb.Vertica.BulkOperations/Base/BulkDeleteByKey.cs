#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Vertica.Data.VerticaClient;
using RepoDb.Enumerations.Vertica;
using RepoDb.Extensions;
using RepoDb.Vertica.BulkOperations;
using RepoDb.Vertica.BulkOperations.Extensions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class VerticaConnectionExtension
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
        private static int BulkDeleteByKeyBase<TPrimaryKey>(this VerticaConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDeleteByKey,
            VerticaTransaction transaction = null)
        {
            TraceResult traceResult = null;
            int result = default;
            Exception executionError = null;
            try
            {
                var keyValueList = primaryKeys?.Select(k => (object)k).AsList();
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierField = GetQualifierFields(tableName, dbFields).First();
                var pseudoTableName = VerticaText.CreatePseudoTableName("K");
                pseudoTableType = ResolvePseudoTableType(pseudoTableType, keyValueList?.Count);

                using var command = CreateTraceCommand(connection, $"BULK DELETE BY KEY FROM {tableName}", bulkCopyTimeout, transaction);
                traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

                if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                {
                    return default;
                }

                try
                {
                    VerticaExecution.CreatePseudoTable(connection, pseudoTableName, new[] { qualifierField }, dbFields, pseudoTableType, trace, traceKey, transaction);

                    using var dataTable = CreateKeyValuesDataTable(qualifierField, keyValueList);
                    var mappings = new[] { new VerticaBulkInsertMapItem(qualifierField.Name, qualifierField.Name) };
                    WriteToServerInternal(connection, pseudoTableName, dataTable, mappings: mappings, bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction);

                    result = VerticaExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction);
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
                        VerticaExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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
        private static async Task<int> BulkDeleteByKeyBaseAsync<TPrimaryKey>(this VerticaConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDeleteByKey,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            TraceResult traceResult = null;
            int result = default;
            Exception executionError = null;
            try
            {
                var keyValueList = primaryKeys?.Select(k => (object)k).AsList();
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false);
                var qualifierField = GetQualifierFields(tableName, dbFields).First();
                var pseudoTableName = VerticaText.CreatePseudoTableName("K");
                pseudoTableType = ResolvePseudoTableType(pseudoTableType, keyValueList?.Count);

                using var command = CreateTraceCommand(connection, $"BULK DELETE BY KEY FROM {tableName}", bulkCopyTimeout, transaction);
                traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

                if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                {
                    return default;
                }

                try
                {
                    await VerticaExecution.CreatePseudoTableAsync(connection, pseudoTableName, new[] { qualifierField }, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);

                    using var dataTable = CreateKeyValuesDataTable(qualifierField, keyValueList);
                    var mappings = new[] { new VerticaBulkInsertMapItem(qualifierField.Name, qualifierField.Name) };
                    await WriteToServerAsyncInternal(connection, pseudoTableName, dataTable, mappings: mappings, bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);

                    result = await VerticaExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
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
                        await VerticaExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, (executionError == null ? cancellationToken : CancellationToken.None)).ConfigureAwait(false);
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
