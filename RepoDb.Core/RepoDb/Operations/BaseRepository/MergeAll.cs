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
        #region MergeAll<TEntity>

        /// <summary>
        /// Merges the multiple data entity objects into the database.
        /// </summary>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public int MergeAll(IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.MergeAll<TEntity>(entities: entities,
                batchSize: batchSize,
                hints: hints,
				transaction: transaction);
        }

        /// <summary>
        /// Merges the multiple data entity objects into the database.
        /// </summary>
        /// <param name="entities">The list of entity objects to be merged.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public int MergeAll(IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.MergeAll<TEntity>(entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                hints: hints,
				transaction: transaction);
        }

        #endregion

        #region MergeAllAsync<TEntity>

        /// <summary>
        /// Merges the multiple dynamic objects into the database. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public Task<int> MergeAllAsync(IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.MergeAllAsync<TEntity>(entities: entities,
                batchSize: batchSize,
                hints: hints,
				transaction: transaction);
        }

        /// <summary>
        /// Merges the multiple dynamic objects into the database. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="entities">The list of entity objects to be merged.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public Task<int> MergeAllAsync(IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.MergeAllAsync<TEntity>(entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                hints: hints,
				transaction: transaction);
        }

        #endregion
    }
}
