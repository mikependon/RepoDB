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
/// Contains the extension methods for <see cref="IDbConnection"/> object that accept a <see cref="ConnectionMergeOptions"/> object.
/// </summary>
public static partial class DbConnectionExtension
{
    #region Merge<TEntity, TResult>

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        ConnectionMergeOptions options)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.Merge<TEntity>(tableName: tableName,
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
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        Field qualifier,
        ConnectionMergeOptions options)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.Merge<TEntity>(tableName: tableName,
            entity: entity,
            qualifier: qualifier,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field> qualifiers,
        ConnectionMergeOptions options)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.Merge<TEntity>(tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The expression that defines the list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, object>> qualifiers,
        ConnectionMergeOptions options)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.Merge<TEntity>(tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        ConnectionMergeOptions options)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.Merge<TEntity, TResult>(tableName: tableName,
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
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        Field qualifier,
        ConnectionMergeOptions options)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.Merge<TEntity, TResult>(tableName: tableName,
            entity: entity,
            qualifier: qualifier,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field> qualifiers,
        ConnectionMergeOptions options)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.Merge<TEntity, TResult>(tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The expression that defines the list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, object>> qualifiers,
        ConnectionMergeOptions options)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.Merge<TEntity, TResult>(tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity>(this IDbConnection connection,
        TEntity entity,
        ConnectionMergeOptions options)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.Merge<TEntity>(entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity>(this IDbConnection connection,
        TEntity entity,
        Field qualifier,
        ConnectionMergeOptions options)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.Merge<TEntity>(entity: entity,
            qualifier: qualifier,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity>(this IDbConnection connection,
        TEntity entity,
        IEnumerable<Field> qualifiers,
        ConnectionMergeOptions options)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.Merge<TEntity>(entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The expression that defines the list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static object Merge<TEntity>(this IDbConnection connection,
        TEntity entity,
        Expression<Func<TEntity, object>> qualifiers,
        ConnectionMergeOptions options)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.Merge<TEntity>(entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        ConnectionMergeOptions options)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.Merge<TEntity, TResult>(entity: entity,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        Field qualifier,
        ConnectionMergeOptions options)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.Merge<TEntity, TResult>(entity: entity,
            qualifier: qualifier,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        IEnumerable<Field> qualifiers,
        ConnectionMergeOptions options)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.Merge<TEntity, TResult>(entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    /// <summary>
    /// Merges an existing row in the table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The expression that defines the list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        Expression<Func<TEntity, object>> qualifiers,
        ConnectionMergeOptions options)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.Merge<TEntity, TResult>(entity: entity,
            qualifiers: qualifiers,
            fields: options.Fields,
            hints: options.Hints,
            commandTimeout: options.CommandTimeout,
            traceKey: options.TraceKey,
            transaction: options.Transaction,
            trace: options.Trace,
            statementBuilder: options.StatementBuilder);
    }

    #endregion

    #region MergeAsync<TEntity, TResult>

    /// <summary>
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        ConnectionMergeOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.MergeAsync<TEntity>(tableName: tableName,
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
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        Field qualifier,
        ConnectionMergeOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.MergeAsync<TEntity>(tableName: tableName,
            entity: entity,
            qualifier: qualifier,
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
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field> qualifiers,
        ConnectionMergeOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.MergeAsync<TEntity>(tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
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
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The expression that defines the list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, object>> qualifiers,
        ConnectionMergeOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.MergeAsync<TEntity>(tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
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
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        ConnectionMergeOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.MergeAsync<TEntity, TResult>(tableName: tableName,
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
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        Field qualifier,
        ConnectionMergeOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.MergeAsync<TEntity, TResult>(tableName: tableName,
            entity: entity,
            qualifier: qualifier,
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
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        IEnumerable<Field> qualifiers,
        ConnectionMergeOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.MergeAsync<TEntity, TResult>(tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
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
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="tableName">The name of the target table.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The expression that defines the list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
        string tableName,
        TEntity entity,
        Expression<Func<TEntity, object>> qualifiers,
        ConnectionMergeOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.MergeAsync<TEntity, TResult>(tableName: tableName,
            entity: entity,
            qualifiers: qualifiers,
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
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        ConnectionMergeOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.MergeAsync<TEntity>(entity: entity,
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
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        Field qualifier,
        ConnectionMergeOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.MergeAsync<TEntity>(entity: entity,
            qualifier: qualifier,
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
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        IEnumerable<Field> qualifiers,
        ConnectionMergeOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.MergeAsync<TEntity>(entity: entity,
            qualifiers: qualifiers,
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
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The expression that defines the list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
        TEntity entity,
        Expression<Func<TEntity, object>> qualifiers,
        ConnectionMergeOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.MergeAsync<TEntity>(entity: entity,
            qualifiers: qualifiers,
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
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        ConnectionMergeOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.MergeAsync<TEntity, TResult>(entity: entity,
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
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifier">The field to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        Field qualifier,
        ConnectionMergeOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.MergeAsync<TEntity, TResult>(entity: entity,
            qualifier: qualifier,
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
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        IEnumerable<Field> qualifiers,
        ConnectionMergeOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.MergeAsync<TEntity, TResult>(entity: entity,
            qualifiers: qualifiers,
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
    /// Merges an existing row in the table in an asynchronous way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    /// <typeparam name="TResult">The target type of the result.</typeparam>
    /// <param name="connection">The connection object to be used.</param>
    /// <param name="entity">The data entity object to be merged.</param>
    /// <param name="qualifiers">The expression that defines the list of fields to be used during merge operation.</param>
    /// <param name="options">The options to be used.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
    /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
    public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
        TEntity entity,
        Expression<Func<TEntity, object>> qualifiers,
        ConnectionMergeOptions options,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        options ??= new ConnectionMergeOptions();
        return connection.MergeAsync<TEntity, TResult>(entity: entity,
            qualifiers: qualifiers,
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
