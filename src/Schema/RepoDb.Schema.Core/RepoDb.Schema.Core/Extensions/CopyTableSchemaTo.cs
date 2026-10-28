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
using static RepoDb.Schema.DbConnectionExtension;

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
        /// <param name="targetTable">The name of the table that is created in the destination database (<c>null</c> if the table keeps its name). The foreign keys that reference the table follow the new name.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="columns">The names of the columns of the source table to be copied. The default is <c>null</c>, which copies all the columns.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="TableExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table. The default is <see cref="RelatedTableBehavior.TableOnly"/>.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResultItem"/> of each table once its schema is created (the requested table and, depending on the <paramref name="relationshipBehavior"/>, its related tables), so a caller can report the progress while the copy is still running. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyTableSchemaResult"/> that describes the schema copy operation.</returns>
        public static CopyTableSchemaResult CopyTableSchemaTo<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            string targetTable = null,
            string targetSchema = null,
            string[] columns = null,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            RelatedTableBehavior relationshipBehavior = RelatedTableBehavior.TableOnly,
            Action<CopyTableSchemaResultItem> createdCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopyTableSchemaTo(ClassMappedNameCache.Get<TEntity>(), destinationConnection, targetTable, targetSchema, columns, tableExistenceBehavior, relationshipBehavior, createdCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the schema of the source table of the current connection into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetTable">The name of the table that is created in the destination database (<c>null</c> if the table keeps its name). The foreign keys that reference the table follow the new name.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="columns">The names of the columns of the source table to be copied. The default is <c>null</c>, which copies all the columns.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="TableExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table. The default is <see cref="RelatedTableBehavior.TableOnly"/>.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResultItem"/> of each table once its schema is created (the requested table and, depending on the <paramref name="relationshipBehavior"/>, its related tables), so a caller can report the progress while the copy is still running. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyTableSchemaResult"/> that describes the schema copy operation.</returns>
        public static CopyTableSchemaResult CopyTableSchemaTo(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            string targetTable = null,
            string targetSchema = null,
            string[] columns = null,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            RelatedTableBehavior relationshipBehavior = RelatedTableBehavior.TableOnly,
            Action<CopyTableSchemaResultItem> createdCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            Validate(connection, tableName, destinationConnection);

            // Copy the table as a set of one table
            return CopyTableSchemasCore(connection, new[] { tableName }, destinationConnection, targetSchema, ColumnSelector.Normalize(columns), null, targetTable, tableExistenceBehavior, relationshipBehavior, createdCallback, null, commandTimeout, traceKey, trace, transaction);
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
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database. The default is <see cref="TableExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the given tables. The default is <see cref="RelatedTableBehavior.TableOnly"/>.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResultItem"/> of each table once its schema is created. The default is <c>null</c>.</param>
        /// <param name="errorCallback">The callback that receives a <see cref="ExecutionError"/> when a statement fails, so the copy continues with the next statement (without it, the exception is thrown).</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemasTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyTableSchemaResult"/> that describes the schema copy operation: the time that it took, the tables that were copied (see <see cref="CopyTableSchemaResult.Items"/>), and the errors and warnings.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static CopyTableSchemaResult CopyTableSchemaTo(this IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            string targetSchema = null,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            RelatedTableBehavior relationshipBehavior = RelatedTableBehavior.TableOnly,
            Action<CopyTableSchemaResultItem> createdCallback = null,
            Action<ExecutionError> errorCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemasTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            return CopyTableSchemasCore(connection, tableNames, destinationConnection, targetSchema, null, null, null, tableExistenceBehavior, relationshipBehavior, createdCallback, errorCallback, commandTimeout, traceKey, trace, transaction);
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
        /// <param name="targetTable">The name of the table that is created in the destination database (<c>null</c> if the table keeps its name). The foreign keys that reference the table follow the new name.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="columns">The names of the columns of the source table to be copied. The default is <c>null</c>, which copies all the columns.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="TableExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table. The default is <see cref="RelatedTableBehavior.TableOnly"/>.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResultItem"/> of each table once its schema is created (the requested table and, depending on the <paramref name="relationshipBehavior"/>, its related tables), so a caller can report the progress while the copy is still running. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyTableSchemaResult"/> that describes the schema copy operation.</returns>
        public static Task<CopyTableSchemaResult> CopyTableSchemaToAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            string targetTable = null,
            string targetSchema = null,
            string[] columns = null,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            RelatedTableBehavior relationshipBehavior = RelatedTableBehavior.TableOnly,
            Action<CopyTableSchemaResultItem> createdCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopyTableSchemaToAsync(ClassMappedNameCache.Get<TEntity>(), destinationConnection, targetTable, targetSchema, columns, tableExistenceBehavior, relationshipBehavior, createdCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the schema of the source table of the current connection into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetTable">The name of the table that is created in the destination database (<c>null</c> if the table keeps its name). The foreign keys that reference the table follow the new name.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="columns">The names of the columns of the source table to be copied. The default is <c>null</c>, which copies all the columns.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="TableExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table. The default is <see cref="RelatedTableBehavior.TableOnly"/>.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResultItem"/> of each table once its schema is created (the requested table and, depending on the <paramref name="relationshipBehavior"/>, its related tables), so a caller can report the progress while the copy is still running. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyTableSchemaResult"/> that describes the schema copy operation.</returns>
        public static async Task<CopyTableSchemaResult> CopyTableSchemaToAsync(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            string targetTable = null,
            string targetSchema = null,
            string[] columns = null,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            RelatedTableBehavior relationshipBehavior = RelatedTableBehavior.TableOnly,
            Action<CopyTableSchemaResultItem> createdCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            Validate(connection, tableName, destinationConnection);
            return await CopyTableSchemasCoreAsync(connection, new[] { tableName }, destinationConnection, targetSchema, ColumnSelector.Normalize(columns), null, targetTable, tableExistenceBehavior, relationshipBehavior, createdCallback, null, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false);
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
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database. The default is <see cref="TableExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the given tables. The default is <see cref="RelatedTableBehavior.TableOnly"/>.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResultItem"/> of each table once its schema is created. The default is <c>null</c>.</param>
        /// <param name="errorCallback">The callback that receives a <see cref="ExecutionError"/> when a statement fails, so the copy continues with the next statement (without it, the exception is thrown).</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemasTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyTableSchemaResult"/> that describes the schema copy operation: the time that it took, the tables that were copied (see <see cref="CopyTableSchemaResult.Items"/>), and the errors and warnings.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task<CopyTableSchemaResult> CopyTableSchemaToAsync(this IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            string targetSchema = null,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            RelatedTableBehavior relationshipBehavior = RelatedTableBehavior.TableOnly,
            Action<CopyTableSchemaResultItem> createdCallback = null,
            Action<ExecutionError> errorCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemasTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return await CopyTableSchemasCoreAsync(connection, tableNames, destinationConnection, targetSchema, null, null, null, tableExistenceBehavior, relationshipBehavior, createdCallback, errorCallback, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false);
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
        /// <param name="mappings">The columns of the source table to be copied, as <see cref="ColumnMapping"/> (the source column and its name in the destination table).</param>
        /// <param name="targetTable">The name of the table that is created in the destination database (<c>null</c> if the table keeps its name). The foreign keys that reference the table follow the new name.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="TableExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table. The default is <see cref="RelatedTableBehavior.TableOnly"/>.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResultItem"/> of each table once its schema is created (the requested table and, depending on the <paramref name="relationshipBehavior"/>, its related tables), so a caller can report the progress while the copy is still running. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyTableSchemaResult"/> that describes the schema copy operation.</returns>
        public static CopyTableSchemaResult CopyTableSchemaTo<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            IList<ColumnMapping> mappings,
            string targetTable = null,
            string targetSchema = null,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            RelatedTableBehavior relationshipBehavior = RelatedTableBehavior.TableOnly,
            Action<CopyTableSchemaResultItem> createdCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopyTableSchemaTo(ClassMappedNameCache.Get<TEntity>(), destinationConnection, mappings, targetTable, targetSchema, tableExistenceBehavior, relationshipBehavior, createdCallback, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the schema of the source table of the current connection into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="mappings">The columns of the source table to be copied, as <see cref="ColumnMapping"/> (the source column and its name in the destination table).</param>
        /// <param name="targetTable">The name of the table that is created in the destination database (<c>null</c> if the table keeps its name). The foreign keys that reference the table follow the new name.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="TableExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table. The default is <see cref="RelatedTableBehavior.TableOnly"/>.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResultItem"/> of each table once its schema is created (the requested table and, depending on the <paramref name="relationshipBehavior"/>, its related tables), so a caller can report the progress while the copy is still running. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyTableSchemaResult"/> that describes the schema copy operation.</returns>
        public static CopyTableSchemaResult CopyTableSchemaTo(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            IList<ColumnMapping> mappings,
            string targetTable = null,
            string targetSchema = null,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            RelatedTableBehavior relationshipBehavior = RelatedTableBehavior.TableOnly,
            Action<CopyTableSchemaResultItem> createdCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            Validate(connection, tableName, destinationConnection);
            ColumnSelector.Normalize(mappings, out _, out _);

            // Copy the table as a set of one table
            return connection.CopyTableSchemaTo(new[] { tableName },
                destinationConnection,
                new[] { new TableMapping(tableName, targetTable, mappings) },
                targetSchema,
                tableExistenceBehavior,
                relationshipBehavior,
                createdCallback: createdCallback,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                trace: trace,
                transaction: transaction);
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
        /// <param name="mappings">The tables to be copied with only their mapped columns, as <see cref="TableMapping"/> (the tables that are not listed keep all their columns).</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database. The default is <see cref="TableExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the given tables. The default is <see cref="RelatedTableBehavior.TableOnly"/>.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResultItem"/> of each table once its schema is created. The default is <c>null</c>.</param>
        /// <param name="errorCallback">The callback that receives a <see cref="ExecutionError"/> when a statement fails, so the copy continues with the next statement (without it, the exception is thrown).</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemasTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopyTableSchemaResult"/> that describes the schema copy operation: the time that it took, the tables that were copied (see <see cref="CopyTableSchemaResult.Items"/>), and the errors and warnings.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static CopyTableSchemaResult CopyTableSchemaTo(this IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            IList<TableMapping> mappings,
            string targetSchema = null,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            RelatedTableBehavior relationshipBehavior = RelatedTableBehavior.TableOnly,
            Action<CopyTableSchemaResultItem> createdCallback = null,
            Action<ExecutionError> errorCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemasTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            ColumnSelector.Normalize(mappings);
            return CopyTableSchemasCore(connection, tableNames, destinationConnection, targetSchema, null, mappings, null, tableExistenceBehavior, relationshipBehavior, createdCallback, errorCallback, commandTimeout, traceKey, trace, transaction);
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
        /// <param name="mappings">The columns of the source table to be copied, as <see cref="ColumnMapping"/> (the source column and its name in the destination table).</param>
        /// <param name="targetTable">The name of the table that is created in the destination database (<c>null</c> if the table keeps its name). The foreign keys that reference the table follow the new name.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="TableExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table. The default is <see cref="RelatedTableBehavior.TableOnly"/>.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResultItem"/> of each table once its schema is created (the requested table and, depending on the <paramref name="relationshipBehavior"/>, its related tables), so a caller can report the progress while the copy is still running. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyTableSchemaResult"/> that describes the schema copy operation.</returns>
        public static Task<CopyTableSchemaResult> CopyTableSchemaToAsync<[DynamicallyAccessedMembers(Trimming.Entity)] TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            IList<ColumnMapping> mappings,
            string targetTable = null,
            string targetSchema = null,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            RelatedTableBehavior relationshipBehavior = RelatedTableBehavior.TableOnly,
            Action<CopyTableSchemaResultItem> createdCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopyTableSchemaToAsync(ClassMappedNameCache.Get<TEntity>(), destinationConnection, mappings, targetTable, targetSchema, tableExistenceBehavior, relationshipBehavior, createdCallback, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the schema of the source table of the current connection into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="mappings">The columns of the source table to be copied, as <see cref="ColumnMapping"/> (the source column and its name in the destination table).</param>
        /// <param name="targetTable">The name of the table that is created in the destination database (<c>null</c> if the table keeps its name). The foreign keys that reference the table follow the new name.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="TableExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the table. The default is <see cref="RelatedTableBehavior.TableOnly"/>.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResultItem"/> of each table once its schema is created (the requested table and, depending on the <paramref name="relationshipBehavior"/>, its related tables), so a caller can report the progress while the copy is still running. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyTableSchemaResult"/> that describes the schema copy operation.</returns>
        public static async Task<CopyTableSchemaResult> CopyTableSchemaToAsync(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            IList<ColumnMapping> mappings,
            string targetTable = null,
            string targetSchema = null,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            RelatedTableBehavior relationshipBehavior = RelatedTableBehavior.TableOnly,
            Action<CopyTableSchemaResultItem> createdCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            Validate(connection, tableName, destinationConnection);
            ColumnSelector.Normalize(mappings, out _, out _);
            return await connection.CopyTableSchemaToAsync(new[] { tableName },
                destinationConnection,
                new[] { new TableMapping(tableName, targetTable, mappings) },
                targetSchema,
                tableExistenceBehavior,
                relationshipBehavior,
                createdCallback: createdCallback,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                trace: trace,
                transaction: transaction,
                cancellationToken: cancellationToken).ConfigureAwait(false);
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
        /// <param name="mappings">The tables to be copied with only their mapped columns, as <see cref="TableMapping"/> (the tables that are not listed keep all their columns).</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in. The default is <c>null</c>, which uses the default schema of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database. The default is <see cref="TableExistsBehavior.Skip"/>.</param>
        /// <param name="relationshipBehavior">Defines which related tables (through the foreign keys) are copied together with the given tables. The default is <see cref="RelatedTableBehavior.TableOnly"/>.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResultItem"/> of each table once its schema is created. The default is <c>null</c>.</param>
        /// <param name="errorCallback">The callback that receives a <see cref="ExecutionError"/> when a statement fails, so the copy continues with the next statement (without it, the exception is thrown).</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemasTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopyTableSchemaResult"/> that describes the schema copy operation: the time that it took, the tables that were copied (see <see cref="CopyTableSchemaResult.Items"/>), and the errors and warnings.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task<CopyTableSchemaResult> CopyTableSchemaToAsync(this IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            IList<TableMapping> mappings,
            string targetSchema = null,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            RelatedTableBehavior relationshipBehavior = RelatedTableBehavior.TableOnly,
            Action<CopyTableSchemaResultItem> createdCallback = null,
            Action<ExecutionError> errorCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemasTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            ColumnSelector.Normalize(mappings);
            return await CopyTableSchemasCoreAsync(connection, tableNames, destinationConnection, targetSchema, null, mappings, null, tableExistenceBehavior, relationshipBehavior, createdCallback, errorCallback, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Copies the schema of the tables into the destination connection: the work of the overloads of the multiple tables, on the columns that are already validated.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableNames">The names of the tables to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in.</param>
        /// <param name="selectedColumns">The distinct names of the columns to be created (<c>null</c> if all the columns are created).</param>
        /// <param name="tableMappings">The columns to be created table by table (<c>null</c> if the columns are the same for every requested table).</param>
        /// <param name="targetTable">The name of the table in the destination database (<c>null</c> if the name of the source table is used). It is only allowed for a single table.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database.</param>
        /// <param name="relationshipBehavior">Defines which related tables are copied together with the given tables.</param>
        /// <param name="createdCallback">The callback that receives the item of each table once its schema is created.</param>
        /// <param name="errorCallback">The callback that receives an error when a statement fails.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <returns>The result of the schema copy operation.</returns>
        /// <remarks>See the overloads of <c>CopyTableSchemaTo</c> for the description of the other arguments.</remarks>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        private static CopyTableSchemaResult CopyTableSchemasCore(IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            string targetSchema,
            IList<string> selectedColumns,
            IList<TableMapping> tableMappings,
            string targetTable,
            TableExistsBehavior tableExistenceBehavior,
            RelatedTableBehavior relationshipBehavior,
            Action<CopyTableSchemaResultItem> createdCallback,
            Action<ExecutionError> errorCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction)
        {
            // Validate
            var names = Validate(connection, tableNames, destinationConnection);
            if (targetTable != null && (string.IsNullOrWhiteSpace(ColumnSelector.Unquote(targetTable)) || names.Count != 1))
            {
                throw new ArgumentException("The name of the table in the destination database can only be given for a single table, and it can not be white space.", "targetTable");
            }
            var requestedNames = names;
            var startTime = DateTime.UtcNow;
            if (names.Count == 0)
            {
                return CreateResult(startTime, new List<CopyTableSchemaResultItem>(), new List<ExecutionError>());
            }

            // Variables
            var schemaReader = SchemaReaderMapper.Get(connection) ??
                throw new MissingMappingException($"There is no schema reader mapping found for '{connection.GetType().FullName}'. Make sure to register one via the '{nameof(SchemaReaderMapper)}'.");
            var schemaComposer = SchemaComposerMapper.Get(destinationConnection) ??
                throw new MissingMappingException($"There is no schema composer mapping found for '{destinationConnection.GetType().FullName}'. Make sure to register one via the '{nameof(SchemaComposerMapper)}'.");
            if (relationshipBehavior != RelatedTableBehavior.TableOnly)
            {
                names = schemaReader.GetRelatedTables(names, relationshipBehavior).ToList();
            }
            var schemas = schemaReader.GetDependencyOrder(names).Select(r => r.Schema).ToList();
            var originals = schemas;
            var renames = new Dictionary<TableInfo, string>();
            if (tableMappings != null)
            {
                var selections = ColumnSelector.Resolve(tableMappings, schemas);
                foreach (var pair in selections.Where(item => item.Value.TargetName != null))
                {
                    renames[pair.Key] = pair.Value.TargetName;
                }
                schemas = ColumnSelector.Apply(schemas, selections).ToList();
            }
            else if (selectedColumns != null)
            {
                var requestedSchemas = requestedNames.Select(schemaReader.GetTableSchema).ToList();
                schemas = ColumnSelector.Apply(schemas, requestedSchemas, selectedColumns).ToList();
            }
            if (targetTable != null)
            {
                renames[schemaReader.GetTableSchema(requestedNames[0]).Table] = ColumnSelector.Unquote(targetTable);
            }
            schemas = ColumnSelector.RenameTables(schemas, renames).ToList();
            var tables = Plan.CreateTables(schemas, GetTargetSchema(targetSchema, destinationConnection), schemaComposer);
            for (var i = 0; i < tables.Count; i++)
            {
                tables[i].Original = originals[i];
            }
            return SchemaPlanExecutor.Execute(startTime, tables, schemaComposer, connection, destinationConnection,
                tableExistenceBehavior, createdCallback, errorCallback, commandTimeout, traceKey, trace, transaction);
        }

        /// <summary>
        /// Copies the schema of the tables into the destination connection: the work of the overloads of the multiple tables, on the columns that are already validated.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableNames">The names of the tables to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The schema of the destination database that the tables are created in.</param>
        /// <param name="selectedColumns">The distinct names of the columns to be created (<c>null</c> if all the columns are created).</param>
        /// <param name="tableMappings">The columns to be created table by table (<c>null</c> if the columns are the same for every requested table).</param>
        /// <param name="targetTable">The name of the table in the destination database (<c>null</c> if the name of the source table is used). It is only allowed for a single table.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database.</param>
        /// <param name="relationshipBehavior">Defines which related tables are copied together with the given tables.</param>
        /// <param name="createdCallback">The callback that receives the item of each table once its schema is created.</param>
        /// <param name="errorCallback">The callback that receives an error when a statement fails.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used on the destination connection.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the result of the schema copy operation.</returns>
        /// <remarks>See the overloads of <c>CopyTableSchemaTo</c> for the description of the other arguments.</remarks>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        private static async Task<CopyTableSchemaResult> CopyTableSchemasCoreAsync(IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            string targetSchema,
            IList<string> selectedColumns,
            IList<TableMapping> tableMappings,
            string targetTable,
            TableExistsBehavior tableExistenceBehavior,
            RelatedTableBehavior relationshipBehavior,
            Action<CopyTableSchemaResultItem> createdCallback,
            Action<ExecutionError> errorCallback,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            // Validate
            var names = Validate(connection, tableNames, destinationConnection);
            if (targetTable != null && (string.IsNullOrWhiteSpace(ColumnSelector.Unquote(targetTable)) || names.Count != 1))
            {
                throw new ArgumentException("The name of the table in the destination database can only be given for a single table, and it can not be white space.", "targetTable");
            }
            var requestedNames = names;
            var startTime = DateTime.UtcNow;
            if (names.Count == 0)
            {
                return CreateResult(startTime, new List<CopyTableSchemaResultItem>(), new List<ExecutionError>());
            }

            // Variables
            var schemaReader = SchemaReaderMapper.Get(connection) ??
                throw new MissingMappingException($"There is no schema reader mapping found for '{connection.GetType().FullName}'. Make sure to register one via the '{nameof(SchemaReaderMapper)}'.");
            var schemaComposer = SchemaComposerMapper.Get(destinationConnection) ??
                throw new MissingMappingException($"There is no schema composer mapping found for '{destinationConnection.GetType().FullName}'. Make sure to register one via the '{nameof(SchemaComposerMapper)}'.");
            if (relationshipBehavior != RelatedTableBehavior.TableOnly)
            {
                names = (await schemaReader.GetRelatedTablesAsync(names, relationshipBehavior, cancellationToken).ConfigureAwait(false)).ToList();
            }
            var relationships = await schemaReader.GetDependencyOrderAsync(names, cancellationToken).ConfigureAwait(false);
            var schemas = relationships.Select(r => r.Schema).ToList();
            var originals = schemas;
            var renames = new Dictionary<TableInfo, string>();
            if (tableMappings != null)
            {
                var selections = ColumnSelector.Resolve(tableMappings, schemas);
                foreach (var pair in selections.Where(item => item.Value.TargetName != null))
                {
                    renames[pair.Key] = pair.Value.TargetName;
                }
                schemas = ColumnSelector.Apply(schemas, selections).ToList();
            }
            else if (selectedColumns != null)
            {
                var requestedSchemas = new List<TableSchema>();
                foreach (var name in requestedNames)
                {
                    requestedSchemas.Add(await schemaReader.GetTableSchemaAsync(name, cancellationToken).ConfigureAwait(false));
                }
                schemas = ColumnSelector.Apply(schemas, requestedSchemas, selectedColumns).ToList();
            }
            if (targetTable != null)
            {
                renames[schemaReader.GetTableSchema(requestedNames[0]).Table] = ColumnSelector.Unquote(targetTable);
            }
            schemas = ColumnSelector.RenameTables(schemas, renames).ToList();
            var tables = Plan.CreateTables(schemas, GetTargetSchema(targetSchema, destinationConnection), schemaComposer);
            for (var i = 0; i < tables.Count; i++)
            {
                tables[i].Original = originals[i];
            }
            return await SchemaPlanExecutor.ExecuteAsync(startTime, tables, schemaComposer, connection, destinationConnection,
                tableExistenceBehavior, createdCallback, errorCallback, commandTimeout, traceKey, trace, transaction, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
