using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A base object for all entity-based repositories.
    /// </summary>
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region QueryAll<TEntity>(TableName)

        /// <summary>
        /// Query all the data from the table.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> QueryAll(string tableName,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAll<TEntity>(tableName: tableName,
                orderBy: orderBy,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        #endregion

        #region QueryAll<TEntity>

        /// <summary>
        /// Query all the data from the table.
        /// </summary>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> QueryAll(IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAll<TEntity>(orderBy: orderBy,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        #endregion

        #region QueryAllAsync<TEntity>

        /// <summary>
        /// Query all the data from the table in an asynchronous way.
        /// </summary>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAllAsync(IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAllAsync<TEntity>(orderBy: orderBy,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        #endregion
    }
}
