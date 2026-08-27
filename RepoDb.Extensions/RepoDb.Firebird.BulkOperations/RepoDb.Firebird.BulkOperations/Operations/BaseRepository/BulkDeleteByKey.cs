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
    /// Entity-typed <see cref="BaseRepository{TEntity, TDbConnection}"/> wrappers for the Firebird
    /// bulk-delete-by-key operation - thin pass-throughs onto <see cref="DbRepository{TDbConnection}"/>'s
    /// own wrapper (see <c>Operations/DbRepository/BulkDeleteByKey.cs</c>).
    /// </summary>
    public static partial class BaseRepositoryExtension
    {
        #region Sync

        /// <summary>
        /// Deletes existing rows from the database in bulk, matched by their primary (or identity) key
        /// value alone. Returns the number of deleted rows.
        /// </summary>
        public static int BulkDeleteByKey<TEntity, TPrimaryKey>(this BaseRepository<TEntity, FbConnection> repository,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDeleteByKey,
            FbTransaction transaction = null)
            where TEntity : class =>
            repository.DbRepository.BulkDeleteByKey(ClassMappedNameCache.Get<TEntity>(), primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        #endregion

        #region Async

        /// <inheritdoc cref="BulkDeleteByKey{TEntity, TPrimaryKey}(BaseRepository{TEntity, FbConnection}, IEnumerable{TPrimaryKey}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkDeleteByKeyAsync<TEntity, TPrimaryKey>(this BaseRepository<TEntity, FbConnection> repository,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDeleteByKey,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            repository.DbRepository.BulkDeleteByKeyAsync(ClassMappedNameCache.Get<TEntity>(), primaryKeys, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        #endregion
    }
}
