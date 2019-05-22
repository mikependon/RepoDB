using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static partial class DbConnectionExtension
    {
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
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : class
        {
            var provider = GetDbOperationProvider(connection);

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
                entities = (IEnumerable<TEntity>)cancellableTraceLog.Parameter ?? entities;
            }

            // Variables for the operation
            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual execution
            result = provider.BulkInsert<TEntity>(connection: connection,
                entities: entities,
                mappings: mappings,
                copyOptions: copyOptions,
                bulkCopyTimeout: bulkCopyTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterBulkInsert(new TraceLog("BulkInsert", entities, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
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
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this IDbConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : class
        {
            var provider = GetDbOperationProvider(connection);

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
                reader = (DbDataReader)cancellableTraceLog.Parameter ?? reader;
            }

            // Variables for the operation
            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual execution
            result = provider.BulkInsert<TEntity>(connection: connection,
                reader: reader,
                mappings: mappings,
                copyOptions: copyOptions,
                bulkCopyTimeout: bulkCopyTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterBulkInsert(new TraceLog("BulkInsert", reader, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
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
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert(this IDbConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            var provider = GetDbOperationProvider(connection);

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
                reader = (DbDataReader)cancellableTraceLog.Parameter ?? reader;
            }

            // Variables for the operation
            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual execution
            result = provider.BulkInsert(connection: connection,
                tableName: tableName,
                reader: reader,
                mappings: mappings,
                copyOptions: copyOptions,
                bulkCopyTimeout: bulkCopyTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterBulkInsert(new TraceLog("BulkInsert", reader, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
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
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkInsertAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : class
        {
            var provider = GetDbOperationProvider(connection);

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
                entities = (IEnumerable<TEntity>)cancellableTraceLog.Parameter ?? entities;
            }

            // Variables for the operation
            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual execution
            result = await provider.BulkInsertAsync<TEntity>(connection: connection,
                entities: entities,
                mappings: mappings,
                copyOptions: copyOptions,
                bulkCopyTimeout: bulkCopyTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterBulkInsert(new TraceLog("BulkInsert", entities, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
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
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkInsertAsync<TEntity>(this IDbConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : class
        {
            var provider = GetDbOperationProvider(connection);

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
                reader = (DbDataReader)cancellableTraceLog.Parameter ?? reader;
            }

            // Variables for the operation
            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual execution
            result = await provider.BulkInsertAsync<TEntity>(connection: connection,
                reader: reader,
                mappings: mappings,
                copyOptions: copyOptions,
                bulkCopyTimeout: bulkCopyTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterBulkInsert(new TraceLog("BulkInsert", reader, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
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
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkInsertAsync(this IDbConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            var provider = GetDbOperationProvider(connection);

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
                reader = (DbDataReader)cancellableTraceLog.Parameter ?? reader;
            }

            // Variables for the operation
            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual execution
            result = await provider.BulkInsertAsync(connection: connection,
                tableName: tableName,
                reader: reader,
                mappings: mappings,
                copyOptions: copyOptions,
                bulkCopyTimeout: bulkCopyTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterBulkInsert(new TraceLog("BulkInsert", reader, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the <see cref="SqlBulkCopy"/> private variable reflected field.
        /// </summary>
        /// <returns>The actual field.</returns>
        private static IDbOperationProvider GetDbOperationProvider(IDbConnection connection)
        {
            // Check the connection
            if (connection == null)
            {
                throw new NullReferenceException("The connection object cannot be null.");
            }

            // Get the provider
            var provider = DbOperationProviderMapper.Get(connection.GetType());

            // Check the presence
            if (provider == null)
            {
                throw new MissingMappingException($"There is no database operation provider mapping found for '{connection.GetType().FullName}'.");
            }

            // Return the provider
            return provider;
        }

        #endregion

    }
}
