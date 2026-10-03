#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Data.Interfaces;
using RepoDb.Interfaces;

namespace RepoDb.Data
{
    /// <summary>
    /// Contains the extension methods of <see cref="IDbConnection"/> object for moving data between databases.
    /// </summary>
    public static class MoveDataToExtension
    {
        #region Public Methods

        #region MoveDataTo (Registered Connection, Sync)

        /// <summary>
        /// Moves all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the same-named table of the destination connection that is registered in the <see cref="ConnectionManager"/>.
        /// The destination table must already exist. The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be moved. The default is <c>null</c>, which moves all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the destination table.</returns>
        public static int MoveDataTo<TEntity>(this IDbConnection connection,
            string destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> callback = null,
            string traceKey = DataTraceKeys.MoveDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.MoveDataTo(ConnectionManager.Get(destinationConnection), where, batchSize, commandTimeout, dataInterceptors, callback, traceKey, trace, transaction);

        /// <summary>
        /// Moves all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the target table of the destination connection that is registered in the <see cref="ConnectionManager"/>.
        /// The target table must already exist and have the same column names as the source table.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be moved. The default is <c>null</c>, which moves all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int MoveDataTo<TEntity>(this IDbConnection connection,
            string targetTable,
            string destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> callback = null,
            string traceKey = DataTraceKeys.MoveDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.MoveDataTo(targetTable, ConnectionManager.Get(destinationConnection), where, batchSize, commandTimeout, dataInterceptors, callback, traceKey, trace, transaction);

        /// <summary>
        /// Moves all the rows of the source table of the current connection into the same-named table of the destination connection
        /// that is registered in the <see cref="ConnectionManager"/>.
        /// The destination table must already exist. The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be moved.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be moved. The default is <c>null</c>, which moves all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the destination table.</returns>
        public static int MoveDataTo(this IDbConnection connection,
            string tableName,
            string destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> callback = null,
            string traceKey = DataTraceKeys.MoveDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null) =>
            connection.MoveDataTo(tableName, ConnectionManager.Get(destinationConnection), where, batchSize, commandTimeout, dataInterceptors, callback, traceKey, trace, transaction);

        /// <summary>
        /// Moves all the rows of the source table of the current connection into the target table of the destination connection
        /// that is registered in the <see cref="ConnectionManager"/>.
        /// The target table must already exist and have the same column names as the source table.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the table to be moved.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be moved. The default is <c>null</c>, which moves all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int MoveDataTo(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            string destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> callback = null,
            string traceKey = DataTraceKeys.MoveDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null) =>
            connection.MoveDataTo(sourceTable, targetTable, ConnectionManager.Get(destinationConnection), where, batchSize, commandTimeout, dataInterceptors, callback, traceKey, trace, transaction);

        #endregion

        #region MoveDataTo (IDbConnection, Sync)

        /// <summary>
        /// Moves all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the same-named table of the destination connection.
        /// The destination table must already exist. The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be moved. The default is <c>null</c>, which moves all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the destination table.</returns>
        public static int MoveDataTo<TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> callback = null,
            string traceKey = DataTraceKeys.MoveDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.MoveDataTo(ClassMappedNameCache.Get<TEntity>(), destinationConnection, where == null ? null : QueryGroup.Parse(where), batchSize, commandTimeout, dataInterceptors, callback, traceKey, trace, transaction);

        /// <summary>
        /// Moves all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the target table of the destination connection.
        /// The target table must already exist and have the same column names as the source table.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be moved. The default is <c>null</c>, which moves all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int MoveDataTo<TEntity>(this IDbConnection connection,
            string targetTable,
            IDbConnection destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> callback = null,
            string traceKey = DataTraceKeys.MoveDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.MoveDataTo(ClassMappedNameCache.Get<TEntity>(), targetTable, destinationConnection, where == null ? null : QueryGroup.Parse(where), batchSize, commandTimeout, dataInterceptors, callback, traceKey, trace, transaction);

        /// <summary>
        /// Moves all the rows of the source table of the current connection into the same-named table of the destination connection.
        /// The destination table must already exist. The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be moved.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be moved. The default is <c>null</c>, which moves all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the destination table.</returns>
        public static int MoveDataTo(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> callback = null,
            string traceKey = DataTraceKeys.MoveDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null) =>
            connection.MoveDataTo(tableName, tableName, destinationConnection, where, batchSize, commandTimeout, dataInterceptors, callback, traceKey, trace, transaction);

        /// <summary>
        /// Moves all the rows of the source table of the current connection into the target table of the destination connection.
        /// The target table must already exist and have the same column names as the source table.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the table to be moved.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be moved. The default is <c>null</c>, which moves all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int MoveDataTo(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            IDbConnection destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> callback = null,
            string traceKey = DataTraceKeys.MoveDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            Validate(connection, sourceTable, targetTable, destinationConnection, batchSize);

            return connection.CopyDataTo(sourceTable, targetTable, destinationConnection, where, batchSize, commandTimeout, dataInterceptors, callback, traceKey, trace, transaction);
        }

        #endregion

        #region MoveDataTo (Registered Connection, Async)

