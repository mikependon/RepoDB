using Microsoft.Data.SqlClient;
using RepoDb.Extensions;
using RepoDb.Interfaces;
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
        #region BulkUpdate

        /// <summary>
        /// Bulk update a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-updated.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkUpdate<TEntity>(this SqlConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
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
                    qualifiers: qualifiers,
                    mappings: mappings,
                    options: options,
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
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkUpdate<TEntity>(this SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
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
                    qualifiers: qualifiers,
                    mappings: mappings,
                    options: options,
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
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkUpdate<TEntity>(this SqlConnection connection,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return BulkUpdateInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
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
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
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
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
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
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
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
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
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
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
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
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        #endregion

        #region BulkUpdateAsync

        /// <summary>
        /// Bulk update a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-updated.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkUpdateAsync<TEntity>(this SqlConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            using (var reader = new DataEntityDataReader<TEntity>(entities))
            {
                return await BulkUpdateAsyncInternal(connection: connection,
                    tableName: ClassMappedNameCache.Get<TEntity>(),
                    reader: reader,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    options: options,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                    transaction: transaction);
            }
        }

        /// <summary>
        /// Bulk update a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-updated.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkUpdateAsync<TEntity>(this SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            using (var reader = new DataEntityDataReader<TEntity>(entities))
            {
                return await BulkUpdateAsyncInternal(connection: connection,
                    tableName: tableName,
                    reader: reader,
                    qualifiers: qualifiers,
                    mappings: mappings,
                    options: options,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                    transaction: transaction);
            }
        }

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkUpdateAsync<TEntity>(this SqlConnection connection,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return await BulkUpdateAsyncInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkUpdateAsync(this SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
        {
            return await BulkUpdateAsyncInternal(connection: connection,
                tableName: tableName,
                reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk update an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkUpdateAsync<TEntity>(this SqlConnection connection,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return await BulkUpdateAsyncInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk update an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkUpdateAsync(this SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
        {
            return await BulkUpdateAsyncInternal(connection: connection,
                tableName: tableName,
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        #endregion

        #region BulkUpdateInternal

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int BulkUpdateInternal(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
        {
            // Variables
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
            var tempTableName = string.Concat("RepoDb-BulkUpdate-", tableName);

            // Add a # prefix if not physical
            if (usePhysicalPseudoTempTable != true)
            {
                tempTableName = string.Concat("#", tempTableName);
            }

            try
            {
                // Get the DB Fields
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var fields = (IEnumerable<Field>)null;
                var primaryKey = dbFields.FirstOrDefault(e => e.IsPrimary) ??
                    dbFields.FirstOrDefault(e => e.IsIdentity);
                var dbSetting = connection.GetDbSetting();

                // Validate the primary keys
                if (qualifiers?.Any() != true)
                {
                    if (primaryKey == null)
                    {
                        throw new MissingPrimaryKeyException($"No primary key found for table '{tableName}'.");
                    }
                    else
                    {
                        qualifiers = new[] { primaryKey.AsField() };
                    }
                }

                // Get the fields (originated from the DB)
                fields = dbFields
                    .Where(e =>
                        qualifiers.Any(q => string.Equals(q.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == false)
                    .Select(e => e.AsField());

                // Filter the fields (based on mappings)
                if (mappings?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.SourceColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There is no field(s) found for this operation.");
                }

                // Create a temporary table
                var sql = GetBulkUpdateCreateTemporaryTableSqlText(tableName,
                    tempTableName, fields, qualifiers, dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

                // Do the bulk insertion first
                BulkInsertInternal(connection,
                    tempTableName,
                    reader,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    transaction);

                // Create the clustered index
                sql = GetBulkUpdateCreateClusteredIndexSqlText(tempTableName, qualifiers, dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

                // Update the actual update
                sql = GetBulkUpdateSqlText(tableName, tempTableName, fields, qualifiers, dbSetting);
                result = connection.ExecuteNonQuery(sql, transaction: transaction);

                // Drop the table after used
                sql = GetBulkUpdateDropTemporaryTableSqlText(tempTableName, dbSetting);
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

        /// <summary>
        /// Bulk update an instance of <see cref="DataTable"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int BulkUpdateInternal(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
        {
            // Variables
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
            var tempTableName = string.Concat("RepoDb-BulkUpdate-", tableName);

            // Add a # prefix if not physical
            if (usePhysicalPseudoTempTable != true)
            {
                tempTableName = string.Concat("#", tempTableName);
            }

            try
            {
                // Get the DB Fields
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var fields = (IEnumerable<Field>)null;
                var primaryKey = dbFields.FirstOrDefault(e => e.IsPrimary) ??
                    dbFields.FirstOrDefault(e => e.IsIdentity);
                var dbSetting = connection.GetDbSetting();

                // Validate the primary keys
                if (qualifiers?.Any() != true)
                {
                    if (primaryKey == null)
                    {
                        throw new MissingPrimaryKeyException($"No primary key found for table '{tableName}'.");
                    }
                    else
                    {
                        qualifiers = new[] { primaryKey.AsField() };
                    }
                }

                // Get the fields (originated from the DB)
                fields = dbFields
                    .Where(e =>
                        qualifiers.Any(q => string.Equals(q.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == false)
                    .Select(e => e.AsField());

                // Filter the fields (based on mappings)
                if (mappings?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.SourceColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There is no field(s) found for this operation.");
                }

                // Create a temporary table
                var sql = GetBulkUpdateCreateTemporaryTableSqlText(tableName,
                    tempTableName, fields, qualifiers, dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

                // Do the bulk insertion first
                BulkInsertInternal(connection,
                    tempTableName,
                    dataTable,
                    rowState,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    transaction);

                // Create the clustered index
                sql = GetBulkUpdateCreateClusteredIndexSqlText(tempTableName, qualifiers, dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

                // Update the actual update
                sql = GetBulkUpdateSqlText(tableName, tempTableName, fields, qualifiers, dbSetting);
                result = connection.ExecuteNonQuery(sql, transaction: transaction);

                // Drop the table after used
                sql = GetBulkUpdateDropTemporaryTableSqlText(tempTableName, dbSetting);
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

        #region BulkUpdateAsyncInternal

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static async Task<int> BulkUpdateAsyncInternal(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
        {
            // Variables
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
            var tempTableName = string.Concat("RepoDb-BulkUpdate-", tableName);

            // Add a # prefix if not physical
            if (usePhysicalPseudoTempTable != true)
            {
                tempTableName = string.Concat("#", tempTableName);
            }

            try
            {
                // Get the DB Fields
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction);
                var fields = (IEnumerable<Field>)null;
                var primaryKey = dbFields.FirstOrDefault(e => e.IsPrimary) ??
                    dbFields.FirstOrDefault(e => e.IsIdentity);
                var dbSetting = connection.GetDbSetting();

                // Validate the primary keys
                if (qualifiers?.Any() != true)
                {
                    if (primaryKey == null)
                    {
                        throw new MissingPrimaryKeyException($"No primary key found for table '{tableName}'.");
                    }
                    else
                    {
                        qualifiers = new[] { primaryKey.AsField() };
                    }
                }

                // Get the fields (originated from the DB)
                fields = dbFields
                    .Where(e =>
                        qualifiers.Any(q => string.Equals(q.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == false)
                    .Select(e => e.AsField());

                // Filter the fields (based on mappings)
                if (mappings?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.SourceColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There is no field(s) found for this operation.");
                }

                // Create a temporary table
                var sql = GetBulkUpdateCreateTemporaryTableSqlText(tableName,
                    tempTableName, fields, qualifiers, dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction);

                // Do the bulk insertion first
                await BulkInsertAsyncInternal(connection,
                    tempTableName,
                    reader,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    transaction);

                // Create the clustered index
                sql = GetBulkUpdateCreateClusteredIndexSqlText(tempTableName, qualifiers, dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction);

                // Update the actual update
                sql = GetBulkUpdateSqlText(tableName, tempTableName, fields, qualifiers, dbSetting);
                result = await connection.ExecuteNonQueryAsync(sql, transaction: transaction);

                // Drop the table after used
                sql = GetBulkUpdateDropTemporaryTableSqlText(tempTableName, dbSetting);
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

        /// <summary>
        /// Bulk update an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static async Task<int> BulkUpdateAsyncInternal(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
        {
            // Variables
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
            var tempTableName = string.Concat("RepoDb-BulkUpdate-", tableName);

            // Add a # prefix if not physical
            if (usePhysicalPseudoTempTable != true)
            {
                tempTableName = string.Concat("#", tempTableName);
            }

            try
            {
                // Get the DB Fields
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction);
                var fields = (IEnumerable<Field>)null;
                var primaryKey = dbFields.FirstOrDefault(e => e.IsPrimary) ??
                    dbFields.FirstOrDefault(e => e.IsIdentity);
                var dbSetting = connection.GetDbSetting();

                // Validate the primary keys
                if (qualifiers?.Any() != true)
                {
                    if (primaryKey == null)
                    {
                        throw new MissingPrimaryKeyException($"No primary key found for table '{tableName}'.");
                    }
                    else
                    {
                        qualifiers = new[] { primaryKey.AsField() };
                    }
                }

                // Get the fields (originated from the DB)
                fields = dbFields
                    .Where(e =>
                        qualifiers.Any(q => string.Equals(q.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == false)
                    .Select(e => e.AsField());

                // Filter the fields (based on mappings)
                if (mappings?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.SourceColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There is no field(s) found for this operation.");
                }

                // Create a temporary table
                var sql = GetBulkUpdateCreateTemporaryTableSqlText(tableName,
                    tempTableName, fields, qualifiers, dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction);

                // Do the bulk insertion first
                await BulkInsertAsyncInternal(connection,
                    tempTableName,
                    dataTable,
                    rowState,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    transaction);

                // Create the clustered index
                sql = GetBulkUpdateCreateClusteredIndexSqlText(tempTableName, qualifiers, dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction);

                // Update the actual update
                sql = GetBulkUpdateSqlText(tableName, tempTableName, fields, qualifiers, dbSetting);
                result = await connection.ExecuteNonQueryAsync(sql, transaction: transaction);

                // Drop the table after used
                sql = GetBulkUpdateDropTemporaryTableSqlText(tempTableName, dbSetting);
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

        #region Helpers

        private static string GetBulkUpdateCreateTemporaryTableSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var builder = new QueryBuilder();
            var selectFields = new List<Field>(qualifiers);

            // Add the fields
            selectFields.AddRange(fields);

            // Compose the statement
            builder.Select()
                .FieldsFrom(selectFields, dbSetting)
                .Into()
                .WriteText(tempTableName.AsQuoted(dbSetting))
                .From()
                .TableNameFrom(tableName, dbSetting)
                .Where()
                .WriteText("1 = 0")
                .End();

            // Return the text
            return builder.ToString();
        }

        private static string GetBulkUpdateSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            // Validate the presence
            if (fields?.Any() != true)
            {
                throw new MissingFieldException("There is no field(s) defined.");
            }

            if (qualifiers?.Any() != true)
            {
                throw new MissingFieldException("There is no qualifer field(s) defined.");
            }

            // Variables needed
            var qualifierFields = qualifiers
                .Select(f => $"S.{f.Name.AsField(dbSetting)} = T.{f.Name.AsField(dbSetting)}")
                .Join(", ");
            var setFields = fields
                .Select(f => $"T.{f.Name.AsField(dbSetting)} = S.{f.Name.AsField(dbSetting)}")
                .Join(", ");
            var builder = new QueryBuilder();

            // Compose the statement
            builder
                .Update()
                .WriteText("T")
                .Set()
                .WriteText(setFields)
                .From()
                .TableNameFrom(tableName, dbSetting)
                .WriteText("T")
                .WriteText("INNER JOIN")
                .TableNameFrom(tempTableName, dbSetting)
                .WriteText("S")
                .WriteText("ON")
                .WriteText(qualifierFields)
                .End();

            // Return the sql
            return builder.ToString();
        }

        private static string GetBulkUpdateCreateClusteredIndexSqlText(string tempTableName,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            // Validate the presence
            if (qualifiers?.Any() != true)
            {
                throw new MissingFieldException("There is no qualifer field(s) defined.");
            }

            // Variables needed
            var clusteredIndexFields = qualifiers
                .Select(f => $"{f.Name.AsQuoted(dbSetting)} ASC")
                .Join(", ");
            var builder = new QueryBuilder();

            // Compose the statement
            builder
                .WriteText("CREATE CLUSTERED INDEX")
                .WriteText($"IX_{tempTableName}".AsQuoted(dbSetting))
                .On()
                .WriteText(tempTableName.AsQuoted(dbSetting))
                .OpenParen()
                .WriteText(clusteredIndexFields)
                .CloseParen()
                .End();

            // Return the sql
            return builder.ToString();
        }

        private static string GetBulkUpdateDropTemporaryTableSqlText(string tempTableName,
            IDbSetting dbSetting)
        {
            return $"DROP TABLE {tempTableName.AsQuoted(dbSetting)};";
        }

        #endregion
    }
}
