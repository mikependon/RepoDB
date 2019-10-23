using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A base object for all entity-based repositories.
    /// </summary>
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region Maximum<TEntity>

        /// <summary>
        /// Maximums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public object Maximum(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Maximum<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Maximums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public object Maximum(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Maximum<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Maximums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public object Maximum(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Maximum<TEntity>(field: field,
                where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <summary>
        /// Maximums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public object Maximum(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Maximum<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Maximums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public object Maximum(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Maximum<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Maximums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public object Maximum(Expression<Func<TEntity, object>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Maximum<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Maximums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public object Maximum(Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Maximum<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Maximums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public object Maximum(Expression<Func<TEntity, object>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Maximum<TEntity>(field: field,
                where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <summary>
        /// Maximums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public object Maximum(Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Maximum<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Maximums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public object Maximum(Expression<Func<TEntity, object>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Maximum<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region MaximumAsync<TEntity>

        /// <summary>
        /// Maximums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public Task<object> MaximumAsync(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaximumAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Maximums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public Task<object> MaximumAsync(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaximumAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Maximums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public Task<object> MaximumAsync(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaximumAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Maximums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public Task<object> MaximumAsync(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaximumAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Maximums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public Task<object> MaximumAsync(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaximumAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Maximums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public Task<object> MaximumAsync(Expression<Func<TEntity, object>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaximumAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Maximums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public Task<object> MaximumAsync(Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaximumAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Maximums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public Task<object> MaximumAsync(Expression<Func<TEntity, object>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaximumAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Maximums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public Task<object> MaximumAsync(Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaximumAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Maximums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public Task<object> MaximumAsync(Expression<Func<TEntity, object>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaximumAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        #endregion
    }
}
