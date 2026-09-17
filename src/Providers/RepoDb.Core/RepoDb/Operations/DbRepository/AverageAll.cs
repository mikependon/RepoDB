#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
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

        #region AverageAll<TEntity>

        /// <summary>
        /// Computes the average value of the target field.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double AverageAll<TEntity>(Field field,
            string hints = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.AverageAll<TEntity>(field: field,
                    hints: hints,
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

        /// <summary>
        /// Computes the average value of the target field.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double AverageAll<TEntity>(Expression<Func<TEntity, object>> field,
            string hints = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.AverageAll<TEntity>(field: field,
                    hints: hints,
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

        /// <summary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAllAsync<TEntity>(Field field,
            string hints = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.AverageAllAsync<TEntity>(field: field,
                    hints: hints,
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

        /// <summary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAllAsync<TEntity>(Expression<Func<TEntity, object>> field,
            string hints = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.AverageAllAsync(field: field,
                    hints: hints,
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

        #region AverageAll<TEntity, TResult>

        /// <summary>
        /// Computes the average value of the target field.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult AverageAll<TEntity, TResult>(Field field,
            string hints = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.AverageAll<TEntity, TResult>(field: field,
                    hints: hints,
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

        /// <summary>
        /// Computes the average value of the target field.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult AverageAll<TEntity, TResult>(Expression<Func<TEntity, TResult>> field,
            string hints = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.AverageAll<TEntity, TResult>(field: field,
                    hints: hints,
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

        /// <summary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAllAsync<TEntity, TResult>(Field field,
            string hints = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.AverageAllAsync<TEntity, TResult>(field: field,
                    hints: hints,
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

        /// <summary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAllAsync<TEntity, TResult>(Expression<Func<TEntity, TResult>> field,
            string hints = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.AverageAllAsync(field: field,
                    hints: hints,
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

        #region AverageAll(TableName)

        /// <summary>
        /// Computes the average value of the target field.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public double AverageAll(string tableName,
            Field field,
            string hints = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.AverageAll(tableName: tableName,
                    field: field,
                    hints: hints,
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

        /// <summary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<double> AverageAllAsync(string tableName,
            Field field,
            string hints = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.AverageAllAsync(tableName: tableName,
                    field: field,
                    hints: hints,
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

        #region AverageAll<TResult>(TableName)

        /// <summary>
        /// Computes the average value of the target field.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The average value of the target field.</returns>
        public TResult AverageAll<TResult>(string tableName,
            Field field,
            string hints = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction transaction = null)
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return dbConnection.AverageAll<TResult>(tableName: tableName,
                    field: field,
                    hints: hints,
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

        /// <summary>
        /// Computes the average value of the target field in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The average value of the target field.</returns>
        public async Task<TResult> AverageAllAsync<TResult>(string tableName,
            Field field,
            string hints = null,
            string traceKey = TraceKeys.AverageAll,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Create a connection
            var dbConnection = (transaction?.Connection ?? CreateConnection());

            try
            {
                // Call the method
                return await dbConnection.AverageAllAsync<TResult>(tableName: tableName,
                    field: field,
                    hints: hints,
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