        /// <summary>
        /// Moves all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the same-named table of the destination connection that is registered in the <see cref="ConnectionManager"/>.
        /// The destination table must already exist. The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be moved. The default is <c>null</c>, which moves all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows inserted into the destination table.</returns>
        public static Task<int> MoveDataToAsync<TEntity>(this IDbConnection connection,
            string destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> callback = null,
            string traceKey = DataTraceKeys.MoveDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.MoveDataToAsync(ConnectionManager.Get(destinationConnection), where, batchSize, commandTimeout, dataInterceptors, callback, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Moves all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the target table of the destination connection that is registered in the <see cref="ConnectionManager"/>.
        /// The target table must already exist and have the same column names as the source table.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be moved. The default is <c>null</c>, which moves all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows inserted into the target table.</returns>
        public static Task<int> MoveDataToAsync<TEntity>(this IDbConnection connection,
            string targetTable,
            string destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> callback = null,
            string traceKey = DataTraceKeys.MoveDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.MoveDataToAsync(targetTable, ConnectionManager.Get(destinationConnection), where, batchSize, commandTimeout, dataInterceptors, callback, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Moves all the rows of the source table of the current connection into the same-named table of the destination connection
        /// that is registered in the <see cref="ConnectionManager"/>.
        /// The destination table must already exist. The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be moved.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be moved. The default is <c>null</c>, which moves all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows inserted into the destination table.</returns>
        public static Task<int> MoveDataToAsync(this IDbConnection connection,
            string tableName,
            string destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> callback = null,
            string traceKey = DataTraceKeys.MoveDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.MoveDataToAsync(tableName, ConnectionManager.Get(destinationConnection), where, batchSize, commandTimeout, dataInterceptors, callback, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Moves all the rows of the source table of the current connection into the target table of the destination connection
        /// that is registered in the <see cref="ConnectionManager"/>.
        /// The target table must already exist and have the same column names as the source table.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the table to be moved.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The name of the destination connection registered in the <see cref="ConnectionManager"/>.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be moved. The default is <c>null</c>, which moves all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows inserted into the target table.</returns>
        public static Task<int> MoveDataToAsync(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            string destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> callback = null,
            string traceKey = DataTraceKeys.MoveDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.MoveDataToAsync(sourceTable, targetTable, ConnectionManager.Get(destinationConnection), where, batchSize, commandTimeout, dataInterceptors, callback, traceKey, trace, transaction, cancellationToken);

        #endregion

        #region MoveDataTo (IDbConnection, Async)

        /// <summary>
        /// Moves all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the same-named table of the destination connection.
        /// The destination table must already exist. The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be moved. The default is <c>null</c>, which moves all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows inserted into the destination table.</returns>
        public static Task<int> MoveDataToAsync<TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> callback = null,
            string traceKey = DataTraceKeys.MoveDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.MoveDataToAsync(ClassMappedNameCache.Get<TEntity>(), destinationConnection, where == null ? null : QueryGroup.Parse(where), batchSize, commandTimeout, dataInterceptors, callback, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Moves all the rows of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the target table of the destination connection.
        /// The target table must already exist and have the same column names as the source table.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the source table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be moved. The default is <c>null</c>, which moves all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows inserted into the target table.</returns>
        public static Task<int> MoveDataToAsync<TEntity>(this IDbConnection connection,
            string targetTable,
            IDbConnection destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> callback = null,
            string traceKey = DataTraceKeys.MoveDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.MoveDataToAsync(ClassMappedNameCache.Get<TEntity>(), targetTable, destinationConnection, where == null ? null : QueryGroup.Parse(where), batchSize, commandTimeout, dataInterceptors, callback, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Moves all the rows of the source table of the current connection into the same-named table of the destination connection.
        /// The destination table must already exist. The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be moved.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be moved. The default is <c>null</c>, which moves all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows inserted into the destination table.</returns>
        public static Task<int> MoveDataToAsync(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> callback = null,
            string traceKey = DataTraceKeys.MoveDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.MoveDataToAsync(tableName, tableName, destinationConnection, where, batchSize, commandTimeout, dataInterceptors, callback, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Moves all the rows of the source table of the current connection into the target table of the destination connection.
        /// The target table must already exist and have the same column names as the source table.
        /// The rows of the source table are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="sourceTable">The name of the table to be moved.</param>
        /// <param name="targetTable">The name of the table in the destination database that receives the rows.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="where">The expression used to filter the rows of the source table to be moved. The default is <c>null</c>, which moves all the rows.</param>
        /// <param name="batchSize">The number of rows to be inserted into the destination table per batch. The default is 1000.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="dataInterceptors">The data interceptors to be used, in order. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyDataProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveDataTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows inserted into the target table.</returns>
        public static async Task<int> MoveDataToAsync(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            IDbConnection destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            Action<CopyDataProgress> callback = null,
            string traceKey = DataTraceKeys.MoveDataTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            Validate(connection, sourceTable, targetTable, destinationConnection, batchSize);

            return await connection.CopyDataToAsync(sourceTable, targetTable, destinationConnection, where, batchSize, commandTimeout, dataInterceptors, callback, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Helpers

        private static void Validate(IDbConnection connection,
            string sourceTable,
            string targetTable,
            IDbConnection destinationConnection,
            int batchSize)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (string.IsNullOrWhiteSpace(sourceTable))
            {
                throw new ArgumentNullException(nameof(sourceTable));
            }
            if (string.IsNullOrWhiteSpace(targetTable))
            {
                throw new ArgumentNullException(nameof(targetTable));
            }
            if (destinationConnection == null)
            {
                throw new ArgumentNullException(nameof(destinationConnection));
            }
            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize), "The batch size must be greater than zero.");
            }
        }

        #endregion
    }
}
