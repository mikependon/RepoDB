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
        #region Update<TEntity>

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public int Update(TEntity entity,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public int Update<TWhat>(TEntity entity,
            TWhat what,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity, TWhat>(entity: entity,
                what: what,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public int Update(TEntity entity,
            object what,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                what: what,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public int Update(TEntity entity,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public int Update(TEntity entity,
            QueryField where,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public int Update(TEntity entity,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public int Update(TEntity entity,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                fields: fields,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region UpdateAsync<TEntity>

        /// <summary>
        /// Updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public Task<int> UpdateAsync(TEntity entity,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public Task<int> UpdateAsync<TWhat>(TEntity entity,
            TWhat what,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.UpdateAsync<TEntity, TWhat>(entity: entity,
                what: what,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public Task<int> UpdateAsync(TEntity entity,
            object what,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                what: what,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public Task<int> UpdateAsync(TEntity entity,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public Task<int> UpdateAsync(TEntity entity,
            QueryField where,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public Task<int> UpdateAsync(TEntity entity,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public Task<int> UpdateAsync(TEntity entity,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                fields: fields,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
