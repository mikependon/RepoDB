using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations;
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
    /// <see cref="DbRepository{TDbConnection}"/> wrappers for the four Oracle bulk operations. Each method
    /// resolves a connection (reusing the transaction's connection when one is supplied, otherwise creating
    /// one via the repository), delegates to the matching <see cref="OracleConnection"/> extension method,
    /// and disposes the connection afterwards only when the repository owns a per-call connection and no
    /// external transaction was supplied - the same lifecycle every other RepoDB provider's DbRepository
    /// bulk wrapper already follows.
    /// </summary>
    public static partial class DbRepositoryExtension
    {
        #region Sync

        /// <summary>
        /// Inserts a list of entities into the database in bulk. Returns the number of inserted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-inserted.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int BulkInsert<TEntity>(this DbRepository<OracleConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            OracleTransaction transaction = null)
            where TEntity : class
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return connection.BulkInsert(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, mappings, bulkCopyTimeout, identityBehavior, transaction);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        /// <summary>
        /// Upserts a list of entities in bulk - inserts new rows and updates existing ones based on the
        /// defined qualifiers (defaults to the primary key). Returns the number of affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int BulkMerge<TEntity>(this DbRepository<OracleConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
            where TEntity : class
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return connection.BulkMerge(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, qualifiers != null ? Field.Parse(qualifiers) : null, bulkCopyTimeout, identityBehavior, pseudoTableType, transaction);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        /// <summary>
        /// Updates existing rows in the database in bulk, matched by the defined qualifiers (defaults to
        /// the primary key). Returns the number of updated rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-updated.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of updated rows.</returns>
        public static int BulkUpdate<TEntity>(this DbRepository<OracleConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
            where TEntity : class
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return connection.BulkUpdate(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, qualifiers != null ? Field.Parse(qualifiers) : null, bulkCopyTimeout, pseudoTableType, transaction);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        /// <summary>
        /// Deletes existing rows from the database in bulk, matched by the defined qualifiers (defaults
        /// to the primary key). Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-deleted.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDelete<TEntity>(this DbRepository<OracleConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
            where TEntity : class
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return connection.BulkDelete(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, qualifiers != null ? Field.Parse(qualifiers) : null, bulkCopyTimeout, pseudoTableType, transaction);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Inserts a list of entities into the database in bulk in an asynchronous way. Returns the number
        /// of inserted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-inserted.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of inserted rows.</returns>
        public static async Task<int> BulkInsertAsync<TEntity>(this DbRepository<OracleConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return await connection.BulkInsertAsync(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, mappings, bulkCopyTimeout, identityBehavior, transaction, cancellationToken);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        /// <summary>
        /// Upserts a list of entities in bulk in an asynchronous way - inserts new rows and updates
        /// existing ones based on the defined qualifiers (defaults to the primary key). Returns the number
        /// of affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of affected rows.</returns>
        public static async Task<int> BulkMergeAsync<TEntity>(this DbRepository<OracleConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            BulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return await connection.BulkMergeAsync(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, qualifiers != null ? Field.Parse(qualifiers) : null, bulkCopyTimeout, identityBehavior, pseudoTableType, transaction, cancellationToken);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        /// <summary>
        /// Updates existing rows in the database in bulk in an asynchronous way, matched by the defined
        /// qualifiers (defaults to the primary key). Returns the number of updated rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-updated.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of updated rows.</returns>
        public static async Task<int> BulkUpdateAsync<TEntity>(this DbRepository<OracleConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return await connection.BulkUpdateAsync(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, qualifiers != null ? Field.Parse(qualifiers) : null, bulkCopyTimeout, pseudoTableType, transaction, cancellationToken);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        /// <summary>
        /// Deletes existing rows from the database in bulk in an asynchronous way, matched by the defined
        /// qualifiers (defaults to the primary key). Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="repository">The repository object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-deleted.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static async Task<int> BulkDeleteAsync<TEntity>(this DbRepository<OracleConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var connection = transaction?.Connection ?? repository.CreateConnection();

            try
            {
                return await connection.BulkDeleteAsync(tableName ?? ClassMappedNameCache.Get<TEntity>(), entities, qualifiers != null ? Field.Parse(qualifiers) : null, bulkCopyTimeout, pseudoTableType, transaction, cancellationToken);
            }
            finally
            {
                DisposeIfOwned(repository, transaction, connection);
            }
        }

        #endregion

        #region Helpers

        private static void DisposeIfOwned(DbRepository<OracleConnection> repository,
            OracleTransaction transaction,
            OracleConnection connection)
        {
            if (repository.ConnectionPersistency == ConnectionPersistency.PerCall && transaction == null)
            {
                connection.Dispose();
            }
        }

        #endregion
    }
}
