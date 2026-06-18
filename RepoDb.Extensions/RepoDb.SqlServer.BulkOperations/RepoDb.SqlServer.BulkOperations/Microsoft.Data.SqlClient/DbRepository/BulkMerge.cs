using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace RepoDb
{
    /// <summary>
    /// An extension class for <see cref="DbRepository{TDbConnection}"/> object.
    /// </summary>
    public static partial class DbRepositoryExtension
    {
        #region BulkMerge<TEntity>

        /// <summary>
        /// Bulk merge a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge<TEntity>(this DbRepository<SqlConnection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            bool isReturnIdentity = false,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null)
            where TEntity : class
        {
            // Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

            // Call the method
            return bulkDbConnector.Connection.BulkMerge(entities: entities,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk merge a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param> 
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge<TEntity>(this DbRepository<SqlConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            bool isReturnIdentity = false,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null)
            where TEntity : class
        {
            // Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

            // Call the method
            return bulkDbConnector.Connection.BulkMerge(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param> 
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge<TEntity>(this DbRepository<SqlConnection> repository,
            DbDataReader reader,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null)
            where TEntity : class
        {
            // Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

            // Call the method
            return bulkDbConnector.Connection.BulkMerge(reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        #endregion

        #region BulkMerge(TableName)

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param> 
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge(this DbRepository<SqlConnection> repository,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null)
        {
            // Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

            // Call the method
            return bulkDbConnector.Connection.BulkMerge(tableName: tableName,
                reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param> 
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge<TEntity>(this DbRepository<SqlConnection> repository,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            bool isReturnIdentity = false,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null)
            where TEntity : class
        {
            // Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

            // Call the method
            return bulkDbConnector.Connection.BulkMerge<TEntity>(dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param> 
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge(this DbRepository<SqlConnection> repository,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            bool isReturnIdentity = false,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null)
        {
            // Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

            // Call the method
            return bulkDbConnector.Connection.BulkMerge(tableName: tableName,
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        #endregion

        #region BulkMergeAsync<TEntity>

        /// <summary>
        /// Bulk merge a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param> 
        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkMergeAsync<TEntity>(this DbRepository<SqlConnection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            bool isReturnIdentity = false,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

            // Call the method
            return await bulkDbConnector.Connection.BulkMergeAsync(entities: entities,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Bulk merge a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param> 
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkMergeAsync<TEntity>(this DbRepository<SqlConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            bool isReturnIdentity = false,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

            // Call the method
            return await bulkDbConnector.Connection.BulkMergeAsync(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param> 
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkMergeAsync<TEntity>(this DbRepository<SqlConnection> repository,
            DbDataReader reader,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

            // Call the method
            return await bulkDbConnector.Connection.BulkMergeAsync(reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region BulkMergeAsync(TableName)

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param> 
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkMergeAsync(this DbRepository<SqlConnection> repository,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

            // Call the method
            return await bulkDbConnector.Connection.BulkMergeAsync(tableName: tableName,
                reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param> 
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkMergeAsync<TEntity>(this DbRepository<SqlConnection> repository,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            bool isReturnIdentity = false,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

            // Call the method
            return await bulkDbConnector.Connection.BulkMergeAsync<TEntity>(dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param> 
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkMergeAsync(this DbRepository<SqlConnection> repository,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            bool isReturnIdentity = false,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

            // Call the method
            return await bulkDbConnector.Connection.BulkMergeAsync(tableName: tableName,
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
