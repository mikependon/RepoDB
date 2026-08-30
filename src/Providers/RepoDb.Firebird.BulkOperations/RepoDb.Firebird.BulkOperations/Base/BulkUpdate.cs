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

        #region BulkUpdateBase<TEntity>

        private static int BulkUpdateBase<TEntity>(this FbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null)
            where TEntity : class
        {
            var entityList = entities.AsList();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
            var stagingFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

            if (!HasUpdateableFields(stagingFields, qualifierFields))
            {
                return 0;
            }

            var pseudoTableName = FirebirdText.CreatePseudoTableName("U");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            int result;
            try
            {
                FirebirdExecution.CreatePseudoTable(connection, pseudoTableName, stagingFields, dbFields, pseudoTableType, trace, traceKey, transaction);
                FirebirdExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);

                var entityFields = mappings?.Any() == true ? mappings.Select(m => new Field(m.SourceColumn)).AsList() : stagingFields;
                using var entityTable = BuildEntityDataTable(entityList, entityFields);
                WriteToServerInternal(connection, pseudoTableName, entityTable, mappings: mappings, bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction);

                result = FirebirdExecution.UpdateFromPseudoTable(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                FirebirdExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            Tracer.InvokeAfterExecution(traceResult, trace, result);
            return result;
        }

        #endregion

        #region BulkUpdateBase<DataTable>

        private static int BulkUpdateBase(this FbConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
            var stagingFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

            if (!HasUpdateableFields(stagingFields, qualifierFields))
            {
                return 0;
            }

            var pseudoTableName = FirebirdText.CreatePseudoTableName("U");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            int result;
            try
            {
                FirebirdExecution.CreatePseudoTable(connection, pseudoTableName, stagingFields, dbFields, pseudoTableType, trace, traceKey, transaction);
                FirebirdExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize, transaction);

                result = FirebirdExecution.UpdateFromPseudoTable(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction);
            }
            finally
            {
                FirebirdExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            Tracer.InvokeAfterExecution(traceResult, trace, result);
            return result;
        }

        #endregion

        #region BulkUpdateBase<DbDataReader>

        private static int BulkUpdateBase(this FbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
            var stagingFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

            if (!HasUpdateableFields(stagingFields, qualifierFields))
            {
                return 0;
            }

            var pseudoTableName = FirebirdText.CreatePseudoTableName("U");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            int result;
            try
            {
                FirebirdExecution.CreatePseudoTable(connection, pseudoTableName, stagingFields, dbFields, pseudoTableType, trace, traceKey, transaction);
                FirebirdExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, reader, mappings ?? GetDefaultMappingsForDataReader(connection, tableName, reader, transaction).AsList(), bulkCopyTimeout, batchSize, transaction);

                result = FirebirdExecution.UpdateFromPseudoTable(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction);
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

        #region BulkUpdateBaseAsync<TEntity>

        private static async Task<int> BulkUpdateBaseAsync<TEntity>(this FbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities.AsList();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
            var stagingFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

            if (!HasUpdateableFields(stagingFields, qualifierFields))
            {
                return 0;
            }

            var pseudoTableName = FirebirdText.CreatePseudoTableName("U");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;
            try
            {
                await FirebirdExecution.CreatePseudoTableAsync(connection, pseudoTableName, stagingFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken);
                await FirebirdExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);

                var entityFields = mappings?.Any() == true ? mappings.Select(m => new Field(m.SourceColumn)).AsList() : stagingFields;
                using var entityTable = BuildEntityDataTable(entityList, entityFields);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityTable, mappings: mappings, bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction, cancellationToken: cancellationToken);

                result = await FirebirdExecution.UpdateFromPseudoTableAsync(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                await FirebirdExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            await Tracer.InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
            return result;
        }

        #endregion

        #region BulkUpdateBaseAsync<DataTable>

        private static async Task<int> BulkUpdateBaseAsync(this FbConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
            var stagingFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

            if (!HasUpdateableFields(stagingFields, qualifierFields))
            {
                return 0;
            }

            var pseudoTableName = FirebirdText.CreatePseudoTableName("U");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;
            try
            {
                await FirebirdExecution.CreatePseudoTableAsync(connection, pseudoTableName, stagingFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken);
                await FirebirdExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize, transaction, cancellationToken);

                result = await FirebirdExecution.UpdateFromPseudoTableAsync(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                await FirebirdExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            await Tracer.InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
            return result;
        }

        #endregion

        #region BulkUpdateBaseAsync<DbDataReader>

        private static async Task<int> BulkUpdateBaseAsync(this FbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
            var stagingFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

            if (!HasUpdateableFields(stagingFields, qualifierFields))
            {
                return 0;
            }

            var pseudoTableName = FirebirdText.CreatePseudoTableName("U");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);

            using var command = CreateTraceCommand(connection, $"BULK UPDATE {tableName}", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;
            try
            {
                await FirebirdExecution.CreatePseudoTableAsync(connection, pseudoTableName, stagingFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken);
                await FirebirdExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, reader, mappings ?? GetDefaultMappingsForDataReader(connection, tableName, reader, transaction).AsList(), bulkCopyTimeout, batchSize, transaction, cancellationToken);

                result = await FirebirdExecution.UpdateFromPseudoTableAsync(connection, tableName, pseudoTableName, stagingFields, qualifierFields, trace, traceKey, transaction, cancellationToken);
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
