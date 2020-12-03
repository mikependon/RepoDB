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
        #region Min<TEntity, TResult>

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public object Min(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public object Min(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public object Min(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity>(field: field,
                where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public object Min(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public object Min(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public object Min(Expression<Func<TEntity, object>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public object Min(Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public object Min(Expression<Func<TEntity, object>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity>(field: field,
                where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public object Min(Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public object Min(Expression<Func<TEntity, object>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public TResult Min<TResult>(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public TResult Min<TResult>(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public TResult Min<TResult>(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity, TResult>(field: field,
                where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public TResult Min<TResult>(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public TResult Min<TResult>(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public TResult Min<TResult>(Expression<Func<TEntity, TResult>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public TResult Min<TResult>(Expression<Func<TEntity, TResult>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public TResult Min<TResult>(Expression<Func<TEntity, TResult>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity, TResult>(field: field,
                where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public TResult Min<TResult>(Expression<Func<TEntity, TResult>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the min value of the target field.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The min value of the target field.</returns>
        public TResult Min<TResult>(Expression<Func<TEntity, TResult>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Min<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region MinAsync<TEntity, TResult>

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<object> MinAsync(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<object> MinAsync(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<object> MinAsync(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<object> MinAsync(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<object> MinAsync(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<object> MinAsync(Expression<Func<TEntity, object>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<object> MinAsync(Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<object> MinAsync(Expression<Func<TEntity, object>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<object> MinAsync(Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<object> MinAsync(Expression<Func<TEntity, object>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<TResult> MinAsync<TResult>(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<TResult> MinAsync<TResult>(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<TResult> MinAsync<TResult>(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<TResult> MinAsync<TResult>(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<TResult> MinAsync<TResult>(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the min value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The min value of the target field.</returns>
        public Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MinAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
