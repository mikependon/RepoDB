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
        #region QuerySingle<TEntity>

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return the single row.
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public TEntity QuerySingle(string tableName,
            object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null)
        {
            return DbRepository.QuerySingle<TEntity>(tableName: tableName,
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
        /// Query the existing rows from the table based on a given expression and return the single row.
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public TEntity QuerySingle<TWhat>(string tableName,
            TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null)
        {
            return DbRepository.QuerySingle<TEntity, TWhat>(tableName: tableName,
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
        /// Query the existing rows from the table based on a given expression and return the single row.
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public TEntity QuerySingle(string tableName,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null)
        {
            return DbRepository.QuerySingle<TEntity>(tableName: tableName,
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
        /// Query the existing rows from the table based on a given expression and return the single row.
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public TEntity QuerySingle(string tableName,
            QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null)
        {
            return DbRepository.QuerySingle<TEntity>(tableName: tableName,
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
        /// Query the existing rows from the table based on a given expression and return the single row.
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public TEntity QuerySingle(string tableName,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null)
        {
            return DbRepository.QuerySingle<TEntity>(tableName: tableName,
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
        /// Query the existing rows from the table based on a given expression and return the single row.
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public TEntity QuerySingle(string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null)
        {
            return DbRepository.QuerySingle<TEntity>(tableName: tableName,
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
        /// Query the existing rows from the table based on a given expression and return the single row.
        /// </summary>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public TEntity QuerySingle(object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null)
        {
            return DbRepository.QuerySingle<TEntity>(what: what,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return the single row.
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public TEntity QuerySingle<TWhat>(TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null)
        {
            return DbRepository.QuerySingle<TEntity, TWhat>(what: what,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return the single row.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public TEntity QuerySingle(Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null)
        {
            return DbRepository.QuerySingle<TEntity>(where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return the single row.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public TEntity QuerySingle(QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null)
        {
            return DbRepository.QuerySingle<TEntity>(where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return the single row.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public TEntity QuerySingle(IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null)
        {
            return DbRepository.QuerySingle<TEntity>(where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return the single row.
        /// </summary>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public TEntity QuerySingle(QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null)
        {
            return DbRepository.QuerySingle<TEntity>(where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                traceKey: traceKey,
				transaction: transaction);
        }

        #endregion

        #region QuerySingleAsync<TEntity>

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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public Task<TEntity> QuerySingleAsync(string tableName,
            object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QuerySingleAsync<TEntity>(tableName: tableName,
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public Task<TEntity> QuerySingleAsync<TWhat>(string tableName,
            TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QuerySingleAsync<TEntity, TWhat>(tableName: tableName,
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public Task<TEntity> QuerySingleAsync(string tableName,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QuerySingleAsync<TEntity>(tableName: tableName,
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public Task<TEntity> QuerySingleAsync(string tableName,
            QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QuerySingleAsync<TEntity>(tableName: tableName,
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public Task<TEntity> QuerySingleAsync(string tableName,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QuerySingleAsync<TEntity>(tableName: tableName,
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public Task<TEntity> QuerySingleAsync(string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QuerySingleAsync<TEntity>(tableName: tableName,
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public Task<TEntity> QuerySingleAsync(object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QuerySingleAsync<TEntity>(what: what,
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public Task<TEntity> QuerySingleAsync<TWhat>(TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QuerySingleAsync<TEntity, TWhat>(what: what,
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public Task<TEntity> QuerySingleAsync(Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QuerySingleAsync<TEntity>(where: where,
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public Task<TEntity> QuerySingleAsync(QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QuerySingleAsync<TEntity>(where: where,
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public Task<TEntity> QuerySingleAsync(IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QuerySingleAsync<TEntity>(where: where,
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
        /// <returns>An instance of the target result type containing the converted result of the single row returned by the query.</returns>
        /// <remarks>
        /// An <see cref="EmptyException"/> is thrown if the query did not return any row.
        /// A <see cref="MultipleRowsFoundException"/> is thrown if the query returned more than one row.
        /// </remarks>
        public Task<TEntity> QuerySingleAsync(QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            string traceKey = TraceKeys.QuerySingle,
			IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return DbRepository.QuerySingleAsync<TEntity>(where: where,
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
