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
        #region CountAll<TEntity>

        /// <summary>
        /// Counts all the number of data from the database table.
        /// </summary>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An integer value that holds the number of data from the database.</returns>
        public long CountAll(string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.CountAll<TEntity>(hints: hints,
                transaction: transaction);
        }

        #endregion

        #region CountAllAsync<TEntity>

        /// <summary>
        /// Counts all the number of data from the database table in an asynchronous way.
        /// </summary>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public Task<long> CountAllAsync(string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.CountAllAsync<TEntity>(hints: hints,
                transaction: transaction);
        }

        #endregion
    }
}
