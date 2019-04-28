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
        #region InsertAll<TEntity>

        /// <summary>
        /// Inserts multiple data in the database.
        /// </summary>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public void InsertAll(IEnumerable<TEntity> entities,
            IDbTransaction transaction = null)
        {
            DbRepository.InsertAll<TEntity>(entities: entities,
                transaction: transaction);
        }

        #endregion

        #region InsertAllAsync<TEntity>

        /// <summary>
        /// Inserts multiple data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public Task InsertAllAsync(IEnumerable<TEntity> entities,
            IDbTransaction transaction = null)
        {
            return DbRepository.InsertAllAsync<TEntity>(entities: entities,
                transaction: transaction);
        }

        #endregion
    }
}
