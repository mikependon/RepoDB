#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Vertica.Data.VerticaClient;
using RepoDb.Enumerations.Vertica;
using RepoDb.Extensions;
using RepoDb.Vertica.BulkOperations;
using RepoDb.Vertica.BulkOperations.Extensions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class VerticaConnectionExtension
    {
        #region Sync

        #region BulkMergeBase<TEntity>

        private static int BulkMergeBase<TEntity>(this VerticaConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkMerge,
            VerticaTransaction transaction = null)
            where TEntity : class
        {
            var entityList = entities.AsList();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == VerticaBulkImportIdentityBehavior.ReturnIdentity && identityField != null;
            var pseudoTableName = VerticaText.CreatePseudoTableName("M");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            int result;
            try
            {
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

                VerticaExecution.CreatePseudoTable(connection, pseudoTableName, mergeFields, dbFields, pseudoTableType, trace, traceKey, transaction);
                VerticaExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);

                var entityFields = mappings?.Any() == true ? mappings.Select(m => new Field(m.SourceColumn)).AsList() : mergeFields;
                using var entityTable = BuildEntityDataTable(entityList, entityFields, includeRowOrder: true);
                WriteToServerInternal(connection, pseudoTableName, entityTable, mappings: WithRowOrderMapping(mappings), bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction);

                result = returnIdentity
                    ? VerticaExecution.MergeFromPseudoTableForReturnIdentity(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField.AsField(), entityList, trace, traceKey, transaction)
                    : VerticaExecution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField?.AsField(), trace, traceKey, transaction);
            }
            finally
            {
                VerticaExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            Tracer.InvokeAfterExecution(traceResult, trace, result);
            return result;
        }

        #endregion

        #region BulkMergeBase<DataTable>

        private static int BulkMergeBase(this VerticaConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkMerge,
            VerticaTransaction transaction = null)
        {
            var rows = GetDataRows(table, rowState).AsList();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == VerticaBulkImportIdentityBehavior.ReturnIdentity && identityField != null;
            var pseudoTableName = VerticaText.CreatePseudoTableName("M");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, rows.Count);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            int result;
            try
            {
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

                VerticaExecution.CreatePseudoTable(connection, pseudoTableName, mergeFields, dbFields, pseudoTableType, trace, traceKey, transaction);
                VerticaExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);

                using var orderedTable = AddRowOrderColumn(table, rows);
                WriteToServerInternal(connection, pseudoTableName, orderedTable, mappings: WithRowOrderMapping(mappings), bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction);

                result = returnIdentity
                    ? VerticaExecution.MergeFromPseudoTableForReturnIdentityForDataTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField.AsField(), rows, trace, traceKey, transaction)
                    : VerticaExecution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField?.AsField(), trace, traceKey, transaction);
            }
            finally
            {
                VerticaExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            Tracer.InvokeAfterExecution(traceResult, trace, result);
            return result;
        }

        #endregion

        #region BulkMergeBase<DbDataReader>

        private static int BulkMergeBase(this VerticaConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkMerge,
            VerticaTransaction transaction = null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var pseudoTableName = VerticaText.CreatePseudoTableName("M");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            int result;
            try
            {
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

                VerticaExecution.CreatePseudoTable(connection, pseudoTableName, mergeFields, dbFields, pseudoTableType, trace, traceKey, transaction);
                VerticaExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, reader, mappings ?? GetDefaultMappingsForDataReader(connection, tableName, reader, transaction).AsList(), bulkCopyTimeout, batchSize, transaction);

                result = VerticaExecution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField?.AsField(), trace, traceKey, transaction);
            }
            finally
            {
                VerticaExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            Tracer.InvokeAfterExecution(traceResult, trace, result);
            return result;
        }

        #endregion

        #endregion

        #region Async

        #region BulkMergeBaseAsync<TEntity>

        private static async Task<int> BulkMergeBaseAsync<TEntity>(this VerticaConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkMerge,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities.AsList();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == VerticaBulkImportIdentityBehavior.ReturnIdentity && identityField != null;
            var pseudoTableName = VerticaText.CreatePseudoTableName("M");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;
            try
            {
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

                await VerticaExecution.CreatePseudoTableAsync(connection, pseudoTableName, mergeFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken);
                await VerticaExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);

                var entityFields = mappings?.Any() == true ? mappings.Select(m => new Field(m.SourceColumn)).AsList() : mergeFields;
                using var entityTable = BuildEntityDataTable(entityList, entityFields, includeRowOrder: true);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityTable, mappings: WithRowOrderMapping(mappings), bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction, cancellationToken: cancellationToken);

                result = returnIdentity
                    ? await VerticaExecution.MergeFromPseudoTableForReturnIdentityAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField.AsField(), entityList, trace, traceKey, transaction, cancellationToken)
                    : await VerticaExecution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField?.AsField(), trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                await VerticaExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            await Tracer.InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
            return result;
        }

        #endregion

        #region BulkMergeBaseAsync<DataTable>

        private static async Task<int> BulkMergeBaseAsync(this VerticaConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportIdentityBehavior identityBehavior = default,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkMerge,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var rows = GetDataRows(table, rowState).AsList();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == VerticaBulkImportIdentityBehavior.ReturnIdentity && identityField != null;
            var pseudoTableName = VerticaText.CreatePseudoTableName("M");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, rows.Count);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;
            try
            {
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

                await VerticaExecution.CreatePseudoTableAsync(connection, pseudoTableName, mergeFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken);
                await VerticaExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);

                using var orderedTable = AddRowOrderColumn(table, rows);
                await WriteToServerAsyncInternal(connection, pseudoTableName, orderedTable, mappings: WithRowOrderMapping(mappings), bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction, cancellationToken: cancellationToken);

                result = returnIdentity
                    ? await VerticaExecution.MergeFromPseudoTableForReturnIdentityForDataTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField.AsField(), rows, trace, traceKey, transaction, cancellationToken)
                    : await VerticaExecution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField?.AsField(), trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                await VerticaExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            await Tracer.InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
            return result;
        }

        #endregion

        #region BulkMergeBaseAsync<DbDataReader>

        private static async Task<int> BulkMergeBaseAsync(this VerticaConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<VerticaBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkMerge,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var identityField = dbFields.GetIdentity();
            var pseudoTableName = VerticaText.CreatePseudoTableName("M");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;
            try
            {
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

                await VerticaExecution.CreatePseudoTableAsync(connection, pseudoTableName, mergeFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken);
                await VerticaExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, reader, mappings ?? GetDefaultMappingsForDataReader(connection, tableName, reader, transaction).AsList(), bulkCopyTimeout, batchSize, transaction, cancellationToken);

                result = await VerticaExecution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField?.AsField(), trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                await VerticaExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            await Tracer.InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
            return result;
        }

        #endregion

        #endregion
    }
}
