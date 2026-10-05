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
                    destinationConnection.ExecuteNonQuery(plan.Steps[i].Statement,
                        traceKey: traceKey,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace);
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
                    await destinationConnection.ExecuteNonQueryAsync(plan.Steps[i].Statement,
                        traceKey: traceKey,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        cancellationToken: cancellationToken).ConfigureAwait(false);
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

        #region Helpers

        /// <summary>
        /// Finds the result of the table that was requested among the results of the tables that were copied (the requested table and its related tables).
        /// </summary>
        /// <param name="results">The results of the copied tables.</param>
        /// <param name="table">The identity of the table that was requested.</param>
        /// <returns>The result of the requested table.</returns>
        private static CopySchemaResult FindResult(IList<CopySchemaResult> results,
            TableInfo table) =>
            results.FirstOrDefault(r => string.Equals(r.TableName, table.Name, StringComparison.Ordinal) &&
                string.Equals(r.SourceSchema, table.Schema, StringComparison.Ordinal));

        /// <summary>
        /// Gets the schema of the destination database that the tables are created in: the given one, or the default schema of the setting of the destination connection (see <see cref="IDbSetting.DefaultSchema"/>).
        /// </summary>
        /// <param name="targetSchema"></param>
        /// <param name="destinationConnection"></param>
        /// <returns>The schema (<c>null</c> if it is not given, and the setting has no default schema).</returns>
        private static string GetTargetSchema(string targetSchema,
            IDbConnection destinationConnection)
        {
            var schema = string.IsNullOrWhiteSpace(targetSchema)
                ? destinationConnection.GetDbSetting().DefaultSchema
                : targetSchema;
            return string.IsNullOrWhiteSpace(schema) ? null : schema;
        }

        /// <summary>
        /// Reads the state of the tables in the destination database: whether each one exists and, when the existing tables are aligned,
        /// the columns and the indexes that they are missing. A statement that the composer does not compose leaves the state unknown.
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="behavior"></param>
        /// <param name="composer"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        private static void ProbeTables(IEnumerable<CopySchemaTable> tables,
            CopySchemaExistsBehavior behavior,
            ISchemaComposer composer,
            IDbConnection destinationConnection,
            int? commandTimeout,
            ITrace trace,
            IDbTransaction transaction)
        {
            bool? Probe(string statement) =>
                string.IsNullOrWhiteSpace(statement)
                    ? (bool?)null
                    : ToBoolean(destinationConnection.ExecuteScalar(statement, commandTimeout: commandTimeout, transaction: transaction, trace: trace));

            foreach (var table in tables)
            {
                table.Exists = Probe(composer.ComposeTableExists(table.Name));
                if (table.Exists == true && behavior == CopySchemaExistsBehavior.Align)
                {
                    table.MissingColumns = table.Schema.Columns.Where(column => Probe(composer.ComposeColumnExists(table.Name, column.Field.Name)) == false).ToList();
                    table.MissingIndexes = table.Schema.Indexes.Where(index => index.Name != null && Probe(composer.ComposeIndexExists(table.Name, index.Name)) == false).ToList();
                }
            }
        }

        /// <summary>
        /// Reads the state of the tables in the destination database (see <see cref="ProbeTables"/>).
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="behavior"></param>
        /// <param name="composer"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="trace"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        private static async Task ProbeTablesAsync(IEnumerable<CopySchemaTable> tables,
            CopySchemaExistsBehavior behavior,
            ISchemaComposer composer,
            IDbConnection destinationConnection,
            int? commandTimeout,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            async Task<bool?> Probe(string statement) =>
                string.IsNullOrWhiteSpace(statement)
                    ? (bool?)null
                    : ToBoolean(await destinationConnection.ExecuteScalarAsync(statement, commandTimeout: commandTimeout, transaction: transaction, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false));

            foreach (var table in tables)
            {
                table.Exists = await Probe(composer.ComposeTableExists(table.Name)).ConfigureAwait(false);
                if (table.Exists == true && behavior == CopySchemaExistsBehavior.Align)
                {
                    table.MissingColumns = new List<ColumnInfo>();
                    foreach (var column in table.Schema.Columns)
                    {
                        if (await Probe(composer.ComposeColumnExists(table.Name, column.Field.Name)).ConfigureAwait(false) == false)
                        {
                            table.MissingColumns.Add(column);
                        }
                    }
                    table.MissingIndexes = new List<IndexInfo>();
                    foreach (var index in table.Schema.Indexes.Where(index => index.Name != null))
                    {
                        if (await Probe(composer.ComposeIndexExists(table.Name, index.Name)).ConfigureAwait(false) == false)
                        {
                            table.MissingIndexes.Add(index);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converts the result of an existence statement (<c>1</c> if it exists).
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool ToBoolean(object result) =>
            result != null && result != DBNull.Value && Convert.ToInt32(result) == 1;

        /// <summary>
        /// Creates the error of a statement that has failed.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="step"></param>
        /// <param name="statementIndex"></param>
        /// <returns></returns>
        private static CopySchemaError CreateError(Exception exception,
            CopySchemaStep step,
            int statementIndex) =>
            new CopySchemaError
            {
                Exception = exception,
                Statement = step.Statement,
                StatementIndex = statementIndex,
                TableName = step.Owner?.Schema.Table?.Name,
                SchemaName = step.Owner?.Schema.Table?.Schema
            };

        /// <summary>
        /// Gets the script that was (or is going to be) executed for the table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="schemaComposer"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetScript(CopySchemaTable table,
            ISchemaComposer schemaComposer)
        {
            switch (table.Outcome)
            {
                case CopySchemaOutcome.Skipped:
                    return Enumerable.Empty<string>();
                case CopySchemaOutcome.Aligned:
                    return table.AlignStatements;
                default:
                    var create = schemaComposer.ComposeSchema(table.Schema) ?? Enumerable.Empty<string>();
                    return table.DropStatement == null ? create : new[] { table.DropStatement }.Concat(create);
            }
        }

        /// <summary>
        /// Calls the callback with the result of each table that is completed.
        /// </summary>
        /// <param name="createdCallback"></param>
        /// <param name="tables"></param>
        /// <param name="errors"></param>
        /// <param name="connection"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="tableExistenceBehavior"></param>
        /// <param name="schemaComposer"></param>
        /// <param name="startTime"></param>
        private static void Report(Action<CopySchemaResult> createdCallback,
            IEnumerable<CopySchemaTable> tables,
            IList<CopySchemaError> errors,
            IDbConnection connection,
            IDbConnection destinationConnection,
            CopySchemaExistsBehavior tableExistenceBehavior,
            ISchemaComposer schemaComposer,
            DateTime startTime)
        {
            if (createdCallback == null)
            {
                return;
            }

            foreach (var table in tables)
            {
                var schema = table.Schema;
                var aligned = table.Outcome == CopySchemaOutcome.Aligned;
                var schemaErrors = errors
                    .Where(error => error.TableName == null ||
                        (string.Equals(error.TableName, schema.Table.Name, StringComparison.Ordinal) &&
                        string.Equals(error.SchemaName, schema.Table.Schema, StringComparison.Ordinal)))
                    .ToList();

                createdCallback(new CopySchemaResult
                {
                    Action = tableExistenceBehavior,
                    Outcome = schemaErrors.Count > 0 ? CopySchemaOutcome.Failed : table.Outcome,
                    TableExisted = table.Exists,
                    AddedColumns = aligned ? table.MissingColumns.Select(column => column.Field.Name).ToList() : new List<string>(),
                    AddedIndexes = aligned ? table.MissingIndexes.Select(index => index.Name).ToList() : new List<string>(),
                    Errors = schemaErrors,
                    TableName = schema.Table.Name,
                    SourceSchema = table.Source.Table.Schema,
                    DestinationSchema = schema.Table.Schema,
                    SourceDatabase = connection.Database,
                    SourceServer = (connection as DbConnection)?.DataSource,
                    SourceDatabaseType = connection.GetType().Name,
                    DestinationDatabase = destinationConnection.Database,
                    DestinationServer = (destinationConnection as DbConnection)?.DataSource,
                    DestinationDatabaseType = destinationConnection.GetType().Name,
                    StartTime = startTime,
                    EndTime = DateTime.UtcNow,
                    Script = string.Join(Environment.NewLine, GetScript(table, schemaComposer)),
                    ColumnCount = schema.Columns.Count,
                    IndexCount = schema.Indexes.Count,
                    ForeignKeyCount = schema.ForeignKeys.Count,
                    UniqueConstraintCount = schema.UniqueConstraints.Count,
                    CheckConstraintCount = schema.CheckConstraints.Count
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="destinationConnection"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private static void Validate(IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }
            if (destinationConnection == null)
            {
                throw new ArgumentNullException(nameof(destinationConnection));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableNames"></param>
        /// <param name="destinationConnection"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private static IList<string> Validate(IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (tableNames == null)
            {
                throw new ArgumentNullException(nameof(tableNames));
            }
            if (destinationConnection == null)
            {
                throw new ArgumentNullException(nameof(destinationConnection));
            }
            var names = tableNames.ToList();
            if (names.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("The table names cannot contain a null or a white-space name.", nameof(tableNames));
            }
            return names;
        }

        #endregion
    }
}