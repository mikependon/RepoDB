using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations.Firebird;
using RepoDb.Extensions;
using RepoDb.Firebird.BulkOperations;
using RepoDb.Firebird.BulkOperations.Extensions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class FirebirdConnectionExtension
    {
        #region Sync

        private static int BulkDeleteByKeyBase<TPrimaryKey>(this FbConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDeleteByKey,
            FbTransaction transaction = null)
        {
            var keyValueList = primaryKeys?.Select(k => (object)k).AsList();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierField = GetQualifierFields(tableName, dbFields).First();
            var pseudoTableName = FirebirdText.CreatePseudoTableName("K");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, keyValueList?.Count);

            using var command = CreateTraceCommand(connection, $"BULK DELETE BY KEY FROM {tableName}", bulkCopyTimeout, transaction);
            var traceResult = Tracer.InvokeBeforeExecution(traceKey, trace, command);

            int result;
            try
            {
                FirebirdExecution.CreatePseudoTable(connection, pseudoTableName, new[] { qualifierField }, dbFields, pseudoTableType, trace, traceKey, transaction);
                FirebirdExecution.CreatePseudoTableIndex(connection, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction);

                using var dataTable = CreateKeyValuesDataTable(qualifierField, keyValueList);
                var mappings = new[] { new FirebirdCommandBatcherMapItem(qualifierField.Name, qualifierField.Name) };
                WriteToServerInternal(connection, pseudoTableName, dataTable, mappings: mappings, bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction);

                result = FirebirdExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction);
            }
            finally
            {
                FirebirdExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            Tracer.InvokeAfterExecution(traceResult, trace, result);
            return result;
        }

        #endregion

        #region Async

        private static async Task<int> BulkDeleteByKeyBaseAsync<TPrimaryKey>(this FbConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDeleteByKey,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var keyValueList = primaryKeys?.Select(k => (object)k).AsList();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var qualifierField = GetQualifierFields(tableName, dbFields).First();
            var pseudoTableName = FirebirdText.CreatePseudoTableName("K");
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, keyValueList?.Count);

            using var command = CreateTraceCommand(connection, $"BULK DELETE BY KEY FROM {tableName}", bulkCopyTimeout, transaction);
            var traceResult = await Tracer.InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;
            try
            {
                await FirebirdExecution.CreatePseudoTableAsync(connection, pseudoTableName, new[] { qualifierField }, dbFields, pseudoTableType, trace, traceKey, transaction, cancellationToken);
                await FirebirdExecution.CreatePseudoTableIndexAsync(connection, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction, cancellationToken);

                using var dataTable = CreateKeyValuesDataTable(qualifierField, keyValueList);
                var mappings = new[] { new FirebirdCommandBatcherMapItem(qualifierField.Name, qualifierField.Name) };
                await WriteToServerAsyncInternal(connection, pseudoTableName, dataTable, mappings: mappings, bulkCopyTimeout: bulkCopyTimeout, batchSize: batchSize, transaction: transaction, cancellationToken: cancellationToken);

                result = await FirebirdExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                await FirebirdExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            await Tracer.InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
            return result;
        }

        #endregion
    }
}
