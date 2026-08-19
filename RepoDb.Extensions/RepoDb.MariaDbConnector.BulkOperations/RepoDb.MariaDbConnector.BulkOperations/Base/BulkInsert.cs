using RepoDb.Connector.MariaDbConnector;
using RepoDb.Enumerations.MariaDb;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.MariaDbConnector.BulkOperations;
using RepoDb.MariaDbConnector.BulkOperations.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    ///
    /// </summary>
    public static partial class MariaDbConnectionExtension
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
        private static int BulkInsertBase<TEntity>(this MariaDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MariaDbBulkImportIdentityBehavior identityBehavior = default,
            MariaDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkInsert,
            MariaDbTransaction transaction = null)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == MariaDbBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

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
        private static int BulkInsertBaseForReturnIdentity<TEntity>(this MariaDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MariaDbBulkImportIdentityBehavior identityBehavior = default,
            MariaDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkInsert,
            MariaDbTransaction transaction = null)
            where TEntity : class
        {
            var entityList = entities.AsList();
            var pseudoTableName = MariaDbText.GetPseudoTableNameForInsert(tableName, pseudoTableType, connection.GetDbSetting());

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

                MariaDbExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                MariaDbExecution.AllowNullForColumn(connection, pseudoTableName, identityField.Name, trace, traceKey, transaction);
                MariaDbExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                var insertFields = GetInsertFields(tableName, dbFields, mappings);
                result = MariaDbExecution.InsertFromPseudoTableForReturnIdentity(connection, tableName, pseudoTableName, insertFields, identityField, entityList, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                MariaDbExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

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
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertBaseNoReturnIdentity<TEntity>(this MariaDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkInsert,
            MariaDbTransaction transaction = null)
            where TEntity : class
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            // Actual Execution
            var identityField = DbFieldCache.Get(connection, tableName, transaction)?.GetIdentity()?.AsField();
            var result = WriteToServerInternal(connection,
                tableName,
                entities,
                mappings,
                bulkCopyTimeout,
                batchSize,
                transaction,
                identityField);

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
        private static int BulkInsertBase(this MariaDbConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MariaDbBulkImportIdentityBehavior identityBehavior = default,
            MariaDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkInsert,
            MariaDbTransaction transaction = null)
        {
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == MariaDbBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

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
        private static int BulkInsertBaseForReturnIdentity(this MariaDbConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MariaDbBulkImportIdentityBehavior identityBehavior = default,
            MariaDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkInsert,
            MariaDbTransaction transaction = null)
        {
            var rows = GetDataRows(table, rowState)?.ToArray();
            var pseudoTableName = MariaDbText.GetPseudoTableNameForInsert(tableName, pseudoTableType, connection.GetDbSetting());

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

                MariaDbExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                MariaDbExecution.AllowNullForColumn(connection, pseudoTableName, identityField.Name, trace, traceKey, transaction);
                MariaDbExecution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                var insertFields = GetInsertFields(tableName, dbFields, mappings);
                result = MariaDbExecution.InsertFromPseudoTableForReturnIdentityForDataTable(connection, tableName, pseudoTableName, insertFields, identityField, rows, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                MariaDbExecution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
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
        private static int BulkInsertBaseNoReturnIdentity(this MariaDbConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkInsert,
            MariaDbTransaction transaction = null)
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            // Actual Execution
            var identityField = DbFieldCache.Get(connection, tableName, transaction)?.GetIdentity()?.AsField();
            var result = WriteToServerInternal(connection,
                tableName,
                table,
                rowState,
                mappings,
                bulkCopyTimeout,
                batchSize,
                identityField);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkInsertBase<DbDataReader>

        /// <summary>
        /// 
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
        /// <returns></returns>
        private static int BulkInsertBase(this MariaDbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MariaDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkInsert,
            MariaDbTransaction transaction = null)
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            // Actual Execution
            var identityField = DbFieldCache.Get(connection, tableName, transaction)?.GetIdentity()?.AsField();
            var result = WriteToServerInternal(connection,
                tableName,
                reader,
                mappings,
                bulkCopyTimeout,
                batchSize,
                transaction,
                identityField);

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
        private static async Task<int> BulkInsertBaseAsync<TEntity>(this MariaDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MariaDbBulkImportIdentityBehavior identityBehavior = default,
            MariaDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkInsert,
            MariaDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == MariaDbBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

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
        private static async Task<int> BulkInsertBaseForReturnIdentityAsync<TEntity>(this MariaDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MariaDbBulkImportIdentityBehavior identityBehavior = default,
            MariaDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkInsert,
            MariaDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities.AsList();
            var pseudoTableName = MariaDbText.GetPseudoTableNameForInsert(tableName, pseudoTableType, connection.GetDbSetting());

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

                await MariaDbExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await MariaDbExecution.AllowNullForColumnAsync(connection, pseudoTableName, identityField.Name, trace, traceKey, transaction, cancellationToken);
                await MariaDbExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityList, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var insertFields = GetInsertFields(tableName, dbFields, mappings);
                result = await MariaDbExecution.InsertFromPseudoTableForReturnIdentityAsync(connection, tableName, pseudoTableName, insertFields, identityField, entityList, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await MariaDbExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

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
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertBaseNoReturnIdentityAsync<TEntity>(this MariaDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkInsert,
            MariaDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            // Actual Execution
            var identityField = (await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken))?.GetIdentity()?.AsField();
            var result = await WriteToServerAsyncInternal(connection,
                tableName,
                entities,
                mappings,
                bulkCopyTimeout,
                batchSize,
                cancellationToken,
                transaction,
                identityField);

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
        private static async Task<int> BulkInsertBaseAsync(this MariaDbConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MariaDbBulkImportIdentityBehavior identityBehavior = default,
            MariaDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkInsert,
            MariaDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == MariaDbBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

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
        private static async Task<int> BulkInsertBaseForReturnIdentityAsync(this MariaDbConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MariaDbBulkImportIdentityBehavior identityBehavior = default,
            MariaDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkInsert,
            MariaDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var rows = GetDataRows(table, rowState)?.ToArray();
            var pseudoTableName = MariaDbText.GetPseudoTableNameForInsert(tableName, pseudoTableType, connection.GetDbSetting());

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

                await MariaDbExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await MariaDbExecution.AllowNullForColumnAsync(connection, pseudoTableName, identityField.Name, trace, traceKey, transaction, cancellationToken);
                await MariaDbExecution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var insertFields = GetInsertFields(tableName, dbFields, mappings);
                result = await MariaDbExecution.InsertFromPseudoTableForReturnIdentityForDataTableAsync(connection, tableName, pseudoTableName, insertFields, identityField, rows, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await MariaDbExecution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
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
        private static async Task<int> BulkInsertBaseNoReturnIdentityAsync(this MariaDbConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkInsert,
            MariaDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            // Actual Execution
            var identityField = (await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken))?.GetIdentity()?.AsField();
            var result = await WriteToServerAsyncInternal(connection,
                tableName,
                table,
                rowState,
                mappings,
                bulkCopyTimeout,
                batchSize,
                cancellationToken,
                identityField);

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #region BulkInsertBaseAsync<DbDataReader>

        /// <summary>
        /// 
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
        private static async Task<int> BulkInsertBaseAsync(this MariaDbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<MariaDbBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MariaDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkInsert,
            MariaDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            // Actual Execution
            var identityField = (await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken))?.GetIdentity()?.AsField();
            var result = await WriteToServerAsyncInternal(connection,
                tableName,
                reader,
                mappings,
                bulkCopyTimeout,
                batchSize,
                cancellationToken,
                transaction,
                identityField);

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbFields"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        /// <exception cref="MissingFieldsException"></exception>
        private static IEnumerable<Field> GetInsertFields(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<MariaDbBulkInsertMapItem> mappings)
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
