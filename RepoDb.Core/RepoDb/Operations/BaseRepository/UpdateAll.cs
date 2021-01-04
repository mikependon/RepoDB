using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region UpdateAll<TEntity>

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public int UpdateAll(IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAll<TEntity>(entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <param name="entities">The list of entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public int UpdateAll(IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAll<TEntity>(entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <param name="entities">The list of entity objects to be used for update.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public int UpdateAll(IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAll<TEntity>(entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region UpdateAllAsync<TEntity>

        /// <summary>
        /// Update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public Task<int> UpdateAllAsync(IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.UpdateAllAsync<TEntity>(entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <param name="entities">The list of entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public Task<int> UpdateAllAsync(IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.UpdateAllAsync<TEntity>(entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <param name="entities">The list of entity objects to be used for update.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public Task<int> UpdateAllAsync(IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.UpdateAllAsync<TEntity>(entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
