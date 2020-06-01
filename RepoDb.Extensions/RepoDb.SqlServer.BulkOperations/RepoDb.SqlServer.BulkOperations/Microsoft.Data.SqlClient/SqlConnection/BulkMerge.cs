using Microsoft.Data.SqlClient;
using RepoDb.Extensions;
using RepoDb.SqlServer.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="SqlConnection"/> object.
    /// </summary>
    public static partial class SqlConnectionExtension
    {
        #region BulkMerge<TEntity>

        /// <summary>
        /// Bulk merge a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge<TEntity>(this SqlConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
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
            using (var reader = new DataEntityDataReader<TEntity>(entities))
            {
                var result = BulkMergeInternal(connection: connection,
                    tableName: ClassMappedNameCache.Get<TEntity>(),
                    reader: reader,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    options: options,
                    hints: hints,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    isReturnIdentity: isReturnIdentity,
                    usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                    transaction: transaction);
                if (isReturnIdentity == true)
                {
                    SetIdentities<TEntity>(entities, result);
                }
                return result.Count;
            }
        }

        /// <summary>
        /// Bulk merge a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge<TEntity>(this SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
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
            using (var reader = new DataEntityDataReader<TEntity>(entities))
            {
                var result = BulkMergeInternal(connection: connection,
                    tableName: tableName,
                    reader: reader,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    options: options,
                    hints: hints,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    isReturnIdentity: isReturnIdentity,
                    usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                    transaction: transaction);
                if (isReturnIdentity == true)
                {
                    SetIdentities<TEntity>(entities, result);
                }
                return result.Count;
            }
        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge<TEntity>(this SqlConnection connection,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return BulkMergeInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: false,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction).Count;
        }

        #endregion

        #region BulkMerge(TableName)

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge(this SqlConnection connection,
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
            return BulkMergeInternal(connection: connection,
                tableName: tableName,
                reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: false,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction).Count;
        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DataTable"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge<TEntity>(this SqlConnection connection,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
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
            return BulkMergeInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
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
        /// Bulk merge an instance of <see cref="DataTable"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge(this SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
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
            return BulkMergeInternal(connection: connection,
                tableName: tableName,
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
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

        #region BulkMergeAsync<TEntity>

        /// <summary>
        /// Bulk merge a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkMergeAsync<TEntity>(this SqlConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
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
            using (var reader = new DataEntityDataReader<TEntity>(entities))
            {
                var result = await BulkMergeAsyncInternal(connection: connection,
                    tableName: ClassMappedNameCache.Get<TEntity>(),
                    reader: reader,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    options: options,
                    hints: hints,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    isReturnIdentity: isReturnIdentity,
                    usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                    transaction: transaction);
                if (isReturnIdentity == true)
                {
                    SetIdentities<TEntity>(entities, result);
                }
                return result.Count;
            }
        }

        /// <summary>
        /// Bulk merge a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkMergeAsync<TEntity>(this SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
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
            using (var reader = new DataEntityDataReader<TEntity>(entities))
            {
                var result = await BulkMergeAsyncInternal(connection: connection,
                    tableName: tableName,
                    reader: reader,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    options: options,
                    hints: hints,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    isReturnIdentity: isReturnIdentity,
                    usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                    transaction: transaction);
                if (isReturnIdentity == true)
                {
                    SetIdentities<TEntity>(entities, result);
                }
                return result.Count;
            }
        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkMergeAsync<TEntity>(this SqlConnection connection,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return (await BulkMergeAsyncInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: false,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction)).Count;
        }

        #endregion

        #region BulkMergeAsync(TableName)

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkMergeAsync(this SqlConnection connection,
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
            return (await BulkMergeAsyncInternal(connection: connection,
                tableName: tableName,
                reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: false,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction)).Count;
        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkMergeAsync<TEntity>(this SqlConnection connection,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
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
            return await BulkMergeAsyncInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
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
        /// Bulk merge an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkMergeAsync(this SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
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
            return await BulkMergeAsyncInternal(connection: connection,
                tableName: tableName,
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
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

        #region BulkMergeInternal

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The instance of <see cref="BulkOperationIdentitiesResult"/> object.</returns>
        internal static BulkOperationIdentitiesResult BulkMergeInternal(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
        {
            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = (transaction != null);
            var count = default(int);
            var identities = new List<object>();

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

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Must be fixed name so the RepoDb.Core caches will not be bloated
            var tempTableName = string.Concat("_RepoDb_BulkMerge_", GetTableName(tableName, dbSetting));

            // Add a # prefix if not physical
            if (usePhysicalPseudoTempTable != true)
            {
                tempTableName = string.Concat("#", tempTableName);
            }

            try
            {
                // Get the DB Fields
                var dbFields = DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var readerFields = Enumerable.Range(0, reader.FieldCount)
                    .Select((index) => reader.GetName(index));
                var fields = dbFields?.Select(dbField => dbField.AsField());
                var primaryDbField = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var primaryOrIdentityDbField = (primaryDbField ?? identityDbField);

                // Validate the primary keys
                if (qualifiers?.Any() != true)
                {
                    if (primaryOrIdentityDbField == null)
                    {
                        throw new MissingPrimaryKeyException($"No primary key or identity key found for table '{tableName}'.");
                    }
                    else
                    {
                        qualifiers = new[] { primaryOrIdentityDbField.AsField() };
                    }
                }

                // Filter the fields (based on the data reader)
                if (readerFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            readerFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Filter the fields (based on the mappings)
                if (mappings?.Any() == true)
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

                // Create a temporary table
                var sql = GetCreateTemporaryTableSqlText(tableName,
                    tempTableName,
                    fields,
                    dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

                // Set the options to KeepIdentity if needed
                if (options == null &&
                    identityDbField?.IsIdentity == true &&
                    qualifiers?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true &&
                    fields?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true)
                {
                    options = SqlBulkCopyOptions.KeepIdentity;
                }

                // Filter the DB Fields
                var filteredDbFields = dbFields?
                    .Where(dbField =>
                        fields?.Any(field => string.Equals(field.Name, dbField.Name, StringComparison.OrdinalIgnoreCase)) == true);

                // Do the bulk insertion first
                BulkInsertInternal(connection,
                    tempTableName,
                    reader,
                    filteredDbFields,
                    mappings,
                    options,
                    null,
                    bulkCopyTimeout,
                    batchSize,
                    false,
                    false,
                    transaction);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

                // Merge the actual merge
                sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    isReturnIdentity.GetValueOrDefault());

                // Identity if the identity is to return
                if (isReturnIdentity != true)
                {
                    count = connection.ExecuteNonQuery(sql, transaction: transaction);
                }
                else
                {
                    using (var dataReader = (DbDataReader)connection.ExecuteReader(sql, transaction: transaction))
                    {
                        while (dataReader.Read())
                        {
                            var value = Converter.DbNullToNull(dataReader.GetFieldValue<object>(0));
                            identities.Add(value);
                            count++;
                        }
                    }
                }

                // Drop the table after used
                sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

                // Commit the transaction
                if (hasTransaction == false)
                {
                    transaction?.Commit();
                }

                // Result
                return new BulkOperationIdentitiesResult(count, identityDbField?.Name, identities);
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

        /// <summary>
        /// Bulk merge an instance of <see cref="DataTable"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int BulkMergeInternal(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
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
            // Variables
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

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Must be fixed name so the RepoDb.Core caches will not be bloated
            var tempTableName = string.Concat("_RepoDb_BulkMerge_", GetTableName(tableName, dbSetting));

            // Add a # prefix if not physical
            if (usePhysicalPseudoTempTable != true)
            {
                tempTableName = string.Concat("#", tempTableName);
            }

            try
            {
                // Get the DB Fields
                var dbFields = DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var tableFields = Enumerable.Range(0, dataTable.Columns.Count)
                    .Select((index) => dataTable.Columns[index].ColumnName);
                var fields = dbFields?.Select(dbField => dbField.AsField());
                var primaryDbField = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var primaryOrIdentityDbField = (primaryDbField ?? identityDbField);

                // Validate the primary keys
                if (qualifiers?.Any() != true)
                {
                    if (primaryOrIdentityDbField == null)
                    {
                        throw new MissingPrimaryKeyException($"No primary key or identity key found for table '{tableName}'.");
                    }
                    else
                    {
                        qualifiers = new[] { primaryOrIdentityDbField.AsField() };
                    }
                }

                // Filter the fields (based on the data table)
                if (tableFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            tableFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Filter the fields (based on the mappings)
                if (mappings?.Any() == true)
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

                // Create a temporary table
                var sql = GetCreateTemporaryTableSqlText(tableName,
                    tempTableName,
                    fields,
                    dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

                // Set the options to KeepIdentity if needed
                if (options == null &&
                    identityDbField?.IsIdentity == true &&
                    qualifiers?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true &&
                    fields?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true)
                {
                    options = SqlBulkCopyOptions.KeepIdentity;
                }

                // Filter the DB Fields
                var filteredDbFields = dbFields?
                    .Where(dbField =>
                        fields?.Any(field => string.Equals(field.Name, dbField.Name, StringComparison.OrdinalIgnoreCase)) == true);

                // Do the bulk insertion first
                BulkInsertInternal(connection,
                    tempTableName,
                    dataTable,
                    rowState,
                    filteredDbFields,
                    mappings,
                    options,
                    null,
                    bulkCopyTimeout,
                    batchSize,
                    false,
                    false,
                    transaction);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

                // Merge the actual merge
                sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    isReturnIdentity.GetValueOrDefault());

                // Identity if the identity is to return
                var column = dataTable.Columns[identityDbField.Name];
                if (isReturnIdentity == true && column?.ReadOnly == false)
                {
                    using (var reader = (DbDataReader)connection.ExecuteReader(sql, transaction: transaction))
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
                    result = connection.ExecuteNonQuery(sql, transaction: transaction);
                }

                // Drop the table after used
                sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

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

            // Result
            return result;
        }

        #endregion

        #region BulkMergeAsyncInternal

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The instance of <see cref="BulkOperationIdentitiesResult"/> object.</returns>
        internal static async Task<BulkOperationIdentitiesResult> BulkMergeAsyncInternal(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
        {
            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = (transaction != null);
            var count = default(int);
            var identities = new List<object>();

            // Check the transaction
            if (transaction == null)
            {
                // Add the transaction if not present
                transaction = (SqlTransaction)(await connection.EnsureOpenAsync()).BeginTransaction();
            }
            else
            {
                // Validate the objects
                SqlConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Must be fixed name so the RepoDb.Core caches will not be bloated
            var tempTableName = string.Concat("_RepoDb_BulkMerge_", GetTableName(tableName, dbSetting));

            // Add a # prefix if not physical
            if (usePhysicalPseudoTempTable != true)
            {
                tempTableName = string.Concat("#", tempTableName);
            }

            try
            {
                // Get the DB Fields
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, true);

                // Variables needed
                var readerFields = Enumerable.Range(0, reader.FieldCount)
                    .Select((index) => reader.GetName(index));
                var fields = dbFields?.Select(dbField => dbField.AsField());
                var primaryDbField = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var primaryOrIdentityDbField = (primaryDbField ?? identityDbField);

                // Validate the primary keys
                if (qualifiers?.Any() != true)
                {
                    if (primaryOrIdentityDbField == null)
                    {
                        throw new MissingPrimaryKeyException($"No primary key or identity key found for table '{tableName}'.");
                    }
                    else
                    {
                        qualifiers = new[] { primaryOrIdentityDbField.AsField() };
                    }
                }

                // Filter the fields (based on the data reader)
                if (readerFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            readerFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Filter the fields (based on the mappings)
                if (mappings?.Any() == true)
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

                // Create a temporary table
                var sql = GetCreateTemporaryTableSqlText(tableName,
                    tempTableName,
                    fields,
                    dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction);

                // Set the options to KeepIdentity if needed
                if (options == null &&
                    identityDbField?.IsIdentity == true &&
                    qualifiers?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true &&
                    fields?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true)
                {
                    options = SqlBulkCopyOptions.KeepIdentity;
                }

                // Filter the DB Fields
                var filteredDbFields = dbFields?
                    .Where(dbField =>
                        fields?.Any(field => string.Equals(field.Name, dbField.Name, StringComparison.OrdinalIgnoreCase)) == true);

                // Do the bulk insertion first
                await BulkInsertAsyncInternal(connection,
                    tempTableName,
                    reader,
                    filteredDbFields,
                    mappings,
                    options,
                    null,
                    bulkCopyTimeout,
                    batchSize,
                    false,
                    false,
                    transaction);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction);

                // Merge the actual merge
                sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    isReturnIdentity.GetValueOrDefault());

                // Identity if the identity is to return
                if (isReturnIdentity != true)
                {
                    count = await connection.ExecuteNonQueryAsync(sql, transaction: transaction);
                }
                else
                {
                    using (var dataReader = (DbDataReader)(await connection.ExecuteReaderAsync(sql, transaction: transaction)))
                    {
                        while (await dataReader.ReadAsync())
                        {
                            var value = Converter.DbNullToNull((await dataReader.GetFieldValueAsync<object>(0)));
                            identities.Add(value);
                            count++;
                        }
                    }
                }

                // Drop the table after used
                sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction);

                // Commit the transaction
                if (hasTransaction == false)
                {
                    transaction?.Commit();
                }

                // Result
                return new BulkOperationIdentitiesResult(count, identityDbField?.Name, identities);
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

        /// <summary>
        /// Bulk merge an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static async Task<int> BulkMergeAsyncInternal(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
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
            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = (transaction != null);
            var result = default(int);

            // Check the transaction
            if (transaction == null)
            {
                // Add the transaction if not present
                transaction = (SqlTransaction)(await connection.EnsureOpenAsync()).BeginTransaction();
            }
            else
            {
                // Validate the objects
                SqlConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Must be fixed name so the RepoDb.Core caches will not be bloated
            var tempTableName = string.Concat("_RepoDb_BulkMerge_", GetTableName(tableName, dbSetting));

            // Add a # prefix if not physical
            if (usePhysicalPseudoTempTable != true)
            {
                tempTableName = string.Concat("#", tempTableName);
            }

            try
            {
                // Get the DB Fields
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, true);

                // Variables needed
                var tableFields = Enumerable.Range(0, dataTable.Columns.Count)
                    .Select((index) => dataTable.Columns[index].ColumnName);
                var fields = dbFields?.Select(dbField => dbField.AsField());
                var primaryDbField = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var primaryOrIdentityDbField = (primaryDbField ?? identityDbField);

                // Validate the primary keys
                if (qualifiers?.Any() != true)
                {
                    if (primaryOrIdentityDbField == null)
                    {
                        throw new MissingPrimaryKeyException($"No primary key or identity key found for table '{tableName}'.");
                    }
                    else
                    {
                        qualifiers = new[] { primaryOrIdentityDbField.AsField() };
                    }
                }

                // Filter the fields (based on the data table)
                if (tableFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            tableFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Filter the fields (based on the mappings)
                if (mappings?.Any() == true)
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

                // Create a temporary table
                var sql = GetCreateTemporaryTableSqlText(tableName,
                    tempTableName,
                    fields,
                    dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction);

                // Filter the DB Fields
                var filteredDbFields = dbFields?
                    .Where(dbField =>
                        fields?.Any(field => string.Equals(field.Name, dbField.Name, StringComparison.OrdinalIgnoreCase)) == true);

                // Do the bulk insertion first
                await BulkInsertAsyncInternal(connection,
                    tempTableName,
                    dataTable,
                    rowState,
                    filteredDbFields,
                    mappings,
                    options,
                    null,
                    bulkCopyTimeout,
                    batchSize,
                    false,
                    false,
                    transaction);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction);

                // Set the options to KeepIdentity if needed
                if (options == null &&
                    identityDbField?.IsIdentity == true &&
                    qualifiers?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true &&
                    fields?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true)
                {
                    options = SqlBulkCopyOptions.KeepIdentity;
                }

                // Merge the actual merge
                sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    isReturnIdentity.GetValueOrDefault());

                // Identity if the identity is to return
                var column = dataTable.Columns[identityDbField.Name];
                if (isReturnIdentity == true && column?.ReadOnly == false)
                {
                    using (var reader = (DbDataReader)(await connection.ExecuteReaderAsync(sql, transaction: transaction)))
                    {
                        while (await reader.ReadAsync())
                        {
                            var value = Converter.DbNullToNull((await reader.GetFieldValueAsync<object>(0)));
                            dataTable.Rows[result][column] = value;
                            result++;
                        }
                    }
                }
                else
                {
                    result = await connection.ExecuteNonQueryAsync(sql, transaction: transaction);
                }

                // Drop the table after used
                sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction);

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

            // Result
            return result;
        }

        #endregion
    }
}
