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
        #region MaximumAll<TEntity>

        /// <summary>
        /// Extracts the maximum value of the target field from all data of the database table.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public object MaximumAll(Field field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaximumAll<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Extracts the maximum value of the target field from all data of the database table.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The maximum value.</returns>
        public object MaximumAll(Expression<Func<TEntity, object>> field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaximumAll<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region MaximumAllAsync<TEntity>

        /// <summary>
        /// Extracts the maximum value of the target field from all data of the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public Task<object> MaximumAllAsync(Field field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaximumAllAsync<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Extracts the maximum value of the target field from all data of the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public Task<object> MaximumAllAsync(Expression<Func<TEntity, object>> field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MaximumAllAsync<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        #endregion
    }
}
