using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MySqlConnector;
using RepoDb.Enumerations.MySqlConnector;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.MySqlConnector.BulkOperations;
using RepoDb.MySqlConnector.BulkOperations.Extensions;

namespace RepoDb
{
    public static partial class MySqlConnectorConnectionExtension
    {
        #region Sync

        /// <summary>
        /// Deletes existing rows from <paramref name="tableName"/> in bulk, matched by their primary (or
        /// identity) key value, via a staging (pseudo) table. See <see cref="BulkDeleteBaseViaKeyValues"/>
        /// for the detailed steps (and for where the <see cref="Tracer"/> Before/After pair is actually
        /// invoked - this overload is a pure pass-through so the bulk operation is only traced once).
        /// </summary>
        /// <typeparam name="TPrimaryKey">The type of the primary/identity key values.</typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKeys">The list of primary/identity key values to be bulk-deleted.</param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns>The number of rows deleted.</returns>
        private static int BulkDeleteByKeyBase<TPrimaryKey>(this MySqlConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MySqlConnectorTraceKeys.MySqlConnectorBulkDeleteByKey,
            MySqlTransaction transaction = null)
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
        /// Asynchronous counterpart of <see cref="BulkDeleteByKeyBase{TPrimaryKey}"/> - see its remarks for
        /// the detailed behavior (identical here).
        /// </summary>
        /// <typeparam name="TPrimaryKey">The type of the primary/identity key values.</typeparam>
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
        /// <returns>The number of rows deleted.</returns>
        private static async Task<int> BulkDeleteByKeyBaseAsync<TPrimaryKey>(this MySqlConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MySqlConnectorTraceKeys.MySqlConnectorBulkDeleteByKey,
            MySqlTransaction transaction = null,
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
        /// Deletes rows from <paramref name="tableName"/> in bulk that are matched by a list of raw scalar
        /// key values (e.g. primary key values), via a staging (pseudo) table: the key values are bulk-written
        /// into a single-column pseudo table shaped after <paramref name="qualifiers"/> (defaulting to the
        /// primary/identity key when not provided), and a single <c>DELETE ... WHERE ROWID IN (SELECT ... INNER JOIN ...)</c>
        /// statement removes every matched row from the real table. Shared by the dedicated <c>BulkDeleteByKey</c> overload
        /// and by the <c>TEntity</c> overload's raw-key-value redirect (see <see cref="IsKeyValueCollection{TEntity}"/>).
        /// This is the "actual base execution" for both of those callers - the single
        /// <see cref="Tracer.InvokeBeforeExecution"/>/<see cref="Tracer.InvokeAfterExecution"/> pair wraps
        /// the entire create/truncate/write/delete/drop sequence below.
        /// </summary>
        /// <exception cref="PrimaryFieldNotFoundException">
        /// No <paramref name="qualifiers"/> were given, and the table has neither a primary nor an identity key.
        /// </exception>
        private static int BulkDeleteBaseViaKeyValues(MySqlConnection connection,
            string tableName,
            IEnumerable<object> keyValues,
            int? bulkCopyTimeout,
            int? batchSize,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            MySqlTransaction transaction)
        {
            var pseudoTableName = MySqlConnectorText.GetPseudoTableNameForDeleteByKey(tableName, pseudoTableType, connection.GetDbSetting());
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
                MySqlConnectorExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, qualifierField, trace, traceKey, transaction);
                MySqlConnectorExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);

                using var dataTable = CreateKeyValuesDataTable(qualifierField, keyValues);
                var mappings = new[] { new MySqlConnectorBulkInsertMapItem(qualifierField.Name, qualifierField.Name) };
                WriteToServerInternal(connection, pseudoTableName, dataTable, null, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                result = MySqlConnectorExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                MySqlConnectorExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="BulkDeleteBaseViaKeyValues"/> - see its remarks for the
        /// detailed behavior (identical here).
        /// </summary>
        private static async Task<int> BulkDeleteBaseViaKeyValuesAsync(MySqlConnection connection,
            string tableName,
            IEnumerable<object> keyValues,
            int? bulkCopyTimeout,
            int? batchSize,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            MySqlTransaction transaction,
            CancellationToken cancellationToken)
        {
            var pseudoTableName = MySqlConnectorText.GetPseudoTableNameForDeleteByKey(tableName, pseudoTableType, connection.GetDbSetting());
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
                await MySqlConnectorExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, qualifierField, trace, traceKey, transaction, cancellationToken);
                await MySqlConnectorExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);

                using var dataTable = CreateKeyValuesDataTable(qualifierField, keyValues);
                var mappings = new[] { new MySqlConnectorBulkInsertMapItem(qualifierField.Name, qualifierField.Name) };
                await WriteToServerAsyncInternal(connection, pseudoTableName, dataTable, null, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                result = await MySqlConnectorExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, new[] { qualifierField }, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await MySqlConnectorExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion
    }
}
