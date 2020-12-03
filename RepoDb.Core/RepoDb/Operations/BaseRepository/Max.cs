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
        #region Max<TEntity, TResult>

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public object Max(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public object Max(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public object Max(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity>(field: field,
                where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public object Max(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public object Max(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public object Max(Expression<Func<TEntity, object>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public object Max(Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public object Max(Expression<Func<TEntity, object>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity>(field: field,
                where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public object Max(Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public object Max(Expression<Func<TEntity, object>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public TResult Max<TResult>(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public TResult Max<TResult>(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public TResult Max<TResult>(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity, TResult>(field: field,
                where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public TResult Max<TResult>(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public TResult Max<TResult>(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public TResult Max<TResult>(Expression<Func<TEntity, TResult>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public TResult Max<TResult>(Expression<Func<TEntity, TResult>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public TResult Max<TResult>(Expression<Func<TEntity, TResult>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity, TResult>(field: field,
                where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public TResult Max<TResult>(Expression<Func<TEntity, TResult>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Computes the max value of the target field.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The max value of the target field.</returns>
        public TResult Max<TResult>(Expression<Func<TEntity, TResult>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Max<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region MaxAsync<TEntity, TResult>

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<object> MaxAsync(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<object> MaxAsync(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<object> MaxAsync(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<object> MaxAsync(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<object> MaxAsync(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<object> MaxAsync(Expression<Func<TEntity, object>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<object> MaxAsync(Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<object> MaxAsync(Expression<Func<TEntity, object>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<object> MaxAsync(Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<object> MaxAsync(Expression<Func<TEntity, object>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<TResult> MaxAsync<TResult>(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<TResult> MaxAsync<TResult>(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<TResult> MaxAsync<TResult>(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<TResult> MaxAsync<TResult>(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<TResult> MaxAsync<TResult>(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Computes the max value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The max value of the target field.</returns>
        public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.MaxAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
