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

        #region Average

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average(Field field,
            object where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average(Field field,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average(Field field,
            QueryField where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                traceKey: traceKey,
				transaction: transaction,
                hints: hints);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average(Field field,
            IEnumerable<QueryField> where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average(Field field,
            QueryGroup where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average(Expression<Func<TEntity, object>> field,
            object where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average(Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average(Expression<Func<TEntity, object>> field,
            QueryField where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                traceKey: traceKey,
				transaction: transaction,
                hints: hints);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average(Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double Average(Expression<Func<TEntity, object>> field,
            QueryGroup where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<double> AverageAsync(Field field,
            object where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<double> AverageAsync(Field field,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<double> AverageAsync(Field field,
            QueryField where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<double> AverageAsync(Field field,
            IEnumerable<QueryField> where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<double> AverageAsync(Field field,
            QueryGroup where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<double> AverageAsync(Expression<Func<TEntity, object>> field,
            object where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<double> AverageAsync(Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<double> AverageAsync(Expression<Func<TEntity, object>> field,
            QueryField where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<double> AverageAsync(Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<double> AverageAsync(Expression<Func<TEntity, object>> field,
            QueryGroup where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region Average<TResult>

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Field field,
            object where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Field field,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Field field,
            QueryField where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                traceKey: traceKey,
				transaction: transaction,
                hints: hints);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Field field,
            IEnumerable<QueryField> where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Field field,
            QueryGroup where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Expression<Func<TEntity, TResult>> field,
            object where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Expression<Func<TEntity, TResult>> field,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Expression<Func<TEntity, TResult>> field,
            QueryField where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                traceKey: traceKey,
				transaction: transaction,
                hints: hints);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Expression<Func<TEntity, TResult>> field,
            IEnumerable<QueryField> where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult Average<TResult>(Expression<Func<TEntity, TResult>> field,
            QueryGroup where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null)
        {
            return DbRepository.Average<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Field field,
            object where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Field field,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Field field,
            QueryField where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Field field,
            IEnumerable<QueryField> where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Field field,
            QueryGroup where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            object where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            QueryField where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            IEnumerable<QueryField> where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <averagemary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </averagemary>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public Task<TResult> AverageAsync<TResult>(Expression<Func<TEntity, TResult>> field,
            QueryGroup where,
            string hints = null,
            string traceKey = TraceKeys.Average,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.AverageAsync<TEntity, TResult>(field: field,
                where: where,
                hints: hints,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion

    }
}
