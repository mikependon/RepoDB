using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public abstract partial class BaseRepository<TEntity, TDbConnection> : IDisposable
    {
        #region Query<TEntity>

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(string tableName,
            object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(tableName: tableName,
                what: what,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query<TWhat>(string tableName,
            TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity, TWhat>(tableName: tableName,
                what: what,
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(string tableName,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(tableName: tableName,
                where: where,
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(string tableName,
            QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(tableName: tableName,
                where: where,
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(string tableName,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(tableName: tableName,
                where: where,
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(what: what,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query<TWhat>(TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity, TWhat>(what: what,
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public IEnumerable<TEntity> Query(QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            return DbRepository.Query<TEntity>(where: where,
                fields: fields,
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
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(string tableName,
            object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryAsync<TEntity>(tableName: tableName,
                what: what,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync<TWhat>(string tableName,
            TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryAsync<TEntity, TWhat>(tableName: tableName,
                what: what,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(string tableName,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryAsync<TEntity>(tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(string tableName,
            QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryAsync<TEntity>(tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(string tableName,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryAsync<TEntity>(tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryAsync<TEntity>(tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryAsync<TEntity>(what: what,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync<TWhat>(TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryAsync<TEntity, TWhat>(what: what,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// </param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public Task<IEnumerable<TEntity>> QueryAsync(QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryAsync<TEntity>(where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
