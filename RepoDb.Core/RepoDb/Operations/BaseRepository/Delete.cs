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
        #region Delete<TEntity>

        /// <summary>
        /// Deletes an existing data from the database.
        /// </summary>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete(object whereOrPrimaryKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(whereOrPrimaryKey: whereOrPrimaryKey,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing data from the database.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete(Expression<Func<TEntity, bool>> where = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing data from the database.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete(QueryField where = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing data from the database.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete(IEnumerable<QueryField> where = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing data from the database.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public int Delete(QueryGroup where = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                transaction: transaction);
        }

        #endregion

        #region DeleteAsync<TEntity>

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<int> DeleteAsync(object whereOrPrimaryKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(whereOrPrimaryKey: whereOrPrimaryKey,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<int> DeleteAsync(Expression<Func<TEntity, bool>> where = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<int> DeleteAsync(QueryField where = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<int> DeleteAsync(IEnumerable<QueryField> where = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing data from the database in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public Task<int> DeleteAsync(QueryGroup where = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                transaction: transaction);
        }

        #endregion
    }
}
