using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class to be a database operation provider.
    /// </summary>
    public interface IDbOperation
    {
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
        int BulkInsert<TEntity>(IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            IDbTransaction transaction = null)
            where TEntity : class;

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
        int BulkInsert<TEntity>(IDbConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            IDbTransaction transaction = null)
            where TEntity : class;

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
        int BulkInsert(IDbConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            IDbTransaction transaction = null);

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
        Task<int> BulkInsertAsync<TEntity>(IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            IDbTransaction transaction = null)
            where TEntity : class;

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
        Task<int> BulkInsertAsync<TEntity>(IDbConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            IDbTransaction transaction = null)
            where TEntity : class;

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
        Task<int> BulkInsertAsync(IDbConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            IDbTransaction transaction = null);

        #endregion
    }
}
