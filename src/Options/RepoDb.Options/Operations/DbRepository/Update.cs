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
/// Contains the extension methods for <see cref="DbRepository{TDbConnection}"/> object that accept an <see cref="RepositoryUpdateOptions"/> object.
/// </summary>
public static partial class DbRepositoryExtension
{
    #region Update<TEntity>

    /// <summary>
    /// Updates an existing row in the table based on the given entity's primary/identity key value.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        RepositoryUpdateOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.Update<TEntity>(tableName: tableName,
            entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity, TDbConnection, TWhat>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        TWhat what,
        RepositoryUpdateOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.Update<TEntity, TWhat>(tableName: tableName,
            entity: entity,
            what: what,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        object what,
        RepositoryUpdateOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.Update<TEntity>(tableName: tableName,
            entity: entity,
            what: what,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, bool>> where,
        RepositoryUpdateOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.Update<TEntity>(tableName: tableName,
            entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        QueryField where,
        RepositoryUpdateOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.Update<TEntity>(tableName: tableName,
            entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        IEnumerable<QueryField> where,
        RepositoryUpdateOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.Update<TEntity>(tableName: tableName,
            entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        QueryGroup where,
        RepositoryUpdateOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.Update<TEntity>(tableName: tableName,
            entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Updates an existing row in the table based on the given entity's primary/identity key value.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        RepositoryUpdateOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.Update<TEntity>(entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity, TDbConnection, TWhat>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        TWhat what,
        RepositoryUpdateOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.Update<TEntity, TWhat>(entity: entity,
            what: what,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        object what,
        RepositoryUpdateOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.Update<TEntity>(entity: entity,
            what: what,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        Expression<Func<TEntity, bool>> where,
        RepositoryUpdateOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.Update<TEntity>(entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        QueryField where,
        RepositoryUpdateOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.Update<TEntity>(entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        IEnumerable<QueryField> where,
        RepositoryUpdateOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.Update<TEntity>(entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static int Update<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        QueryGroup where,
        RepositoryUpdateOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.Update<TEntity>(entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    #endregion

    #region UpdateAsync<TEntity>

    /// <summary>
    /// Updates an existing row in the table based on the given entity's primary/identity key value in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        RepositoryUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.UpdateAsync<TEntity>(tableName: tableName,
            entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity, TDbConnection, TWhat>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        TWhat what,
        RepositoryUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.UpdateAsync<TEntity, TWhat>(tableName: tableName,
            entity: entity,
            what: what,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        object what,
        RepositoryUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.UpdateAsync<TEntity>(tableName: tableName,
            entity: entity,
            what: what,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, bool>> where,
        RepositoryUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.UpdateAsync<TEntity>(tableName: tableName,
            entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        QueryField where,
        RepositoryUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.UpdateAsync<TEntity>(tableName: tableName,
            entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        IEnumerable<QueryField> where,
        RepositoryUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.UpdateAsync<TEntity>(tableName: tableName,
            entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        QueryGroup where,
        RepositoryUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.UpdateAsync<TEntity>(tableName: tableName,
            entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing row in the table based on the given entity's primary/identity key value in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        RepositoryUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.UpdateAsync<TEntity>(entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity, TDbConnection, TWhat>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        TWhat what,
        RepositoryUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.UpdateAsync<TEntity, TWhat>(entity: entity,
            what: what,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="what">The dynamic expression or the key value to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        object what,
        RepositoryUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.UpdateAsync<TEntity>(entity: entity,
            what: what,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        Expression<Func<TEntity, bool>> where,
        RepositoryUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.UpdateAsync<TEntity>(entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        QueryField where,
        RepositoryUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.UpdateAsync<TEntity>(entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        IEnumerable<QueryField> where,
        RepositoryUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.UpdateAsync<TEntity>(entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing row in the table based on a given expression in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be used.</param>
    /// <param name="where">The query expression to be used.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The number of rows that has been updated from the table.</returns>
    public static Task<int> UpdateAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        QueryGroup where,
        RepositoryUpdateOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryUpdateOptions();
        return repository.UpdateAsync<TEntity>(entity: entity,
            where: where,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    #endregion
}
