using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="SqlConnection"/> object.
    /// </summary>
    public static partial class SqlConnectionExtension
    {
        #region BulkUpdate<TEntity>

        /// <summary>
        /// Bulk update a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-updated.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkUpdate<TEntity>(this SqlConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            using (var reader = new DataEntityDataReader<TEntity>(entities))
            {
                return BulkUpdateInternal(connection: connection,
                    tableName: ClassMappedNameCache.Get<TEntity>(),
                    reader: reader,
                    qualifiers: ParseExpression(qualifiers),
                    mappings: mappings,
                    options: options,
                    hints: hints,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                    transaction: transaction);
            }
        }

        /// <summary>
        /// Bulk update a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-updated.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkUpdate<TEntity>(this SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            using (var reader = new DataEntityDataReader<TEntity>(entities))
            {
                return BulkUpdateInternal(connection: connection,
                    tableName: tableName,
                    reader: reader,
                    qualifiers: ParseExpression(qualifiers),
                    mappings: mappings,
                    options: options,
                    hints: hints,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                    transaction: transaction);
            }
        }

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkUpdate<TEntity>(this SqlConnection connection,
            DbDataReader reader,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return BulkUpdateInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                reader: reader,
                qualifiers: ParseExpression(qualifiers),
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        #endregion

        #region BulkUpdate(TableName)

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkUpdate(this SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
        {
            return BulkUpdateInternal(connection: connection,
                tableName: tableName,
                reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk update an instance of <see cref="DataTable"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkUpdate<TEntity>(this SqlConnection connection,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return BulkUpdateInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk update an instance of <see cref="DataTable"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkUpdate(this SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
        {
            return BulkUpdateInternal(connection: connection,
                tableName: tableName,
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        #endregion

        #region BulkUpdateAsync<TEntity>

        /// <summary>
        /// Bulk update a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-updated.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkUpdateAsync<TEntity>(this SqlConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            using (var reader = new DataEntityDataReader<TEntity>(entities))
            {
                return await BulkUpdateAsyncInternal(connection: connection,
                    tableName: ClassMappedNameCache.Get<TEntity>(),
                    reader: reader,
                    qualifiers: ParseExpression(qualifiers),
                    mappings: mappings,
                    options: options,
                    hints: hints,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                    transaction: transaction,
                    cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Bulk update a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-updated.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkUpdateAsync<TEntity>(this SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            using (var reader = new DataEntityDataReader<TEntity>(entities))
            {
                return await BulkUpdateAsyncInternal(connection: connection,
                    tableName: tableName,
                    reader: reader,
                    qualifiers: ParseExpression(qualifiers),
                    mappings: mappings,
                    options: options,
                    hints: hints,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                    transaction: transaction,
                    cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkUpdateAsync<TEntity>(this SqlConnection connection,
            DbDataReader reader,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return BulkUpdateAsyncInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                reader: reader,
                qualifiers: ParseExpression(qualifiers),
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region BulkUpdateAsync(TableName)

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkUpdateAsync(this SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return BulkUpdateAsyncInternal(connection: connection,
                tableName: tableName,
                reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Bulk update an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkUpdateAsync<TEntity>(this SqlConnection connection,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return BulkUpdateAsyncInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Bulk update an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkUpdateAsync(this SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return BulkUpdateAsyncInternal(connection: connection,
                tableName: tableName,
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region BulkUpdateInternal

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static int BulkUpdateInternal(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null) =>
            BulkUpdateInternalBase<SqlBulkCopy, SqlBulkCopyOptions, SqlBulkCopyColumnMappingCollection,
                SqlBulkCopyColumnMapping, SqlTransaction>(connection,
                tableName,
                reader,
                qualifiers,
                mappings,
                options.GetValueOrDefault(),
                hints,
                bulkCopyTimeout,
                batchSize,
                usePhysicalPseudoTempTable,
                transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dataTable"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static int BulkUpdateInternal(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null) =>
            BulkUpdateInternalBase<SqlBulkCopy, SqlBulkCopyOptions, SqlBulkCopyColumnMappingCollection,
                SqlBulkCopyColumnMapping, SqlTransaction>(connection,
                tableName,
                dataTable,
                qualifiers,
                rowState,
                mappings,
                options.GetValueOrDefault(),
                hints,
                bulkCopyTimeout,
                batchSize,
                usePhysicalPseudoTempTable,
                transaction);

        #endregion

        #region BulkUpdateAsyncInternal

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static Task<int> BulkUpdateAsyncInternal(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkUpdateAsyncInternalBase<SqlBulkCopy, SqlBulkCopyOptions, SqlBulkCopyColumnMappingCollection,
                SqlBulkCopyColumnMapping, SqlTransaction>(connection,
                tableName,
                reader,
                qualifiers,
                mappings,
                options.GetValueOrDefault(),
                hints,
                bulkCopyTimeout,
                batchSize,
                usePhysicalPseudoTempTable,
                transaction,
                cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dataTable"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static Task<int> BulkUpdateAsyncInternal(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkUpdateAsyncInternalBase<SqlBulkCopy, SqlBulkCopyOptions, SqlBulkCopyColumnMappingCollection,
                SqlBulkCopyColumnMapping, SqlTransaction>(connection,
                tableName,
                dataTable,
                qualifiers,
                rowState,
                mappings,
                options.GetValueOrDefault(),
                hints,
                bulkCopyTimeout,
                batchSize,
                usePhysicalPseudoTempTable,
                transaction,
                cancellationToken);

        #endregion
    }
}
