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
        /// <param name="targetSchema">The name of the schema of the destination database that the tables are created in (it must already exist). The default is <c>null</c>, which creates the tables in the default schema of the destination database (see <see cref="IDbSetting.DefaultSchema"/> of the setting of the destination connection), or in the schema of the source table if the setting has no default schema. The tables of all the schemas are created in the same schema, so two tables with the same name (i.e.: <c>dbo.Item</c> and <c>Sales.Item</c>) cannot be copied together.</param>
        /// <param name="columns">The names of the columns of the table that are created in the destination database (the names are not case sensitive). The default is <c>null</c>, which creates all the columns. The indexes, the primary key, the unique constraints, the check constraints and the foreign keys that use a column that is not given are not created. The tables that are copied only because they are related to the table keep all their columns, without the foreign keys that reference a column that is not given. An <see cref="ArgumentException"/> is thrown, before anything is created, if a column is not a column of the source table.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database: <see cref="CopyTableSchemaExistsBehavior.Skip"/> leaves it as is, <see cref="CopyTableSchemaExistsBehavior.Align"/> adds its missing columns and indexes, <see cref="CopyTableSchemaExistsBehavior.Throw"/> throws before anything is created and <see cref="CopyTableSchemaExistsBehavior.Drop"/> drops it and creates it again. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>. Whether the table exists is checked with the statements of the composer, so a composer that does not compose them (empty statement) is treated as if the table does not exist. <b>WARNING:</b> <see cref="CopyTableSchemaExistsBehavior.Drop"/> permanently deletes the existing table and its data.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) are copied together with it. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>. The result is still the one of the given table, use the multiple tables overload to receive the result of each related table.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="renamedColumns">The new names of the columns (the source column name and the name that the column has in the destination database; the names are not case sensitive), i.e.: when a table is copied to receive the data of other column names. The columns that are not listed keep their name. The primary key, the indexes, the unique constraints, the check constraints and the foreign keys follow the renamed columns. The default is <c>null</c>, which keeps all the names.</param>
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
            IDbTransaction transaction = null,
            IDictionary<string, string> renamedColumns = null)
            where TEntity : class =>
            connection.CopyTableSchemaTo(ClassMappedNameCache.Get<TEntity>(), destinationConnection, targetSchema, columns, tableExistenceBehavior, relationshipBehavior, commandTimeout, traceKey, trace, transaction, renamedColumns);

        /// <summary>
        /// Copies the schema of the source table of the current connection into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The name of the schema of the destination database that the tables are created in (it must already exist). The default is <c>null</c>, which creates the tables in the default schema of the destination database (see <see cref="IDbSetting.DefaultSchema"/> of the setting of the destination connection), or in the schema of the source table if the setting has no default schema. The tables of all the schemas are created in the same schema, so two tables with the same name (i.e.: <c>dbo.Item</c> and <c>Sales.Item</c>) cannot be copied together.</param>
        /// <param name="columns">The names of the columns of the table that are created in the destination database (the names are not case sensitive). The default is <c>null</c>, which creates all the columns. The indexes, the primary key, the unique constraints, the check constraints and the foreign keys that use a column that is not given are not created. The tables that are copied only because they are related to the table keep all their columns, without the foreign keys that reference a column that is not given. An <see cref="ArgumentException"/> is thrown, before anything is created, if a column is not a column of the source table.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database: <see cref="CopyTableSchemaExistsBehavior.Skip"/> leaves it as is, <see cref="CopyTableSchemaExistsBehavior.Align"/> adds its missing columns and indexes, <see cref="CopyTableSchemaExistsBehavior.Throw"/> throws before anything is created and <see cref="CopyTableSchemaExistsBehavior.Drop"/> drops it and creates it again. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>. Whether the table exists is checked with the statements of the composer, so a composer that does not compose them (empty statement) is treated as if the table does not exist. <b>WARNING:</b> <see cref="CopyTableSchemaExistsBehavior.Drop"/> permanently deletes the existing table and its data.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) are copied together with it. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>. The result is still the one of the given table, use the multiple tables overload to receive the result of each related table.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="renamedColumns">The new names of the columns (the source column name and the name that the column has in the destination database; the names are not case sensitive), i.e.: when a table is copied to receive the data of other column names. The columns that are not listed keep their name. The primary key, the indexes, the unique constraints, the check constraints and the foreign keys follow the renamed columns. The default is <c>null</c>, which keeps all the names.</param>
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
            IDbTransaction transaction = null,
            IDictionary<string, string> renamedColumns = null)
        {
            Validate(connection, tableName, destinationConnection);

            // Copy the table as a set of one table, the callback receives the result of each table that is copied
            var results = new List<CopyTableSchemaResult>();
            connection.CopyTableSchemaTo(new[] { tableName },
                destinationConnection,
                targetSchema,
                columns,
                tableExistenceBehavior,
                relationshipBehavior,
                createdCallback: results.Add,
                renamedColumns: renamedColumns,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                trace: trace,
                transaction: transaction);
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
        /// <param name="tableNames">The names of the tables whose schema is to be copied. The tables can be given in any order, and can reference each other (even in a circular way).</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The name of the schema of the destination database that the tables are created in (it must already exist). The default is <c>null</c>, which creates the tables in the default schema of the destination database (see <see cref="IDbSetting.DefaultSchema"/> of the setting of the destination connection), or in the schema of the source table if the setting has no default schema. The tables of all the schemas are created in the same schema, so two tables with the same name (i.e.: <c>dbo.Item</c> and <c>Sales.Item</c>) cannot be copied together.</param>
        /// <param name="columns">The names of the columns of the given tables that are created in the destination database (the names are not case sensitive). The default is <c>null</c>, which creates all the columns. The indexes, the primary key, the unique constraints, the check constraints and the foreign keys that use a column that is not given are not created. The tables that are copied only because they are related to the given tables keep all their columns, without the foreign keys that reference a column that is not given. An <see cref="ArgumentException"/> is thrown, before anything is created, if a column is not a column of one of the given tables.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database: <see cref="CopyTableSchemaExistsBehavior.Skip"/> leaves it as is, <see cref="CopyTableSchemaExistsBehavior.Align"/> adds its missing columns and indexes, <see cref="CopyTableSchemaExistsBehavior.Throw"/> throws before anything is created and <see cref="CopyTableSchemaExistsBehavior.Drop"/> drops it and creates it again. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>. Whether the table exists is checked with the statements of the composer, so a composer that does not compose them (empty statement) is treated as if the table does not exist. <b>WARNING:</b> <see cref="CopyTableSchemaExistsBehavior.Drop"/> permanently deletes the existing table and its data.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the given tables (through the foreign keys) are copied together with them. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>. Only the foreign keys of the source database are read to find them.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResult"/> of a table every time its schema is created in the destination database (the table, its indexes and its foreign keys), in the order that the schemas are completed. The errors that were raised for the table are in the <see cref="CopyTableSchemaResult.Errors"/> of its result. The default is <c>null</c>.</param>
        /// <param name="errorCallback">The callback that receives a <see cref="CopyTableSchemaError"/> every time a statement fails while the schemas are being created. The error is also added to the <see cref="CopyTableSchemaResult.Errors"/> of the table that it belongs to, and the copy continues with the next statement; throw from the callback to stop the copy. Without a callback, the exception is thrown. The errors of reading the schemas are not reported to it. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemasTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="renamedColumns">The new names of the columns (the source column name and the name that the column has in the destination database; the names are not case sensitive), i.e.: when a table is copied to receive the data of other column names. The columns that are not listed keep their name. The primary key, the indexes, the unique constraints, the check constraints and the foreign keys follow the renamed columns. The default is <c>null</c>, which keeps all the names.</param>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static void CopyTableSchemaTo(this IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            string targetSchema = null,
            string[] columns = null,
            CopyTableSchemaExistsBehavior tableExistenceBehavior = CopyTableSchemaExistsBehavior.Skip,
            CopyTableSchemaRelationshipBehavior relationshipBehavior = CopyTableSchemaRelationshipBehavior.TableOnly,
            Action<CopyTableSchemaResult> createdCallback = null,
            Action<CopyTableSchemaError> errorCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemasTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            IDictionary<string, string> renamedColumns = null)
        {
            // Validate
            var names = Validate(connection, tableNames, destinationConnection);
            var requestedNames = names;
            var selectedColumns = CopyTableSchemaColumns.Normalize(columns);
            var renamed = CopyTableSchemaColumns.NormalizeRenamed(renamedColumns);
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
            if (selectedColumns != null || renamed != null)
            {
                var requestedSchemas = requestedNames.Select(schemaReader.GetTableSchema).ToList();
                schemas = CopyTableSchemaColumns.Apply(schemas, requestedSchemas, selectedColumns, renamed).ToList();
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
        /// <param name="targetSchema">The name of the schema of the destination database that the tables are created in (it must already exist). The default is <c>null</c>, which creates the tables in the default schema of the destination database (see <see cref="IDbSetting.DefaultSchema"/> of the setting of the destination connection), or in the schema of the source table if the setting has no default schema. The tables of all the schemas are created in the same schema, so two tables with the same name (i.e.: <c>dbo.Item</c> and <c>Sales.Item</c>) cannot be copied together.</param>
        /// <param name="columns">The names of the columns of the table that are created in the destination database (the names are not case sensitive). The default is <c>null</c>, which creates all the columns. The indexes, the primary key, the unique constraints, the check constraints and the foreign keys that use a column that is not given are not created. The tables that are copied only because they are related to the table keep all their columns, without the foreign keys that reference a column that is not given. An <see cref="ArgumentException"/> is thrown, before anything is created, if a column is not a column of the source table.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database: <see cref="CopyTableSchemaExistsBehavior.Skip"/> leaves it as is, <see cref="CopyTableSchemaExistsBehavior.Align"/> adds its missing columns and indexes, <see cref="CopyTableSchemaExistsBehavior.Throw"/> throws before anything is created and <see cref="CopyTableSchemaExistsBehavior.Drop"/> drops it and creates it again. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>. Whether the table exists is checked with the statements of the composer, so a composer that does not compose them (empty statement) is treated as if the table does not exist. <b>WARNING:</b> <see cref="CopyTableSchemaExistsBehavior.Drop"/> permanently deletes the existing table and its data.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) are copied together with it. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>. The result is still the one of the given table, use the multiple tables overload to receive the result of each related table.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="renamedColumns">The new names of the columns (the source column name and the name that the column has in the destination database; the names are not case sensitive), i.e.: when a table is copied to receive the data of other column names. The columns that are not listed keep their name. The primary key, the indexes, the unique constraints, the check constraints and the foreign keys follow the renamed columns. The default is <c>null</c>, which keeps all the names.</param>
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
            CancellationToken cancellationToken = default,
            IDictionary<string, string> renamedColumns = null)
            where TEntity : class =>
            connection.CopyTableSchemaToAsync(ClassMappedNameCache.Get<TEntity>(), destinationConnection, targetSchema, columns, tableExistenceBehavior, relationshipBehavior, commandTimeout, traceKey, trace, transaction, cancellationToken, renamedColumns);

        /// <summary>
        /// Copies the schema of the source table of the current connection into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="targetSchema">The name of the schema of the destination database that the tables are created in (it must already exist). The default is <c>null</c>, which creates the tables in the default schema of the destination database (see <see cref="IDbSetting.DefaultSchema"/> of the setting of the destination connection), or in the schema of the source table if the setting has no default schema. The tables of all the schemas are created in the same schema, so two tables with the same name (i.e.: <c>dbo.Item</c> and <c>Sales.Item</c>) cannot be copied together.</param>
        /// <param name="columns">The names of the columns of the table that are created in the destination database (the names are not case sensitive). The default is <c>null</c>, which creates all the columns. The indexes, the primary key, the unique constraints, the check constraints and the foreign keys that use a column that is not given are not created. The tables that are copied only because they are related to the table keep all their columns, without the foreign keys that reference a column that is not given. An <see cref="ArgumentException"/> is thrown, before anything is created, if a column is not a column of the source table.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database: <see cref="CopyTableSchemaExistsBehavior.Skip"/> leaves it as is, <see cref="CopyTableSchemaExistsBehavior.Align"/> adds its missing columns and indexes, <see cref="CopyTableSchemaExistsBehavior.Throw"/> throws before anything is created and <see cref="CopyTableSchemaExistsBehavior.Drop"/> drops it and creates it again. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>. Whether the table exists is checked with the statements of the composer, so a composer that does not compose them (empty statement) is treated as if the table does not exist. <b>WARNING:</b> <see cref="CopyTableSchemaExistsBehavior.Drop"/> permanently deletes the existing table and its data.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the table (through the foreign keys) are copied together with it. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>. The result is still the one of the given table, use the multiple tables overload to receive the result of each related table.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="renamedColumns">The new names of the columns (the source column name and the name that the column has in the destination database; the names are not case sensitive), i.e.: when a table is copied to receive the data of other column names. The columns that are not listed keep their name. The primary key, the indexes, the unique constraints, the check constraints and the foreign keys follow the renamed columns. The default is <c>null</c>, which keeps all the names.</param>
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
            CancellationToken cancellationToken = default,
            IDictionary<string, string> renamedColumns = null)
        {
            Validate(connection, tableName, destinationConnection);
            var results = new List<CopyTableSchemaResult>();
            await connection.CopyTableSchemaToAsync(new[] { tableName },
                destinationConnection,
                targetSchema,
                columns,
                tableExistenceBehavior,
                relationshipBehavior,
                createdCallback: results.Add,
                renamedColumns: renamedColumns,
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

        #region CopyTableSchemaTo (IDbConnection, Async, Multiple Tables)

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
        /// <param name="columns">The names of the columns of the given tables that are created in the destination database (the names are not case sensitive). The default is <c>null</c>, which creates all the columns. The indexes, the primary key, the unique constraints, the check constraints and the foreign keys that use a column that is not given are not created. The tables that are copied only because they are related to the given tables keep all their columns, without the foreign keys that reference a column that is not given. An <see cref="ArgumentException"/> is thrown, before anything is created, if a column is not a column of one of the given tables.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database: <see cref="CopyTableSchemaExistsBehavior.Skip"/> leaves it as is, <see cref="CopyTableSchemaExistsBehavior.Align"/> adds its missing columns and indexes, <see cref="CopyTableSchemaExistsBehavior.Throw"/> throws before anything is created and <see cref="CopyTableSchemaExistsBehavior.Drop"/> drops it and creates it again. The default is <see cref="CopyTableSchemaExistsBehavior.Skip"/>. Whether the table exists is checked with the statements of the composer, so a composer that does not compose them (empty statement) is treated as if the table does not exist. <b>WARNING:</b> <see cref="CopyTableSchemaExistsBehavior.Drop"/> permanently deletes the existing table and its data.</param>
        /// <param name="relationshipBehavior">Defines which of the tables that are related to the given tables (through the foreign keys) are copied together with them. The default is <see cref="CopyTableSchemaRelationshipBehavior.TableOnly"/>. Only the foreign keys of the source database are read to find them.</param>
        /// <param name="createdCallback">The callback that receives the <see cref="CopyTableSchemaResult"/> of a table every time its schema is created in the destination database (the table, its indexes and its foreign keys), in the order that the schemas are completed. The errors that were raised for the table are in the <see cref="CopyTableSchemaResult.Errors"/> of its result. The default is <c>null</c>.</param>
        /// <param name="errorCallback">The callback that receives a <see cref="CopyTableSchemaError"/> every time a statement fails while the schemas are being created. The error is also added to the <see cref="CopyTableSchemaResult.Errors"/> of the table that it belongs to, and the copy continues with the next statement; throw from the callback to stop the copy. Without a callback, the exception is thrown. The errors of reading the schemas are not reported to it. The default is <c>null</c>.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopyTableSchemasTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="renamedColumns">The new names of the columns (the source column name and the name that the column has in the destination database; the names are not case sensitive), i.e.: when a table is copied to receive the data of other column names. The columns that are not listed keep their name. The primary key, the indexes, the unique constraints, the check constraints and the foreign keys follow the renamed columns. The default is <c>null</c>, which keeps all the names.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
        public static async Task CopyTableSchemaToAsync(this IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            string targetSchema = null,
            string[] columns = null,
            CopyTableSchemaExistsBehavior tableExistenceBehavior = CopyTableSchemaExistsBehavior.Skip,
            CopyTableSchemaRelationshipBehavior relationshipBehavior = CopyTableSchemaRelationshipBehavior.TableOnly,
            Action<CopyTableSchemaResult> createdCallback = null,
            Action<CopyTableSchemaError> errorCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopyTableSchemasTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default,
            IDictionary<string, string> renamedColumns = null)
        {
            // Validate
            var names = Validate(connection, tableNames, destinationConnection);
            var requestedNames = names;
            var selectedColumns = CopyTableSchemaColumns.Normalize(columns);
            var renamed = CopyTableSchemaColumns.NormalizeRenamed(renamedColumns);
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
            if (selectedColumns != null || renamed != null)
            {
                var requestedSchemas = new List<TableSchema>();
                foreach (var name in requestedNames)
                {
                    requestedSchemas.Add(await schemaReader.GetTableSchemaAsync(name, cancellationToken).ConfigureAwait(false));
                }
                schemas = CopyTableSchemaColumns.Apply(schemas, requestedSchemas, selectedColumns, renamed).ToList();
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

        #endregion
    }
}
