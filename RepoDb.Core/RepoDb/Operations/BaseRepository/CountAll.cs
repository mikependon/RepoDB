using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region CountAll<TEntity>

        /// <summary>
        /// Count the number of rows from the table.
        /// </summary>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An integer value that holds the number of rows from the table.</returns>
        public long CountAll(string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.CountAll<TEntity>(hints: hints,
                transaction: transaction);
        }

        #endregion

        #region CountAllAsync<TEntity>

        /// <summary>
        /// Count the number of rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An integer value that holds the number of rows from the table.</returns>
        public Task<long> CountAllAsync(string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.CountAllAsync<TEntity>(hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
