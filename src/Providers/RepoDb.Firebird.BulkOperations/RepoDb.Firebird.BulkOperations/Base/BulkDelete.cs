#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Diagnostics.CodeAnalysis;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations.Firebird;
using RepoDb.Extensions;
using RepoDb.Firebird.BulkOperations;
using RepoDb.Firebird.BulkOperations.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class FirebirdConnectionExtension
    {
        #region Sync

        #region BulkDeleteBase<TEntity>

        private static int BulkDeleteBase<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this FbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null)
            where TEntity : class
        {
            TraceResult traceResult = null;
            int result = default;
            Exception executionError = null;
            try
            {
                // Validate the source columns (based on the column mappings behavior), before any data is written
                ValidateColumnMappings(connection, tableName, entities, hasMappings: false, transaction);

                var entityList = entities.AsList();
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var pseudoTableName = FirebirdText.CreatePseudoTableName("D");
                pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);

                using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);
                traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

                if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                {
                    return default;
                }

                try
                {
                    FirebirdExecution.CreatePseudoTable(connection, pseudoTableName, qualifierFields, dbFields, pseudoTableType, trace, traceKey, transaction);
                    FirebirdExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);

                    using var entityTable = BuildEntityDataTable(entityList, qualifierFields);
                    WriteToServerInternal(connection, pseudoTableName, entityTable, bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction);

                    result = FirebirdExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction);
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
                        FirebirdExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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

        #region BulkDeleteBase<DataTable>

        private static int BulkDeleteBase(this FbConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null)
        {
            TraceResult traceResult = null;
            int result = default;
            Exception executionError = null;
            try
            {
                // Validate the source columns (based on the column mappings behavior), before any data is written
                ValidateColumnMappings(connection, tableName, table, hasMappings: false, transaction);

                if (table == null)
                {
                    throw new ArgumentNullException(nameof(table));
                }

                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var pseudoTableName = FirebirdText.CreatePseudoTableName("D");
                pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);

                using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);
                traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

                if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                {
                    return default;
                }

                try
                {
                    FirebirdExecution.CreatePseudoTable(connection, pseudoTableName, qualifierFields, dbFields, pseudoTableType, trace, traceKey, transaction);
                    FirebirdExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);

                    var mappings = qualifierFields.Select(f => new FirebirdCommandBatcherMapItem(f.Name, f.Name)).AsList();
                    WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize, transaction);

                    result = FirebirdExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction);
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
                        FirebirdExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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

        #region BulkDeleteBase<DbDataReader>

        private static int BulkDeleteBase(this FbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null)
        {
            TraceResult traceResult = null;
            int result = default;
            Exception executionError = null;
            try
            {
                // Validate the source columns (based on the column mappings behavior), before any data is written
                ValidateColumnMappings(connection, tableName, reader, hasMappings: false, transaction);

                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var pseudoTableName = FirebirdText.CreatePseudoTableName("D");
                pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);

                using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);
                traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

                if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                {
                    return default;
                }

                try
                {
                    FirebirdExecution.CreatePseudoTable(connection, pseudoTableName, qualifierFields, dbFields, pseudoTableType, trace, traceKey, transaction);
                    FirebirdExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);

                    var mappings = qualifierFields.Select(f => new FirebirdCommandBatcherMapItem(f.Name, f.Name)).AsList();
                    WriteToServerInternal(connection, pseudoTableName, reader, mappings, bulkCopyTimeout, batchSize, transaction);

                    result = FirebirdExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction);
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
                        FirebirdExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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

        #endregion

        #region Async

        #region BulkDeleteBaseAsync<TEntity>

        private static async Task<int> BulkDeleteBaseAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this FbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            TraceResult traceResult = null;
            int result = default;
            Exception executionError = null;
            try
            {
                // Validate the source columns (based on the column mappings behavior), before any data is written
                ValidateColumnMappings(connection, tableName, entities, hasMappings: false, transaction);

                var entityList = entities.AsList();
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var pseudoTableName = FirebirdText.CreatePseudoTableName("D");
                pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);

                using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);
                traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

                if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                {
                    return default;
                }

                try
                {
                    await FirebirdExecution.CreatePseudoTableAsync(connection, pseudoTableName, qualifierFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                    await FirebirdExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);

                    using var entityTable = BuildEntityDataTable(entityList, qualifierFields);
                    await WriteToServerAsyncInternal(connection, pseudoTableName, entityTable, bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);

                    result = await FirebirdExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
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
                        await FirebirdExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, (executionError == null ? cancellationToken : CancellationToken.None)).ConfigureAwait(false);
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

        #region BulkDeleteBaseAsync<DataTable>

        private static async Task<int> BulkDeleteBaseAsync(this FbConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            TraceResult traceResult = null;
            int result = default;
            Exception executionError = null;
            try
            {
                // Validate the source columns (based on the column mappings behavior), before any data is written
                ValidateColumnMappings(connection, tableName, table, hasMappings: false, transaction);

                if (table == null)
                {
                    throw new ArgumentNullException(nameof(table));
                }

                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var pseudoTableName = FirebirdText.CreatePseudoTableName("D");
                pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);

                using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);
                traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

                if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                {
                    return default;
                }

                try
                {
                    await FirebirdExecution.CreatePseudoTableAsync(connection, pseudoTableName, qualifierFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                    await FirebirdExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);

                    var mappings = qualifierFields.Select(f => new FirebirdCommandBatcherMapItem(f.Name, f.Name)).AsList();
                    await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize, transaction, cancellationToken).ConfigureAwait(false);

                    result = await FirebirdExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
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
                        await FirebirdExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, (executionError == null ? cancellationToken : CancellationToken.None)).ConfigureAwait(false);
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

        #region BulkDeleteBaseAsync<DbDataReader>

        private static async Task<int> BulkDeleteBaseAsync(this FbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            TraceResult traceResult = null;
            int result = default;
            Exception executionError = null;
            try
            {
                // Validate the source columns (based on the column mappings behavior), before any data is written
                ValidateColumnMappings(connection, tableName, reader, hasMappings: false, transaction);

                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken).ConfigureAwait(false);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var pseudoTableName = FirebirdText.CreatePseudoTableName("D");
                pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);

                using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);
                traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken).ConfigureAwait(false);

                if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                {
                    return default;
                }

                try
                {
                    await FirebirdExecution.CreatePseudoTableAsync(connection, pseudoTableName, qualifierFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
                    await FirebirdExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);

                    var mappings = qualifierFields.Select(f => new FirebirdCommandBatcherMapItem(f.Name, f.Name)).AsList();
                    await WriteToServerAsyncInternal(connection, pseudoTableName, reader, mappings, bulkCopyTimeout, batchSize, transaction, cancellationToken).ConfigureAwait(false);

                    result = await FirebirdExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken).ConfigureAwait(false);
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
                        await FirebirdExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, (executionError == null ? cancellationToken : CancellationToken.None)).ConfigureAwait(false);
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

        #endregion
    }
}
