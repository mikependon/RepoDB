using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RepoDb.Enumerations.SqlServer;
using RepoDb.Interfaces;
using RepoDb.SqlServer.BulkOperations;

namespace RepoDb
{
    /// <summary>
    /// An extension class for <see cref="DbRepository{TDbConnection}"/> object.
    /// </summary>
    public static partial class DbRepositoryExtension
    {
        #region BulkDeleteByKey<TPrimaryKey>

        /// <summary>
        /// Bulk delete the rows from the database by a list of primary key (or identity) values.
        /// </summary>
        /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-delete operation.</param>
        /// <param name="primaryKeys">The list of the primary keys to be bulk-deleted.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkDeleteByKey<TPrimaryKey>(this DbRepository<SqlConnection> repository,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            SqlBulkCopyOptions options = default,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkDeleteByKey,
            SqlTransaction transaction = null)
        {
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

            //// Call the method
            return bulkDbConnector.Connection.BulkDeleteByKey(tableName: tableName,
                primaryKeys: primaryKeys,
                options: options,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction);
        }

        #endregion

        #region BulkDeleteByKeyAsync<TPrimaryKey>

        /// <summary>
        /// Bulk delete the rows from the database by a list of primary key (or identity) values in an asynchronous way.
        /// </summary>
        /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-delete operation.</param>
        /// <param name="primaryKeys">The list of the primary keys to be bulk-deleted.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkDeleteByKeyAsync<TPrimaryKey>(this DbRepository<SqlConnection> repository,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            SqlBulkCopyOptions options = default,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkDeleteByKey,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

            //// Call the method
            return await bulkDbConnector.Connection.BulkDeleteByKeyAsync(tableName: tableName,
                primaryKeys: primaryKeys,
                options: options,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
