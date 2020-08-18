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
        /// Deletes an existing row from the table.
        /// </summary>
        /// <param name="entity">The data entity object to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int Delete(TEntity entity,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(entity: entity,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table.
        /// </summary>
        /// <typeparam name="TExpressionOrKey">The type of the expression or the key.</typeparam>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int Delete<TExpressionOrKey>(TExpressionOrKey whereOrPrimaryKey,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity, TExpressionOrKey>(whereOrPrimaryKey: whereOrPrimaryKey,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table.
        /// </summary>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int Delete(object whereOrPrimaryKey,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(whereOrPrimaryKey: whereOrPrimaryKey,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int Delete(Expression<Func<TEntity, bool>> where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int Delete(QueryField where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int Delete(IEnumerable<QueryField> where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int Delete(QueryGroup where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region DeleteAsync<TEntity>

        /// <summary>
        /// Deletes an existing row from the table in an asynchronous way.
        /// </summary>
        /// <param name="entity">The data entity object to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAsync(TEntity entity,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(entity: entity,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TExpressionOrKey">The type of the expression or the key.</typeparam>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAsync<TExpressionOrKey>(TExpressionOrKey whereOrPrimaryKey,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity, TExpressionOrKey>(whereOrPrimaryKey: whereOrPrimaryKey,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAsync(object whereOrPrimaryKey,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(whereOrPrimaryKey: whereOrPrimaryKey,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAsync(Expression<Func<TEntity, bool>> where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAsync(QueryField where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAsync(IEnumerable<QueryField> where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Delete the rows from the table in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAsync(QueryGroup where,
            string hints = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<TEntity>(where: where,
                hints: hints,
                transaction: transaction);
        }

        #endregion

        #region Delete(TableName)

        /// <summary>
        /// Deletes an existing row from the table.
        /// </summary>
        /// <typeparam name="T">The type of the dynamic expression or the key.</typeparam>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="what">The data entity object, dynamic expression or the key of the row to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int Delete<T>(string tableName,
            T what,
            string hints,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete<T>(tableName: tableName,
                what: what,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing row from the table.
        /// </summary>
        /// <typeparam name="T">The type of the dynamic expression or the key.</typeparam>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="what">The data entity object, dynamic expression or the key of the row to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public int Delete(string tableName,
            object what,
            string hints,
            IDbTransaction transaction = null)
        {
            return DbRepository.Delete(tableName: tableName,
                what: what,
                hints: hints,
                transaction: transaction);
        }

        #endregion


        #region Delete(TableName)

        /// <summary>
        /// Deletes an existing row from the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="T">The type of the dynamic expression or the key.</typeparam>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="what">The data entity object, dynamic expression or the key of the row to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAsync<T>(string tableName,
            T what,
            string hints,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync<T>(tableName: tableName,
                what: what,
                hints: hints,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes an existing row from the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="T">The type of the dynamic expression or the key.</typeparam>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="what">The data entity object, dynamic expression or the key of the row to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows that has been deleted from the table.</returns>
        public Task<int> DeleteAsync(string tableName,
            object what,
            string hints,
            IDbTransaction transaction = null)
        {
            return DbRepository.DeleteAsync(tableName: tableName,
                what: what,
                hints: hints,
                transaction: transaction);
        }

        #endregion
    }
}
