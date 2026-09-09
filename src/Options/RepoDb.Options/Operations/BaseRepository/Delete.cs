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
/// Contains the extension methods for <see cref="BaseRepository{TEntity, TDbConnection}"/> object that accept a <see cref="RepositoryDeleteOptions"/> object.
/// </summary>
public static partial class BaseRepositoryExtension
{
    #region Delete<TEntity>

    /// <summary>
    /// Deletes an existing row from the table based on the given entity's primary/identity key value.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int Delete<TEntity, TDbConnection>(this BaseRepository<TEntity, TDbConnection> repository,
        TEntity entity,
        RepositoryDeleteOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryDeleteOptions();
        return repository.Delete(entity: entity,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int Delete<TEntity, TDbConnection, TWhat>(this BaseRepository<TEntity, TDbConnection> repository,
        TWhat what,
        RepositoryDeleteOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryDeleteOptions();
        return repository.Delete<TWhat>(what: what,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int Delete<TEntity, TDbConnection>(this BaseRepository<TEntity, TDbConnection> repository,
        object what,
        RepositoryDeleteOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryDeleteOptions();
        return repository.Delete(what: what,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int Delete<TEntity, TDbConnection>(this BaseRepository<TEntity, TDbConnection> repository,
        Expression<Func<TEntity, bool>> where,
        RepositoryDeleteOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryDeleteOptions();
        return repository.Delete(where: where,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int Delete<TEntity, TDbConnection>(this BaseRepository<TEntity, TDbConnection> repository,
        QueryField where,
        RepositoryDeleteOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryDeleteOptions();
        return repository.Delete(where: where,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int Delete<TEntity, TDbConnection>(this BaseRepository<TEntity, TDbConnection> repository,
        IEnumerable<QueryField> where,
        RepositoryDeleteOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryDeleteOptions();
        return repository.Delete(where: where,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static int Delete<TEntity, TDbConnection>(this BaseRepository<TEntity, TDbConnection> repository,
        QueryGroup where,
        RepositoryDeleteOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryDeleteOptions();
        return repository.Delete(where: where,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    #endregion

    #region DeleteAsync<TEntity>

    /// <summary>
    /// Deletes an existing row from the table based on the given entity's primary/identity key value in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static Task<int> DeleteAsync<TEntity, TDbConnection>(this BaseRepository<TEntity, TDbConnection> repository,
        TEntity entity,
        RepositoryDeleteOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryDeleteOptions();
        return repository.DeleteAsync(entity: entity,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static Task<int> DeleteAsync<TEntity, TDbConnection, TWhat>(this BaseRepository<TEntity, TDbConnection> repository,
        TWhat what,
        RepositoryDeleteOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryDeleteOptions();
        return repository.DeleteAsync<TWhat>(what: what,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static Task<int> DeleteAsync<TEntity, TDbConnection>(this BaseRepository<TEntity, TDbConnection> repository,
        object what,
        RepositoryDeleteOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryDeleteOptions();
        return repository.DeleteAsync(what: what,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static Task<int> DeleteAsync<TEntity, TDbConnection>(this BaseRepository<TEntity, TDbConnection> repository,
        Expression<Func<TEntity, bool>> where,
        RepositoryDeleteOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryDeleteOptions();
        return repository.DeleteAsync(where: where,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static Task<int> DeleteAsync<TEntity, TDbConnection>(this BaseRepository<TEntity, TDbConnection> repository,
        QueryField where,
        RepositoryDeleteOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryDeleteOptions();
        return repository.DeleteAsync(where: where,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static Task<int> DeleteAsync<TEntity, TDbConnection>(this BaseRepository<TEntity, TDbConnection> repository,
        IEnumerable<QueryField> where,
        RepositoryDeleteOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryDeleteOptions();
        return repository.DeleteAsync(where: where,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes an existing row from the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been deleted from the table.</returns>
    public static Task<int> DeleteAsync<TEntity, TDbConnection>(this BaseRepository<TEntity, TDbConnection> repository,
        QueryGroup where,
        RepositoryDeleteOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryDeleteOptions();
        return repository.DeleteAsync(where: where,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    #endregion
}
