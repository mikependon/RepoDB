using RepoDb.Connector.MariaDb;
using RepoDb.Enumerations.MariaDb;
using RepoDb.Interfaces;
using RepoDb.MariaDb.BulkOperations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Entity-typed <see cref="BaseRepository{TEntity, TDbConnection}"/> wrapper for the MariaDb
    /// bulk-delete-by-key operation. This method is a thin pass-through onto
    /// <see cref="DbRepository{TDbConnection}"/>'s own wrapper (see
    /// <c>Operations/DbRepository/BulkDeleteByKey.cs</c>), which in turn calls the
    /// <see cref="MariaDbConnection"/> extension method - matching the three-tier pattern used throughout
    /// the rest of RepoDB.
    /// </summary>
    public static partial class BaseRepositoryExtension
    {
        #region Sync

        /// <summary>
        /// Deletes existing rows from <paramref name="tableName"/> in bulk, matched by their primary
        /// (or identity) key value. Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPrimaryKey">The type of the primary/identity key values.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="primaryKeys">The list of primary keys to be bulk-deleted.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDeleteByKey<TEntity, TPrimaryKey>(this BaseRepository<TEntity, MariaDbConnection> repository,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MariaDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkDeleteByKey,
            MariaDbTransaction transaction = null)
            where TEntity : class =>
            repository.DbRepository.BulkDeleteByKey(tableName, primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        #endregion

        #region Async

        /// <summary>
        /// Deletes existing rows from <paramref name="tableName"/> in bulk in an asynchronous way,
        /// matched by their primary (or identity) key value. Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPrimaryKey">The type of the primary/identity key values.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="primaryKeys">The list of primary keys to be bulk-deleted.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static Task<int> BulkDeleteByKeyAsync<TEntity, TPrimaryKey>(this BaseRepository<TEntity, MariaDbConnection> repository,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MariaDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkDeleteByKey,
            MariaDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            repository.DbRepository.BulkDeleteByKeyAsync(tableName, primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        #endregion
    }
}
