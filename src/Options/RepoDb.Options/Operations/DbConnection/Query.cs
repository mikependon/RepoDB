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
/// Contains the extension methods for <see cref="IDbConnection"/> object that accept a <see cref="ConnectionQueryOptions"/> object.
/// </summary>
public static partial class DbConnectionExtension
{
    #region Query<TEntity>

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        string tableName,
        object what,
        ConnectionQueryOptions options)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.Query<TEntity>(tableName: tableName,
            what: what,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity, TWhat>(this IDbConnection connection,
        string tableName,
        TWhat what,
        ConnectionQueryOptions options)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.Query<TEntity, TWhat>(tableName: tableName,
            what: what,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        string tableName,
        Expression<Func<TEntity, bool>> where,
        ConnectionQueryOptions options)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.Query<TEntity>(tableName: tableName,
            where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        string tableName,
        QueryField where,
        ConnectionQueryOptions options)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.Query<TEntity>(tableName: tableName,
            where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<QueryField> where,
        ConnectionQueryOptions options)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.Query<TEntity>(tableName: tableName,
            where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        string tableName,
        QueryGroup where,
        ConnectionQueryOptions options)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.Query<TEntity>(tableName: tableName,
            where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        object what,
        ConnectionQueryOptions options)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.Query<TEntity>(what: what,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity, TWhat>(this IDbConnection connection,
        TWhat what,
        ConnectionQueryOptions options)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.Query<TEntity, TWhat>(what: what,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        QueryField where,
        ConnectionQueryOptions options)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.Query<TEntity>(where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        Expression<Func<TEntity, bool>> where,
        ConnectionQueryOptions options)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.Query<TEntity>(where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        IEnumerable<QueryField> where,
        ConnectionQueryOptions options)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.Query<TEntity>(where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
        QueryGroup where,
        ConnectionQueryOptions options)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.Query<TEntity>(where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    #endregion

    #region QueryAsync<TEntity>

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        string tableName,
        object what,
        ConnectionQueryOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return await connection.QueryAsync<TEntity>(tableName: tableName,
            what: what,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity, TWhat>(this IDbConnection connection,
        string tableName,
        TWhat what,
        ConnectionQueryOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return await connection.QueryAsync<TEntity, TWhat>(tableName: tableName,
            what: what,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        string tableName,
        Expression<Func<TEntity, bool>> where,
        ConnectionQueryOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.QueryAsync<TEntity>(tableName: tableName,
            where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        string tableName,
        QueryField where,
        ConnectionQueryOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.QueryAsync<TEntity>(tableName: tableName,
            where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        string tableName,
        IEnumerable<QueryField> where,
        ConnectionQueryOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.QueryAsync<TEntity>(tableName: tableName,
            where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        string tableName,
        QueryGroup where,
        ConnectionQueryOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.QueryAsync<TEntity>(tableName: tableName,
            where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        object what,
        ConnectionQueryOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return await connection.QueryAsync<TEntity>(what: what,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity, TWhat>(this IDbConnection connection,
        TWhat what,
        ConnectionQueryOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return await connection.QueryAsync<TEntity, TWhat>(what: what,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        QueryField where,
        ConnectionQueryOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.QueryAsync<TEntity>(where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        IEnumerable<QueryField> where,
        ConnectionQueryOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.QueryAsync<TEntity>(where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        Expression<Func<TEntity, bool>> where,
        ConnectionQueryOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.QueryAsync<TEntity>(where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
        QueryGroup where,
        ConnectionQueryOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionQueryOptions();
        return connection.QueryAsync<TEntity>(where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            cacheItemExpiration: options.CacheItemExpiration,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cache: options.Cache,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder,
            cancellationToken: cancellationToken);
    }

    #endregion
}
