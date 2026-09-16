#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public partial class DbRepository<TDbConnection> : IDisposable
        where TDbConnection : DbConnection, new()
    {
        #region Truncate<TEntity>

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <returns>The number of rows affected.</returns>
        public int Truncate<TEntity>(string traceKey = TraceKeys.Truncate)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = CreateConnection();

            try
            {
                // Call the method
                return dbConnection.Truncate<TEntity>(
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: null,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection);
            }
        }

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected.</returns>
        public int Truncate<TEntity>(string traceKey = TraceKeys.Truncate,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = transaction?.Connection ?? CreateConnection();

            try
            {
                // Call the method
                return dbConnection.Truncate<TEntity>(commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
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

        #region TruncateAsync<TEntity>

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> TruncateAsync<TEntity>(string traceKey = TraceKeys.Truncate)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = CreateConnection();

            try
            {
                // Call the method
                return await dbConnection.TruncateAsync<TEntity>(commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: null,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: CancellationToken.None).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection);
            }
        }

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> TruncateAsync<TEntity>(string traceKey = TraceKeys.Truncate,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = CreateConnection();

            try
            {
                // Call the method
                return await dbConnection.TruncateAsync<TEntity>(commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: null,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection);
            }
        }

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> TruncateAsync<TEntity>(string traceKey = TraceKeys.Truncate,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = transaction?.Connection ?? CreateConnection();

            try
            {
                // Call the method
                return await dbConnection.TruncateAsync<TEntity>(commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
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

        #region Truncate(TableName)

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <returns>The number of rows affected.</returns>
        public int Truncate(string tableName,
            string traceKey = TraceKeys.Truncate)
        {
            // Create a connection
            var dbConnection = CreateConnection();

            try
            {
                // Call the method
                return dbConnection.Truncate(tableName: tableName,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: null,
                    trace: Trace,
                    statementBuilder: StatementBuilder);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection);
            }
        }

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected.</returns>
        public int Truncate(string tableName,
            string traceKey = TraceKeys.Truncate,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var dbConnection = transaction?.Connection ?? CreateConnection();

            try
            {
                // Call the method
                return dbConnection.Truncate(tableName: tableName,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
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

        #region TruncateAsync(TableName)

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> TruncateAsync(string tableName,
            string traceKey = TraceKeys.Truncate)
        {
            // Create a connection
            var dbConnection = CreateConnection();

            try
            {
                // Call the method
                return await dbConnection.TruncateAsync(tableName: tableName,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: null,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: CancellationToken.None).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection);
            }
        }

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> TruncateAsync(string tableName,
            string traceKey = TraceKeys.Truncate,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var dbConnection = CreateConnection();

            try
            {
                // Call the method
                return await dbConnection.TruncateAsync(tableName: tableName,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: null,
                    trace: Trace,
                    statementBuilder: StatementBuilder,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Dispose the connection
                DisposeConnectionForPerCall(dbConnection);
            }
        }

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> TruncateAsync(string tableName,
            string traceKey = TraceKeys.Truncate,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var dbConnection = transaction?.Connection ?? CreateConnection();

            try
            {
                // Call the method
                return await dbConnection.TruncateAsync(tableName: tableName,
                    commandTimeout: CommandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
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
