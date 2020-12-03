using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
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
            return DbRepository.TruncateAsync<TEntity>(transaction: null,
                cancellationToken: CancellationToken.None);
        }

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        public Task<int> TruncateAsync(CancellationToken cancellationToken = default)
        {
            return DbRepository.TruncateAsync<TEntity>(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        public Task<int> TruncateAsync(IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.TruncateAsync<TEntity>(transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
