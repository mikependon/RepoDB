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
        #region MergeAll<TEntity>

        /// <summary>
        /// Insert multiple rows or update the existing rows in the table.
        /// </summary>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public int MergeAll(IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MergeAll<TEntity>(entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Insert multiple rows or update the existing rows in the table.
        /// </summary>
        /// <param name="entities">The list of entity objects to be merged.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public int MergeAll(IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MergeAll<TEntity>(entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Insert multiple rows or update the existing rows in the table.
        /// </summary>
        /// <param name="entities">The list of entity objects to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public int MergeAll(IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MergeAll<TEntity>(entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region MergeAllAsync<TEntity>

        /// <summary>
        /// Insert the multiple dynamic objects (as new rows) or update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is explicitly defined.
        /// </summary>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public Task<int> MergeAllAsync(IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MergeAllAsync<TEntity>(entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Insert the multiple dynamic objects (as new rows) or update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is explicitly defined.
        /// </summary>
        /// <param name="entities">The list of entity objects to be merged.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public Task<int> MergeAllAsync(IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MergeAllAsync<TEntity>(entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Insert the multiple dynamic objects (as new rows) or update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is explicitly defined.
        /// </summary>
        /// <param name="entities">The list of entity objects to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public Task<int> MergeAllAsync(IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MergeAllAsync<TEntity>(entities: entities,
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
