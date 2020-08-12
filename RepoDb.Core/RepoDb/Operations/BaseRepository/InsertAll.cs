using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A base object for all entity-based repositories.
    /// </summary>
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region InsertAll<TEntity>(TableName)

        /// <summary>
        /// Insert multiple rows in the table.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of inserted rows in the table.</returns>
        public int InsertAll(string tableName,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.InsertAll<TEntity>(tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region InsertAll<TEntity>

        /// <summary>
        /// Insert multiple rows in the table.
        /// </summary>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of inserted rows in the table.</returns>
        public int InsertAll(IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.InsertAll<TEntity>(entities: entities,
                batchSize: batchSize,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region InsertAll<TEntity>(TableName)

        /// <summary>
        /// Insert multiple rows in the table in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of inserted rows in the table.</returns>
        public Task<int> InsertAllAsync(string tableName,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.InsertAllAsync<TEntity>(tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region InsertAllAsync<TEntity>

        /// <summary>
        /// Insert multiple rows in the table in an asynchronous way.
        /// </summary>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of inserted rows in the table.</returns>
        public Task<int> InsertAllAsync(IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.InsertAllAsync<TEntity>(entities: entities,
                batchSize: batchSize,
                hints: hints,
                transaction: transaction);
        }

        #endregion
    }
}
