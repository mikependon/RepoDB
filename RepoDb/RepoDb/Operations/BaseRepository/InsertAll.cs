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
        #region InsertAll<TEntity>

        /// <summary>
        /// Inserts multiple data in the database.
        /// </summary>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <returns>The number of inserted rows.</returns>
        public void InsertAll(IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IDbTransaction transaction = null)
        {
            DbRepository.InsertAll<TEntity>(entities: entities,
                batchSize: batchSize,
                transaction: transaction);
        }

        #endregion

        #region InsertAllAsync<TEntity>

        /// <summary>
        /// Inserts multiple data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public Task InsertAllAsync(IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IDbTransaction transaction = null)
        {
            return DbRepository.InsertAllAsync<TEntity>(entities: entities,
                batchSize: batchSize,
                transaction: transaction);
        }

        #endregion
    }
}
