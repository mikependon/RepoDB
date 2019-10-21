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
        #region Sum<TEntity>

        /// <summary>
        /// Sums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public object Sum(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Sum<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Sums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public object Sum(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Sum<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Sums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public object Sum(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Sum<TEntity>(field: field,
                where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <summary>
        /// Sums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public object Sum(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Sum<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Sums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public object Sum(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Sum<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Sums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public object Sum(Expression<Func<TEntity, object>> field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Sum<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Sums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public object Sum(Expression<Func<TEntity, object>> field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Sum<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Sums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public object Sum(Expression<Func<TEntity, object>> field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Sum<TEntity>(field: field,
                where: where,
                transaction: transaction,
                hints: hints);
        }

        /// <summary>
        /// Sums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public object Sum(Expression<Func<TEntity, object>> field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Sum<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Sums the target field from the database table.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public object Sum(Expression<Func<TEntity, object>> field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Sum<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region SumAsync<TEntity>

        /// <summary>
        /// Sums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public Task<object> SumAsync(Field field,
            object where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.SumAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Sums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public Task<object> SumAsync(Field field,
            Expression<Func<TEntity, bool>> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.SumAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Sums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public Task<object> SumAsync(Field field,
            QueryField where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.SumAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Sums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public Task<object> SumAsync(Field field,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.SumAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Sums the target field from the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public Task<object> SumAsync(Field field,
            QueryGroup where = null,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.SumAsync<TEntity>(field: field,
                where: where,
                hints: hints,
                transaction: transaction);
        }

        #endregion
    }
}
