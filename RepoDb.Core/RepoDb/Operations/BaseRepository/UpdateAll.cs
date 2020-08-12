using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A base object for all entity-based repositories.
    /// </summary>
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region UpdateAll<TEntity>(TableName)

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public int UpdateAll(string tableName,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAll<TEntity>(tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public int UpdateAll(string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAll<TEntity>(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of entity objects to be used for update.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public int UpdateAll(string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAll<TEntity>(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region UpdateAll<TEntity>

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
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
        /// Update the existing rows in the table.
        /// </summary>
        /// <param name="entities">The list of entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
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

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <param name="entities">The list of entity objects to be used for update.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public int UpdateAll(IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers,
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

        #region UpdateAllAsync<TEntity>(TableName)

        /// <summary>
        /// Update the existing rows in the table in an aysnchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public Task<int> UpdateAllAsync(string tableName,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAllAsync<TEntity>(tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Update the existing rows in the table in an aysnchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public Task<int> UpdateAllAsync(string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAllAsync<TEntity>(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Update the existing rows in the table in an aysnchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of entity objects to be used for update.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public Task<int> UpdateAllAsync(string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAllAsync<TEntity>(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region UpdateAllAsync<TEntity>

        /// <summary>
        /// Update the existing rows in the table in an aysnchronous way.
        /// </summary>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
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
        /// Update the existing rows in the table in an aysnchronous way.
        /// </summary>
        /// <param name="entities">The list of entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
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

        /// <summary>
        /// Update the existing rows in the table in an aysnchronous way.
        /// </summary>
        /// <param name="entities">The list of entity objects to be used for update.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public Task<int> UpdateAllAsync(IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers,
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
