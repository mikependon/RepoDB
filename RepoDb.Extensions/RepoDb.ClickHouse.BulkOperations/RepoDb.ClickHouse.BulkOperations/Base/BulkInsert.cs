using RepoDb.Enumerations.ClickHouse;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.ClickHouse.BulkOperations;
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
    public static partial class ClickHouseConnectionExtension
    {
        #region Sync

        #region BulkInsertBase<TEntity>

        /// <summary>
        /// Bulk-writes <paramref name="entities"/> directly into <paramref name="tableName"/> - no pseudo/
        /// staging table involved (unlike <c>BulkMerge</c>/<c>BulkUpdate</c>/<c>BulkDelete</c>), since a plain
        /// insert has no need to correlate against anything already in the real table. Guards
        /// <paramref name="identityBehavior"/> up front: ClickHouse has no identity/auto-increment/sequence of
        /// any kind (see <c>RepoDb.DbHelpers.ClickHouseDbHelper.GetScopeIdentity</c> in
        /// <c>RepoDb.ClickHouse</c>), so <see cref="ClickHouseBulkImportIdentityBehavior.ReturnIdentity"/> -
        /// which the MySQL/Oracle providers support via an <c>AUTO_INCREMENT</c>/sequence pre-assignment
        /// trick - cannot be honored here.
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
        /// <exception cref="NotSupportedException"><paramref name="identityBehavior"/> is <see cref="ClickHouseBulkImportIdentityBehavior.ReturnIdentity"/>.</exception>
        private static int BulkInsertBase<TEntity>(this ClickHouseConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<ClickHouseBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ClickHouseBulkImportIdentityBehavior identityBehavior = default,
            ClickHouseBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = ClickHouseTraceKeys.ClickHouseBulkInsert,
            DbTransaction transaction = null)
            where TEntity : class
        {
            GuardReturnIdentity(identityBehavior);

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
                batchSize,
                transaction);

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkInsertBase<DataTable>

        /// <summary>
        /// <see cref="DataTable"/> counterpart of <see cref="BulkInsertBase{TEntity}"/> - see its remarks for
        /// the detailed behavior (identical here).
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
        /// <exception cref="NotSupportedException"><paramref name="identityBehavior"/> is <see cref="ClickHouseBulkImportIdentityBehavior.ReturnIdentity"/>.</exception>
        private static int BulkInsertBase(this ClickHouseConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<ClickHouseBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ClickHouseBulkImportIdentityBehavior identityBehavior = default,
            ClickHouseBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = ClickHouseTraceKeys.ClickHouseBulkInsert,
            DbTransaction transaction = null)
        {
            GuardReturnIdentity(identityBehavior);

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
        private static int BulkInsertBase(this ClickHouseConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<ClickHouseBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ClickHouseBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = ClickHouseTraceKeys.ClickHouseBulkInsert,
            DbTransaction transaction = null)
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
                batchSize,
                transaction);

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
        /// Asynchronous counterpart of <see cref="BulkInsertBase{TEntity}"/>.
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
        /// <exception cref="NotSupportedException"><paramref name="identityBehavior"/> is <see cref="ClickHouseBulkImportIdentityBehavior.ReturnIdentity"/>.</exception>
        private static async Task<int> BulkInsertBaseAsync<TEntity>(this ClickHouseConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<ClickHouseBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ClickHouseBulkImportIdentityBehavior identityBehavior = default,
            ClickHouseBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = ClickHouseTraceKeys.ClickHouseBulkInsert,
            DbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            GuardReturnIdentity(identityBehavior);

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
                cancellationToken,
                transaction);

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #region BulkInsertBaseAsync<DataTable>

        /// <summary>
        /// Asynchronous counterpart of the <see cref="DataTable"/> overload of <c>BulkInsertBase</c>.
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
        /// <exception cref="NotSupportedException"><paramref name="identityBehavior"/> is <see cref="ClickHouseBulkImportIdentityBehavior.ReturnIdentity"/>.</exception>
        private static async Task<int> BulkInsertBaseAsync(this ClickHouseConnection connection,
            string tableName,
            DataTable table,
            DataRowState? rowState = null,
            IEnumerable<ClickHouseBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ClickHouseBulkImportIdentityBehavior identityBehavior = default,
            ClickHouseBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = ClickHouseTraceKeys.ClickHouseBulkInsert,
            DbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            GuardReturnIdentity(identityBehavior);

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
        private static async Task<int> BulkInsertBaseAsync(this ClickHouseConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<ClickHouseBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            ClickHouseBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = ClickHouseTraceKeys.ClickHouseBulkInsert,
            DbTransaction transaction = null,
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
                cancellationToken,
                transaction);

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// Throws when the caller explicitly asked for <see cref="ClickHouseBulkImportIdentityBehavior.ReturnIdentity"/> -
        /// ClickHouse has no identity/auto-increment/sequence of any kind, so there is no generated value to
        /// return. Silently falling back to <see cref="ClickHouseBulkImportIdentityBehavior.KeepIdentity"/>
        /// instead would leave the caller believing their entities' identity properties had been populated
        /// when they had not.
        /// </summary>
        /// <param name="identityBehavior"></param>
        /// <exception cref="NotSupportedException"></exception>
        private static void GuardReturnIdentity(ClickHouseBulkImportIdentityBehavior identityBehavior)
        {
            if (identityBehavior == ClickHouseBulkImportIdentityBehavior.ReturnIdentity)
            {
                throw new NotSupportedException(
                    "ClickHouse has no session-wide scope identity, sequence, or auto-increment mechanism, " +
                    "so 'ClickHouseBulkImportIdentityBehavior.ReturnIdentity' is not supported. Use 'KeepIdentity' instead.");
            }
        }

        #endregion
    }
}
