using System;
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
        #region SumAll<TEntity>

        /// <summary>
        /// Sums the target field from all data of the database table.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public object SumAll(Field field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.SumAll<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Sums the target field from all data of the database table.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The sum value.</returns>
        public object SumAll(Expression<Func<TEntity, object>> field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.SumAll<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region SumAllAsync<TEntity>

        /// <summary>
        /// Sums the target field from all data of the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public Task<object> SumAllAsync(Field field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.SumAllAsync<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Sums the target field from all data of the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public Task<object> SumAllAsync(Expression<Func<TEntity, object>> field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.SumAllAsync<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        #endregion
    }
}
