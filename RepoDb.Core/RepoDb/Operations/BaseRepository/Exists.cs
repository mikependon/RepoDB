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
        #region Exists<TEntity>

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public bool Exists(object what = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Exists<TEntity>(what: what,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public bool Exists(Expression<Func<TEntity, bool>> where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Exists<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public bool Exists(QueryField where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Exists<TEntity>(where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public bool Exists(IEnumerable<QueryField> where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Exists<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Check whether the rows are existing in the table.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public bool Exists(QueryGroup where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Exists<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region ExistsAsync<TEntity>

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public Task<bool> ExistsAsync(object what,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.ExistsAsync<TEntity>(what: what,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public Task<bool> ExistsAsync<TWhat>(TWhat what,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.ExistsAsync<TEntity, TWhat>(what: what,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> where,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.ExistsAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public Task<bool> ExistsAsync(QueryField where,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.ExistsAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public Task<bool> ExistsAsync(IEnumerable<QueryField> where,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.ExistsAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Check whether the rows are existing in the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A boolean value that indicates whether the rows are existing in the table.</returns>
        public Task<bool> ExistsAsync(QueryGroup where,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.ExistsAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
