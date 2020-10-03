using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// An extension class for <see cref="BaseRepository{TEntity, TDbConnection}"/> object.
    /// </summary>
    public static partial class BaseRepositoryExtension
    {
        #region BulkInsert<TEntity>

        /// <summary>
        /// Bulk insert a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return repository.DbRepository.BulkInsert<TEntity>(entities: entities,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk insert a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return repository.DbRepository.BulkInsert<TEntity>(tableName: tableName,
                entities: entities,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        #endregion

        #region BulkInsertAsync<TEntity>

        /// <summary>
        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkInsertAsync<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return repository.DbRepository.BulkInsertAsync<TEntity>(entities: entities,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkInsertAsync<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return repository.DbRepository.BulkInsertAsync<TEntity>(tableName: tableName,
                entities: entities,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                isReturnIdentity: isReturnIdentity,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
