#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using RepoDb.Interfaces;
using RepoDb.Schema;

namespace RepoDb.Data
{
    /// <summary>
    /// Contains the extension methods of <see cref="IDbConnection"/> object for moving data between databases.
    /// </summary>
    public static class MoveToExtension
    {
        #region Public Methods

        #region MoveTo (Registered Connection, Sync)

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
        /// <param name="schemaProvider">The schema provider to be used. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the destination table.</returns>
        public static int MoveTo<TEntity>(this IDbConnection connection,
            string destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            ISchemaProvider schemaProvider = null,
            Action<CopyProgress> callback = null,
            string traceKey = DataTraceKeys.MoveTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.MoveTo(ConnectionManager.Get(destinationConnection), where, batchSize, commandTimeout, dataInterceptors, schemaProvider, callback, traceKey, trace, transaction);

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
        /// <param name="schemaProvider">The schema provider to be used. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int MoveTo<TEntity>(this IDbConnection connection,
            string targetTable,
            string destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            ISchemaProvider schemaProvider = null,
            Action<CopyProgress> callback = null,
            string traceKey = DataTraceKeys.MoveTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.MoveTo(targetTable, ConnectionManager.Get(destinationConnection), where, batchSize, commandTimeout, dataInterceptors, schemaProvider, callback, traceKey, trace, transaction);

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
        /// <param name="schemaProvider">The schema provider to be used. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the destination table.</returns>
        public static int MoveTo(this IDbConnection connection,
            string tableName,
            string destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            ISchemaProvider schemaProvider = null,
            Action<CopyProgress> callback = null,
            string traceKey = DataTraceKeys.MoveTo,
            ITrace trace = null,
            IDbTransaction transaction = null) =>
            connection.MoveTo(tableName, ConnectionManager.Get(destinationConnection), where, batchSize, commandTimeout, dataInterceptors, schemaProvider, callback, traceKey, trace, transaction);

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
        /// <param name="schemaProvider">The schema provider to be used. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int MoveTo(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            string destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            ISchemaProvider schemaProvider = null,
            Action<CopyProgress> callback = null,
            string traceKey = DataTraceKeys.MoveTo,
            ITrace trace = null,
            IDbTransaction transaction = null) =>
            connection.MoveTo(sourceTable, targetTable, ConnectionManager.Get(destinationConnection), where, batchSize, commandTimeout, dataInterceptors, schemaProvider, callback, traceKey, trace, transaction);

        #endregion

        #region MoveTo (IDbConnection, Sync)

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
        /// <param name="schemaProvider">The schema provider to be used. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the destination table.</returns>
        public static int MoveTo<TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            ISchemaProvider schemaProvider = null,
            Action<CopyProgress> callback = null,
            string traceKey = DataTraceKeys.MoveTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.MoveTo(ClassMappedNameCache.Get<TEntity>(), destinationConnection, where == null ? null : QueryGroup.Parse(where), batchSize, commandTimeout, dataInterceptors, schemaProvider, callback, traceKey, trace, transaction);

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
        /// <param name="schemaProvider">The schema provider to be used. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int MoveTo<TEntity>(this IDbConnection connection,
            string targetTable,
            IDbConnection destinationConnection,
            Expression<Func<TEntity, bool>> where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            ISchemaProvider schemaProvider = null,
            Action<CopyProgress> callback = null,
            string traceKey = DataTraceKeys.MoveTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.MoveTo(ClassMappedNameCache.Get<TEntity>(), targetTable, destinationConnection, where == null ? null : QueryGroup.Parse(where), batchSize, commandTimeout, dataInterceptors, schemaProvider, callback, traceKey, trace, transaction);

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
        /// <param name="schemaProvider">The schema provider to be used. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the destination table.</returns>
        public static int MoveTo(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            ISchemaProvider schemaProvider = null,
            Action<CopyProgress> callback = null,
            string traceKey = DataTraceKeys.MoveTo,
            ITrace trace = null,
            IDbTransaction transaction = null) =>
            connection.MoveTo(tableName, tableName, destinationConnection, where, batchSize, commandTimeout, dataInterceptors, schemaProvider, callback, traceKey, trace, transaction);

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
        /// <param name="schemaProvider">The schema provider to be used. The default is <c>null</c>.</param>
        /// <param name="callback">The callback that receives the <see cref="CopyProgress"/> (batch number, rows in the batch, total rows copied so far, start and end time) after every batch is inserted. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="DataTraceKeys.MoveTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The number of rows inserted into the target table.</returns>
        public static int MoveTo(this IDbConnection connection,
            string sourceTable,
            string targetTable,
            IDbConnection destinationConnection,
            QueryGroup where = null,
            int batchSize = 1000,
            int? commandTimeout = null,
            IList<ICopyDataInterceptor> dataInterceptors = null,
            ISchemaProvider schemaProvider = null,
            Action<CopyProgress> callback = null,
            string traceKey = DataTraceKeys.MoveTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
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

            var options = new CopyProgress { StartTime = DateTime.UtcNow };
            var batch = new List<object>(batchSize);

            void Flush()
            {
                var inserted = destinationConnection.InsertAll(targetTable, batch, batchSize, commandTimeout: commandTimeout, traceKey: traceKey, transaction: transaction, trace: trace);
                options.BatchNumber++;
                options.RowCount = inserted;
                options.TotalCopiedRowCount += inserted;
                options.EndTime = DateTime.UtcNow;
                batch.Clear();
                callback?.Invoke(options);
            }

            foreach (object row in where == null ? connection.QueryAll(sourceTable, commandTimeout: commandTimeout, traceKey: traceKey, trace: trace) : connection.Query(sourceTable, where, commandTimeout: commandTimeout, traceKey: traceKey, trace: trace))
            {
                batch.Add(row);
                if (batch.Count == batchSize)
                {
                    Flush();
                }
            }

            if (batch.Count > 0)
            {
                Flush();
            }
            return options.TotalCopiedRowCount;
        }

        #endregion

        #endregion
    }
}
