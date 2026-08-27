using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations.Firebird;
using RepoDb.Interfaces;
using RepoDb.Firebird.BulkOperations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// <see cref="DbRepository{TDbConnection}"/> wrappers for the Firebird bulk-delete-by-key operation. See
    /// the remarks on <c>Operations/DbRepository/BulkInsert.cs</c> for the connection-lifecycle pattern
    /// shared by every wrapper in this folder.
    /// </summary>
    public static partial class DbRepositoryExtension
    {
        #region Sync

        /// <summary>
        /// Deletes existing rows from the database in bulk, matched by their primary (or identity) key
        /// value alone. Returns the number of deleted rows.
        /// </summary>
        public static int BulkDeleteByKey<TEntity, TPrimaryKey>(this DbRepository<FbConnection> repository,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDeleteByKey,
            FbTransaction transaction = null)
            where TEntity : class =>
            repository.BulkDeleteByKey(ClassMappedNameCache.Get<TEntity>(), primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <inheritdoc cref="BulkDeleteByKey{TEntity, TPrimaryKey}(DbRepository{FbConnection}, IEnumerable{TPrimaryKey}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static int BulkDeleteByKey<TPrimaryKey>(this DbRepository<FbConnection> repository,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDeleteByKey,
            FbTransaction transaction = null)
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

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

        /// <inheritdoc cref="BulkDeleteByKey{TEntity, TPrimaryKey}(DbRepository{FbConnection}, IEnumerable{TPrimaryKey}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static async Task<int> BulkDeleteByKeyAsync<TEntity, TPrimaryKey>(this DbRepository<FbConnection> repository,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDeleteByKey,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            await repository.BulkDeleteByKeyAsync(ClassMappedNameCache.Get<TEntity>(), primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkDeleteByKey{TEntity, TPrimaryKey}(DbRepository{FbConnection}, IEnumerable{TPrimaryKey}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static async Task<int> BulkDeleteByKeyAsync<TPrimaryKey>(this DbRepository<FbConnection> repository,
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
            var connection = transaction?.Connection ?? repository.CreateConnection();

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
