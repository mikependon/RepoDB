#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.Options;

/// <summary>
/// Contains the extension methods for <see cref="DbRepository{TDbConnection}"/> object that accept an <see cref="RepositoryInsertOptions"/> object.
/// </summary>
public static partial class DbRepositoryExtension
{
    #region Insert<TEntity>

    /// <summary>
    /// Inserts a new row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be inserted.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Insert<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        RepositoryInsertOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryInsertOptions();
        return repository.Insert<TEntity>(tableName: tableName,
            entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Inserts a new row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be inserted.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Insert<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        RepositoryInsertOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryInsertOptions();
        return repository.Insert<TEntity, TResult>(tableName: tableName,
            entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Inserts a new row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be inserted.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Insert<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        RepositoryInsertOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryInsertOptions();
        return repository.Insert<TEntity>(entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Inserts a new row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be inserted.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Insert<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        RepositoryInsertOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryInsertOptions();
        return repository.Insert<TEntity, TResult>(entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    #endregion

    #region InsertAsync<TEntity>

    /// <summary>
    /// Inserts a new row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be inserted.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> InsertAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        RepositoryInsertOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryInsertOptions();
        return repository.InsertAsync<TEntity>(tableName: tableName,
            entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Inserts a new row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be inserted.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> InsertAsync<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        RepositoryInsertOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryInsertOptions();
        return repository.InsertAsync<TEntity, TResult>(tableName: tableName,
            entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Inserts a new row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be inserted.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> InsertAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        RepositoryInsertOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryInsertOptions();
        return repository.InsertAsync<TEntity>(entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Inserts a new row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be inserted.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> InsertAsync<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        RepositoryInsertOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryInsertOptions();
        return repository.InsertAsync<TEntity, TResult>(entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    #endregion
}
