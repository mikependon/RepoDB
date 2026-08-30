#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations.Firebird;
using RepoDb.Extensions;
using RepoDb.Firebird.BulkOperations;
using RepoDb.Firebird.BulkOperations.Extensions;
using RepoDb.Interfaces;
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

        private static int BulkDeleteBase<TEntity>(this FbConnection connection,
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
            var entityList = entities.AsList();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
            var pseudoTableName = FirebirdText.CreatePseudoTableName("D");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            int result;
            try
            {
                FirebirdExecution.CreatePseudoTable(connection, pseudoTableName, qualifierFields, dbFields, pseudoTableType, trace, traceKey, transaction);
                FirebirdExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);

                using var entityTable = BuildEntityDataTable(entityList, qualifierFields);
                WriteToServerInternal(connection, pseudoTableName, entityTable, bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction);

                result = FirebirdExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                FirebirdExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            Tracer.InvokeAfterExecution(traceResult, trace, result);
            return result;
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
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
            var pseudoTableName = FirebirdText.CreatePseudoTableName("D");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            int result;
            try
            {
                FirebirdExecution.CreatePseudoTable(connection, pseudoTableName, qualifierFields, dbFields, pseudoTableType, trace, traceKey, transaction);
                FirebirdExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);

                var mappings = qualifierFields.Select(f => new FirebirdCommandBatcherMapItem(f.Name, f.Name)).AsList();
                WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize, transaction);

                result = FirebirdExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                FirebirdExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            Tracer.InvokeAfterExecution(traceResult, trace, result);
            return result;
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
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
            var pseudoTableName = FirebirdText.CreatePseudoTableName("D");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            int result;
            try
            {
                FirebirdExecution.CreatePseudoTable(connection, pseudoTableName, qualifierFields, dbFields, pseudoTableType, trace, traceKey, transaction);
                FirebirdExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);

                var mappings = qualifierFields.Select(f => new FirebirdCommandBatcherMapItem(f.Name, f.Name)).AsList();
                WriteToServerInternal(connection, pseudoTableName, reader, mappings, bulkCopyTimeout, batchSize, transaction);

                result = FirebirdExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                FirebirdExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            Tracer.InvokeAfterExecution(traceResult, trace, result);
            return result;
        }

        #endregion

        #endregion

        #region Async

        #region BulkDeleteBaseAsync<TEntity>

        private static async Task<int> BulkDeleteBaseAsync<TEntity>(this FbConnection connection,
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
            var entityList = entities.AsList();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
            var pseudoTableName = FirebirdText.CreatePseudoTableName("D");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;
            try
            {
                await FirebirdExecution.CreatePseudoTableAsync(connection, pseudoTableName, qualifierFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken);
                await FirebirdExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);

                using var entityTable = BuildEntityDataTable(entityList, qualifierFields);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityTable, bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction, cancellationToken: cancellationToken);

                result = await FirebirdExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                await FirebirdExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            await Tracer.InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
            return result;
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
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
            var pseudoTableName = FirebirdText.CreatePseudoTableName("D");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;
            try
            {
                await FirebirdExecution.CreatePseudoTableAsync(connection, pseudoTableName, qualifierFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken);
                await FirebirdExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);

                var mappings = qualifierFields.Select(f => new FirebirdCommandBatcherMapItem(f.Name, f.Name)).AsList();
                await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize, transaction, cancellationToken);

                result = await FirebirdExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                await FirebirdExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            await Tracer.InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
            return result;
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
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
            var pseudoTableName = FirebirdText.CreatePseudoTableName("D");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;
            try
            {
                await FirebirdExecution.CreatePseudoTableAsync(connection, pseudoTableName, qualifierFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken);
                await FirebirdExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);

                var mappings = qualifierFields.Select(f => new FirebirdCommandBatcherMapItem(f.Name, f.Name)).AsList();
                await WriteToServerAsyncInternal(connection, pseudoTableName, reader, mappings, bulkCopyTimeout, batchSize, transaction, cancellationToken);

                result = await FirebirdExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                await FirebirdExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            await Tracer.InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
            return result;
        }

        #endregion

        #endregion
    }
}
