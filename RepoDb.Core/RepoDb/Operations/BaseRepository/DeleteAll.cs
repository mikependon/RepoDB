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
        #region DeleteAll<TEntity>

        /// <summary>
        /// Deletes all the data from the database.
        /// </summary>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public int DeleteAll(string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAll<TEntity>(hints: hints,
				transaction: transaction);
        }

        #endregion

        #region DeleteAllAsync<TEntity>

        /// <summary>
        /// Deletes all the data from the database in an asynchronous way.
        /// </summary>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public Task<int> DeleteAllAsync(string hints = null,
			IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAllAsync<TEntity>();
        }

        #endregion
    }
}
