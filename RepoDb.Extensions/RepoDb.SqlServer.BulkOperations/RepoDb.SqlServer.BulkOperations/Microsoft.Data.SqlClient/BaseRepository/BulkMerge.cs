using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// An extension class for <see cref="BaseRepository{TEntity, TDbConnection}"/> object.
    /// </summary>
    public static partial class BaseRepositoryExtension
    {
        #region BulkMerge<TEntity>

        /// <summary>
        /// Bulk merge a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return repository.DbRepository.BulkMerge<TEntity>(entities: entities,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk merge a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return repository.DbRepository.BulkMerge<TEntity>(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        #endregion

        #region BulkMergeAsync<TEntity>

        /// <summary>
        /// Bulk merge a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkMergeAsync<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return repository.DbRepository.BulkMergeAsync<TEntity>(entities: entities,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        /// <summary>
        /// Bulk merge a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkMergeAsync<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions? options = null,
            string hints = null,
            int? batchSize = null,
            bool? usePhysicalPseudoTempTable = null,
            SqlTransaction transaction = null)
            where TEntity : class
        {
            return repository.DbRepository.BulkMergeAsync<TEntity>(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: usePhysicalPseudoTempTable,
                transaction: transaction);
        }

        #endregion
    }
}
