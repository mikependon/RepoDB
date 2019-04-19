using System;
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

        #endregion
    }
}
