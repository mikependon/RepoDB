using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// An extension class for <see cref="BaseRepository{TEntity, TDbConnection}"/> object.
    /// </summary>
    public static partial class BaseRepositoryExtension
    {
        #region Sync

        #region BinaryBulkDeleteByKey<TPrimaryKey>

        /// <summary>
        /// Delete the existing rows by bulk via a list of primary keys. Underneath this operation is a call directly to the existing
        /// <see cref="NpgsqlConnection.BeginBinaryExport(string)"/> method via the 'BinaryImport' extended method.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="primaryKeys">The list of primary keys that targets the rows to be bulk-deleted from the target table.</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the primary keys will be sent together in one-go.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <returns>The number of rows that has been deleted from the target table.</returns>
        public static int BinaryBulkDeleteByKey<TEntity, TPrimaryKey>(this BaseRepository<TEntity, NpgsqlConnection> repository,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null)
            where TEntity : class =>
            repository.DbRepository.BinaryBulkDeleteByKey<TPrimaryKey>(tableName: tableName,
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
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="tableName">The name of the target table from the database.</param>
        /// <param name="primaryKeys">The list of primary keys that targets the rows to be bulk-deleted from the target table.</param>
        /// <param name="bulkCopyTimeout">The timeout expiration of the operation (see <see cref="NpgsqlBinaryImporter.Timeout"/>).</param>
        /// <param name="batchSize">The size per batch to be sent to the database. If not specified, all the primary keys will be sent together in one-go.</param>
        /// <param name="pseudoTableType">The value that defines whether an actual or temporary table will be created for the pseudo-table.</param>
        /// <param name="transaction">The current transaction object in used. If not specified, an implicit transaction will be created and used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been deleted from the target table.</returns>
        public static async Task<int> BinaryBulkDeleteByKeyAsync<TEntity, TPrimaryKey>(this BaseRepository<TEntity, NpgsqlConnection> repository,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            BulkImportPseudoTableType pseudoTableType = default,
            NpgsqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            await repository.DbRepository.BinaryBulkDeleteByKeyAsync<TPrimaryKey>(tableName: tableName,
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
