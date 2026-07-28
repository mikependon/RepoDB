using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations.Oracle;
using RepoDb.Oracle.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class OracleConnectionExtension
    {
        #region Sync

        /// <summary>
        /// Merges a list of entities into the database in bulk - inserts new rows and updates existing
        /// ones based on the defined qualifiers (defaults to the primary key). Returns the number of
        /// affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int BulkMerge<TEntity>(this OracleConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
            where TEntity : class =>
            BulkMergeBase(connection, ClassMappedNameCache.Get<TEntity>(), entities, ParseQualifiers(qualifiers), mappings, bulkCopyTimeout, identityBehavior, pseudoTableType, transaction);

        /// <summary>
        /// Merges a list of entities into the database in bulk - inserts new rows and updates existing
        /// ones based on the defined qualifiers (defaults to the primary key). Returns the number of
        /// affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int BulkMerge<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
            where TEntity : class =>
            BulkMergeBase(connection, tableName, entities, qualifiers, mappings, bulkCopyTimeout, identityBehavior, pseudoTableType, transaction);

        /// <summary>
        /// Merges the rows of a <see cref="DataTable"/> into the database in bulk. Returns the number of
        /// affected rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="table">The source <see cref="DataTable"/>.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="rowState">The state of the rows to be included; when null, every row is included.</param>
        /// <param name="mappings">The explicit mapping of the source columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows.</returns>
        public static int BulkMerge(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null) =>
            BulkMergeBase(connection, tableName, table, qualifiers, rowState, mappings, bulkCopyTimeout, identityBehavior, pseudoTableType, transaction);

        #endregion

        #region Async

        /// <summary>
        /// Merges a list of entities into the database in bulk in an asynchronous way - inserts new rows
        /// and updates existing ones based on the defined qualifiers (defaults to the primary key). Returns
        /// the number of affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> BulkMergeAsync<TEntity>(this OracleConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkMergeBaseAsync(connection, ClassMappedNameCache.Get<TEntity>(), entities, ParseQualifiers(qualifiers), mappings, bulkCopyTimeout, identityBehavior, pseudoTableType, transaction, cancellationToken);

        /// <summary>
        /// Merges a list of entities into the database in bulk in an asynchronous way - inserts new rows
        /// and updates existing ones based on the defined qualifiers (defaults to the primary key). Returns
        /// the number of affected rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-merged.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="mappings">The explicit mapping of the source properties/columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> BulkMergeAsync<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkMergeBaseAsync(connection, tableName, entities, qualifiers, mappings, bulkCopyTimeout, identityBehavior, pseudoTableType, transaction, cancellationToken);

        /// <summary>
        /// Merges the rows of a <see cref="DataTable"/> into the database in bulk in an asynchronous way.
        /// Returns the number of affected rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="table">The source <see cref="DataTable"/>.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="rowState">The state of the rows to be included; when null, every row is included.</param>
        /// <param name="mappings">The explicit mapping of the source columns to the destination columns.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column during the operation.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of affected rows.</returns>
        public static Task<int> BulkMergeAsync(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkMergeBaseAsync(connection, tableName, table, qualifiers, rowState, mappings, bulkCopyTimeout, identityBehavior, pseudoTableType, transaction, cancellationToken);

        #endregion

        #region Helpers

        private static IEnumerable<Field> ParseQualifiers<TEntity>(Expression<Func<TEntity, object>> qualifiers)
            where TEntity : class =>
            qualifiers != null ? Field.Parse(qualifiers) : null;

        #endregion
    }
}
