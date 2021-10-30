using Npgsql;
using RepoDb.Enumerations.PostgreSql;
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

        #region BinaryBulkDeleteByKey<TPrimaryKey>

        /// <summary>
        /// Delete the existing rows by bulk via a list of primary keys. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the 'BinaryImport' extended method.
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
        public static int BinaryBulkDeleteByKey<TPrimaryKey>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
            where TPrimaryKey : class =>
            BinaryBulkDeleteByKeyBase<TPrimaryKey>(connection: connection,
                tableName: ClassMappedNameCache.Get<TPrimaryKey>(),
                primaryKeys: primaryKeys,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                transaction: transaction);

        #endregion

        #endregion

        #region Async

        #region BinaryBulkDeleteByKey<TPrimaryKey>

        /// <summary>
        /// Delete the existing rows by bulk via a list of primary keys in an asynchronous way. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the 'BinaryImport' extended method.
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
        public static async Task<int> BinaryBulkDeleteByKeyAsync<TPrimaryKey>(this NpgsqlConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TPrimaryKey : class =>
            await BinaryBulkDeleteByKeyBaseAsync<TPrimaryKey>(connection: connection,
                tableName: ClassMappedNameCache.Get<TPrimaryKey>(),
                primaryKeys: primaryKeys,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                transaction: transaction,
                cancellationToken: cancellationToken);

        #endregion

        #endregion
    }
}
