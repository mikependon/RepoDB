#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using static RepoDb.Schema.DbConnection;

namespace RepoDb.Schema
{
    /// <summary>
    /// Contains the extension methods of <see cref="IDbConnection"/> object for copying the schema of a table or multiple tables between databases.
    /// </summary>
    public static class CopyTableSchemaToExtension
    {
        #region Public Methods

        #region CopyTableSchemaTo (IDbConnection, Sync)

        /// <summary>
        /// Copies the schema of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="columns">The names of the columns of the source table to be copied. The default is <c>null</c>, which copies all the columns.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyTableSchemaResult"/> that describes the schema copy operation.</returns>
        public static CopyTableSchemaResult CopyTableSchemaTo<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            string targetSchema = null,
            string[] columns = null,
            CopyTableSchemaExistsBehavior tableExistenceBehavior = CopyTableSchemaExistsBehavior.Skip,
            CopyTableSchemaRelationshipBehavior relationshipBehavior = CopyTableSchemaRelationshipBehavior.TableOnly,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopyTableSchemaTo(ClassMappedNameCache.Get<TEntity>(), destinationConnection, targetSchema, columns, tableExistenceBehavior, relationshipBehavior, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the schema of the source table of the current connection into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="columns">The names of the columns of the source table to be copied. The default is <c>null</c>, which copies all the columns.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyTableSchemaResult"/> that describes the schema copy operation.</returns>
        public static CopyTableSchemaResult CopyTableSchemaTo(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            string targetSchema = null,
            string[] columns = null,
            CopyTableSchemaExistsBehavior tableExistenceBehavior = CopyTableSchemaExistsBehavior.Skip,
            CopyTableSchemaRelationshipBehavior relationshipBehavior = CopyTableSchemaRelationshipBehavior.TableOnly,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            Validate(connection, tableName, destinationConnection);

            // Copy the table as a set of one table, the callback receives the result of each table that is copied
            var results = new List<CopyTableSchemaResult>();
            CopyTableSchemasCore(connection, new[] { tableName }, destinationConnection, targetSchema, CopyTableSchemaColumns.Normalize(columns), null, tableExistenceBehavior, relationshipBehavior, results.Add, null, commandTimeout, traceKey, trace, transaction);
            return results.Count <= 1
                ? results.FirstOrDefault()
                : FindResult(results, SchemaReaderMapper.Get(connection).GetTableSchema(tableName).Table);
        }

        #endregion

        #region CopyTableSchemaTo (IDbConnection, Sync, Multiple Tables)

        /// <summary>
        /// Copies the schema of the tables of the current connection into the same-named tables of the destination connection.
        /// The tables are created together: all the tables first, then all the indexes and then all the foreign keys, so the tables can
        /// reference each other (even in a circular way) as long as the referenced tables are part of the given tables or already exist
        /// in the destination database. The source tables are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableNames">The names of the tables whose schema is to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the given tables. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResult"/> of each table once its schema is created. The default is <c>null</c>.</param>
        /// <param name="errorCallback">The callback that receives a <see cref="CopyTableSchemaError"/> when a statement fails, so the copy continues with the next statement (without it, the exception is thrown).</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemasTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static void CopyTableSchemaTo(this IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            string targetSchema = null,
            CopyTableSchemaExistsBehavior tableExistenceBehavior = CopyTableSchemaExistsBehavior.Skip,
            CopyTableSchemaRelationshipBehavior relationshipBehavior = CopyTableSchemaRelationshipBehavior.TableOnly,
            Action<CopyTableSchemaResult> createdCallback = null,
            Action<CopyTableSchemaError> errorCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemasTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            CopyTableSchemasCore(connection, tableNames, destinationConnection, targetSchema, null, null, tableExistenceBehavior, relationshipBehavior, createdCallback, errorCallback, commandTimeout, traceKey, trace, transaction);
        }

        #endregion

        #region CopyTableSchemaTo (IDbConnection, Async)

