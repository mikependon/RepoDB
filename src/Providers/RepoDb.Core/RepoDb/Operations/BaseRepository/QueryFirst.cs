#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

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
        #region QueryFirst<TEntity>

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public TEntity QueryFirst(string tableName,
            object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null)
        {
            return DbRepository.QueryFirst<TEntity>(tableName: tableName,
                what: what,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public TEntity QueryFirst<TWhat>(string tableName,
            TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null)
        {
            return DbRepository.QueryFirst<TEntity, TWhat>(tableName: tableName,
                what: what,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public TEntity QueryFirst(string tableName,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null)
        {
            return DbRepository.QueryFirst<TEntity>(tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public TEntity QueryFirst(string tableName,
            QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null)
        {
            return DbRepository.QueryFirst<TEntity>(tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public TEntity QueryFirst(string tableName,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null)
        {
            return DbRepository.QueryFirst<TEntity>(tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public TEntity QueryFirst(string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null)
        {
            return DbRepository.QueryFirst<TEntity>(tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public TEntity QueryFirst(object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null)
        {
            return DbRepository.QueryFirst<TEntity>(what: what,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public TEntity QueryFirst<TWhat>(TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null)
        {
            return DbRepository.QueryFirst<TEntity, TWhat>(what: what,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public TEntity QueryFirst(Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null)
        {
            return DbRepository.QueryFirst<TEntity>(where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public TEntity QueryFirst(QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null)
        {
            return DbRepository.QueryFirst<TEntity>(where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public TEntity QueryFirst(IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null)
        {
            return DbRepository.QueryFirst<TEntity>(where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public TEntity QueryFirst(QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null)
        {
            return DbRepository.QueryFirst<TEntity>(where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        #endregion

        #region QueryFirstAsync<TEntity>

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public Task<TEntity> QueryFirstAsync(string tableName,
            object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryFirstAsync<TEntity>(tableName: tableName,
                what: what,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public Task<TEntity> QueryFirstAsync<TWhat>(string tableName,
            TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryFirstAsync<TEntity, TWhat>(tableName: tableName,
                what: what,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public Task<TEntity> QueryFirstAsync(string tableName,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryFirstAsync<TEntity>(tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public Task<TEntity> QueryFirstAsync(string tableName,
            QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryFirstAsync<TEntity>(tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public Task<TEntity> QueryFirstAsync(string tableName,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryFirstAsync<TEntity>(tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public Task<TEntity> QueryFirstAsync(string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryFirstAsync<TEntity>(tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public Task<TEntity> QueryFirstAsync(object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryFirstAsync<TEntity>(what: what,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public Task<TEntity> QueryFirstAsync<TWhat>(TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryFirstAsync<TEntity, TWhat>(what: what,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public Task<TEntity> QueryFirstAsync(Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryFirstAsync<TEntity>(where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public Task<TEntity> QueryFirstAsync(QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryFirstAsync<TEntity>(where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public Task<TEntity> QueryFirstAsync(IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryFirstAsync<TEntity>(where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public Task<TEntity> QueryFirstAsync(QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QueryFirst,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QueryFirstAsync<TEntity>(where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
