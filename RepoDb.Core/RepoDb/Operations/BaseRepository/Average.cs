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
        #region Average<TEntity, TResult>

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public object Average(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public object Average(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public object Average(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public object Average(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public object Average(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public object Average(Expression<Func<TEntity, object>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public object Average(Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public object Average(Expression<Func<TEntity, object>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public object Average(Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public object Average(Expression<Func<TEntity, object>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Expression<Func<TEntity, TResult>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Expression<Func<TEntity, TResult>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Expression<Func<TEntity, TResult>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Expression<Func<TEntity, TResult>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Expression<Func<TEntity, TResult>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region AverageAsync<TEntity, TResult>

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<object> AverageAsync(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<object> AverageAsync(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<object> AverageAsync(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<object> AverageAsync(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<object> AverageAsync(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<object> AverageAsync(Expression<Func<TEntity, object>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<object> AverageAsync(Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<object> AverageAsync(Expression<Func<TEntity, object>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<object> AverageAsync(Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<object> AverageAsync(Expression<Func<TEntity, object>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
