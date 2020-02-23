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
        #region UpdateAll<TEntity>

        /// <summary>
        /// Updates existing multiple data in the database.
        /// </summary>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public int UpdateAll(IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAll<TEntity>(entities: entities,
                batchSize: batchSize,
                hints: hints,
				transaction: transaction);
        }

        /// <summary>
        /// Updates existing multiple data in the database.
        /// </summary>
        /// <param name="entities">The list of entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public int UpdateAll(IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAll<TEntity>(entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                hints: hints,
				transaction: transaction);
        }

        #endregion

        #region UpdateAllAsync<TEntity>

        /// <summary>
        /// Updates existing multiple data in the database in an aysnchronous way.
        /// </summary>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public Task<int> UpdateAllAsync(IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAllAsync<TEntity>(entities: entities,
                batchSize: batchSize,
                hints: hints,
				transaction: transaction);
        }

        /// <summary>
        /// Updates existing multiple data in the database in an aysnchronous way.
        /// </summary>
        /// <param name="entities">The list of entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public Task<int> UpdateAllAsync(IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAllAsync<TEntity>(entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                hints: hints,
				transaction: transaction);
        }

        #endregion
    }
}
