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

        #region BulkMergeBase<TEntity>

        private static int BulkMergeBase<TEntity>(this FbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null)
            where TEntity : class
        {
            var entityList = entities.AsList();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == FirebirdBulkImportIdentityBehavior.ReturnIdentity && identityField != null;
            var pseudoTableName = FirebirdText.CreatePseudoTableName("M");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            int result;
            try
            {
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

                FirebirdExecution.CreatePseudoTable(connection, pseudoTableName, mergeFields, dbFields, pseudoTableType, trace, traceKey, transaction);
                FirebirdExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);

                var entityFields = mappings?.Any() == true ? mappings.Select(m => new Field(m.SourceColumn)).AsList() : mergeFields;
                using var entityTable = BuildEntityDataTable(entityList, entityFields, includeRowOrder: true);
                WriteToServerInternal(connection, pseudoTableName, entityTable, mappings: WithRowOrderMapping(mappings), bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction);

                result = returnIdentity
                    ? FirebirdExecution.MergeFromPseudoTableForReturnIdentity(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField.AsField(), entityList, trace, traceKey, transaction)
                    : FirebirdExecution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField?.AsField(), trace, traceKey, transaction);
            }
            finally
            {
                FirebirdExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            Tracer.InvokeAfterExecution(traceResult, trace, result);
            return result;
        }

        #endregion

        #region BulkMergeBase<DataTable>

        private static int BulkMergeBase(this FbConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null)
        {
            var rows = GetDataRows(table, rowState).AsList();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == FirebirdBulkImportIdentityBehavior.ReturnIdentity && identityField != null;
            var pseudoTableName = FirebirdText.CreatePseudoTableName("M");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, rows.Count);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            int result;
            try
            {
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

                FirebirdExecution.CreatePseudoTable(connection, pseudoTableName, mergeFields, dbFields, pseudoTableType, trace, traceKey, transaction);
                FirebirdExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);

                using var orderedTable = AddRowOrderColumn(table, rows);
                WriteToServerInternal(connection, pseudoTableName, orderedTable, mappings: WithRowOrderMapping(mappings), bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction);

                result = returnIdentity
                    ? FirebirdExecution.MergeFromPseudoTableForReturnIdentityForDataTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField.AsField(), rows, trace, traceKey, transaction)
                    : FirebirdExecution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField?.AsField(), trace, traceKey, transaction);
            }
            finally
            {
                FirebirdExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            Tracer.InvokeAfterExecution(traceResult, trace, result);
            return result;
        }

        #endregion

        #region BulkMergeBase<DbDataReader>

        private static int BulkMergeBase(this FbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var pseudoTableName = FirebirdText.CreatePseudoTableName("M");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            int result;
            try
            {
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

                FirebirdExecution.CreatePseudoTable(connection, pseudoTableName, mergeFields, dbFields, pseudoTableType, trace, traceKey, transaction);
                FirebirdExecution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, reader, mappings ?? GetDefaultMappingsForDataReader(connection, tableName, reader, transaction).AsList(), bulkCopyTimeout, batchSize, transaction);

                result = FirebirdExecution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField?.AsField(), trace, traceKey, transaction);
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

        #region BulkMergeBaseAsync<TEntity>

        private static async Task<int> BulkMergeBaseAsync<TEntity>(this FbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities.AsList();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == FirebirdBulkImportIdentityBehavior.ReturnIdentity && identityField != null;
            var pseudoTableName = FirebirdText.CreatePseudoTableName("M");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;
            try
            {
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

                await FirebirdExecution.CreatePseudoTableAsync(connection, pseudoTableName, mergeFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken);
                await FirebirdExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);

                var entityFields = mappings?.Any() == true ? mappings.Select(m => new Field(m.SourceColumn)).AsList() : mergeFields;
                using var entityTable = BuildEntityDataTable(entityList, entityFields, includeRowOrder: true);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityTable, mappings: WithRowOrderMapping(mappings), bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction, cancellationToken: cancellationToken);

                result = returnIdentity
                    ? await FirebirdExecution.MergeFromPseudoTableForReturnIdentityAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField.AsField(), entityList, trace, traceKey, transaction, cancellationToken)
                    : await FirebirdExecution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField?.AsField(), trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                await FirebirdExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            await Tracer.InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
            return result;
        }

        #endregion

        #region BulkMergeBaseAsync<DataTable>

        private static async Task<int> BulkMergeBaseAsync(this FbConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var rows = GetDataRows(table, rowState).AsList();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == FirebirdBulkImportIdentityBehavior.ReturnIdentity && identityField != null;
            var pseudoTableName = FirebirdText.CreatePseudoTableName("M");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, rows.Count);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;
            try
            {
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

                await FirebirdExecution.CreatePseudoTableAsync(connection, pseudoTableName, mergeFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken);
                await FirebirdExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);

                using var orderedTable = AddRowOrderColumn(table, rows);
                await WriteToServerAsyncInternal(connection, pseudoTableName, orderedTable, mappings: WithRowOrderMapping(mappings), bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction, cancellationToken: cancellationToken);

                result = returnIdentity
                    ? await FirebirdExecution.MergeFromPseudoTableForReturnIdentityForDataTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField.AsField(), rows, trace, traceKey, transaction, cancellationToken)
                    : await FirebirdExecution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField?.AsField(), trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                await FirebirdExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            await Tracer.InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
            return result;
        }

        #endregion

        #region BulkMergeBaseAsync<DbDataReader>

        private static async Task<int> BulkMergeBaseAsync(this FbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var identityField = dbFields.GetIdentity();
            var pseudoTableName = FirebirdText.CreatePseudoTableName("M");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;
            try
            {
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields).AsList();

                await FirebirdExecution.CreatePseudoTableAsync(connection, pseudoTableName, mergeFields, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken);
                await FirebirdExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, reader, mappings ?? GetDefaultMappingsForDataReader(connection, tableName, reader, transaction).AsList(), bulkCopyTimeout, batchSize, transaction, cancellationToken);

                result = await FirebirdExecution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField?.AsField(), trace, traceKey, transaction, cancellationToken);
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
