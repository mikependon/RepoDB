using System;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region MinAll<TEntity>

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public object MinAll(Field field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MinAll<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public object MinAll(Expression<Func<TEntity, object>> field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MinAll<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public TResult MinAll<TResult>(Field field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MinAll<TEntity, TResult>(field: field,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public TResult MinAll<TResult>(Expression<Func<TEntity, TResult>> field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MinAll<TEntity, TResult>(field: field,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region MinAllAsync<TEntity>

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<object> MinAllAsync(Field field,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAllAsync<TEntity>(field: field,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<object> MinAllAsync(Expression<Func<TEntity, object>> field,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAllAsync<TEntity>(field: field,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<TResult> MinAllAsync<TResult>(Field field,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAllAsync<TEntity, TResult>(field: field,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<TResult> MinAllAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAllAsync<TEntity, TResult>(field: field,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
