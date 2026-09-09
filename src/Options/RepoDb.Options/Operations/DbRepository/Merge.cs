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
/// Contains the extension methods for <see cref="DbRepository{TDbConnection}"/> object that accept a <see cref="RepositoryMergeOptions"/> object.
/// </summary>
public static partial class DbRepositoryExtension
{
    #region Merge<TEntity>

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        RepositoryMergeOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.Merge<TEntity>(tableName: tableName,
            entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        Field qualifier,
        RepositoryMergeOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.Merge<TEntity>(tableName: tableName,
            entity: entity,
            qualifier: qualifier,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        IEnumerable<Field> qualifiers,
        RepositoryMergeOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.Merge<TEntity>(tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The expression that defines the list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, object>> qualifiers,
        RepositoryMergeOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.Merge<TEntity>(tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        RepositoryMergeOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.Merge<TEntity, TResult>(tableName: tableName,
            entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        Field qualifier,
        RepositoryMergeOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.Merge<TEntity, TResult>(tableName: tableName,
            entity: entity,
            qualifier: qualifier,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        IEnumerable<Field> qualifiers,
        RepositoryMergeOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.Merge<TEntity, TResult>(tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The expression that defines the list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, object>> qualifiers,
        RepositoryMergeOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.Merge<TEntity, TResult>(tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        RepositoryMergeOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.Merge<TEntity>(entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        Field qualifier,
        RepositoryMergeOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.Merge<TEntity>(entity: entity,
            qualifier: qualifier,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        IEnumerable<Field> qualifiers,
        RepositoryMergeOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.Merge<TEntity>(entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The expression that defines the list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        Expression<Func<TEntity, object>> qualifiers,
        RepositoryMergeOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.Merge<TEntity>(entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        RepositoryMergeOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.Merge<TEntity, TResult>(entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        Field qualifier,
        RepositoryMergeOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.Merge<TEntity, TResult>(entity: entity,
            qualifier: qualifier,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        IEnumerable<Field> qualifiers,
        RepositoryMergeOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.Merge<TEntity, TResult>(entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The expression that defines the list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        Expression<Func<TEntity, object>> qualifiers,
        RepositoryMergeOptions options)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.Merge<TEntity, TResult>(entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction);
    }

    #endregion

    #region MergeAsync<TEntity>

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> MergeAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        RepositoryMergeOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.MergeAsync<TEntity>(tableName: tableName,
            entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> MergeAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        Field qualifier,
        RepositoryMergeOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.MergeAsync<TEntity>(tableName: tableName,
            entity: entity,
            qualifier: qualifier,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> MergeAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        IEnumerable<Field> qualifiers,
        RepositoryMergeOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.MergeAsync<TEntity>(tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The expression that defines the list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> MergeAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, object>> qualifiers,
        RepositoryMergeOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.MergeAsync<TEntity>(tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> MergeAsync<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        RepositoryMergeOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.MergeAsync<TEntity, TResult>(tableName: tableName,
            entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> MergeAsync<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        Field qualifier,
        RepositoryMergeOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.MergeAsync<TEntity, TResult>(tableName: tableName,
            entity: entity,
            qualifier: qualifier,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> MergeAsync<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        IEnumerable<Field> qualifiers,
        RepositoryMergeOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.MergeAsync<TEntity, TResult>(tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The expression that defines the list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> MergeAsync<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, object>> qualifiers,
        RepositoryMergeOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.MergeAsync<TEntity, TResult>(tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> MergeAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        RepositoryMergeOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.MergeAsync<TEntity>(entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> MergeAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        Field qualifier,
        RepositoryMergeOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.MergeAsync<TEntity>(entity: entity,
            qualifier: qualifier,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> MergeAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        IEnumerable<Field> qualifiers,
        RepositoryMergeOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.MergeAsync<TEntity>(entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The expression that defines the list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> MergeAsync<TEntity, TDbConnection>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        Expression<Func<TEntity, object>> qualifiers,
        RepositoryMergeOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.MergeAsync<TEntity>(entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> MergeAsync<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        RepositoryMergeOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.MergeAsync<TEntity, TResult>(entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> MergeAsync<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        Field qualifier,
        RepositoryMergeOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.MergeAsync<TEntity, TResult>(entity: entity,
            qualifier: qualifier,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> MergeAsync<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        IEnumerable<Field> qualifiers,
        RepositoryMergeOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.MergeAsync<TEntity, TResult>(entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TDbConnection">The type of the <see cref="DbConnection"/> object.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="repository">The repository instance to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The expression that defines the list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> MergeAsync<TEntity, TDbConnection, TResult>(this DbRepository<TDbConnection> repository,
        TEntity entity,
        Expression<Func<TEntity, object>> qualifiers,
        RepositoryMergeOptions options,
        CancellationToken cancellationToken = default)
        where TDbConnection : DbConnection, new()
        where TEntity : class
    {
        options ??= new RepositoryMergeOptions();
        return repository.MergeAsync<TEntity, TResult>(entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            cancellationToken: cancellationToken);
    }

    #endregion
}
