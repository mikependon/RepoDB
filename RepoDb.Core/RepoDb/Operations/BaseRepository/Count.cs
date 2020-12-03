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
        #region Count<TEntity>

        /// <summary>
        /// Count the number of rows from the table.
        /// </summary>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An integer value that holds the number of rows from the table.</returns>
        public long Count(object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Count the number of rows from the table.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An integer value that holds the number of rows from the table.</returns>
        public long Count(Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Count the number of rows from the table.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An integer value that holds the number of rows from the table.</returns>
        public long Count(QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <summary>
        /// Count the number of rows from the table.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An integer value that holds the number of rows from the table.</returns>
        public long Count(IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Count the number of rows from the table.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An integer value that holds the number of rows from the table.</returns>
        public long Count(QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Count<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region CountAsync<TEntity>

        /// <summary>
        /// Count the number of rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An integer value that holds the number of rows from the table.</returns>
        public Task<long> CountAsync(object where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Count the number of rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An integer value that holds the number of rows from the table.</returns>
        public Task<long> CountAsync(Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Count the number of rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An integer value that holds the number of rows from the table.</returns>
        public Task<long> CountAsync(QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Count the number of rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An integer value that holds the number of rows from the table.</returns>
        public Task<long> CountAsync(IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Count the number of rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An integer value that holds the number of rows from the table.</returns>
        public Task<long> CountAsync(QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.CountAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
