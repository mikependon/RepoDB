using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations;
using RepoDb.Enumerations.Firebird;
using RepoDb.Interfaces;
using RepoDb.Firebird.BulkOperations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// <see cref="DbRepository{TDbConnection}"/> wrappers for the Firebird bulk-insert operation. Each
    /// method resolves a connection (reusing the transaction's connection when one is supplied, otherwise
    /// creating one via the repository), delegates to the matching <see cref="FbConnection"/> extension
    /// method, and disposes the connection afterwards only when the repository owns a per-call connection
    /// and no external transaction was supplied.
    /// </summary>
    public static partial class DbRepositoryExtension
    {
        #region Sync

        /// <summary>
        /// Inserts a list of entities into the database in bulk. Returns the number of inserted rows.
        /// </summary>
        public static int BulkInsert<TEntity>(this DbRepository<FbConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkInsert,
            FbTransaction transaction = null)
            where TEntity : class =>
            repository.BulkInsert(ClassMappedNameCache.Get<TEntity>(), entities, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);

        /// <inheritdoc cref="BulkInsert{TEntity}(DbRepository{FbConnection}, IEnumerable{TEntity}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportIdentityBehavior, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static int BulkInsert<TEntity>(this DbRepository<FbConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkInsert,
            FbTransaction transaction = null)
            where TEntity : class
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return connection.BulkInsert(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        #endregion

        #region Async

        /// <inheritdoc cref="BulkInsert{TEntity}(DbRepository{FbConnection}, IEnumerable{TEntity}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportIdentityBehavior, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static async Task<int> BulkInsertAsync<TEntity>(this DbRepository<FbConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkInsert,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            await repository.BulkInsertAsync(ClassMappedNameCache.Get<TEntity>(), entities, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkInsert{TEntity}(DbRepository{FbConnection}, IEnumerable{TEntity}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportIdentityBehavior, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static async Task<int> BulkInsertAsync<TEntity>(this DbRepository<FbConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkInsert,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return await connection.BulkInsertAsync(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        #endregion

        #region Helpers

        private static void DisposeIfOwned(DbRepository<FbConnection> repository,
            FbTransaction transaction,
            FbConnection connection)
        {
            if (repository.ConnectionPersistency == ConnectionPersistency.PerCall && transaction == null)
            {
                connection.Dispose();
            }
        }

        #endregion
    }
}
