using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static partial class DbConnectionExtension
    {
        #region Privates

        private static bool m_bulkInsertRowsCopiedFieldHasSet = false;
        private static FieldInfo m_bulkInsertRowsCopiedField = null;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the <see cref="SqlBulkCopy"/> private variable reflected field.
        /// </summary>
        /// <returns>The actual field.</returns>
        public static FieldInfo GetRowsCopiedField()
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

        #endregion

        #region BulkInsert

        /// <summary>
        /// Bulk insert a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="copyOptions">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : class
        {
            return BulkInsertInternal<TEntity>(connection: connection,
                entities: entities,
                mappings: mappings,
                copyOptions: copyOptions,
                bulkCopyTimeout: bulkCopyTimeout,
                transaction: transaction,
                trace: trace);
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="copyOptions">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this IDbConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : class
        {
            return BulkInsertInternal<TEntity>(connection: connection,
                reader: reader,
                mappings: mappings,
                copyOptions: copyOptions,
                bulkCopyTimeout: bulkCopyTimeout,
                transaction: transaction,
                trace: trace);
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="copyOptions">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static int BulkInsert(this IDbConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            return BulkInsertInternal(connection: connection,
                tableName: tableName,
                reader: reader,
                mappings: mappings,
                copyOptions: copyOptions,
                bulkCopyTimeout: bulkCopyTimeout,
                transaction: transaction,
                trace: trace);
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="copyOptions">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        internal static int BulkInsertInternal<TEntity>(this IDbConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : class
        {
            return BulkInsertInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                reader: reader,
                mappings: mappings,
                copyOptions: copyOptions,
                bulkCopyTimeout: bulkCopyTimeout,
                transaction: transaction,
                trace: trace);
        }

        /// <summary>
        /// Bulk insert a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="copyOptions">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        internal static int BulkInsertInternal<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : class
        {
            // Validate, only supports SqlConnection
            if (connection is SqlConnection == false)
            {
                throw new NotSupportedException("The bulk-insert is only applicable for SQL Server database connection.");
            }

            // Validate, only supports SqlConnection
            if (transaction != null)
            {
                if (transaction is SqlTransaction == false)
                {
                    throw new NotSupportedException("The bulk-insert is only applicable for SQL Server database connection.");
                }
                ValidateTransactionConnectionObject(connection, transaction);
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog("BulkInsert", entities, null);
                trace.BeforeBulkInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("BulkInsert");
                    }
                    return 0;
                }
            }

            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = new DataEntityDataReader<TEntity>(entities, connection))
            {
                using (var sqlBulkCopy = new SqlBulkCopy((SqlConnection)connection, copyOptions, (SqlTransaction)transaction))
                {
                    // Set the destinationtable
                    sqlBulkCopy.DestinationTableName = ClassMappedNameCache.Get<TEntity>();

                    // Set the timeout
                    if (bulkCopyTimeout != null && bulkCopyTimeout.HasValue)
                    {
                        sqlBulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
                    }

                    // Add the mappings
                    if (mappings == null)
                    {
                        foreach (var property in reader.Properties)
                        {
                            var columnName = property.GetUnquotedMappedName();
                            sqlBulkCopy.ColumnMappings.Add(columnName, columnName);
                        }
                    }
                    else
                    {
                        foreach (var mapItem in mappings)
                        {
                            sqlBulkCopy.ColumnMappings.Add(mapItem.SourceColumn, mapItem.DestinationColumn);
                        }
                    }

                    // Open the connection and do the operation
                    connection.EnsureOpen();
                    sqlBulkCopy.WriteToServer(reader);

                    // Hack the 'SqlBulkCopy' object
                    var field = GetRowsCopiedField();

                    // Identify if we found the field
                    if (field != null)
                    {
                        result = (int)field.GetValue(sqlBulkCopy);
                    }
                    else
                    {
                        // Return the record
                        result = reader.RecordsAffected;
                    }
                }
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterBulkInsert(new TraceLog("BulkInsert", entities, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="copyOptions">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        internal static int BulkInsertInternal(this IDbConnection connection,
            DbDataReader reader,
            string tableName,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            // Validate, only supports SqlConnection
            if (connection is SqlConnection == false)
            {
                throw new NotSupportedException("The bulk-insert is only applicable for SQL Server database connection.");
            }

            // Validate, only supports SqlConnection
            if (transaction != null)
            {
                if (transaction is SqlTransaction == false)
                {
                    throw new NotSupportedException("The bulk-insert is only applicable for SQL Server database connection.");
                }
                ValidateTransactionConnectionObject(connection, transaction);
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog("BulkInsert", reader, null);
                trace.BeforeBulkInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("BulkInsert");
                    }
                    return 0;
                }
            }

            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var sqlBulkCopy = new SqlBulkCopy((SqlConnection)connection, copyOptions, (SqlTransaction)transaction))
            {
                // Set the destinationtable
                sqlBulkCopy.DestinationTableName = tableName;

                // Set the timeout
                if (bulkCopyTimeout != null && bulkCopyTimeout.HasValue)
                {
                    sqlBulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
                }

                // Add the mappings
                if (mappings != null)
                {
                    foreach (var mapItem in mappings)
                    {
                        sqlBulkCopy.ColumnMappings.Add(mapItem.SourceColumn, mapItem.DestinationColumn);
                    }
                }

                // Open the connection and do the operation
                connection.EnsureOpen();
                sqlBulkCopy.WriteToServer(reader);

                // Hack the 'SqlBulkCopy' object
                var field = GetRowsCopiedField();

                // Identify if we found the field
                if (field != null)
                {
                    result = (int)field.GetValue(sqlBulkCopy);
                }
                else
                {
                    // Return the record
                    result = reader.RecordsAffected;
                }
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterBulkInsert(new TraceLog("BulkInsert", reader, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
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
        /// <param name="copyOptions">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static Task<int> BulkInsertAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : class
        {
            return BulkInsertAsyncInternal<TEntity>(connection: connection,
                entities: entities,
                mappings: mappings,
                copyOptions: copyOptions,
                bulkCopyTimeout: bulkCopyTimeout,
                transaction: transaction,
                trace: trace);
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="copyOptions">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static Task<int> BulkInsertAsync<TEntity>(this IDbConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : class
        {
            return BulkInsertAsyncInternal<TEntity>(connection: connection,
                reader: reader,
                mappings: mappings,
                copyOptions: copyOptions,
                bulkCopyTimeout: bulkCopyTimeout,
                transaction: transaction,
                trace: trace);
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="copyOptions">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static Task<int> BulkInsertAsync(this IDbConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            return BulkInsertAsyncInternal(connection: connection,
                tableName: tableName,
                reader: reader,
                mappings: mappings,
                copyOptions: copyOptions,
                bulkCopyTimeout: bulkCopyTimeout,
                transaction: transaction,
                trace: trace);
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="copyOptions">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        internal static Task<int> BulkInsertAsyncInternal<TEntity>(this IDbConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : class
        {
            return BulkInsertAsyncInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                reader: reader,
                mappings: mappings,
                copyOptions: copyOptions,
                bulkCopyTimeout: bulkCopyTimeout,
                transaction: transaction,
                trace: trace);
        }

        /// <summary>
        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="copyOptions">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        internal static async Task<int> BulkInsertAsyncInternal<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : class
        {
            // Validate, only supports SqlConnection
            if (connection is SqlConnection == false)
            {
                throw new NotSupportedException("The bulk-insert is only applicable for SQL Server database connection.");
            }

            // Validate, only supports SqlConnection
            if (transaction != null)
            {
                if (transaction is SqlTransaction == false)
                {
                    throw new NotSupportedException("The bulk-insert is only applicable for SQL Server database connection.");
                }
                ValidateTransactionConnectionObject(connection, transaction);
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog("BulkInsert", entities, null);
                trace.BeforeBulkInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("BulkInsert");
                    }
                    return 0;
                }
            }

            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = new DataEntityDataReader<TEntity>(entities, connection))
            {
                using (var sqlBulkCopy = new SqlBulkCopy((SqlConnection)connection, copyOptions, (SqlTransaction)transaction))
                {
                    // Set the destinationtable
                    sqlBulkCopy.DestinationTableName = ClassMappedNameCache.Get<TEntity>();

                    // Set the timeout
                    if (bulkCopyTimeout != null && bulkCopyTimeout.HasValue)
                    {
                        sqlBulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
                    }

                    // Add the mappings
                    if (mappings == null)
                    {
                        foreach (var property in reader.Properties)
                        {
                            var columnName = property.GetUnquotedMappedName();
                            sqlBulkCopy.ColumnMappings.Add(columnName, columnName);
                        }
                    }
                    else
                    {
                        foreach (var mapItem in mappings)
                        {
                            sqlBulkCopy.ColumnMappings.Add(mapItem.SourceColumn, mapItem.DestinationColumn);
                        }
                    }

                    // Open the connection and do the operation
                    connection.EnsureOpen();
                    await sqlBulkCopy.WriteToServerAsync(reader);

                    // Hack the 'SqlBulkCopy' object
                    var field = GetRowsCopiedField();

                    // Identify if we found the field
                    if (field != null)
                    {
                        result = (int)field.GetValue(sqlBulkCopy);
                    }
                    else
                    {
                        // Return the record
                        result = reader.RecordsAffected;
                    }
                }
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterBulkInsert(new TraceLog("BulkInsert", entities, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="copyOptions">The bulk-copy options to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        internal static async Task<int> BulkInsertAsyncInternal(this IDbConnection connection,
            DbDataReader reader,
            string tableName,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            // Validate, only supports SqlConnection
            if (connection is SqlConnection == false)
            {
                throw new NotSupportedException("The bulk-insert is only applicable for SQL Server database connection.");
            }

            // Validate, only supports SqlConnection
            if (transaction != null)
            {
                if (transaction is SqlTransaction == false)
                {
                    throw new NotSupportedException("The bulk-insert is only applicable for SQL Server database connection.");
                }
                ValidateTransactionConnectionObject(connection, transaction);
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog("BulkInsert", reader, null);
                trace.BeforeBulkInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("BulkInsert");
                    }
                    return 0;
                }
            }

            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var sqlBulkCopy = new SqlBulkCopy((SqlConnection)connection, copyOptions, (SqlTransaction)transaction))
            {
                // Set the destinationtable
                sqlBulkCopy.DestinationTableName = tableName;

                // Set the timeout
                if (bulkCopyTimeout != null && bulkCopyTimeout.HasValue)
                {
                    sqlBulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
                }

                // Add the mappings
                if (mappings != null)
                {
                    foreach (var mapItem in mappings)
                    {
                        sqlBulkCopy.ColumnMappings.Add(mapItem.SourceColumn, mapItem.DestinationColumn);
                    }
                }

                // Open the connection and do the operation
                connection.EnsureOpen();
                await sqlBulkCopy.WriteToServerAsync(reader);

                // Hack the 'SqlBulkCopy' object
                var field = GetRowsCopiedField();

                // Identify if we found the field
                if (field != null)
                {
                    result = (int)field.GetValue(sqlBulkCopy);
                }
                else
                {
                    // Return the record
                    result = reader.RecordsAffected;
                }
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterBulkInsert(new TraceLog("BulkInsert", reader, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion
    }
}
