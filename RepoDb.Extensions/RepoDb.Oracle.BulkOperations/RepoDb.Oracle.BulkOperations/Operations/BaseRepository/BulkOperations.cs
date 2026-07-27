using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations.Oracle;
using RepoDb.Oracle.BulkOperations;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Entity-typed <see cref="BaseRepository{TEntity, TDbConnection}"/> wrappers for the four Oracle bulk
    /// operations. Each method is a thin pass-through onto <see cref="DbRepository{TDbConnection}"/>'s own
    /// wrapper (see <c>Operations/DbRepository/BulkOperations.cs</c>), which in turn calls the
    /// <see cref="OracleConnection"/> extension methods - matching the three-tier pattern used throughout
    /// the rest of RepoDB. DataTable-based overloads are deliberately not duplicated at this tier or the
    /// <see cref="DbRepository{TDbConnection}"/> tier - a <see cref="System.Data.DataTable"/> isn't tied to
    /// a single entity type, so those calls read more naturally straight off <see cref="OracleConnection"/>.
    /// </summary>
    public static partial class BaseRepositoryExtension
    {
        #region Sync

        /// <summary>
        /// Inserts a list of entities into the database in bulk. Returns the number of inserted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-inserted.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse when <paramref name="identityBehavior"/> is <see cref="OracleBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int BulkInsert<TEntity>(this BaseRepository<TEntity, OracleConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
            where TEntity : class =>
            repository.DbRepository.BulkInsert(ClassMappedNameCache.Get<TEntity>(), entities, mappings, bulkCopyTimeout, identityBehavior, pseudoTableType, transaction);

        /// <summary>
        /// Upserts a list of entities in bulk - inserts new rows and updates existing ones based on the
        /// defined qualifiers (defaults to the primary key). Returns the number of affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int BulkMerge<TEntity>(this BaseRepository<TEntity, OracleConnection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
            where TEntity : class =>
            repository.DbRepository.BulkMerge(ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, bulkCopyTimeout, identityBehavior, pseudoTableType, transaction);

        /// <summary>
        /// Updates existing rows in the database in bulk, matched by the defined qualifiers (defaults to
        /// the primary key). Returns the number of updated rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-updated.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of updated rows.</returns>
        public static int BulkUpdate<TEntity>(this BaseRepository<TEntity, OracleConnection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
            where TEntity : class =>
            repository.DbRepository.BulkUpdate(ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, bulkCopyTimeout, pseudoTableType, transaction);

        /// <summary>
        /// Deletes existing rows from the database in bulk, matched by the defined qualifiers (defaults
        /// to the primary key). Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-deleted.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDelete<TEntity>(this BaseRepository<TEntity, OracleConnection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
            where TEntity : class =>
            repository.DbRepository.BulkDelete(ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, bulkCopyTimeout, pseudoTableType, transaction);

        #endregion

        #region Async

        /// <summary>
        /// Inserts a list of entities into the database in bulk in an asynchronous way. Returns the number
        /// of inserted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-inserted.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse when <paramref name="identityBehavior"/> is <see cref="OracleBulkImportIdentityBehavior.ReturnIdentity"/>.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of inserted rows.</returns>
        public static Task<int> BulkInsertAsync<TEntity>(this BaseRepository<TEntity, OracleConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            repository.DbRepository.BulkInsertAsync(ClassMappedNameCache.Get<TEntity>(), entities, mappings, bulkCopyTimeout, identityBehavior, pseudoTableType, transaction, cancellationToken);

        /// <summary>
        /// Upserts a list of entities in bulk in an asynchronous way - inserts new rows and updates
        /// existing ones based on the defined qualifiers (defaults to the primary key). Returns the number
        /// of affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> BulkMergeAsync<TEntity>(this BaseRepository<TEntity, OracleConnection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            repository.DbRepository.BulkMergeAsync(ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, bulkCopyTimeout, identityBehavior, pseudoTableType, transaction, cancellationToken);

        /// <summary>
        /// Updates existing rows in the database in bulk in an asynchronous way, matched by the defined
        /// qualifiers (defaults to the primary key). Returns the number of updated rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-updated.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of updated rows.</returns>
        public static Task<int> BulkUpdateAsync<TEntity>(this BaseRepository<TEntity, OracleConnection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            repository.DbRepository.BulkUpdateAsync(ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, bulkCopyTimeout, pseudoTableType, transaction, cancellationToken);

        /// <summary>
        /// Deletes existing rows from the database in bulk in an asynchronous way, matched by the defined
        /// qualifiers (defaults to the primary key). Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-deleted.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static Task<int> BulkDeleteAsync<TEntity>(this BaseRepository<TEntity, OracleConnection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            repository.DbRepository.BulkDeleteAsync(ClassMappedNameCache.Get<TEntity>(), entities, qualifiers, bulkCopyTimeout, pseudoTableType, transaction, cancellationToken);

        #endregion
    }
}
