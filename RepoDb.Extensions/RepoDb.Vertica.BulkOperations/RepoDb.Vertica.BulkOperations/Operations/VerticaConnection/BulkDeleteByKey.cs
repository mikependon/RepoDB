using Vertica.Data.VerticaClient;
using RepoDb.Enumerations.Vertica;
using RepoDb.Interfaces;
using RepoDb.Vertica.BulkOperations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class VerticaConnectionExtension
    {
        #region Sync

        /// <summary>
        /// Deletes existing rows from the database in bulk, matched by a bare list of primary/identity key
        /// values rather than full entities. Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that maps to the target table.</typeparam>
        /// <typeparam name="TPrimaryKey">The type of the primary/identity key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="primaryKeys">The list of primary/identity key values identifying the rows to delete.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create for the delete operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDeleteByKey<TEntity, TPrimaryKey>(this VerticaConnection connection,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDeleteByKey,
            VerticaTransaction transaction = null)
            where TEntity : class =>
            BulkDeleteByKeyBase(connection, ClassMappedNameCache.Get<TEntity>(), primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Deletes existing rows from the database in bulk, matched by a bare list of primary/identity key
        /// values rather than full entities. Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TPrimaryKey">The type of the primary/identity key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="primaryKeys">The list of primary/identity key values identifying the rows to delete.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create for the delete operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDeleteByKey<TPrimaryKey>(this VerticaConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDeleteByKey,
            VerticaTransaction transaction = null) =>
            BulkDeleteByKeyBase(connection, tableName, primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        #endregion

        #region Async

        /// <summary>
        /// Deletes existing rows from the database in bulk in an asynchronous way, matched by a bare list
        /// of primary/identity key values rather than full entities. Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that maps to the target table.</typeparam>
        /// <typeparam name="TPrimaryKey">The type of the primary/identity key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="primaryKeys">The list of primary/identity key values identifying the rows to delete.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create for the delete operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static Task<int> BulkDeleteByKeyAsync<TEntity, TPrimaryKey>(this VerticaConnection connection,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDeleteByKey,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkDeleteByKeyBaseAsync(connection, ClassMappedNameCache.Get<TEntity>(), primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Deletes existing rows from the database in bulk in an asynchronous way, matched by a bare list
        /// of primary/identity key values rather than full entities. Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TPrimaryKey">The type of the primary/identity key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="primaryKeys">The list of primary/identity key values identifying the rows to delete.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create for the delete operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static Task<int> BulkDeleteByKeyAsync<TPrimaryKey>(this VerticaConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDeleteByKey,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkDeleteByKeyBaseAsync(connection, tableName, primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        #endregion
    }
}
