using IBM.Data.Db2;
using RepoDb.Enumerations.Db2;
using RepoDb.Interfaces;
using RepoDb.Db2.BulkOperations;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Entity-typed <see cref="BaseRepository{TEntity, TDbConnection}"/> wrappers for the Db2 bulk-merge
    /// operation. Each method is a thin pass-through onto <see cref="DbRepository{TDbConnection}"/>'s own
    /// wrapper (see <c>Operations/DbRepository/BulkMerge.cs</c>), which in turn calls the
    /// <see cref="DB2Connection"/> extension methods - matching the three-tier pattern used throughout
    /// the rest of RepoDB. DataTable and <c>DbDataReader</c>-based overloads are deliberately not duplicated
    /// at this tier or the <see cref="DbRepository{TDbConnection}"/> tier - they aren't tied to a single
    /// entity type, so those calls read more naturally straight off <see cref="DB2Connection"/>.
    /// </summary>
    public static partial class BaseRepositoryExtension
    {
        #region Sync

        /// <summary>
        /// Merges a list of entities into the database in bulk - inserts new rows and updates existing
        /// ones based on the defined qualifiers (defaults to the primary key). Returns the number of
        /// affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyOptions">The options that control the behavior of the underlying <see cref="DB2BulkCopy"/> operation.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int BulkMerge<TEntity>(this BaseRepository<TEntity, DB2Connection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportIdentityBehavior identityBehavior = default,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null)
            where TEntity : class =>
            repository.DbRepository.BulkMerge(ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Merges a list of entities into the database in bulk - inserts new rows and updates existing
        /// ones based on the defined qualifiers (defaults to the primary key). Returns the number of
        /// affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyOptions">The options that control the behavior of the underlying <see cref="DB2BulkCopy"/> operation.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int BulkMerge<TEntity>(this BaseRepository<TEntity, DB2Connection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportIdentityBehavior identityBehavior = default,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null)
            where TEntity : class =>
            repository.DbRepository.BulkMerge(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);

        #endregion

        #region Async

        /// <summary>
        /// Merges a list of entities into the database in bulk in an asynchronous way - inserts new rows
        /// and updates existing ones based on the defined qualifiers (defaults to the primary key). Returns
        /// the number of affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyOptions">The options that control the behavior of the underlying <see cref="DB2BulkCopy"/> operation.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> BulkMergeAsync<TEntity>(this BaseRepository<TEntity, DB2Connection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportIdentityBehavior identityBehavior = default,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            repository.DbRepository.BulkMergeAsync(ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Merges a list of entities into the database in bulk in an asynchronous way - inserts new rows
        /// and updates existing ones based on the defined qualifiers (defaults to the primary key). Returns
        /// the number of affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyOptions">The options that control the behavior of the underlying <see cref="DB2BulkCopy"/> operation.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> BulkMergeAsync<TEntity>(this BaseRepository<TEntity, DB2Connection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportIdentityBehavior identityBehavior = default,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            repository.DbRepository.BulkMergeAsync(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        #endregion
    }
}
