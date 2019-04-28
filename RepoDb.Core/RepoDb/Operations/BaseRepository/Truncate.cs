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
        #region Truncate<TEntity>

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        public int Truncate()
        {
            return DbRepository.Truncate<TEntity>();
        }

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected.</returns>
        public int Truncate(IDbTransaction transaction = null)
        {
            return DbRepository.Truncate<TEntity>(transaction: transaction);
        }

        #endregion

        #region TruncateAsync<TEntity>

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        public Task<int> TruncateAsync()
        {
            return DbRepository.TruncateAsync<TEntity>();
        }

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected.</returns>
        public Task<int> TruncateAsync(IDbTransaction transaction = null)
        {
            return DbRepository.TruncateAsync<TEntity>(transaction: transaction);
        }

        #endregion
    }
}
