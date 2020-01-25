using System;
using System.Data;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A base object for all entity-based repositories.
    /// </summary>
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region Insert<TEntity>

        /// <summary>
        /// Inserts a new data in the database.
        /// </summary>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public object Insert(TEntity entity,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.Insert<TEntity>(entity: entity,
                hints: hints,
				transaction: transaction);
        }

        /// <summary>
        /// Inserts a new data in the database.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public TResult Insert<TResult>(TEntity entity,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.Insert<TEntity, TResult>(entity: entity,
                hints: hints,
				transaction: transaction);
        }

        #endregion

        #region InsertAsync<TEntity>

        /// <summary>
        /// Inserts a new data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public Task<object> InsertAsync(TEntity entity,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.InsertAsync<TEntity>(entity: entity,
                hints: hints,
				transaction: transaction);
        }

        /// <summary>
        /// Inserts a new data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public Task<TResult> InsertAsync<TResult>(TEntity entity,
            string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.InsertAsync<TEntity, TResult>(entity: entity,
                hints: hints,
				transaction: transaction);
        }

        #endregion
    }
}
