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
        #region Query<TEntity>(TableName)

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(string tableName,
            object whereOrPrimaryKey = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(tableName: tableName,
                whereOrPrimaryKey: whereOrPrimaryKey,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(string tableName,
            Expression<Func<TEntity, bool>> where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(tableName: tableName,
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(string tableName,
            QueryField where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(tableName: tableName,
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(string tableName,
            IEnumerable<QueryField> where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(tableName: tableName,
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(string tableName,
            QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(tableName: tableName,
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        #endregion

        #region Query<TEntity>

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(object whereOrPrimaryKey = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(whereOrPrimaryKey: whereOrPrimaryKey,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(QueryField where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(IEnumerable<QueryField> where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        #endregion

        #region QueryAsync<TEntity>(TableName)

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(string tableName,
            object whereOrPrimaryKey = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(tableName: tableName,
                whereOrPrimaryKey: whereOrPrimaryKey,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(string tableName,
            Expression<Func<TEntity, bool>> where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(tableName: tableName,
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(string tableName,
            QueryField where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(tableName: tableName,
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(string tableName,
            IEnumerable<QueryField> where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(tableName: tableName,
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(string tableName,
            QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(tableName: tableName,
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        #endregion

        #region QueryAsync<TEntity>

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(object whereOrPrimaryKey = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(whereOrPrimaryKey: whereOrPrimaryKey,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(QueryField where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(IEnumerable<QueryField> where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        #endregion
    }
}
