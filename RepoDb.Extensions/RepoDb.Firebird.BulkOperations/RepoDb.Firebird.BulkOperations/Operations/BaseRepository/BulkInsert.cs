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
    /// bulk-insert operation - thin pass-throughs onto <see cref="DbRepository{TDbConnection}"/>'s own
    /// wrapper (see <c>Operations/DbRepository/BulkInsert.cs</c>).
    /// </summary>
    public static partial class BaseRepositoryExtension
    {
        #region Sync

        /// <summary>
        /// Inserts a list of entities into the database in bulk. Returns the number of inserted rows.
        /// </summary>
        public static int BulkInsert<TEntity>(this BaseRepository<TEntity, FbConnection> repository,
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
            repository.DbRepository.BulkInsert(ClassMappedNameCache.Get<TEntity>(), entities, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);

        /// <inheritdoc cref="BulkInsert{TEntity}(BaseRepository{TEntity, FbConnection}, IEnumerable{TEntity}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportIdentityBehavior, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static int BulkInsert<TEntity>(this BaseRepository<TEntity, FbConnection> repository,
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
            where TEntity : class =>
            repository.DbRepository.BulkInsert(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);

        #endregion

        #region Async

        /// <inheritdoc cref="BulkInsert{TEntity}(BaseRepository{TEntity, FbConnection}, IEnumerable{TEntity}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportIdentityBehavior, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkInsertAsync<TEntity>(this BaseRepository<TEntity, FbConnection> repository,
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
            repository.DbRepository.BulkInsertAsync(ClassMappedNameCache.Get<TEntity>(), entities, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkInsert{TEntity}(BaseRepository{TEntity, FbConnection}, IEnumerable{TEntity}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportIdentityBehavior, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkInsertAsync<TEntity>(this BaseRepository<TEntity, FbConnection> repository,
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
            where TEntity : class =>
            repository.DbRepository.BulkInsertAsync(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        #endregion
    }
}
