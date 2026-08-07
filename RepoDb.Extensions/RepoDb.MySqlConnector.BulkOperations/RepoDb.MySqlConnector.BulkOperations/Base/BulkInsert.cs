using MySqlConnector;
using RepoDb.Enumerations.MySqlConnector;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.MySqlConnector.BulkOperations;
using RepoDb.MySqlConnector.BulkOperations.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    ///
    /// </summary>
    public static partial class MySqlConnectorConnectionExtension
    {
        #region Sync

        #region BulkInsertBase<TEntity>

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBase<TEntity>(this MySqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MySqlConnectorBulkImportIdentityBehavior identityBehavior = default,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MySqlConnectorTraceKeys.MySqlConnectorBulkInsert,
            MySqlTransaction transaction = null)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == MySqlConnectorBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return connection.BulkInsertBaseForReturnIdentity(tableName,
                    entityList,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction);
            }
            else
            {
                return connection.BulkInsertBaseNoReturnIdentity(tableName,
                    entityList,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    trace,
                    traceKey,
                    transaction);
            }
        }

        /// <summary>
        /// Inserts <paramref name="entities"/> into <paramref name="tableName"/> via a staging (pseudo)
        /// table: the entities are bulk-written into the pseudo table, and a single set-based
        /// <c>INSERT ... SELECT ... RETURNING ... BULK COLLECT INTO ...</c> statement moves every staged
        /// row into the real table, assigning every generated identity value back onto the matching
        /// element of <paramref name="entities"/> - see the remarks on
        /// <see cref="MySqlConnectorText.GetInsertFromPseudoTableForReturnIdentitySql"/> for the ordering caveat.
        /// This is the "actual base execution" - the single <see cref="Tracer.InvokeBeforeExecution"/>/
        /// <see cref="Tracer.InvokeAfterExecution"/> pair for the whole <c>BulkInsert</c> call wraps it.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBaseForReturnIdentity<TEntity>(this MySqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MySqlConnectorBulkImportIdentityBehavior identityBehavior = default,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MySqlConnectorTraceKeys.MySqlConnectorBulkInsert,
            MySqlTransaction transaction = null)
            where TEntity : class
        {
            var entityList = entities.AsList();
            var pseudoTableName = MySqlConnectorText.GetPseudoTableNameForInsert(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();

                MySqlConnectorExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                MySqlConnectorExecution.AllowNullForColumn(connection, pseudoTableName, identityField.Name, trace, traceKey, transaction);
                MySqlConnectorExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                var insertFields = GetInsertFields(tableName, dbFields, mappings);
                result = MySqlConnectorExecution.InsertFromPseudoTableForReturnIdentity(connection, tableName, pseudoTableName, insertFields, identityField, entityList, trace, traceKey, transaction);
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
        /// Bulk-writes <paramref name="entities"/> directly into <paramref name="tableName"/> (no staging
        /// table - a plain <c>BulkInsert</c> has nothing to reconcile against existing rows the way
        /// <c>BulkMerge</c>/<c>BulkUpdate</c>/<c>BulkDelete</c> do). This is the "actual base execution" -
        /// the single <see cref="Tracer.InvokeBeforeExecution"/>/<see cref="Tracer.InvokeAfterExecution"/>
        /// pair for the whole <c>BulkInsert</c> call wraps it.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBaseNoReturnIdentity<TEntity>(this MySqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ITrace trace = null,
            string traceKey = MySqlConnectorTraceKeys.MySqlConnectorBulkInsert,
            MySqlTransaction transaction = null)
            where TEntity : class
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            // Actual Execution
            var result = WriteToServerInternal(connection,
                tableName,
                entities,
                mappings,
                bulkCopyTimeout,
                batchSize);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkInsertBase<DataTable>

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBase(this MySqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MySqlConnectorBulkImportIdentityBehavior identityBehavior = default,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MySqlConnectorTraceKeys.MySqlConnectorBulkInsert,
            MySqlTransaction transaction = null)
        {
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == MySqlConnectorBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return connection.BulkInsertBaseForReturnIdentity(tableName,
                    table,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction);
            }
            else
            {
                return connection.BulkInsertBaseNoReturnIdentity(tableName,
                    table,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    trace,
                    traceKey,
                    transaction);
            }
        }

        /// <summary>
        /// <see cref="DataTable"/> counterpart of <see cref="BulkInsertBaseForReturnIdentity{TEntity}"/> - see
        /// its remarks for the detailed behavior (identical here), except the generated identity values are
        /// assigned back onto the matching row's identity column of <paramref name="table"/> instead of an
        /// entity property.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBaseForReturnIdentity(this MySqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MySqlConnectorBulkImportIdentityBehavior identityBehavior = default,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MySqlConnectorTraceKeys.MySqlConnectorBulkInsert,
            MySqlTransaction transaction = null)
        {
            var rows = GetDataRows(table, rowState)?.ToArray();
            var pseudoTableName = MySqlConnectorText.GetPseudoTableNameForInsert(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();

                MySqlConnectorExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                MySqlConnectorExecution.AllowNullForColumn(connection, pseudoTableName, identityField.Name, trace, traceKey, transaction);
                MySqlConnectorExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                var insertFields = GetInsertFields(tableName, dbFields, mappings);
                result = MySqlConnectorExecution.InsertFromPseudoTableForReturnIdentityForDataTable(connection, tableName, pseudoTableName, insertFields, identityField, rows, trace, traceKey, transaction);
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
        /// Bulk-writes the rows of <paramref name="table"/> directly into <paramref name="tableName"/> -
        /// see <see cref="BulkInsertBaseNoReturnIdentity{TEntity}"/> for the detailed remarks (identical
        /// here). This is the "actual base execution" that the <see cref="Tracer"/> Before/After pair wraps.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBaseNoReturnIdentity(this MySqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ITrace trace = null,
            string traceKey = MySqlConnectorTraceKeys.MySqlConnectorBulkInsert,
            MySqlTransaction transaction = null)
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            // Actual Execution
            var result = WriteToServerInternal(connection,
                tableName,
                table,
                rowState,
                mappings,
                bulkCopyTimeout,
                batchSize);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkInsertBase<DbDataReader>

        /// <summary>
        /// Streams <paramref name="reader"/> directly into <paramref name="tableName"/> - no staging
        /// table, no identity return-value support (a forward-only, single-pass reader cannot be rewound
        /// to retry/reconcile identity values the way the in-memory <c>TEntity</c>/<see cref="DataTable"/>
        /// overloads eventually will via <see cref="MySqlConnectorBulkImportIdentityBehavior.ReturnIdentity"/> -
        /// hence no <c>identityBehavior</c> parameter here at all). This is both the "outer" call and the
        /// "actual base execution" in one - there is no return-identity branch to redirect through, so the
        /// single <see cref="Tracer.InvokeBeforeExecution"/>/<see cref="Tracer.InvokeAfterExecution"/> pair
        /// wraps the whole thing directly.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType">
        /// Unused for a plain streaming insert (there is no staging table to create) - accepted purely for
        /// signature symmetry with the other <c>DbDataReader</c> bulk operations, where it does matter.
        /// </param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBase(this MySqlConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MySqlConnectorTraceKeys.MySqlConnectorBulkInsert,
            MySqlTransaction transaction = null)
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            // Actual Execution
            var result = WriteToServerInternal(connection,
                tableName,
                reader,
                mappings,
                bulkCopyTimeout,
                batchSize);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #endregion

        #region Async

        #region BulkInsertBaseAsync<TEntity>

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseAsync<TEntity>(this MySqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MySqlConnectorBulkImportIdentityBehavior identityBehavior = default,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MySqlConnectorTraceKeys.MySqlConnectorBulkInsert,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == MySqlConnectorBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return await connection.BulkInsertBaseForReturnIdentityAsync(tableName,
                    entityList,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }
            else
            {
                return await connection.BulkInsertBaseNoReturnIdentityAsync(tableName,
                    entityList,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="BulkInsertBaseForReturnIdentity{TEntity}"/> - see its
        /// remarks for the detailed behavior (identical here).
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseForReturnIdentityAsync<TEntity>(this MySqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MySqlConnectorBulkImportIdentityBehavior identityBehavior = default,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MySqlConnectorTraceKeys.MySqlConnectorBulkInsert,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities.AsList();
            var pseudoTableName = MySqlConnectorText.GetPseudoTableNameForInsert(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();

                await MySqlConnectorExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await MySqlConnectorExecution.AllowNullForColumnAsync(connection, pseudoTableName, identityField.Name, trace, traceKey, transaction, cancellationToken);
                await MySqlConnectorExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var insertFields = GetInsertFields(tableName, dbFields, mappings);
                result = await MySqlConnectorExecution.InsertFromPseudoTableForReturnIdentityAsync(connection, tableName, pseudoTableName, insertFields, identityField, entityList, trace, traceKey, transaction, cancellationToken);
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

        /// <summary>
        /// Asynchronous counterpart of <see cref="BulkInsertBaseNoReturnIdentity{TEntity}"/> - see its
        /// remarks for the detailed behavior (identical here).
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseNoReturnIdentityAsync<TEntity>(this MySqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ITrace trace = null,
            string traceKey = MySqlConnectorTraceKeys.MySqlConnectorBulkInsert,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            // Actual Execution
            var result = await WriteToServerAsyncInternal(connection,
                tableName,
                entities,
                mappings,
                bulkCopyTimeout,
                batchSize,
                cancellationToken);

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #region BulkInsertBaseAsync<DataTable>

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseAsync(this MySqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MySqlConnectorBulkImportIdentityBehavior identityBehavior = default,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MySqlConnectorTraceKeys.MySqlConnectorBulkInsert,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == MySqlConnectorBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return await connection.BulkInsertBaseForReturnIdentityAsync(tableName,
                    table,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }
            else
            {
                return await connection.BulkInsertBaseNoReturnIdentityAsync(tableName,
                    table,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    batchSize,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="BulkInsertBaseForReturnIdentity(MySqlConnection, string, DataTable, DataRowState?, IEnumerable{MySqlConnectorBulkInsertMapItem}, int?, int?, MySqlConnectorBulkImportIdentityBehavior, MySqlConnectorBulkImportPseudoTableType, ITrace, string, MySqlTransaction)"/> -
        /// see its remarks for the detailed behavior (identical here).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseForReturnIdentityAsync(this MySqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MySqlConnectorBulkImportIdentityBehavior identityBehavior = default,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MySqlConnectorTraceKeys.MySqlConnectorBulkInsert,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var rows = GetDataRows(table, rowState)?.ToArray();
            var pseudoTableName = MySqlConnectorText.GetPseudoTableNameForInsert(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();

                await MySqlConnectorExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await MySqlConnectorExecution.AllowNullForColumnAsync(connection, pseudoTableName, identityField.Name, trace, traceKey, transaction, cancellationToken);
                await MySqlConnectorExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var insertFields = GetInsertFields(tableName, dbFields, mappings);
                result = await MySqlConnectorExecution.InsertFromPseudoTableForReturnIdentityForDataTableAsync(connection, tableName, pseudoTableName, insertFields, identityField, rows, trace, traceKey, transaction, cancellationToken);
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseNoReturnIdentityAsync(this MySqlConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ITrace trace = null,
            string traceKey = MySqlConnectorTraceKeys.MySqlConnectorBulkInsert,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            // Actual Execution
            var result = await WriteToServerAsyncInternal(connection,
                tableName,
                table,
                rowState,
                mappings,
                bulkCopyTimeout,
                batchSize,
                cancellationToken);

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #region BulkInsertBaseAsync<DbDataReader>

        /// <summary>
        /// Asynchronous counterpart of <see cref="BulkInsertBase(MySqlConnection, string, DbDataReader, IEnumerable{MySqlConnectorBulkInsertMapItem}, int?, int?, MySqlConnectorBulkImportPseudoTableType, ITrace, string, MySqlTransaction)"/> -
        /// see its remarks for the detailed behavior (identical here).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseAsync(this MySqlConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MySqlConnectorTraceKeys.MySqlConnectorBulkInsert,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            // Actual Execution
            var result = await WriteToServerAsyncInternal(connection,
                tableName,
                reader,
                mappings,
                bulkCopyTimeout,
                batchSize,
                cancellationToken);

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// Resolves the field(s) to insert - every real column, filtered down to just the ones named in
        /// <paramref name="mappings"/> when provided. Used only by the identity-return path, which (unlike
        /// the plain fire-and-forget path) needs an explicit column list to build the pseudo-table-to-real-table
        /// <c>INSERT ... SELECT</c> statement.
        /// </summary>
        /// <exception cref="MissingFieldsException">No field(s) were found for <paramref name="tableName"/>.</exception>
        private static IEnumerable<Field> GetInsertFields(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<MySqlConnectorBulkInsertMapItem> mappings)
        {
            var fields = dbFields?.GetAsFields();

            if (mappings?.Any() == true)
            {
                fields = fields?.Where(field =>
                    mappings.Any(mapping => string.Equals(mapping.DestinationColumn, field.Name, StringComparison.OrdinalIgnoreCase)));
            }

            if (fields?.Any() != true)
            {
                throw new MissingFieldsException($"There are no field(s) found for table '{tableName}' for this operation.");
            }

            return fields;
        }

        #endregion
    }
}
