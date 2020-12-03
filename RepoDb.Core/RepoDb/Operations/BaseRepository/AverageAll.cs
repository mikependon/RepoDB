using System;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region AverageAll<TEntity>

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public object AverageAll(Field field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.AverageAll<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public object AverageAll(Expression<Func<TEntity, object>> field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.AverageAll<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult AverageAll<TResult>(Field field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.AverageAll<TEntity, TResult>(field: field,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult AverageAll<TResult>(Expression<Func<TEntity, TResult>> field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.AverageAll<TEntity, TResult>(field: field,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region AverageAllAsync<TEntity>

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<object> AverageAllAsync(Field field,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAllAsync<TEntity>(field: field,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<object> AverageAllAsync(Expression<Func<TEntity, object>> field,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAllAsync<TEntity>(field: field,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAllAsync<TResult>(Field field,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAllAsync<TEntity, TResult>(field: field,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAllAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAllAsync<TEntity, TResult>(field: field,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
