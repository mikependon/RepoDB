using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="NpgsqlConnection"/> object.
    /// </summary>
    public static partial class NpgsqlConnectionExtension
    {
        #region Sync

        #region BulkDeleteByKey<TPrimaryKey>

        /// <summary>
        /// Delete the existing rows by bulk via a list of primary keys. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the 'BinaryImportInternal' extended method.
        /// </summary>
        /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
        /// <param name="connection">The current connection object in used.</param>
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="primaryKeys">The list of primary keys that targets the rows to be bulk-deleted from the target table.</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the primary keys will be sent together in one-go.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows that has been deleted from the target table.</returns>
        public static int BulkDeleteByKey<TPrimaryKey>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            PostgreSqlBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = PostgreSqlTraceKeys.PostgreSqlBulkDeleteByKey,
            NpgsqlTransaction transaction = null) =>
            BulkDeleteByKeyBase<TPrimaryKey>(connection: connection,
                tableName: tableName,
                primaryKeys: primaryKeys,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction);

        #endregion

        #endregion

        #region Async

        #region BulkDeleteByKey<TPrimaryKey>

        /// <summary>
        /// Delete the existing rows by bulk via a list of primary keys in an asynchronous way. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the 'BinaryImportInternal' extended method.
        /// </summary>
        /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
        /// <param name="connection">The current connection object in used.</param>
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="primaryKeys">The list of primary keys that targets the rows to be bulk-deleted from the target table.</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the primary keys will be sent together in one-go.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been deleted from the target table.</returns>
        public static async Task<int> BulkDeleteByKeyAsync<TPrimaryKey>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            PostgreSqlBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = PostgreSqlTraceKeys.PostgreSqlBulkDeleteByKey,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            await BulkDeleteByKeyBaseAsync<TPrimaryKey>(connection: connection,
                tableName: tableName,
                primaryKeys: primaryKeys,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction,
                cancellationToken: cancellationToken);

        #endregion

        #endregion
    }
}
