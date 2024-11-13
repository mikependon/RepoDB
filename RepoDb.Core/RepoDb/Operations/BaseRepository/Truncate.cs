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
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <returns>The number of rows affected.</returns>
        public int Truncate(string traceKey = TraceKeys.Truncate)
        {
            return DbRepository.Truncate<TEntity>(traceKey: traceKey);
        }

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected.</returns>
        public int Truncate(string traceKey = TraceKeys.Truncate,
            IDbTransaction? transaction = null)
        {
            return DbRepository.Truncate<TEntity>(traceKey: traceKey,
                transaction: transaction);
        }

        #endregion

        #region TruncateAsync<TEntity>

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <returns>The number of rows affected.</returns>
        public Task<int> TruncateAsync(string traceKey = TraceKeys.Truncate)
        {
            return DbRepository.TruncateAsync<TEntity>(transaction: null,
                cancellationToken: CancellationToken.None);
        }

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
		/// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        public Task<int> TruncateAsync(string traceKey = TraceKeys.Truncate,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.TruncateAsync<TEntity>(traceKey: traceKey,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        public Task<int> TruncateAsync(string traceKey = TraceKeys.Truncate,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.TruncateAsync<TEntity>(traceKey: traceKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
