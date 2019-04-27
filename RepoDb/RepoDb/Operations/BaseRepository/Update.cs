using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A base object for all entity-based repositories.
    /// </summary>
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region Update<TEntity>

        /// <summary>
        /// Updates an existing data in the database.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update(TEntity entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update(TEntity entity,
            object whereOrPrimaryKey,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                whereOrPrimaryKey: whereOrPrimaryKey,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update(TEntity entity,
            Expression<Func<TEntity, bool>> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update(TEntity entity,
            QueryField where,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update(TEntity entity,
            IEnumerable<QueryField> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database based on the given query expression.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Update(TEntity entity,
            QueryGroup where,
            IDbTransaction transaction = null)
        {
            return DbRepository.Update<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        #endregion

        #region UpdateAsync<TEntity>

        /// <summary>
        /// Updates an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<int> UpdateAsync(TEntity entity,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<int> UpdateAsync(TEntity entity,
            object whereOrPrimaryKey,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                whereOrPrimaryKey: whereOrPrimaryKey,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<int> UpdateAsync(TEntity entity,
            Expression<Func<TEntity, bool>> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<int> UpdateAsync(TEntity entity,
            QueryField where,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<int> UpdateAsync(TEntity entity,
            IEnumerable<QueryField> where,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<int> UpdateAsync(TEntity entity,
            QueryGroup where,
            IDbTransaction transaction = null)
        {
            return DbRepository.UpdateAsync<TEntity>(entity: entity,
                where: where,
                transaction: transaction);
        }

        #endregion
    }
}
