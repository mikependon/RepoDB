#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public partial class DbRepository<TDbConnection> : IDisposable
        where TDbConnection : DbConnection, new()
    {
        #region QueryFirst<TEntity>

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public TEntity QueryFirst<TEntity>(string tableName,
            object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst<TEntity>(tableName,
                    what: what,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public TEntity QueryFirst<TEntity, TWhat>(string tableName,
            TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst<TEntity, TWhat>(tableName,
                    what: what,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        public TEntity QueryFirst<TEntity>(string tableName,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst<TEntity>(tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        public TEntity QueryFirst<TEntity>(string tableName,
            QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst<TEntity>(tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        public TEntity QueryFirst<TEntity>(string tableName,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst<TEntity>(tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        public TEntity QueryFirst<TEntity>(string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst<TEntity>(tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public TEntity QueryFirst<TEntity>(object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst<TEntity>(what: what,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of the target result type containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public TEntity QueryFirst<TEntity, TWhat>(TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst<TEntity, TWhat>(what: what,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        public TEntity QueryFirst<TEntity>(Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst<TEntity>(where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        public TEntity QueryFirst<TEntity>(QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null, IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst<TEntity>(where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        public TEntity QueryFirst<TEntity>(IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst<TEntity>(where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        public TEntity QueryFirst<TEntity>(QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst<TEntity>(where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        #endregion

        #region QueryFirstAsync<TEntity>

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
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
        public async Task<TEntity> QueryFirstAsync<TEntity>(string tableName,
            object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync<TEntity>(tableName: tableName,
                    what: what,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
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
        public async Task<TEntity> QueryFirstAsync<TEntity, TWhat>(string tableName,
            TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync<TEntity, TWhat>(tableName: tableName,
                    what: what,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        public async Task<TEntity> QueryFirstAsync<TEntity>(string tableName,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync(tableName: tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        public async Task<TEntity> QueryFirstAsync<TEntity>(string tableName,
            QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync<TEntity>(tableName: tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        public async Task<TEntity> QueryFirstAsync<TEntity>(string tableName,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync<TEntity>(tableName: tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        public async Task<TEntity> QueryFirstAsync<TEntity>(string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync<TEntity>(tableName: tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
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
        public async Task<TEntity> QueryFirstAsync<TEntity>(object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync<TEntity>(what: what,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
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
        public async Task<TEntity> QueryFirstAsync<TEntity, TWhat>(TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync<TEntity, TWhat>(what: what,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        public async Task<TEntity> QueryFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync(where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        public async Task<TEntity> QueryFirstAsync<TEntity>(QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync<TEntity>(where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        public async Task<TEntity> QueryFirstAsync<TEntity>(IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync<TEntity>(where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        public async Task<TEntity> QueryFirstAsync<TEntity>(QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync<TEntity>(where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        #endregion

        #region QueryFirst(TableName)

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A dynamic object containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public dynamic QueryFirst<TWhat>(string tableName,
            TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst<TWhat>(tableName: tableName,
                    what: what,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression and return only the first row.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A dynamic object containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public dynamic QueryFirst(string tableName,
            object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst(tableName: tableName,
                    what: what,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
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
        /// <returns>A dynamic object containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public dynamic QueryFirst(string tableName,
            QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null, IDbTransaction transaction = null)
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst(tableName: tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
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
        /// <returns>A dynamic object containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public dynamic QueryFirst(string tableName,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst(tableName: tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
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
        /// <returns>A dynamic object containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public dynamic QueryFirst(string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.QueryFirst(tableName: tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        #endregion

        #region QueryFirstAsync(TableName)

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A dynamic object containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public async Task<dynamic> QueryFirstAsync<TWhat>(string tableName,
            TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync(tableName: tableName,
                    what: what,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way and return only the first row.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A dynamic object containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public async Task<dynamic> QueryFirstAsync(string tableName,
            object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync(tableName: tableName,
                    what: what,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
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
        /// <returns>A dynamic object containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public async Task<dynamic> QueryFirstAsync(string tableName,
            QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync(tableName: tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
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
        /// <returns>A dynamic object containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public async Task<dynamic> QueryFirstAsync(string tableName,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync(tableName: tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
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
        /// <returns>A dynamic object containing the converted result of the first row returned by the query.</returns>
        /// <remarks>An <see cref="EmptyException"/> is thrown if the query did not return any row.</remarks>
        public async Task<dynamic> QueryFirstAsync(string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string traceKey = TraceKeys.QueryFirst,
            string cacheKey = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.QueryFirstAsync(tableName: tableName,
                    where: where,
                    fields: fields,
                    orderBy: orderBy,
                    top: top,
                    hints: hints,
                    cacheKey: cacheKey,
                    cacheItemExpiration: CacheItemExpiration,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    cache: Cache,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection, transaction);
            }
        }

        #endregion
    }
}
