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
        #region Delete<TEntity>

        /// <summary>
        /// Deletes an existing row from the table.
        /// </summary>
        /// <param name="entity">The data entity object to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int Delete(TEntity entity,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(entity: entity,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int Delete<TWhat>(TWhat what,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity, TWhat>(what: what,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table.
        /// </summary>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int Delete(object what,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(what: what,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int Delete(Expression<Func<TEntity, bool>> where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int Delete(QueryField where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int Delete(IEnumerable<QueryField> where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int Delete(QueryGroup where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region DeleteAsync<TEntity>

        /// <summary>
        /// Deletes an existing row from the table in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAsync(TEntity entity,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.DeleteAsync<TEntity>(entity: entity,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Delete the rows from the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAsync<TWhat>(TWhat what,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.DeleteAsync<TEntity, TWhat>(what: what,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Delete the rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAsync(object what,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.DeleteAsync<TEntity>(what: what,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Delete the rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAsync(Expression<Func<TEntity, bool>> where,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Delete the rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAsync(QueryField where,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Delete the rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAsync(IEnumerable<QueryField> where,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Delete the rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAsync(QueryGroup where,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
