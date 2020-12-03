using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region DeleteAll<TEntity>

        /// <summary>
        /// Delete the rows from the table.
        /// </summary>
        /// <param name="entities">The list of data entity objects to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int DeleteAll(IEnumerable<TEntity> entities,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAll<TEntity>(entities: entities,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table.
        /// </summary>
        /// <typeparam name="TKey">The type of the key column.</typeparam>
        /// <param name="keys">The list of the keys to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int DeleteAll<TKey>(IEnumerable<TKey> keys,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAll<TEntity, TKey>(keys: keys,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table.
        /// </summary>
        /// <param name="keys">The list of the keys to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int DeleteAll(IEnumerable<object> keys,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAll<TEntity>(keys: keys,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete all the rows from the table.
        /// </summary>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int DeleteAll(string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAll<TEntity>(hints: hints,
                transaction: transaction);
        }

        #endregion

        #region DeleteAllAsync<TEntity>

        /// <summary>
        /// Delete the rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="entities">The list of data entity objects to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAllAsync(IEnumerable<TEntity> entities,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.DeleteAllAsync<TEntity>(entities: entities,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Delete the rows from the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TKey">The type of the key column.</typeparam>
        /// <param name="keys">The list of the keys to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAllAsync<TKey>(IEnumerable<TKey> keys,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.DeleteAllAsync<TEntity, TKey>(keys: keys,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Delete the rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="keys">The list of the keys to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAllAsync(IEnumerable<object> keys,
            string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.DeleteAllAsync<TEntity>(keys: keys,
                hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Delete all the rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAllAsync(string hints = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.DeleteAllAsync<TEntity>(hints: hints,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
