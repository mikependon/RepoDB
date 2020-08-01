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
        /// Inserts the multiple data entity objects (as new rows) in the table.
        /// </summary>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
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

        #region InsertAllAsync<TEntity>

        /// <summary>
        /// Inserts the multiple data entity objects (as new rows) in the table in an asynchronous way.
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
