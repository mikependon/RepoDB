using RepoDb.Connector.MariaDbConnector;
using RepoDb.Enumerations.MariaDb;
using RepoDb.Interfaces;
using RepoDb.MariaDbConnector.BulkOperations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// <see cref="DbRepository{TDbConnection}"/> wrapper for the MariaDb bulk-delete-by-key operation. The
    /// method resolves a connection (reusing the transaction's connection when one is supplied, otherwise
    /// creating one via the repository), delegates to the matching <see cref="MariaDbConnection"/> extension
    /// method, and disposes the connection afterwards only when the repository owns a per-call connection
    /// and no external transaction was supplied - the same lifecycle every other RepoDB provider's
    /// DbRepository bulk wrapper already follows.
    /// </summary>
    public static partial class DbRepositoryExtension
    {
        #region Sync

        /// <summary>
        /// Deletes existing rows from <paramref name="tableName"/> in bulk, matched by their primary
        /// (or identity) key value. Returns the number of deleted rows.
        /// </summary>
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
        public static int BulkDeleteByKey<TPrimaryKey>(this DbRepository<MariaDbConnection> repository,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MariaDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkDeleteByKey,
            MariaDbTransaction transaction = null)
        {
            var connection = (MariaDbConnection)transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return connection.BulkDeleteByKey(tableName, primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Deletes existing rows from <paramref name="tableName"/> in bulk in an asynchronous way,
        /// matched by their primary (or identity) key value. Returns the number of deleted rows.
        /// </summary>
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
        public static async Task<int> BulkDeleteByKeyAsync<TPrimaryKey>(this DbRepository<MariaDbConnection> repository,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            MariaDbBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = MariaDbTraceKeys.MariaDbBulkDeleteByKey,
            MariaDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var connection = (MariaDbConnection)transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return await connection.BulkDeleteByKeyAsync(tableName, primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        #endregion
    }
}
