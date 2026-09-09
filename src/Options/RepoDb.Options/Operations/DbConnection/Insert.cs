#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.Options;

/// <summary>
/// Contains the extension methods for <see cref="IDbConnection"/> object that accept an <see cref="ConnectionInsertOptions"/> object.
/// </summary>
public static partial class DbConnectionExtension
{
    #region Insert<TEntity, TResult>

    /// <summary>
    /// Inserts a new row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be inserted.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Insert<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        ConnectionInsertOptions options)
        where TEntity : class
    {
        options ??= new ConnectionInsertOptions();
        return connection.Insert<TEntity>(tableName: tableName,
            entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Inserts a new row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be inserted.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Insert<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        ConnectionInsertOptions options)
        where TEntity : class
    {
        options ??= new ConnectionInsertOptions();
        return connection.Insert<TEntity, TResult>(tableName: tableName,
            entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Inserts a new row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be inserted.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Insert<TEntity>(this IDbConnection connection,
        TEntity entity,
        ConnectionInsertOptions options)
        where TEntity : class
    {
        options ??= new ConnectionInsertOptions();
        return connection.Insert<TEntity>(entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Inserts a new row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be inserted.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Insert<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        ConnectionInsertOptions options)
        where TEntity : class
    {
        options ??= new ConnectionInsertOptions();
        return connection.Insert<TEntity, TResult>(entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    #endregion

    #region InsertAsync<TEntity, TResult>

    /// <summary>
    /// Inserts a new row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be inserted.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> InsertAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        ConnectionInsertOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionInsertOptions();
        return connection.InsertAsync<TEntity>(tableName: tableName,
            entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Inserts a new row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be inserted.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> InsertAsync<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        ConnectionInsertOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionInsertOptions();
        return connection.InsertAsync<TEntity, TResult>(tableName: tableName,
            entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Inserts a new row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be inserted.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> InsertAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        ConnectionInsertOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionInsertOptions();
        return connection.InsertAsync<TEntity>(entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Inserts a new row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be inserted.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> InsertAsync<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        ConnectionInsertOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionInsertOptions();
        return connection.InsertAsync<TEntity, TResult>(entity: entity,
            fields: options.Fields,
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
