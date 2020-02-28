using Microsoft.Data.SqlClient;
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
        #region Privates

        private static bool m_microsoftDataBulkInsertRowsCopiedFieldHasBeenSet = false;

        #endregion

        #region BulkInsert<TEntity>

        /// <summary>
        /// Bulk insert a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this SqlConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            using (var reader = new DataEntityDataReader<TEntity>(entities))
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
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            using (var reader = new DataEntityDataReader<TEntity>(entities))
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
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this SqlConnection connection,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
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
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
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
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert(this SqlConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null)
        {
            return BulkInsertInternal(connection: connection,
                tableName: tableName,
                dataTable: dataTable,
                rowState: rowState,
                dbFields: null,
                mappings: mappings,
                options: options,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
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
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkInsertAsync<TEntity>(this SqlConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            using (var reader = new DataEntityDataReader<TEntity>(entities))
            {
                return await BulkInsertAsyncInternal(connection: connection,
                    tableName: ClassMappedNameCache.Get<TEntity>(),
                    reader: reader,
                    dbFields: null,
                    mappings: mappings,
                    options: options,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    transaction: transaction);
            }
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
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkInsertAsync<TEntity>(this SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            using (var reader = new DataEntityDataReader<TEntity>(entities))
            {
                return await BulkInsertAsyncInternal(connection: connection,
                    tableName: tableName,
                    reader: reader,
                    dbFields: null,
                    mappings: mappings,
                    options: options,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    transaction: transaction);
            }
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
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkInsertAsync<TEntity>(this SqlConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null)
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
                transaction: transaction);
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
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkInsertAsync(this SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null)
        {
            return await BulkInsertAsyncInternal(connection: connection,
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
        /// Bulk insert an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-insert operation.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkInsertAsync<TEntity>(this SqlConnection connection,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return await BulkInsertAsyncInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                dataTable: dataTable,
                rowState: rowState,
                dbFields: null,
                mappings: mappings,
                options: options,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                transaction: transaction);
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
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkInsertAsync(this SqlConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null)
        {
            return await BulkInsertAsyncInternal(connection: connection,
                tableName: tableName,
                dataTable: dataTable,
                rowState: rowState,
                dbFields: null,
                mappings: mappings,
                options: options,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                transaction: transaction);
        }

        #endregion

        #region BulkInsertInternal

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="dbFields">The list of <see cref="DbField"/> objects.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int BulkInsertInternal(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null)
        {
            // Validate the objects
            SqlConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);

            // Get the DB Fields
            dbFields = dbFields ?? DbFieldCache.Get(connection, tableName, transaction);
            if (dbFields?.Any() != true)
            {
                throw new InvalidOperationException($"No database fields found for '{tableName}'.");
            }

            // Variables needed
            var result = 0;
            var readerFields = Enumerable
                .Range(0, reader.FieldCount)
                .Select((index) => reader.GetName(index));
            var mappingFields = new List<Tuple<string, string>>();

            // To fix the casing problem of the bulk inserts
            foreach (var dbField in dbFields)
            {
                var readerField = readerFields.FirstOrDefault(field =>
                    string.Equals(field, dbField.Name, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(readerField))
                {
                    mappingFields.Add(new Tuple<string, string>(readerField, dbField.Name));
                }
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var sqlBulkCopy = new SqlBulkCopy(connection, options.GetValueOrDefault(), transaction))
            {
                // Set the destinationtable
                sqlBulkCopy.DestinationTableName = tableName;

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
                if (mappings == null)
                {
                    // Iterate the filtered fields
                    foreach (var field in mappingFields)
                    {
                        sqlBulkCopy.ColumnMappings.Add(field.Item1, field.Item2);
                    }
                }
                else
                {
                    // Iterate the provided mappings
                    foreach (var mapItem in mappings)
                    {
                        sqlBulkCopy.ColumnMappings.Add(mapItem.SourceColumn, mapItem.DestinationColumn);
                    }
                }

                // Open the connection and do the operation
                connection.EnsureOpen();
                sqlBulkCopy.WriteToServer(reader);

                // Hack the 'SqlBulkCopy' object
                var copiedField = GetRowsCopiedFieldFromMicrosoftDataSqlBulkCopy();

                // Set the return value
                result = copiedField != null ? (int)copiedField.GetValue(sqlBulkCopy) : reader.RecordsAffected;
            }

            // Result
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
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int BulkInsertInternal(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null)
        {
            // Validate the objects
            SqlConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);

            // Get the DB Fields
            dbFields = dbFields ?? DbFieldCache.Get(connection, tableName, transaction);
            if (dbFields?.Any() != true)
            {
                throw new InvalidOperationException($"No database fields found for '{tableName}'.");
            }

            // Variables needed
            var result = 0;
            var tableFields = GetDataColumns(dataTable)
                .Select(column => column.ColumnName);
            var mappingFields = new List<Tuple<string, string>>();

            // To fix the casing problem of the bulk inserts
            foreach (var dbField in dbFields)
            {
                var tableField = tableFields.FirstOrDefault(field =>
                    string.Equals(field, dbField.Name, StringComparison.OrdinalIgnoreCase));
                if (tableField != null)
                {
                    mappingFields.Add(new Tuple<string, string>(tableField, dbField.Name));
                }
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var sqlBulkCopy = new SqlBulkCopy(connection, options.GetValueOrDefault(), transaction))
            {
                // Set the destinationtable
                sqlBulkCopy.DestinationTableName = tableName;

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
                if (mappings == null)
                {
                    // Iterate the filtered fields
                    foreach (var field in mappingFields)
                    {
                        sqlBulkCopy.ColumnMappings.Add(field.Item1, field.Item2);
                    }
                }
                else
                {
                    // Iterate the provided mappings
                    foreach (var mapItem in mappings)
                    {
                        sqlBulkCopy.ColumnMappings.Add(mapItem.SourceColumn, mapItem.DestinationColumn);
                    }
                }

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

                // Set the return value
                result = GetDataRows(dataTable, rowState).Count();
            }

            // Result
            return result;
        }

        #endregion

        #region BulkInsertAsyncInternal

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="dbFields">The list of <see cref="DbField"/> objects.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static async Task<int> BulkInsertAsyncInternal(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null)
        {
            // Validate the objects
            SqlConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);

            // Get the DB Fields
            dbFields = dbFields ?? await DbFieldCache.GetAsync(connection, tableName, transaction);
            if (dbFields?.Any() != true)
            {
                throw new InvalidOperationException($"No database fields found for '{tableName}'.");
            }

            // Variables needed
            var result = 0;
            var readerFields = Enumerable
                .Range(0, reader.FieldCount)
                .Select((index) => reader.GetName(index));
            var mappingFields = new List<Tuple<string, string>>();

            // To fix the casing problem of the bulk inserts
            foreach (var dbField in dbFields)
            {
                var readerField = readerFields.FirstOrDefault(field =>
                    string.Equals(field, dbField.Name, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(readerField))
                {
                    mappingFields.Add(new Tuple<string, string>(readerField, dbField.Name));
                }
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var sqlBulkCopy = new SqlBulkCopy(connection, options.GetValueOrDefault(), transaction))
            {
                // Set the destinationtable
                sqlBulkCopy.DestinationTableName = tableName;

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
                if (mappings == null)
                {
                    // Iterate the filtered fields
                    foreach (var field in mappingFields)
                    {
                        sqlBulkCopy.ColumnMappings.Add(field.Item1, field.Item2);
                    }
                }
                else
                {
                    // Iterate the provided mappings
                    foreach (var mapItem in mappings)
                    {
                        sqlBulkCopy.ColumnMappings.Add(mapItem.SourceColumn, mapItem.DestinationColumn);
                    }
                }

                // Open the connection and do the operation
                await connection.EnsureOpenAsync();
                await sqlBulkCopy.WriteToServerAsync(reader);

                // Hack the 'SqlBulkCopy' object
                var copiedField = GetRowsCopiedFieldFromMicrosoftDataSqlBulkCopy();

                // Set the return value
                result = copiedField != null ? (int)copiedField.GetValue(sqlBulkCopy) : reader.RecordsAffected;
            }

            // Result
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
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static async Task<int> BulkInsertAsyncInternal(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction transaction = null)
        {
            // Validate the objects
            SqlConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);

            // Get the DB Fields
            dbFields = dbFields ?? await DbFieldCache.GetAsync(connection, tableName, transaction);
            if (dbFields?.Any() != true)
            {
                throw new InvalidOperationException($"No database fields found for '{tableName}'.");
            }

            // Variables needed
            var result = 0;
            var tableFields = GetDataColumns(dataTable)
                .Select(column => column.ColumnName);
            var mappingFields = new List<Tuple<string, string>>();

            // To fix the casing problem of the bulk inserts
            foreach (var dbField in dbFields)
            {
                var tableField = tableFields.FirstOrDefault(field =>
                    string.Equals(field, dbField.Name, StringComparison.OrdinalIgnoreCase));
                if (tableField != null)
                {
                    mappingFields.Add(new Tuple<string, string>(tableField, dbField.Name));
                }
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var sqlBulkCopy = new SqlBulkCopy(connection, options.GetValueOrDefault(), transaction))
            {
                // Set the destinationtable
                sqlBulkCopy.DestinationTableName = tableName;

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
                if (mappings == null)
                {
                    // Iterate the filtered fields
                    foreach (var field in mappingFields)
                    {
                        sqlBulkCopy.ColumnMappings.Add(field.Item1, field.Item2);
                    }
                }
                else
                {
                    // Iterate the provided mappings
                    foreach (var mapItem in mappings)
                    {
                        sqlBulkCopy.ColumnMappings.Add(mapItem.SourceColumn, mapItem.DestinationColumn);
                    }
                }

                // Open the connection and do the operation
                connection.EnsureOpen();
                if (rowState.HasValue == true)
                {
                    await sqlBulkCopy.WriteToServerAsync(dataTable, rowState.Value);
                }
                else
                {
                    await sqlBulkCopy.WriteToServerAsync(dataTable);
                }

                // Set the return value
                result = GetDataRows(dataTable, rowState).Count();
            }

            // Result
            return result;
        }

        #endregion
    }
}
