using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.SqlServer.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="SqlConnection"/> object.
    /// </summary>
    public static partial class SqlConnectionExtension
    {
        #region BulkInsert<TEntity>

        /// <summary>
        /// Bulk insert a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this SqlConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return BulkInsertInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                dbFields: null,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk insert a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return BulkInsertInternal(connection: connection,
                tableName: tableName,
                entities: entities,
                dbFields: null,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this SqlConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return BulkInsertInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                reader: reader,
                dbFields: null,
                mappings: mappings,
                options: options,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                transaction: transaction);
        }

        #endregion

        #region BulkInsert(TableName)

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert(this SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null)
        {
            return BulkInsertInternal(connection: connection,
                tableName: tableName,
                reader: reader,
                dbFields: null,
                mappings: mappings,
                options: options,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DataTable"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-insert operation.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this SqlConnection connection,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return BulkInsertInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                dataTable: dataTable,
                rowState: rowState,
                dbFields: null,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DataTable"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-insert operation.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert(this SqlConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
        {
            return BulkInsertInternal(connection: connection,
                tableName: tableName,
                dataTable: dataTable,
                rowState: rowState,
                dbFields: null,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        #endregion

        #region BulkInsertAsync<TEntity>

        /// <summary>
        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkInsertAsync<TEntity>(this SqlConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await BulkInsertAsyncInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                dbFields: null,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkInsertAsync<TEntity>(this SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await BulkInsertAsyncInternal(connection: connection,
                tableName: tableName,
                entities: entities,
                dbFields: null,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkInsertAsync<TEntity>(this SqlConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await BulkInsertAsyncInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                reader: reader,
                dbFields: null,
                mappings: mappings,
                options: options,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region BulkInsertAsync(TableName)

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkInsertAsync(this SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return await BulkInsertAsyncInternal(connection: connection,
                tableName: tableName,
                reader: reader,
                dbFields: null,
                mappings: mappings,
                options: options,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-insert operation.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkInsertAsync<TEntity>(this SqlConnection connection,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await BulkInsertAsyncInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                dataTable: dataTable,
                rowState: rowState,
                dbFields: null,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-insert operation.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkInsertAsync(this SqlConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return await BulkInsertAsyncInternal(connection: connection,
                tableName: tableName,
                dataTable: dataTable,
                rowState: rowState,
                dbFields: null,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region BulkInsertInternal

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static int BulkInsertInternal<TEntity>(SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class =>
            BulkInsertInternalBase<TEntity, SqlBulkCopy, SqlBulkCopyOptions, SqlBulkCopyColumnMappingCollection,
                SqlBulkCopyColumnMapping, SqlTransaction>(connection,
                tableName,
                entities,
                dbFields,
                mappings,
                options.GetValueOrDefault(),
                hints,
                bulkCopyTimeout,
                batchSize,
                isReturnIdentity,
                usePhysicalPseudoTempTable,
                transaction,
                GetRowsCopiedFieldFromSystemDataSqlBulkCopy());

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="dbFields">The list of <see cref="DbField"/> objects.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int BulkInsertInternal(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
        {
            // Variables needed
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = (transaction != null);
            var result = default(int);

            // Check the transaction
            if (transaction == null)
            {
                // Add the transaction if not present
                transaction = (SqlTransaction)connection.EnsureOpen().BeginTransaction();
            }
            else
            {
                // Validate the objects
                SqlConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);
            }

            try
            {
                // Get the DB Fields
                dbFields = dbFields ?? DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var readerFields = Enumerable
                    .Range(0, reader.FieldCount)
                    .Select((index) => reader.GetName(index));
                var fields = dbFields?.Select(dbField => dbField.AsField());

                // Before Execution Time
                var beforeExecutionTime = DateTime.UtcNow;

                // Filter the fields (based on the data reader)
                if (readerFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            readerFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Ensure to have the mappings
                if (mappings == null)
                {
                    mappings = fields?.Select(f =>
                    {
                        var readerField = readerFields.FirstOrDefault(rf =>
                            string.Equals(rf, f.Name, StringComparison.OrdinalIgnoreCase));
                        return new BulkInsertMapItem(readerField ?? f.Name, f.Name);
                    });
                }
                else
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.SourceColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Throw an error if there are no mappings
                if (mappings?.Any() != true)
                {
                    throw new MissingMappingException("There are no mapping(s) found for this operation.");
                }

                // Actual Execution
                using (var sqlBulkCopy = new SqlBulkCopy(connection, options.GetValueOrDefault(), transaction))
                {
                    var withPseudoExecution = (isReturnIdentity == true && identityDbField != null);
                    var tempTableName = (string)null;

                    // Create the temp table if necessary
                    if (withPseudoExecution)
                    {
                        // Must be fixed name so the RepoDb.Core caches will not be bloated
                        tempTableName = string.Concat("_RepoDb_BulkInsert_", GetTableName(tableName, dbSetting));

                        // Add a # prefix if not physical
                        if (usePhysicalPseudoTempTable != true)
                        {
                            tempTableName = string.Concat("#", tempTableName);
                        }

                        // Create a temporary table
                        var sql = GetCreateTemporaryTableSqlText(tableName,
                            tempTableName,
                            fields,
                            dbSetting);
                        connection.ExecuteNonQuery(sql, transaction: transaction);
                    }

                    // Set the destinationtable
                    sqlBulkCopy.DestinationTableName = (tempTableName ?? tableName);

                    // Set the timeout
                    if (bulkCopyTimeout != null && bulkCopyTimeout.HasValue)
                    {
                        sqlBulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
                    }

                    // Set the batch szie
                    if (batchSize != null && batchSize.HasValue)
                    {
                        sqlBulkCopy.BatchSize = batchSize.Value;
                    }

                    // Add the mappings
                    mappings
                        .AsList()
                        .ForEach(mapItem =>
                            sqlBulkCopy.ColumnMappings.Add(mapItem.SourceColumn, mapItem.DestinationColumn));

                    // Open the connection and do the operation
                    connection.EnsureOpen();
                    sqlBulkCopy.WriteToServer(reader);

                    // Check if this is with pseudo
                    if (withPseudoExecution)
                    {
                        // Merge the actual merge
                        var sql = GetBulkInsertSqlText(tableName,
                            tempTableName,
                            fields,
                            identityDbField?.AsField(),
                            hints,
                            dbSetting);

                        // Execute the SQL
                        result = connection.ExecuteNonQuery(sql, commandTimeout: bulkCopyTimeout, transaction: transaction);

                        // Drop the table after used
                        sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                        connection.ExecuteNonQuery(sql, transaction: transaction);
                    }
                    else
                    {
                        // Hack the 'SqlBulkCopy' object
                        var copiedField = GetRowsCopiedFieldFromSystemDataSqlBulkCopy();

                        // Set the return value
                        result = copiedField != null ? (int)copiedField.GetValue(sqlBulkCopy) : reader.RecordsAffected;
                    }
                }

                // Commit the transaction
                if (hasTransaction == false)
                {
                    transaction?.Commit();
                }
            }
            catch
            {
                // Rollback the transaction
                if (hasTransaction == false)
                {
                    transaction?.Rollback();
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose the transaction
                if (hasTransaction == false)
                {
                    transaction?.Dispose();
                }
            }

            // Return the result
            return result;
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DataTable"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-insert operation.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="dbFields">The list of <see cref="DbField"/> objects.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int BulkInsertInternal(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
        {
            // Variables needed
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = (transaction != null);
            var result = default(int);

            // Check the transaction
            if (transaction == null)
            {
                // Add the transaction if not present
                transaction = (SqlTransaction)(connection.EnsureOpen()).BeginTransaction();
            }
            else
            {
                // Validate the objects
                SqlConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);
            }

            try
            {
                // Get the DB Fields
                dbFields = dbFields ?? DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var tableFields = GetDataColumns(dataTable)
                    .Select(column => column.ColumnName);
                var fields = dbFields?.Select(dbField => dbField.AsField());

                // Before Execution Time
                var beforeExecutionTime = DateTime.UtcNow;

                // Filter the fields (based on the data reader)
                if (tableFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            tableFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Ensure to have the mappings
                if (mappings == null)
                {
                    mappings = fields?.Select(f =>
                    {
                        var readerField = tableFields.FirstOrDefault(rf =>
                            string.Equals(rf, f.Name, StringComparison.OrdinalIgnoreCase));
                        return new BulkInsertMapItem(readerField ?? f.Name, f.Name);
                    });
                }
                else
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.SourceColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Throw an error if there are no mappings
                if (mappings?.Any() != true)
                {
                    throw new MissingMappingException("There are no mapping(s) found for this operation.");
                }

                // Actual Execution
                using (var sqlBulkCopy = new SqlBulkCopy(connection, options.GetValueOrDefault(), transaction))
                {
                    var withPseudoExecution = (isReturnIdentity == true && identityDbField != null);
                    var tempTableName = (string)null;
                    var sql = (string)null;

                    // Create the temp table if necessary
                    if (withPseudoExecution)
                    {
                        // Must be fixed name so the RepoDb.Core caches will not be bloated
                        tempTableName = string.Concat("_RepoDb_BulkInsert_", GetTableName(tableName, dbSetting));

                        // Add a # prefix if not physical
                        if (usePhysicalPseudoTempTable != true)
                        {
                            tempTableName = string.Concat("#", tempTableName);
                        }

                        // Create a temporary table
                        sql = GetCreateTemporaryTableSqlText(tableName,
                           tempTableName,
                           fields,
                           dbSetting);
                        connection.ExecuteNonQuery(sql, transaction: transaction);
                    }

                    // Set the destinationtable
                    sqlBulkCopy.DestinationTableName = (tempTableName ?? tableName);

                    // Set the timeout
                    if (bulkCopyTimeout != null && bulkCopyTimeout.HasValue)
                    {
                        sqlBulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
                    }

                    // Set the batch szie
                    if (batchSize != null && batchSize.HasValue)
                    {
                        sqlBulkCopy.BatchSize = batchSize.Value;
                    }

                    // Add the mappings
                    mappings
                        .AsList()
                        .ForEach(mapItem =>
                            sqlBulkCopy.ColumnMappings.Add(mapItem.SourceColumn, mapItem.DestinationColumn));

                    // Open the connection and do the operation
                    connection.EnsureOpen();
                    if (rowState.HasValue == true)
                    {
                        sqlBulkCopy.WriteToServer(dataTable, rowState.Value);
                    }
                    else
                    {
                        sqlBulkCopy.WriteToServer(dataTable);
                    }

                    // Check if this is with pseudo
                    if (withPseudoExecution)
                    {
                        var column = dataTable.Columns[identityDbField.Name];

                        if (isReturnIdentity == true)
                        {
                            sql = GetBulkInsertSqlText(tableName,
                                tempTableName,
                                fields,
                                identityDbField?.AsField(),
                                hints,
                                dbSetting);

                            // Identify the column
                            if (column?.ReadOnly == false)
                            {
                                using (var reader = (DbDataReader)(connection.ExecuteReader(sql, commandTimeout: bulkCopyTimeout, transaction: transaction)))
                                {
                                    while (reader.Read())
                                    {
                                        var value = Converter.DbNullToNull(reader.GetFieldValue<object>(0));
                                        dataTable.Rows[result][column] = value;
                                        result++;
                                    }
                                }
                            }
                            else
                            {
                                result = connection.ExecuteNonQuery(sql, commandTimeout: bulkCopyTimeout, transaction: transaction);
                            }

                            // Drop the table after used
                            sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                            connection.ExecuteNonQuery(sql, transaction: transaction);
                        }
                    }
                    else
                    {
                        // Get the result from the table rows count
                        result = GetDataRows(dataTable, rowState).Count();
                    }
                }

                // Commit the transaction
                if (hasTransaction == false)
                {
                    transaction?.Commit();
                }

                // Return the result
                return result;
            }
            catch
            {
                // Rollback the transaction
                if (hasTransaction == false)
                {
                    transaction?.Rollback();
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose the transaction
                if (hasTransaction == false)
                {
                    transaction?.Dispose();
                }
            }
        }

        #endregion

        #region BulkInsertAsyncInternal

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static Task<int> BulkInsertAsyncInternal<TEntity>(SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkInsertAsyncInternalBase<TEntity, SqlBulkCopy, SqlBulkCopyOptions, SqlBulkCopyColumnMappingCollection,
                SqlBulkCopyColumnMapping, SqlTransaction>(connection,
                tableName,
                entities,
                dbFields,
                mappings,
                options.GetValueOrDefault(),
                hints,
                bulkCopyTimeout,
                batchSize,
                isReturnIdentity,
                usePhysicalPseudoTempTable,
                transaction,
                cancellationToken,
                GetRowsCopiedFieldFromSystemDataSqlBulkCopy());

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="dbFields">The list of <see cref="DbField"/> objects.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static async Task<int> BulkInsertAsyncInternal(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Variables needed
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = (transaction != null);
            var result = default(int);

            // Check the transaction
            if (transaction == null)
            {
                // Add the transaction if not present
                transaction = (SqlTransaction)(await connection.EnsureOpenAsync(cancellationToken)).BeginTransaction();
            }
            else
            {
                // Validate the objects
                SqlConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);
            }

            try
            {
                // Get the DB Fields
                dbFields = dbFields ?? await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken);

                // Variables needed
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var readerFields = Enumerable
                    .Range(0, reader.FieldCount)
                    .Select((index) => reader.GetName(index));
                var fields = dbFields?.Select(dbField => dbField.AsField());

                // Before Execution Time
                var beforeExecutionTime = DateTime.UtcNow;

                // Filter the fields (based on the data reader)
                if (readerFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            readerFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Ensure to have the mappings
                if (mappings == null)
                {
                    mappings = fields?.Select(f =>
                    {
                        var readerField = readerFields.FirstOrDefault(rf =>
                            string.Equals(rf, f.Name, StringComparison.OrdinalIgnoreCase));
                        return new BulkInsertMapItem(readerField ?? f.Name, f.Name);
                    });
                }
                else
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.SourceColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Throw an error if there are no mappings
                if (mappings?.Any() != true)
                {
                    throw new MissingMappingException("There are no mapping(s) found for this operation.");
                }

                // Actual Execution
                using (var sqlBulkCopy = new SqlBulkCopy(connection, options.GetValueOrDefault(), transaction))
                {
                    var withPseudoExecution = (isReturnIdentity == true && identityDbField != null);
                    var tempTableName = (string)null;

                    // Create the temp table if necessary
                    if (withPseudoExecution)
                    {
                        // Must be fixed name so the RepoDb.Core caches will not be bloated
                        tempTableName = string.Concat("_RepoDb_BulkInsert_", GetTableName(tableName, dbSetting));

                        // Add a # prefix if not physical
                        if (usePhysicalPseudoTempTable != true)
                        {
                            tempTableName = string.Concat("#", tempTableName);
                        }

                        // Create a temporary table
                        var sql = GetCreateTemporaryTableSqlText(tableName,
                            tempTableName,
                            fields,
                            dbSetting);
                        await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);
                    }

                    // Set the destinationtable
                    sqlBulkCopy.DestinationTableName = (tempTableName ?? tableName);

                    // Set the timeout
                    if (bulkCopyTimeout != null && bulkCopyTimeout.HasValue)
                    {
                        sqlBulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
                    }

                    // Set the batch szie
                    if (batchSize != null && batchSize.HasValue)
                    {
                        sqlBulkCopy.BatchSize = batchSize.Value;
                    }

                    // Add the mappings
                    mappings
                        .AsList()
                        .ForEach(mapItem =>
                            sqlBulkCopy.ColumnMappings.Add(mapItem.SourceColumn, mapItem.DestinationColumn));

                    // Open the connection and do the operation
                    await connection.EnsureOpenAsync(cancellationToken);
                    await sqlBulkCopy.WriteToServerAsync(reader, cancellationToken);

                    // Check if this is with pseudo
                    if (withPseudoExecution)
                    {
                        // Merge the actual merge
                        var sql = GetBulkInsertSqlText(tableName,
                            tempTableName,
                            fields,
                            identityDbField?.AsField(),
                            hints,
                            dbSetting);

                        // Execute the SQL
                        result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken);

                        // Drop the table after used
                        sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                        await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);
                    }
                    else
                    {
                        // Hack the 'SqlBulkCopy' object
                        var copiedField = GetRowsCopiedFieldFromSystemDataSqlBulkCopy();

                        // Set the return value
                        result = copiedField != null ? (int)copiedField.GetValue(sqlBulkCopy) : reader.RecordsAffected;
                    }
                }

                // Commit the transaction
                if (hasTransaction == false)
                {
                    transaction?.Commit();
                }
            }
            catch
            {
                // Rollback the transaction
                if (hasTransaction == false)
                {
                    transaction?.Rollback();
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose the transaction
                if (hasTransaction == false)
                {
                    transaction?.Dispose();
                }
            }

            // Return the result
            return result;
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-insert operation.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="dbFields">The list of <see cref="DbField"/> objects.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static async Task<int> BulkInsertAsyncInternal(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Variables needed
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = (transaction != null);
            var result = default(int);

            // Check the transaction
            if (transaction == null)
            {
                // Add the transaction if not present
                transaction = (SqlTransaction)(await connection.EnsureOpenAsync(cancellationToken)).BeginTransaction();
            }
            else
            {
                // Validate the objects
                SqlConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);
            }

            try
            {
                // Get the DB Fields
                dbFields = dbFields ?? await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken);

                // Variables needed
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var tableFields = GetDataColumns(dataTable)
                    .Select(column => column.ColumnName);
                var fields = dbFields?.Select(dbField => dbField.AsField());

                // Before Execution Time
                var beforeExecutionTime = DateTime.UtcNow;

                // Filter the fields (based on the data reader)
                if (tableFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            tableFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Ensure to have the mappings
                if (mappings == null)
                {
                    mappings = fields?.Select(f =>
                    {
                        var readerField = tableFields.FirstOrDefault(rf =>
                            string.Equals(rf, f.Name, StringComparison.OrdinalIgnoreCase));
                        return new BulkInsertMapItem(readerField ?? f.Name, f.Name);
                    });
                }
                else
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.SourceColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Throw an error if there are no mappings
                if (mappings?.Any() != true)
                {
                    throw new MissingMappingException("There are no mapping(s) found for this operation.");
                }

                // Actual Execution
                using (var sqlBulkCopy = new SqlBulkCopy(connection, options.GetValueOrDefault(), transaction))
                {
                    var withPseudoExecution = (isReturnIdentity == true && identityDbField != null);
                    var tempTableName = (string)null;
                    var sql = (string)null;

                    // Create the temp table if necessary
                    if (withPseudoExecution)
                    {
                        // Must be fixed name so the RepoDb.Core caches will not be bloated
                        tempTableName = string.Concat("_RepoDb_BulkInsert_", GetTableName(tableName, dbSetting));

                        // Add a # prefix if not physical
                        if (usePhysicalPseudoTempTable != true)
                        {
                            tempTableName = string.Concat("#", tempTableName);
                        }

                        // Create a temporary table
                        sql = GetCreateTemporaryTableSqlText(tableName,
                           tempTableName,
                           fields,
                           dbSetting);
                        await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);
                    }

                    // Set the destinationtable
                    sqlBulkCopy.DestinationTableName = (tempTableName ?? tableName);

                    // Set the timeout
                    if (bulkCopyTimeout != null && bulkCopyTimeout.HasValue)
                    {
                        sqlBulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
                    }

                    // Set the batch szie
                    if (batchSize != null && batchSize.HasValue)
                    {
                        sqlBulkCopy.BatchSize = batchSize.Value;
                    }

                    // Add the mappings
                    mappings
                        .AsList()
                        .ForEach(mapItem =>
                            sqlBulkCopy.ColumnMappings.Add(mapItem.SourceColumn, mapItem.DestinationColumn));

                    // Open the connection and do the operation
                    await connection.EnsureOpenAsync(cancellationToken);
                    if (rowState.HasValue == true)
                    {
                        await sqlBulkCopy.WriteToServerAsync(dataTable, rowState.Value, cancellationToken);
                    }
                    else
                    {
                        await sqlBulkCopy.WriteToServerAsync(dataTable, cancellationToken);
                    }

                    // Check if this is with pseudo
                    if (withPseudoExecution)
                    {
                        var column = dataTable.Columns[identityDbField.Name];

                        if (isReturnIdentity == true)
                        {
                            sql = GetBulkInsertSqlText(tableName,
                                tempTableName,
                                fields,
                                identityDbField?.AsField(),
                                hints,
                                dbSetting);

                            // Identify the column
                            if (column?.ReadOnly == false)
                            {
                                using (var reader = (DbDataReader)(await connection.ExecuteReaderAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken)))
                                {
                                    while (await reader.ReadAsync(cancellationToken))
                                    {
                                        var value = Converter.DbNullToNull(await reader.GetFieldValueAsync<object>(0, cancellationToken));
                                        dataTable.Rows[result][column] = value;
                                        result++;
                                    }
                                }
                            }
                            else
                            {
                                result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken);
                            }

                            // Drop the table after used
                            sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                            await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);
                        }
                    }
                    else
                    {
                        // Get the result from the table rows count
                        result = GetDataRows(dataTable, rowState).Count();
                    }
                }

                // Commit the transaction
                if (hasTransaction == false)
                {
                    transaction?.Commit();
                }

                // Return the result
                return result;
            }
            catch
            {
                // Rollback the transaction
                if (hasTransaction == false)
                {
                    transaction?.Rollback();
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose the transaction
                if (hasTransaction == false)
                {
                    transaction?.Dispose();
                }
            }
        }

        #endregion
    }
}
