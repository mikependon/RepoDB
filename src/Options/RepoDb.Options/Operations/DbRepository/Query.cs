#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.Options;

/// <summary>
/// Contains the extension methods for <see cref="DbRepository{TDbConnection}"/> object that accept a <see cref="RepositoryQueryOptions"/> object.
/// </summary>
public static partial class DbRepositoryExtension
{
    #region Query<TEntity>

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        object what,
        RepositoryQueryOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.Query<TEntity>(tableName: tableName,
            what: what,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity, TDbConnection, TWhat>(this DbRepository<TDbConnection> repository,
        string tableName,
        TWhat what,
        RepositoryQueryOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.Query<TEntity, TWhat>(tableName: tableName,
            what: what,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        Expression<Func<TEntity, bool>> where,
        RepositoryQueryOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.Query<TEntity>(tableName: tableName,
            where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        QueryField where,
        RepositoryQueryOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.Query<TEntity>(tableName: tableName,
            where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        IEnumerable<QueryField> where,
        RepositoryQueryOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.Query<TEntity>(tableName: tableName,
            where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        QueryGroup where,
        RepositoryQueryOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.Query<TEntity>(tableName: tableName,
            where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        object what,
        RepositoryQueryOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.Query<TEntity>(what: what,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity, TDbConnection, TWhat>(this DbRepository<TDbConnection> repository,
        TWhat what,
        RepositoryQueryOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.Query<TEntity, TWhat>(what: what,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        Expression<Func<TEntity, bool>> where,
        RepositoryQueryOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.Query<TEntity>(where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        QueryField where,
        RepositoryQueryOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.Query<TEntity>(where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        IEnumerable<QueryField> where,
        RepositoryQueryOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.Query<TEntity>(where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static IEnumerable<TEntity> Query<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        QueryGroup where,
        RepositoryQueryOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.Query<TEntity>(where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    #endregion

    #region QueryAsync<TEntity>

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        object what,
        RepositoryQueryOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.QueryAsync<TEntity>(tableName: tableName,
            what: what,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity, TDbConnection, TWhat>(this DbRepository<TDbConnection> repository,
        string tableName,
        TWhat what,
        RepositoryQueryOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.QueryAsync<TEntity, TWhat>(tableName: tableName,
            what: what,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        Expression<Func<TEntity, bool>> where,
        RepositoryQueryOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.QueryAsync<TEntity>(tableName: tableName,
            where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        QueryField where,
        RepositoryQueryOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.QueryAsync<TEntity>(tableName: tableName,
            where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        IEnumerable<QueryField> where,
        RepositoryQueryOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.QueryAsync<TEntity>(tableName: tableName,
            where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        QueryGroup where,
        RepositoryQueryOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.QueryAsync<TEntity>(tableName: tableName,
            where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        object what,
        RepositoryQueryOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.QueryAsync<TEntity>(what: what,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity, TDbConnection, TWhat>(this DbRepository<TDbConnection> repository,
        TWhat what,
        RepositoryQueryOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.QueryAsync<TEntity, TWhat>(what: what,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        Expression<Func<TEntity, bool>> where,
        RepositoryQueryOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.QueryAsync<TEntity>(where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        QueryField where,
        RepositoryQueryOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.QueryAsync<TEntity>(where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        IEnumerable<QueryField> where,
        RepositoryQueryOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.QueryAsync<TEntity>(where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Query the existing rows from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>An enumerable list of data entity objects.</returns>
    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        QueryGroup where,
        RepositoryQueryOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryQueryOptions();
        return repository.QueryAsync<TEntity>(where: where,
            fields: options.Fields,
            orderBy: options.OrderBy,
            top: options.Top,
            hints: options.Hints,
            cacheKey: options.CacheKey,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    #endregion
}
