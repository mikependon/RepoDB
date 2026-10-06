#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

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
    public static class CopySchemaToExtension
    {
        #region Public Methods

        #region CopySchemaTo (IDbConnection, Sync)

        /// <summary>
        /// Copies the schema of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The name of the schema of the destination database that the tables are created in (it must already exist). The default is <c>null</c>, which creates the tables in the default schema of the destination database (see <see cref="IDbSetting.DefaultSchema"/> of the setting of the destination connection), or in the schema of the source table if the setting has no default schema. The tables of all the schemas are created in the same schema, so two tables with the same name (i.e.: <c>dbo.Item</c> and <c>Sales.Item</c>) cannot be copied together.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database: <see cref="CopySchemaExistsBehavior.Skip"/> leaves it as is, <see cref="CopySchemaExistsBehavior.Align"/> adds its missing columns and indexes, <see cref="CopySchemaExistsBehavior.Throw"/> throws before anything is created and <see cref="CopySchemaExistsBehavior.Drop"/> drops it and creates it again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>. Whether the table exists is checked with the statements of the composer, so a composer that does not compose them (empty statement) is treated as if the table does not exist. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing table and its data.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) are copied together with it. The default is <see cref="CopySchemaRelationshipBehavior.TableOnly"/>. The result is still the one of the given table, use the multiple tables overload to receive the result of each related table.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopySchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopySchemaResult"/> that describes the schema copy operation.</returns>
        public static CopySchemaResult CopySchemaTo<TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            string targetSchema = null,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            CopySchemaRelationshipBehavior relationshipBehavior = CopySchemaRelationshipBehavior.TableOnly,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopySchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopySchemaTo(ClassMappedNameCache.Get<TEntity>(), destinationConnection, targetSchema, tableExistenceBehavior, relationshipBehavior, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the schema of the source table of the current connection into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The name of the schema of the destination database that the tables are created in (it must already exist). The default is <c>null</c>, which creates the tables in the default schema of the destination database (see <see cref="IDbSetting.DefaultSchema"/> of the setting of the destination connection), or in the schema of the source table if the setting has no default schema. The tables of all the schemas are created in the same schema, so two tables with the same name (i.e.: <c>dbo.Item</c> and <c>Sales.Item</c>) cannot be copied together.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database: <see cref="CopySchemaExistsBehavior.Skip"/> leaves it as is, <see cref="CopySchemaExistsBehavior.Align"/> adds its missing columns and indexes, <see cref="CopySchemaExistsBehavior.Throw"/> throws before anything is created and <see cref="CopySchemaExistsBehavior.Drop"/> drops it and creates it again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>. Whether the table exists is checked with the statements of the composer, so a composer that does not compose them (empty statement) is treated as if the table does not exist. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing table and its data.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) are copied together with it. The default is <see cref="CopySchemaRelationshipBehavior.TableOnly"/>. The result is still the one of the given table, use the multiple tables overload to receive the result of each related table.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopySchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopySchemaResult"/> that describes the schema copy operation.</returns>
        public static CopySchemaResult CopySchemaTo(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            string targetSchema = null,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            CopySchemaRelationshipBehavior relationshipBehavior = CopySchemaRelationshipBehavior.TableOnly,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopySchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            Validate(connection, tableName, destinationConnection);

            // Copy the table as a set of one table, the callback receives the result of each table that is copied
            var results = new List<CopySchemaResult>();
            connection.CopySchemaTo(new[] { tableName },
                destinationConnection,
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

        #region CopySchemaTo (IDbConnection, Sync, Multiple Tables)

        /// <summary>
        /// Copies the schema of the tables of the current connection into the same-named tables of the destination connection.
        /// The tables are created together: all the tables first, then all the indexes and then all the foreign keys, so the tables can
        /// reference each other (even in a circular way) as long as the referenced tables are part of the given tables or already exist
        /// in the destination database. The source tables are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableNames">The names of the tables whose schema is to be copied. The tables can be given in any order, and can reference each other (even in a circular way).</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The name of the schema of the destination database that the tables are created in (it must already exist). The default is <c>null</c>, which creates the tables in the default schema of the destination database (see <see cref="IDbSetting.DefaultSchema"/> of the setting of the destination connection), or in the schema of the source table if the setting has no default schema. The tables of all the schemas are created in the same schema, so two tables with the same name (i.e.: <c>dbo.Item</c> and <c>Sales.Item</c>) cannot be copied together.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database: <see cref="CopySchemaExistsBehavior.Skip"/> leaves it as is, <see cref="CopySchemaExistsBehavior.Align"/> adds its missing columns and indexes, <see cref="CopySchemaExistsBehavior.Throw"/> throws before anything is created and <see cref="CopySchemaExistsBehavior.Drop"/> drops it and creates it again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>. Whether the table exists is checked with the statements of the composer, so a composer that does not compose them (empty statement) is treated as if the table does not exist. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing table and its data.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the given tables (through the foreign keys) are copied together with them. The default is <see cref="CopySchemaRelationshipBehavior.TableOnly"/>. Only the foreign keys of the source database are read to find them.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopySchemaResult"/> of a table every time its schema is created in the destination database (the table, its indexes and its foreign keys), in the order that the schemas are completed. The errors that were raised for the table are in the <see cref="CopySchemaResult.Errors"/> of its result. The default is <c>null</c>.</param>
        /// <param name="errorCallback">The callback that receives a <see cref="CopySchemaError"/> every time a statement fails while the schemas are being created. The error is also added to the <see cref="CopySchemaResult.Errors"/> of the table that it belongs to, and the copy continues with the next statement; throw from the callback to stop the copy. Without a callback, the exception is thrown. The errors of reading the schemas are not reported to it. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopySchemasTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        public static void CopySchemaTo(this IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            string targetSchema = null,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            CopySchemaRelationshipBehavior relationshipBehavior = CopySchemaRelationshipBehavior.TableOnly,
            Action<CopySchemaResult> createdCallback = null,
            Action<CopySchemaError> errorCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopySchemasTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            // Validate
            var names = Validate(connection, tableNames, destinationConnection);
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
            if (relationshipBehavior != CopySchemaRelationshipBehavior.TableOnly)
            {
                names = schemaReader.GetRelatedTables(names, relationshipBehavior).ToList();
            }
            var schemas = schemaReader.GetDependencyOrder(names).Select(r => r.Schema).ToList();
            var tables = CopySchemaPlan.CreateTables(schemas, GetTargetSchema(targetSchema, destinationConnection), schemaComposer);
            ProbeTables(tables, tableExistenceBehavior, schemaComposer, destinationConnection, commandTimeout, trace, transaction);
            var plan = CopySchemaPlan.Create(tables, tableExistenceBehavior, schemaComposer);
            var errors = new List<CopySchemaError>();

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

        #endregion

        #region CopySchemaTo (IDbConnection, Async)

        /// <summary>
        /// Copies the schema of the table mapped to the <typeparamref name="TEntity"/> type of the current connection
        /// into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that is mapped to the table.</typeparam>
        /// <param name="connection">The source connection.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The name of the schema of the destination database that the tables are created in (it must already exist). The default is <c>null</c>, which creates the tables in the default schema of the destination database (see <see cref="IDbSetting.DefaultSchema"/> of the setting of the destination connection), or in the schema of the source table if the setting has no default schema. The tables of all the schemas are created in the same schema, so two tables with the same name (i.e.: <c>dbo.Item</c> and <c>Sales.Item</c>) cannot be copied together.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database: <see cref="CopySchemaExistsBehavior.Skip"/> leaves it as is, <see cref="CopySchemaExistsBehavior.Align"/> adds its missing columns and indexes, <see cref="CopySchemaExistsBehavior.Throw"/> throws before anything is created and <see cref="CopySchemaExistsBehavior.Drop"/> drops it and creates it again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>. Whether the table exists is checked with the statements of the composer, so a composer that does not compose them (empty statement) is treated as if the table does not exist. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing table and its data.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) are copied together with it. The default is <see cref="CopySchemaRelationshipBehavior.TableOnly"/>. The result is still the one of the given table, use the multiple tables overload to receive the result of each related table.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopySchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopySchemaResult"/> that describes the schema copy operation.</returns>
        public static Task<CopySchemaResult> CopySchemaToAsync<TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            string targetSchema = null,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            CopySchemaRelationshipBehavior relationshipBehavior = CopySchemaRelationshipBehavior.TableOnly,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopySchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopySchemaToAsync(ClassMappedNameCache.Get<TEntity>(), destinationConnection, targetSchema, tableExistenceBehavior, relationshipBehavior, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the schema of the source table of the current connection into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The name of the schema of the destination database that the tables are created in (it must already exist). The default is <c>null</c>, which creates the tables in the default schema of the destination database (see <see cref="IDbSetting.DefaultSchema"/> of the setting of the destination connection), or in the schema of the source table if the setting has no default schema. The tables of all the schemas are created in the same schema, so two tables with the same name (i.e.: <c>dbo.Item</c> and <c>Sales.Item</c>) cannot be copied together.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database: <see cref="CopySchemaExistsBehavior.Skip"/> leaves it as is, <see cref="CopySchemaExistsBehavior.Align"/> adds its missing columns and indexes, <see cref="CopySchemaExistsBehavior.Throw"/> throws before anything is created and <see cref="CopySchemaExistsBehavior.Drop"/> drops it and creates it again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>. Whether the table exists is checked with the statements of the composer, so a composer that does not compose them (empty statement) is treated as if the table does not exist. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing table and its data.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) are copied together with it. The default is <see cref="CopySchemaRelationshipBehavior.TableOnly"/>. The result is still the one of the given table, use the multiple tables overload to receive the result of each related table.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopySchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopySchemaResult"/> that describes the schema copy operation.</returns>
        public static async Task<CopySchemaResult> CopySchemaToAsync(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            string targetSchema = null,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            CopySchemaRelationshipBehavior relationshipBehavior = CopySchemaRelationshipBehavior.TableOnly,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopySchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            Validate(connection, tableName, destinationConnection);

            // Copy the table as a set of one table, the callback receives the result of each table that is copied
            var results = new List<CopySchemaResult>();
            await connection.CopySchemaToAsync(new[] { tableName },
                destinationConnection,
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

        #region CopySchemaTo (IDbConnection, Async, Multiple Tables)

        /// <summary>
        /// Copies the schema of the tables of the current connection into the same-named tables of the destination connection.
        /// The tables are created together: all the tables first, then all the indexes and then all the foreign keys, so the tables can
        /// reference each other (even in a circular way) as long as the referenced tables are part of the given tables or already exist
        /// in the destination database. The source tables are left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableNames">The names of the tables whose schema is to be copied. The tables can be given in any order, and can reference each other (even in a circular way).</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The name of the schema of the destination database that the tables are created in (it must already exist). The default is <c>null</c>, which creates the tables in the default schema of the destination database (see <see cref="IDbSetting.DefaultSchema"/> of the setting of the destination connection), or in the schema of the source table if the setting has no default schema. The tables of all the schemas are created in the same schema, so two tables with the same name (i.e.: <c>dbo.Item</c> and <c>Sales.Item</c>) cannot be copied together.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database: <see cref="CopySchemaExistsBehavior.Skip"/> leaves it as is, <see cref="CopySchemaExistsBehavior.Align"/> adds its missing columns and indexes, <see cref="CopySchemaExistsBehavior.Throw"/> throws before anything is created and <see cref="CopySchemaExistsBehavior.Drop"/> drops it and creates it again. The default is <see cref="CopySchemaExistsBehavior.Skip"/>. Whether the table exists is checked with the statements of the composer, so a composer that does not compose them (empty statement) is treated as if the table does not exist. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.Drop"/> permanently deletes the existing table and its data.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the given tables (through the foreign keys) are copied together with them. The default is <see cref="CopySchemaRelationshipBehavior.TableOnly"/>. Only the foreign keys of the source database are read to find them.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopySchemaResult"/> of a table every time its schema is created in the destination database (the table, its indexes and its foreign keys), in the order that the schemas are completed. The errors that were raised for the table are in the <see cref="CopySchemaResult.Errors"/> of its result. The default is <c>null</c>.</param>
        /// <param name="errorCallback">The callback that receives a <see cref="CopySchemaError"/> every time a statement fails while the schemas are being created. The error is also added to the <see cref="CopySchemaResult.Errors"/> of the table that it belongs to, and the copy continues with the next statement; throw from the callback to stop the copy. Without a callback, the exception is thrown. The errors of reading the schemas are not reported to it. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopySchemasTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task CopySchemaToAsync(this IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            string targetSchema = null,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.Skip,
            CopySchemaRelationshipBehavior relationshipBehavior = CopySchemaRelationshipBehavior.TableOnly,
            Action<CopySchemaResult> createdCallback = null,
            Action<CopySchemaError> errorCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopySchemasTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Validate
            var names = Validate(connection, tableNames, destinationConnection);
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
            if (relationshipBehavior != CopySchemaRelationshipBehavior.TableOnly)
            {
                names = (await schemaReader.GetRelatedTablesAsync(names, relationshipBehavior, cancellationToken).ConfigureAwait(false)).ToList();
            }
            var relationships = await schemaReader.GetDependencyOrderAsync(names, cancellationToken).ConfigureAwait(false);
            var schemas = relationships.Select(r => r.Schema).ToList();
            var tables = CopySchemaPlan.CreateTables(schemas, GetTargetSchema(targetSchema, destinationConnection), schemaComposer);
            await ProbeTablesAsync(tables, tableExistenceBehavior, schemaComposer, destinationConnection, commandTimeout, trace, transaction, cancellationToken).ConfigureAwait(false);
            var plan = CopySchemaPlan.Create(tables, tableExistenceBehavior, schemaComposer);
            var errors = new List<CopySchemaError>();

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

        #endregion
    }
}
