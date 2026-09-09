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
/// Contains the extension methods for <see cref="IDbConnection"/> object that accept an <see cref="ConnectionUpdateOptions"/> object.
/// </summary>
public static partial class DbConnectionExtension
{
    #region Update<TEntity>

    /// <summary>
    /// Updates an existing row in the table based on the given entity's primary/identity key value.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        ConnectionUpdateOptions options)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.Update<TEntity>(tableName: tableName,
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
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity, TWhat>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        TWhat what,
        ConnectionUpdateOptions options)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.Update<TEntity, TWhat>(tableName: tableName,
            entity: entity,
            what: what,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        object what,
        ConnectionUpdateOptions options)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.Update<TEntity>(tableName: tableName,
            entity: entity,
            what: what,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, bool>> where,
        ConnectionUpdateOptions options)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.Update<TEntity>(tableName: tableName,
            entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        QueryField where,
        ConnectionUpdateOptions options)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.Update<TEntity>(tableName: tableName,
            entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<QueryField> where,
        ConnectionUpdateOptions options)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.Update<TEntity>(tableName: tableName,
            entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        QueryGroup where,
        ConnectionUpdateOptions options)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.Update<TEntity>(tableName: tableName,
            entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table based on the given entity's primary/identity key value.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        TEntity entity,
        ConnectionUpdateOptions options)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.Update<TEntity>(entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity, TWhat>(this IDbConnection connection,
        TEntity entity,
        TWhat what,
        ConnectionUpdateOptions options)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.Update<TEntity, TWhat>(entity: entity,
            what: what,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        TEntity entity,
        object what,
        ConnectionUpdateOptions options)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.Update<TEntity>(entity: entity,
            what: what,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        TEntity entity,
        Expression<Func<TEntity, bool>> where,
        ConnectionUpdateOptions options)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.Update<TEntity>(entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        TEntity entity,
        QueryField where,
        ConnectionUpdateOptions options)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.Update<TEntity>(entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        TEntity entity,
        IEnumerable<QueryField> where,
        ConnectionUpdateOptions options)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.Update<TEntity>(entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity>(this IDbConnection connection,
        TEntity entity,
        QueryGroup where,
        ConnectionUpdateOptions options)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.Update<TEntity>(entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    #endregion

    #region UpdateAsync<TEntity>

    /// <summary>
    /// Updates an existing row in the table based on the given entity's primary/identity key value in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        ConnectionUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.UpdateAsync<TEntity>(tableName: tableName,
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
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity, TWhat>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        TWhat what,
        ConnectionUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.UpdateAsync<TEntity, TWhat>(tableName: tableName,
            entity: entity,
            what: what,
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
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        object what,
        ConnectionUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.UpdateAsync<TEntity>(tableName: tableName,
            entity: entity,
            what: what,
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
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, bool>> where,
        ConnectionUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.UpdateAsync<TEntity>(tableName: tableName,
            entity: entity,
            where: where,
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
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        QueryField where,
        ConnectionUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.UpdateAsync<TEntity>(tableName: tableName,
            entity: entity,
            where: where,
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
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<QueryField> where,
        ConnectionUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.UpdateAsync<TEntity>(tableName: tableName,
            entity: entity,
            where: where,
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
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        QueryGroup where,
        ConnectionUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.UpdateAsync<TEntity>(tableName: tableName,
            entity: entity,
            where: where,
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
    /// Updates an existing row in the table based on the given entity's primary/identity key value in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        ConnectionUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.UpdateAsync<TEntity>(entity: entity,
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
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity, TWhat>(this IDbConnection connection,
        TEntity entity,
        TWhat what,
        ConnectionUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.UpdateAsync<TEntity, TWhat>(entity: entity,
            what: what,
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
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        object what,
        ConnectionUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.UpdateAsync<TEntity>(entity: entity,
            what: what,
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
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        Expression<Func<TEntity, bool>> where,
        ConnectionUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.UpdateAsync<TEntity>(entity: entity,
            where: where,
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
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        QueryField where,
        ConnectionUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.UpdateAsync<TEntity>(entity: entity,
            where: where,
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
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        IEnumerable<QueryField> where,
        ConnectionUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.UpdateAsync<TEntity>(entity: entity,
            where: where,
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
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        QueryGroup where,
        ConnectionUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionUpdateOptions();
        return connection.UpdateAsync<TEntity>(entity: entity,
            where: where,
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
