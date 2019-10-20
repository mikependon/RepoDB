using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RepoDb.DbOperationProviders
{
    /// <summary>
    /// A class that contains the operations for SQL Server database.
    /// </summary>
    internal partial class SqlServerDbOperation : IDbOperation
    {
        #region Privates

        private bool m_bulkInsertRowsCopiedFieldHasSet = false;
        private FieldInfo m_bulkInsertRowsCopiedField = null;

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="SqlServerDbOperation"/>.
        /// </summary>
        public SqlServerDbOperation() { }

        #region Methods

        /// <summary>
        /// Validates the type of the <see cref="IDbConnection"/> object.
        /// </summary>
        /// <param name="connection">The connection to validate.</param>
        private void ValidateConnection(IDbConnection connection)
        {
            if (connection is SqlConnection == false)
            {
                throw new NotSupportedException("The bulk-insert is only applicable for SQL Server database connection.");
            }
        }

        /// <summary>
        /// Validates the type of the <see cref="IDbTransaction"/> object.
        /// </summary>
        /// <param name="transaction">The connection to validate.</param>
        private void ValidateTransaction(IDbTransaction transaction)
        {
            if (transaction != null && transaction is SqlTransaction == false)
            {
                throw new NotSupportedException("The bulk-insert is only applicable for SQL Server database connection.");
            }
        }

        /// <summary>
        /// Gets the <see cref="SqlBulkCopy"/> private variable reflected field.
        /// </summary>
        /// <returns>The actual field.</returns>
        private FieldInfo GetRowsCopiedField()
        {
            // Check if the call has made earlier
            if (m_bulkInsertRowsCopiedFieldHasSet == true)
            {
                return m_bulkInsertRowsCopiedField;
            }

            // Set the flag
            m_bulkInsertRowsCopiedFieldHasSet = true;

            // Get the field (whether null or not)
            m_bulkInsertRowsCopiedField = typeof(SqlBulkCopy)
                .GetField("_rowsCopied", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            // Return the value
            return m_bulkInsertRowsCopiedField;
        }

        /// <summary>
        /// Returns all the <see cref="DataTable"/> objects of the <see cref="DataTable"/>.
        /// </summary>
        /// <param name="dataTable">The instance of <see cref="DataTable"/> where the list of <see cref="DataColumn"/> will be extracted.</param>
        /// <returns>The list of <see cref="DataColumn"/> objects.</returns>
        private IEnumerable<DataColumn> GetDataColumns(DataTable dataTable)
        {
            foreach (var column in dataTable.Columns.OfType<DataColumn>())
            {
                yield return column;
            }
        }

        /// <summary>
        /// Returns all the <see cref="DataRow"/> objects of the <see cref="DataTable"/> by state.
        /// </summary>
        /// <param name="dataTable">The instance of <see cref="DataTable"/> where the list of <see cref="DataRow"/> will be extracted.</param>
        /// <param name="rowState">The state of the <see cref="DataRow"/> objects to be extracted.</param>
        /// <returns>The list of <see cref="DataRow"/> objects.</returns>
        private IEnumerable<DataRow> GetDataRows(DataTable dataTable, DataRowState rowState)
        {
            foreach (var row in dataTable.Rows.OfType<DataRow>().Where(r => r.RowState == rowState))
            {
                yield return row;
            }
        }

        #endregion

        #region BulkInsert

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
        public int BulkInsert<TEntity>(IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Validate the objects
            ValidateConnection(connection);
            ValidateTransaction(transaction);
            DbConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);

            // Variables for the operation
            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = new DataEntityDataReader<TEntity>(entities, connection))
            {
                using (var sqlBulkCopy = new SqlBulkCopy((SqlConnection)connection, options, (SqlTransaction)transaction))
                {
                    // Set the destinationtable
                    sqlBulkCopy.DestinationTableName = ClassMappedNameCache.Get<TEntity>();

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
                        // Variables needed
                        var dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<TEntity>(), transaction);
                        var fields = reader.Properties.AsFields();
                        var filteredFields = new List<Tuple<string, string>>();
                        var dbSetting = connection.GetDbSetting();

                        // To fix the casing problem of the bulk inserts
                        foreach (var dbField in dbFields)
                        {
                            var field = fields.FirstOrDefault(f =>
                                string.Equals(f.Name.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
                            if (field != null)
                            {
                                filteredFields.Add(new Tuple<string, string>(field.Name, dbField.Name));
                            }
                        }

                        // Iterate the filtered fields
                        foreach (var field in filteredFields)
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
                    var copiedField = GetRowsCopiedField();

                    // Set the return value
                    result = copiedField != null ? (int)copiedField.GetValue(sqlBulkCopy) : reader.RecordsAffected;
                }
            }

            // Result
            return result;
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
        public int BulkInsert<TEntity>(IDbConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            return BulkInsert(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                reader: reader,
                mappings: mappings,
                options: options,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                transaction: transaction);
        }

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
        public int BulkInsert(IDbConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            IDbTransaction transaction = null)
        {
            // Validate the objects
            ValidateConnection(connection);
            ValidateTransaction(transaction);
            DbConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);

            // Variables for the operation
            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var sqlBulkCopy = new SqlBulkCopy((SqlConnection)connection, options, (SqlTransaction)transaction))
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
                    // Variables needed
                    var dbSetting = connection.GetDbSetting();
                    var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                    var fields = Enumerable.Range(0, reader.FieldCount).Select((index) => reader.GetName(index));
                    var filteredFields = new List<Tuple<string, string>>();

                    // To fix the casing problem of the bulk inserts
                    foreach (var dbField in dbFields)
                    {
                        var readerField = fields.FirstOrDefault(field =>
                            string.Equals(field.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
                        if (!string.IsNullOrEmpty(readerField))
                        {
                            filteredFields.Add(new Tuple<string, string>(readerField, dbField.Name.AsUnquoted(true, dbSetting)));
                        }
                    }

                    // Iterate the filtered fields
                    foreach (var field in filteredFields)
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
                var copiedField = GetRowsCopiedField();

                // Set the return value
                result = copiedField != null ? (int)copiedField.GetValue(sqlBulkCopy) : reader.RecordsAffected;
            }

            // Result
            return result;
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
        public int BulkInsert<TEntity>(IDbConnection connection,
            DataTable dataTable,
            DataRowState rowState = DataRowState.Unchanged,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            return BulkInsert(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                dataTable: dataTable,
                rowState: rowState,
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
        public int BulkInsert(IDbConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState rowState = DataRowState.Unchanged,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            IDbTransaction transaction = null)
        {
            // Validate the objects
            ValidateConnection(connection);
            ValidateTransaction(transaction);
            DbConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);

            // Variables for the operation
            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var sqlBulkCopy = new SqlBulkCopy((SqlConnection)connection, options, (SqlTransaction)transaction))
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
                    // Variables needed
                    var dbSetting = connection.GetDbSetting();
                    var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                    var fields = GetDataColumns(dataTable).Select(column => column.ColumnName);
                    var filteredFields = new List<Tuple<string, string>>();

                    // To fix the casing problem of the bulk inserts
                    foreach (var dbField in dbFields)
                    {
                        var field = fields.FirstOrDefault(f =>
                            string.Equals(f.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
                        if (field != null)
                        {
                            filteredFields.Add(new Tuple<string, string>(field, dbField.Name.AsUnquoted(true, dbSetting)));
                        }
                    }

                    // Iterate the filtered fields
                    foreach (var field in filteredFields)
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
                sqlBulkCopy.WriteToServer(dataTable, rowState);

                // Set the return value
                result = GetDataRows(dataTable, rowState).Count();
            }

            // Result
            return result;
        }

        #endregion

        #region BulkInsertAsync

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
        public async Task<int> BulkInsertAsync<TEntity>(IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Validate the objects
            ValidateConnection(connection);
            ValidateTransaction(transaction);
            DbConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);

            // Variables for the operation
            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = new DataEntityDataReader<TEntity>(entities, connection))
            {
                using (var sqlBulkCopy = new SqlBulkCopy((SqlConnection)connection, options, (SqlTransaction)transaction))
                {
                    // Set the destinationtable
                    sqlBulkCopy.DestinationTableName = ClassMappedNameCache.Get<TEntity>();

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
                        // Variables needed
                        var dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<TEntity>(), transaction);
                        var fields = reader.Properties.AsFields();
                        var filteredFields = new List<Tuple<string, string>>();
                        var dbSetting = connection.GetDbSetting();

                        // To fix the casing problem of the bulk inserts
                        foreach (var dbField in dbFields)
                        {
                            var field = fields.FirstOrDefault(f =>
                                string.Equals(f.Name.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
                            if (field != null)
                            {
                                filteredFields.Add(new Tuple<string, string>(field.Name, dbField.Name));
                            }
                        }

                        // Iterate the filtered fields
                        foreach (var field in filteredFields)
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
                    var copiedField = GetRowsCopiedField();

                    // Set the return value
                    result = copiedField != null ? (int)copiedField.GetValue(sqlBulkCopy) : reader.RecordsAffected;
                }
            }

            // Result
            return result;
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
        public Task<int> BulkInsertAsync<TEntity>(IDbConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            return BulkInsertAsync(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                reader: reader,
                mappings: mappings,
                options: options,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                transaction: transaction);
        }

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
        public async Task<int> BulkInsertAsync(IDbConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            IDbTransaction transaction = null)
        {
            // Validate the objects
            ValidateConnection(connection);
            ValidateTransaction(transaction);
            DbConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);

            // Variables for the operation
            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var sqlBulkCopy = new SqlBulkCopy((SqlConnection)connection, options, (SqlTransaction)transaction))
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
                    // Variables needed
                    var dbSetting = connection.GetDbSetting();
                    var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction);
                    var fields = Enumerable.Range(0, reader.FieldCount).Select((index) => reader.GetName(index));
                    var filteredFields = new List<Tuple<string, string>>();

                    // To fix the casing problem of the bulk inserts
                    foreach (var dbField in dbFields)
                    {
                        var readerField = fields.FirstOrDefault(field =>
                            string.Equals(field.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
                        if (!string.IsNullOrEmpty(readerField))
                        {
                            filteredFields.Add(new Tuple<string, string>(readerField, dbField.Name.AsUnquoted(true, dbSetting)));
                        }
                    }

                    // Iterate the filtered fields
                    foreach (var field in filteredFields)
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
                var copiedField = GetRowsCopiedField();

                // Set the return value
                result = copiedField != null ? (int)copiedField.GetValue(sqlBulkCopy) : reader.RecordsAffected;
            }

            // Result
            return result;
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
        public Task<int> BulkInsertAsync<TEntity>(IDbConnection connection,
            DataTable dataTable,
            DataRowState rowState = DataRowState.Unchanged,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            return BulkInsertAsync(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                dataTable: dataTable,
                rowState: rowState,
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
        public async Task<int> BulkInsertAsync(IDbConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState rowState = DataRowState.Unchanged,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            IDbTransaction transaction = null)
        {
            // Validate the objects
            ValidateConnection(connection);
            ValidateTransaction(transaction);
            DbConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);

            // Variables for the operation
            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var sqlBulkCopy = new SqlBulkCopy((SqlConnection)connection, options, (SqlTransaction)transaction))
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
                    // Variables needed
                    var dbSetting = connection.GetDbSetting();
                    var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction);
                    var fields = GetDataColumns(dataTable).Select(column => column.ColumnName);
                    var filteredFields = new List<Tuple<string, string>>();

                    // To fix the casing problem of the bulk inserts
                    foreach (var dbField in dbFields)
                    {
                        var field = fields.FirstOrDefault(f =>
                            string.Equals(f.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
                        if (field != null)
                        {
                            filteredFields.Add(new Tuple<string, string>(field, dbField.Name.AsUnquoted(true, dbSetting)));
                        }
                    }

                    // Iterate the filtered fields
                    foreach (var field in filteredFields)
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
                await sqlBulkCopy.WriteToServerAsync(dataTable, rowState);

                // Set the return value
                result = GetDataRows(dataTable, rowState).Count();
            }

            // Result
            return result;
        }

        #endregion
    }
}
