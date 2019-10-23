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
        #region MinimumAll<TEntity>

        /// <summary>
        /// Extracts the minimum value of the target field from all data of the database table.
        /// </summary>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public object MinimumAll(Field field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MinimumAll<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Extracts the minimum value of the target field from all data of the database table.
        /// </summary>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The minimum value.</returns>
        public object MinimumAll(Expression<Func<TEntity, object>> field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MinimumAll<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region MinimumAllAsync<TEntity>

        /// <summary>
        /// Extracts the minimum value of the target field from all data of the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public Task<object> MinimumAllAsync(Field field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MinimumAllAsync<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Extracts the minimum value of the target field from all data of the database table in an asynchronous way.
        /// </summary>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public Task<object> MinimumAllAsync(Expression<Func<TEntity, object>> field,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.MinimumAllAsync<TEntity>(field: field,
                hints: hints,
                transaction: transaction);
        }

        #endregion
    }
}
