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
        #region Insert<TEntity>

        /// <summary>
        /// Inserts a new data in the database.
        /// </summary>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public object Insert(TEntity entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.Insert<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Inserts a new data in the database.
        /// </summary>
        /// <typeparam name="TResult">The type of the primary key result.</typeparam>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public TResult Insert<TResult>(TEntity entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.Insert<TEntity, TResult>(entity: entity,
                transaction: transaction);
        }

        #endregion

        #region InsertAsync<TEntity>

        /// <summary>
        /// Inserts a new data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public Task<object> InsertAsync(TEntity entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.InsertAsync<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Inserts a new data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The type of the primary key result.</typeparam>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public Task<TResult> InsertAsync<TResult>(TEntity entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.InsertAsync<TEntity, TResult>(entity: entity,
                transaction: transaction);
        }

        #endregion
    }
}
