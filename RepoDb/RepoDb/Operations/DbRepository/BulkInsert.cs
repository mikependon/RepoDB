using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A base object for all shared-based repositories.
    /// </summary>
    public partial class DbRepository<TDbConnection> : IDisposable where TDbConnection : DbConnection
    {
        #region BulkInsert

        /// <summary>
        /// Bulk insert a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="copyOptions">The bulk-copy options to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int BulkInsert<TEntity>(IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            using (var connection = CreateConnection())
            {
                // Call the method
                return connection.BulkInsert<TEntity>(entities: entities,
                    mappings: mappings,
                    copyOptions: copyOptions,
                    bulkCopyTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace);
            }
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="copyOptions">The bulk-copy options to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int BulkInsert<TEntity>(DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            using (var connection = CreateConnection())
            {

                // Call the method
                return connection.BulkInsert<TEntity>(reader: reader,
                    mappings: mappings,
                    copyOptions: copyOptions,
                    bulkCopyTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace);
            }
        }

        #endregion

        #region BulkInsertAsync

        /// <summary>
        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="copyOptions">The bulk-copy options to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> BulkInsertAsync<TEntity>(IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = CreateConnection();

            try
            {
                // Call the method
                return await connection.BulkInsertAsync<TEntity>(entities: entities,
                    mappings: mappings,
                    copyOptions: copyOptions,
                    bulkCopyTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection);
            }
        }

        /// <summary>
        /// Bulk insert an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="copyOptions">The bulk-copy options to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public async Task<int> BulkInsertAsync<TEntity>(DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var connection = CreateConnection();

            try
            {
                // Call the method
                return await connection.BulkInsertAsync<TEntity>(reader: reader,
                    mappings: mappings,
                    copyOptions: copyOptions,
                    bulkCopyTimeout: CommandTimeout,
                    transaction: transaction,
                    trace: Trace);
            }
            catch
            {
                // Throw back the error
                throw;
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(connection);
            }
        }

        #endregion
    }
}
