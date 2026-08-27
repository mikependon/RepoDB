using FirebirdSql.Data.FirebirdClient;
using System;
using System.Linq.Expressions;
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
    /// bulk-merge operation - thin pass-throughs onto <see cref="DbRepository{TDbConnection}"/>'s own
    /// wrapper (see <c>Operations/DbRepository/BulkMerge.cs</c>).
    /// </summary>
    public static partial class BaseRepositoryExtension
    {
        #region Sync

        /// <summary>
        /// Upserts a list of entities into the database in bulk - inserting rows that don't already exist and
        /// updating rows that do. Returns the number of affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows for update. When not specified, the primary/identity key is used.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse when <paramref name="identityBehavior"/> is <see cref="FirebirdBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected (inserted or updated) rows.</returns>
        public static int BulkMerge<TEntity>(this BaseRepository<TEntity, FbConnection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null)
            where TEntity : class =>
            repository.DbRepository.BulkMerge(ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Upserts a list of entities into the database in bulk, targeting an explicitly named table -
        /// inserting rows that don't already exist and updating rows that do. Returns the number of affected
        /// rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows for update. When not specified, the primary/identity key is used.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse when <paramref name="identityBehavior"/> is <see cref="FirebirdBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected (inserted or updated) rows.</returns>
        public static int BulkMerge<TEntity>(this BaseRepository<TEntity, FbConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null)
            where TEntity : class =>
            repository.DbRepository.BulkMerge(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);

        #endregion

        #region Async

        /// <summary>
        /// Upserts a list of entities into the database in bulk in an asynchronous way - inserting rows that
        /// don't already exist and updating rows that do. Returns the number of affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows for update. When not specified, the primary/identity key is used.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse when <paramref name="identityBehavior"/> is <see cref="FirebirdBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of affected (inserted or updated) rows.</returns>
        public static Task<int> BulkMergeAsync<TEntity>(this BaseRepository<TEntity, FbConnection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            repository.DbRepository.BulkMergeAsync(ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Upserts a list of entities into the database in bulk in an asynchronous way, targeting an
        /// explicitly named table - inserting rows that don't already exist and updating rows that do.
        /// Returns the number of affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows for update. When not specified, the primary/identity key is used.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse when <paramref name="identityBehavior"/> is <see cref="FirebirdBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of affected (inserted or updated) rows.</returns>
        public static Task<int> BulkMergeAsync<TEntity>(this BaseRepository<TEntity, FbConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            repository.DbRepository.BulkMergeAsync(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        #endregion
    }
}
