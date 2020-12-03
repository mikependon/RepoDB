using System;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region MaxAll<TEntity>

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public object MaxAll(Field field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaxAll<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public object MaxAll(Expression<Func<TEntity, object>> field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaxAll<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public TResult MaxAll<TResult>(Field field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaxAll<TEntity, TResult>(field: field,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public TResult MaxAll<TResult>(Expression<Func<TEntity, TResult>> field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaxAll<TEntity, TResult>(field: field,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region MaxAllAsync<TEntity>

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<object> MaxAllAsync(Field field,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAllAsync<TEntity>(field: field,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<object> MaxAllAsync(Expression<Func<TEntity, object>> field,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAllAsync<TEntity>(field: field,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<TResult> MaxAllAsync<TResult>(Field field,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAllAsync<TEntity, TResult>(field: field,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<TResult> MaxAllAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAllAsync<TEntity, TResult>(field: field,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
