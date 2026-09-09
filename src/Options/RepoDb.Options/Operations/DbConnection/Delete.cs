#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.Options;

/// <summary>
/// Contains the extension methods for <see cref="IDbConnection"/> object that accept a <see cref="ConnectionDeleteOptions"/> object.
/// </summary>
public static partial class DbConnectionExtension
{
    #region Delete<TEntity>

    /// <summary>
    /// Deletes an existing row from the table based on the given entity's primary/identity key value.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int Delete<TEntity>(this IDbConnection connection,
        TEntity entity,
        ConnectionDeleteOptions options)
        where TEntity : class
    {
        options ??= new ConnectionDeleteOptions();
        return connection.Delete<TEntity>(entity: entity,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int Delete<TEntity, TWhat>(this IDbConnection connection,
        TWhat what,
        ConnectionDeleteOptions options)
        where TEntity : class
    {
        options ??= new ConnectionDeleteOptions();
        return connection.Delete<TEntity, TWhat>(what: what,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int Delete<TEntity>(this IDbConnection connection,
        object what,
        ConnectionDeleteOptions options)
        where TEntity : class
    {
        options ??= new ConnectionDeleteOptions();
        return connection.Delete<TEntity>(what: what,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int Delete<TEntity>(this IDbConnection connection,
        Expression<Func<TEntity, bool>> where,
        ConnectionDeleteOptions options)
        where TEntity : class
    {
        options ??= new ConnectionDeleteOptions();
        return connection.Delete<TEntity>(where: where,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int Delete<TEntity>(this IDbConnection connection,
        QueryField where,
        ConnectionDeleteOptions options)
        where TEntity : class
    {
        options ??= new ConnectionDeleteOptions();
        return connection.Delete<TEntity>(where: where,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int Delete<TEntity>(this IDbConnection connection,
        IEnumerable<QueryField> where,
        ConnectionDeleteOptions options)
        where TEntity : class
    {
        options ??= new ConnectionDeleteOptions();
        return connection.Delete<TEntity>(where: where,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int Delete<TEntity>(this IDbConnection connection,
        QueryGroup where,
        ConnectionDeleteOptions options)
        where TEntity : class
    {
        options ??= new ConnectionDeleteOptions();
        return connection.Delete<TEntity>(where: where,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    #endregion

    #region DeleteAsync<TEntity>

    /// <summary>
    /// Deletes an existing row from the table based on the given entity's primary/identity key value in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        ConnectionDeleteOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionDeleteOptions();
        return connection.DeleteAsync<TEntity>(entity: entity,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static Task<int> DeleteAsync<TEntity, TWhat>(this IDbConnection connection,
        TWhat what,
        ConnectionDeleteOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionDeleteOptions();
        return connection.DeleteAsync<TEntity, TWhat>(what: what,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection,
        object what,
        ConnectionDeleteOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionDeleteOptions();
        return connection.DeleteAsync<TEntity>(what: what,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection,
        Expression<Func<TEntity, bool>> where,
        ConnectionDeleteOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionDeleteOptions();
        return connection.DeleteAsync<TEntity>(where: where,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection,
        QueryField where,
        ConnectionDeleteOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionDeleteOptions();
        return connection.DeleteAsync<TEntity>(where: where,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection,
        IEnumerable<QueryField> where,
        ConnectionDeleteOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionDeleteOptions();
        return connection.DeleteAsync<TEntity>(where: where,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection,
        QueryGroup where,
        ConnectionDeleteOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionDeleteOptions();
        return connection.DeleteAsync<TEntity>(where: where,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    #endregion
}
