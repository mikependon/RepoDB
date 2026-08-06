using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RepoDb.Enumerations.SqlServer;
using RepoDb.Interfaces;
using RepoDb.SqlServer.BulkOperations;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="SqlConnection"/> object.
    /// </summary>
    public static partial class SqlConnectionExtension
    {
        #region BulkDeleteByKey<TPrimaryKey>

        /// <summary>
        /// Bulk delete the rows from the database by a list of primary key (or identity) values.
        /// </summary>
        /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-delete operation.</param>
        /// <param name="primaryKeys">The list of the primary keys to be bulk-deleted.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkDeleteByKey<TPrimaryKey>(this SqlConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkDeleteByKey,
            SqlTransaction transaction = null)
        {
            return BulkDeleteByKeyInternalBase(connection: connection,
                tableName: tableName,
                primaryKeys: primaryKeys?.Cast<object>(),
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                transaction: transaction,
                trace: trace,
                traceKey: traceKey);
        }

        #endregion

        #region BulkDeleteByKeyAsync<TPrimaryKey>

        /// <summary>
        /// Bulk delete the rows from the database by a list of primary key (or identity) values in an asynchronous way.
        /// </summary>
        /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-delete operation.</param>
        /// <param name="primaryKeys">The list of the primary keys to be bulk-deleted.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkDeleteByKeyAsync<TPrimaryKey>(this SqlConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkDeleteByKey,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return BulkDeleteByKeyAsyncInternalBase(connection: connection,
                tableName: tableName,
                primaryKeys: primaryKeys?.Cast<object>(),
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                transaction: transaction,
                trace: trace,
                traceKey: traceKey,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
