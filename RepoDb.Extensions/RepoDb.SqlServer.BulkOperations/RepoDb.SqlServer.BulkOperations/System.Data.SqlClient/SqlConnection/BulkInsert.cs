using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
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
        public static Task<int> BulkInsertAsync<TEntity>(this SqlConnection connection,
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
            return BulkInsertAsyncInternal(connection: connection,
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
        public static Task<int> BulkInsertAsync<TEntity>(this SqlConnection connection,
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
            return BulkInsertAsyncInternal(connection: connection,
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
        public static Task<int> BulkInsertAsync<TEntity>(this SqlConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return BulkInsertAsyncInternal(connection: connection,
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
        public static Task<int> BulkInsertAsync(this SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return BulkInsertAsyncInternal(connection: connection,
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
        public static Task<int> BulkInsertAsync<TEntity>(this SqlConnection connection,
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
            return BulkInsertAsyncInternal(connection: connection,
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
        public static Task<int> BulkInsertAsync(this SqlConnection connection,
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
            return BulkInsertAsyncInternal(connection: connection,
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
                transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="dbFields"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static int BulkInsertInternal(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null) =>
            BulkInsertInternalBase<SqlBulkCopy, SqlBulkCopyOptions, SqlBulkCopyColumnMappingCollection,
                SqlBulkCopyColumnMapping, SqlTransaction>(connection,
                tableName,
                reader,
                dbFields,
                mappings,
                options.GetValueOrDefault(),
                bulkCopyTimeout,
                batchSize,
                transaction);

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
            SqlTransaction transaction = null) =>
            BulkInsertInternalBase<SqlBulkCopy, SqlBulkCopyOptions, SqlBulkCopyColumnMappingCollection,
                SqlBulkCopyColumnMapping, SqlTransaction>(connection,
                tableName,
                dataTable,
                rowState,
                dbFields,
                mappings,
                options.GetValueOrDefault(),
                hints,
                bulkCopyTimeout,
                batchSize,
                isReturnIdentity,
                usePhysicalPseudoTempTable,
                transaction);

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
            where TEntity : class => BulkInsertAsyncInternalBase<TEntity, SqlBulkCopy, SqlBulkCopyOptions, SqlBulkCopyColumnMappingCollection,
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
                cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="dbFields"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static Task<int> BulkInsertAsyncInternal(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkInsertAsyncInternalBase<SqlBulkCopy, SqlBulkCopyOptions, SqlBulkCopyColumnMappingCollection,
                SqlBulkCopyColumnMapping, SqlTransaction>(connection,
                tableName,
                reader,
                dbFields,
                mappings,
                options.GetValueOrDefault(),
                bulkCopyTimeout,
                batchSize,
                transaction,
                cancellationToken);

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
        internal static Task<int> BulkInsertAsyncInternal(SqlConnection connection,
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
            CancellationToken cancellationToken = default) =>
            BulkInsertAsyncInternalBase<SqlBulkCopy, SqlBulkCopyOptions, SqlBulkCopyColumnMappingCollection,
                SqlBulkCopyColumnMapping, SqlTransaction>(connection,
                tableName,
                dataTable,
                rowState,
                dbFields,
                mappings,
                options.GetValueOrDefault(),
                hints,
                bulkCopyTimeout,
                batchSize,
                isReturnIdentity,
                usePhysicalPseudoTempTable,
                transaction,
                cancellationToken);

        #endregion
    }
}
