using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region QueryAll<TEntity>

        /// <summary>
        /// Query all the data from the table.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> QueryAll(string tableName,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAll<TEntity>(tableName: tableName,
                fields: fields,
                orderBy: orderBy,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query all the data from the table.
        /// </summary>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> QueryAll(IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAll<TEntity>(fields: fields,
                orderBy: orderBy,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        #endregion

        #region QueryAllAsync<TEntity>

        /// <summary>
        /// Query all the data from the table in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAllAsync(string tableName,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryAllAsync<TEntity>(tableName: tableName,
                fields: fields,
                orderBy: orderBy,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query all the data from the table in an asynchronous way.
        /// </summary>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAllAsync(IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryAllAsync<TEntity>(fields: fields,
                orderBy: orderBy,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