        /// <summary>
        /// Copies the schema of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="columns">The names of the columns of the source table to be copied. The default is <c>null</c>, which copies all the columns.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyTableSchemaResult"/> that describes the schema copy operation.</returns>
        public static Task<CopyTableSchemaResult> CopyTableSchemaToAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            string targetSchema = null,
            string[] columns = null,
            CopyTableSchemaExistsBehavior tableExistenceBehavior = CopyTableSchemaExistsBehavior.Skip,
            CopyTableSchemaRelationshipBehavior relationshipBehavior = CopyTableSchemaRelationshipBehavior.TableOnly,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopyTableSchemaToAsync(ClassMappedNameCache.Get<TEntity>(), destinationConnection, targetSchema, columns, tableExistenceBehavior, relationshipBehavior, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the schema of the source table of the current connection into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="columns">The names of the columns of the source table to be copied. The default is <c>null</c>, which copies all the columns.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyTableSchemaResult"/> that describes the schema copy operation.</returns>
        public static async Task<CopyTableSchemaResult> CopyTableSchemaToAsync(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            string targetSchema = null,
            string[] columns = null,
            CopyTableSchemaExistsBehavior tableExistenceBehavior = CopyTableSchemaExistsBehavior.Skip,
            CopyTableSchemaRelationshipBehavior relationshipBehavior = CopyTableSchemaRelationshipBehavior.TableOnly,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            Validate(connection, tableName, destinationConnection);
            var results = new List<CopyTableSchemaResult>();
            await CopyTableSchemasCoreAsync(connection, new[] { tableName }, destinationConnection, targetSchema, CopyTableSchemaColumns.Normalize(columns), null, tableExistenceBehavior, relationshipBehavior, results.Add, null, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false);
            return results.Count <= 1
                ? results.FirstOrDefault()
                : FindResult(results, (await SchemaReaderMapper.Get(connection).GetTableSchemaAsync(tableName, cancellationToken).ConfigureAwait(false)).Table);
        }

        #endregion

        #region CopyTableSchemaTo (IDbConnection, Async, Multiple Tables)

        /// <summary>
        /// Copies the schema of the tables of the current connection into the same-named tables of the destination connection.
        /// The tables are created together: all the tables first, then all the indexes and then all the foreign keys, so the tables can
        /// reference each other (even in a circular way) as long as the referenced tables are part of the given tables or already exist
        /// in the destination database. The source tables are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableNames">The names of the tables whose schema is to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the given tables. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResult"/> of each table once its schema is created. The default is <c>null</c>.</param>
        /// <param name="errorCallback">The callback that receives a <see cref="CopyTableSchemaError"/> when a statement fails, so the copy continues with the next statement (without it, the exception is thrown).</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemasTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task CopyTableSchemaToAsync(this IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            string targetSchema = null,
            CopyTableSchemaExistsBehavior tableExistenceBehavior = CopyTableSchemaExistsBehavior.Skip,
            CopyTableSchemaRelationshipBehavior relationshipBehavior = CopyTableSchemaRelationshipBehavior.TableOnly,
            Action<CopyTableSchemaResult> createdCallback = null,
            Action<CopyTableSchemaError> errorCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemasTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            await CopyTableSchemasCoreAsync(connection, tableNames, destinationConnection, targetSchema, null, null, tableExistenceBehavior, relationshipBehavior, createdCallback, errorCallback, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region CopyTableSchemaTo With Column Mappings (IDbConnection, Sync)

        /// <summary>
        /// Copies the schema of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="mappings">The columns of the source table to be copied, as <see cref="CopyTableSchemaColumnMapping"/> (the source column and its name in the destination table).</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyTableSchemaResult"/> that describes the schema copy operation.</returns>
        public static CopyTableSchemaResult CopyTableSchemaTo<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            IList<CopyTableSchemaColumnMapping> mappings,
            string targetSchema = null,
            CopyTableSchemaExistsBehavior tableExistenceBehavior = CopyTableSchemaExistsBehavior.Skip,
            CopyTableSchemaRelationshipBehavior relationshipBehavior = CopyTableSchemaRelationshipBehavior.TableOnly,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopyTableSchemaTo(ClassMappedNameCache.Get<TEntity>(), destinationConnection, mappings, targetSchema, tableExistenceBehavior, relationshipBehavior, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the schema of the source table of the current connection into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="mappings">The columns of the source table to be copied, as <see cref="CopyTableSchemaColumnMapping"/> (the source column and its name in the destination table).</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyTableSchemaResult"/> that describes the schema copy operation.</returns>
        public static CopyTableSchemaResult CopyTableSchemaTo(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            IList<CopyTableSchemaColumnMapping> mappings,
            string targetSchema = null,
            CopyTableSchemaExistsBehavior tableExistenceBehavior = CopyTableSchemaExistsBehavior.Skip,
            CopyTableSchemaRelationshipBehavior relationshipBehavior = CopyTableSchemaRelationshipBehavior.TableOnly,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            Validate(connection, tableName, destinationConnection);
            CopyTableSchemaColumns.Normalize(mappings, out _, out _);

            // Copy the table as a set of one table, the callback receives the result of each table that is copied
            var results = new List<CopyTableSchemaResult>();
            connection.CopyTableSchemaTo(new[] { tableName },
                destinationConnection,
                new[] { new CopyTableSchemaTable(tableName, mappings) },
                targetSchema,
                tableExistenceBehavior,
                relationshipBehavior,
                createdCallback: results.Add,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                trace: trace,
                transaction: transaction);
            return results.Count <= 1
                ? results.FirstOrDefault()
                : FindResult(results, SchemaReaderMapper.Get(connection).GetTableSchema(tableName).Table);
        }

        #endregion

        #region CopyTableSchemaTo With Column Mappings (IDbConnection, Sync, Multiple Tables)

        /// <summary>
        /// Copies the schema of the tables of the current connection into the same-named tables of the destination connection.
        /// The tables are created together: all the tables first, then all the indexes and then all the foreign keys, so the tables can
        /// reference each other (even in a circular way) as long as the referenced tables are part of the given tables or already exist
        /// in the destination database. The source tables are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableNames">The names of the tables whose schema is to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="mappings">The tables to be copied with only their mapped columns, as <see cref="CopyTableSchemaTable"/> (the tables that are not listed keep all their columns).</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the given tables. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResult"/> of each table once its schema is created. The default is <c>null</c>.</param>
        /// <param name="errorCallback">The callback that receives a <see cref="CopyTableSchemaError"/> when a statement fails, so the copy continues with the next statement (without it, the exception is thrown).</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemasTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static void CopyTableSchemaTo(this IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            IList<CopyTableSchemaTable> mappings,
            string targetSchema = null,
            CopyTableSchemaExistsBehavior tableExistenceBehavior = CopyTableSchemaExistsBehavior.Skip,
            CopyTableSchemaRelationshipBehavior relationshipBehavior = CopyTableSchemaRelationshipBehavior.TableOnly,
            Action<CopyTableSchemaResult> createdCallback = null,
            Action<CopyTableSchemaError> errorCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemasTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            CopyTableSchemaColumns.Normalize(mappings);
            CopyTableSchemasCore(connection, tableNames, destinationConnection, targetSchema, null, mappings, tableExistenceBehavior, relationshipBehavior, createdCallback, errorCallback, commandTimeout, traceKey, trace, transaction);
        }

        #endregion

        #region CopyTableSchemaTo With Column Mappings (IDbConnection, Async)

        /// <summary>
        /// Copies the schema of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="mappings">The columns of the source table to be copied, as <see cref="CopyTableSchemaColumnMapping"/> (the source column and its name in the destination table).</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyTableSchemaResult"/> that describes the schema copy operation.</returns>
        public static Task<CopyTableSchemaResult> CopyTableSchemaToAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            IList<CopyTableSchemaColumnMapping> mappings,
            string targetSchema = null,
            CopyTableSchemaExistsBehavior tableExistenceBehavior = CopyTableSchemaExistsBehavior.Skip,
            CopyTableSchemaRelationshipBehavior relationshipBehavior = CopyTableSchemaRelationshipBehavior.TableOnly,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopyTableSchemaToAsync(ClassMappedNameCache.Get<TEntity>(), destinationConnection, mappings, targetSchema, tableExistenceBehavior, relationshipBehavior, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the schema of the source table of the current connection into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="mappings">The columns of the source table to be copied, as <see cref="CopyTableSchemaColumnMapping"/> (the source column and its name in the destination table).</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyTableSchemaResult"/> that describes the schema copy operation.</returns>
        public static async Task<CopyTableSchemaResult> CopyTableSchemaToAsync(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            IList<CopyTableSchemaColumnMapping> mappings,
            string targetSchema = null,
            CopyTableSchemaExistsBehavior tableExistenceBehavior = CopyTableSchemaExistsBehavior.Skip,
            CopyTableSchemaRelationshipBehavior relationshipBehavior = CopyTableSchemaRelationshipBehavior.TableOnly,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            Validate(connection, tableName, destinationConnection);
            CopyTableSchemaColumns.Normalize(mappings, out _, out _);
            var results = new List<CopyTableSchemaResult>();
            await connection.CopyTableSchemaToAsync(new[] { tableName },
                destinationConnection,
                new[] { new CopyTableSchemaTable(tableName, mappings) },
                targetSchema,
                tableExistenceBehavior,
                relationshipBehavior,
                createdCallback: results.Add,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                trace: trace,
                transaction: transaction,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return results.Count <= 1
                ? results.FirstOrDefault()
                : FindResult(results, (await SchemaReaderMapper.Get(connection).GetTableSchemaAsync(tableName, cancellationToken).ConfigureAwait(false)).Table);
        }

        #endregion

        #region CopyTableSchemaTo With Column Mappings (IDbConnection, Async, Multiple Tables)

        /// <summary>
        /// Copies the schema of the tables of the current connection into the same-named tables of the destination connection.
        /// The tables are created together: all the tables first, then all the indexes and then all the foreign keys, so the tables can
        /// reference each other (even in a circular way) as long as the referenced tables are part of the given tables or already exist
        /// in the destination database. The source tables are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableNames">The names of the tables whose schema is to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="mappings">The tables to be copied with only their mapped columns, as <see cref="CopyTableSchemaTable"/> (the tables that are not listed keep all their columns).</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the given tables. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResult"/> of each table once its schema is created. The default is <c>null</c>.</param>
        /// <param name="errorCallback">The callback that receives a <see cref="CopyTableSchemaError"/> when a statement fails, so the copy continues with the next statement (without it, the exception is thrown).</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemasTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task CopyTableSchemaToAsync(this IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            IList<CopyTableSchemaTable> mappings,
            string targetSchema = null,
            CopyTableSchemaExistsBehavior tableExistenceBehavior = CopyTableSchemaExistsBehavior.Skip,
            CopyTableSchemaRelationshipBehavior relationshipBehavior = CopyTableSchemaRelationshipBehavior.TableOnly,
            Action<CopyTableSchemaResult> createdCallback = null,
            Action<CopyTableSchemaError> errorCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemasTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            CopyTableSchemaColumns.Normalize(mappings);
            await CopyTableSchemasCoreAsync(connection, tableNames, destinationConnection, targetSchema, null, mappings, tableExistenceBehavior, relationshipBehavior, createdCallback, errorCallback, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Copies the schema of the tables into the destination connection: the work of the overloads of the multiple tables, on the columns that are already validated.
        /// </summary>
        /// <param name="selectedColumns">The distinct names of the columns to be created (<c>null</c> if all the columns are created).</param>
        /// <param name="tableMappings">The columns to be created table by table (<c>null</c> if the columns are the same for every requested table).</param>
        /// <remarks>See the overloads of <c>CopyTableSchemaTo</c> for the description of the other arguments.</remarks>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        private static void CopyTableSchemasCore(IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            string targetSchema,
            IList<string> selectedColumns,
            IList<CopyTableSchemaTable> tableMappings,
            CopyTableSchemaExistsBehavior tableExistenceBehavior,
            CopyTableSchemaRelationshipBehavior relationshipBehavior,
            Action<CopyTableSchemaResult> createdCallback,
            Action<CopyTableSchemaError> errorCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction)
        {
            // Validate
            var names = Validate(connection, tableNames, destinationConnection);
            var requestedNames = names;
            if (names.Count == 0)
            {
                return;
            }

            // Variables
            var startTime = DateTime.UtcNow;
            var schemaReader = SchemaReaderMapper.Get(connection) ??
                throw new MissingMappingException($"There is no schema reader mapping found for '{connection.GetType().FullName}'. Make sure to register one via the '{nameof(SchemaReaderMapper)}'.");
            var schemaComposer = SchemaComposerMapper.Get(destinationConnection) ??
                throw new MissingMappingException($"There is no schema composer mapping found for '{destinationConnection.GetType().FullName}'. Make sure to register one via the '{nameof(SchemaComposerMapper)}'.");
            if (relationshipBehavior != CopyTableSchemaRelationshipBehavior.TableOnly)
            {
                names = schemaReader.GetRelatedTables(names, relationshipBehavior).ToList();
            }
            var schemas = schemaReader.GetDependencyOrder(names).Select(r => r.Schema).ToList();
            if (tableMappings != null)
            {
                schemas = CopyTableSchemaColumns.Apply(schemas, CopyTableSchemaColumns.Resolve(tableMappings, schemas)).ToList();
            }
            else if (selectedColumns != null)
            {
                var requestedSchemas = requestedNames.Select(schemaReader.GetTableSchema).ToList();
                schemas = CopyTableSchemaColumns.Apply(schemas, requestedSchemas, selectedColumns).ToList();
            }
            var tables = CopyTableSchemaPlan.CreateTables(schemas, GetTargetSchema(targetSchema, destinationConnection), schemaComposer);
            ProbeTables(tables, tableExistenceBehavior, schemaComposer, destinationConnection, commandTimeout, trace, transaction);
            var plan = CopyTableSchemaPlan.Create(tables, tableExistenceBehavior, schemaComposer);
            var errors = new List<CopyTableSchemaError>();

            // Report the tables that have nothing to execute
            Report(createdCallback, plan.Immediate, errors, connection, destinationConnection, tableExistenceBehavior, schemaComposer, startTime);

            // Iterate
            for (var i = 0; i < plan.Steps.Count; i++)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(plan.Steps[i].Statement))
                    {
                        destinationConnection.ExecuteNonQuery(plan.Steps[i].Statement,
                            traceKey: traceKey,
                            commandTimeout: commandTimeout,
                            transaction: transaction,
                            trace: trace);
                    }
                }
                catch (Exception e) when (errorCallback != null && !(e is OperationCanceledException))
                {
                    var error = CreateError(e, plan.Steps[i], i);
                    errors.Add(error);
                    errorCallback(error);
                }

                // Report
                Report(createdCallback, plan.Completions[i], errors, connection, destinationConnection, tableExistenceBehavior, schemaComposer, startTime);
            }

            // Report
            Report(createdCallback, plan.Completions[-1], errors, connection, destinationConnection, tableExistenceBehavior, schemaComposer, startTime);
        }

        /// <summary>
        /// Copies the schema of the tables into the destination connection: the work of the overloads of the multiple tables, on the columns that are already validated.
        /// </summary>
        /// <param name="selectedColumns">The distinct names of the columns to be created (<c>null</c> if all the columns are created).</param>
        /// <param name="tableMappings">The columns to be created table by table (<c>null</c> if the columns are the same for every requested table).</param>
        /// <remarks>See the overloads of <c>CopyTableSchemaTo</c> for the description of the other arguments.</remarks>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        private static async Task CopyTableSchemasCoreAsync(IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            string targetSchema,
            IList<string> selectedColumns,
            IList<CopyTableSchemaTable> tableMappings,
            CopyTableSchemaExistsBehavior tableExistenceBehavior,
            CopyTableSchemaRelationshipBehavior relationshipBehavior,
            Action<CopyTableSchemaResult> createdCallback,
            Action<CopyTableSchemaError> errorCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            // Validate
            var names = Validate(connection, tableNames, destinationConnection);
            var requestedNames = names;
            if (names.Count == 0)
            {
                return;
            }

            // Variables
            var startTime = DateTime.UtcNow;
            var schemaReader = SchemaReaderMapper.Get(connection) ??
                throw new MissingMappingException($"There is no schema reader mapping found for '{connection.GetType().FullName}'. Make sure to register one via the '{nameof(SchemaReaderMapper)}'.");
            var schemaComposer = SchemaComposerMapper.Get(destinationConnection) ??
                throw new MissingMappingException($"There is no schema composer mapping found for '{destinationConnection.GetType().FullName}'. Make sure to register one via the '{nameof(SchemaComposerMapper)}'.");
            if (relationshipBehavior != CopyTableSchemaRelationshipBehavior.TableOnly)
            {
                names = (await schemaReader.GetRelatedTablesAsync(names, relationshipBehavior, cancellationToken).ConfigureAwait(false)).ToList();
            }
            var relationships = await schemaReader.GetDependencyOrderAsync(names, cancellationToken).ConfigureAwait(false);
            var schemas = relationships.Select(r => r.Schema).ToList();
            if (tableMappings != null)
            {
                schemas = CopyTableSchemaColumns.Apply(schemas, CopyTableSchemaColumns.Resolve(tableMappings, schemas)).ToList();
            }
            else if (selectedColumns != null)
            {
                var requestedSchemas = new List<TableSchema>();
                foreach (var name in requestedNames)
                {
                    requestedSchemas.Add(await schemaReader.GetTableSchemaAsync(name, cancellationToken).ConfigureAwait(false));
                }
                schemas = CopyTableSchemaColumns.Apply(schemas, requestedSchemas, selectedColumns).ToList();
            }
            var tables = CopyTableSchemaPlan.CreateTables(schemas, GetTargetSchema(targetSchema, destinationConnection), schemaComposer);
            await ProbeTablesAsync(tables, tableExistenceBehavior, schemaComposer, destinationConnection, commandTimeout, trace, transaction, cancellationToken).ConfigureAwait(false);
            var plan = CopyTableSchemaPlan.Create(tables, tableExistenceBehavior, schemaComposer);
            var errors = new List<CopyTableSchemaError>();

            // Report the tables that have nothing to execute
            Report(createdCallback, plan.Immediate, errors, connection, destinationConnection, tableExistenceBehavior, schemaComposer, startTime);

            // Iterate
            for (var i = 0; i < plan.Steps.Count; i++)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(plan.Steps[i].Statement))
                    {
                        await destinationConnection.ExecuteNonQueryAsync(plan.Steps[i].Statement,
                            traceKey: traceKey,
                            commandTimeout: commandTimeout,
                            transaction: transaction,
                            trace: trace,
                            cancellationToken: cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (Exception e) when (errorCallback != null && !(e is OperationCanceledException))
                {
                    var error = CreateError(e, plan.Steps[i], i);
                    errors.Add(error);
                    errorCallback(error);
                }

                // Report
                Report(createdCallback, plan.Completions[i], errors, connection, destinationConnection, tableExistenceBehavior, schemaComposer, startTime);
            }

            // Report
            Report(createdCallback, plan.Completions[-1], errors, connection, destinationConnection, tableExistenceBehavior, schemaComposer, startTime);
        }

        #endregion
    }
}
