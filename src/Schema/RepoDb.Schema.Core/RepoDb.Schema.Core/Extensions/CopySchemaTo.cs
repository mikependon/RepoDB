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
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="CopySchemaExistsBehavior.SkipOnExists"/>. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.DropOnExists"/> permanently deletes the existing table and its data.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopySchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopySchemaResult"/> that describes the schema copy operation.</returns>
        public static CopySchemaResult CopySchemaTo<TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.SkipOnExists,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopySchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
            where TEntity : class =>
            connection.CopySchemaTo(ClassMappedNameCache.Get<TEntity>(), destinationConnection, tableExistenceBehavior, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Copies the schema of the source table of the current connection into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="CopySchemaExistsBehavior.SkipOnExists"/>. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.DropOnExists"/> permanently deletes the existing table and its data.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopySchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopySchemaResult"/> that describes the schema copy operation.</returns>
        public static CopySchemaResult CopySchemaTo(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.SkipOnExists,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopySchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            Validate(connection, tableName, destinationConnection);

            // Copy the table as a set of one table, the callback receives its result
            CopySchemaResult result = null;
            connection.CopySchemaTo(new[] { tableName },
                destinationConnection,
                tableExistenceBehavior,
                createdCallback: r => result = r,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                trace: trace,
                transaction: transaction);
            return result;
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
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database. The default is <see cref="CopySchemaExistsBehavior.SkipOnExists"/>. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.DropOnExists"/> permanently deletes the existing table and its data.</param>
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
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.SkipOnExists,
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
            var statements = schemaComposer.ComposeSchemas(schemas).ToList();
            var completions = GetCompletions(schemas, statements.Count);
            var owners = GetOwners(schemas, statements.Count);
            var errors = new List<CopySchemaError>();

            // Iterate
            for (var i = 0; i < statements.Count; i++)
            {
                try
                {
                    destinationConnection.ExecuteNonQuery(statements[i],
                        traceKey: traceKey,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace);
                }
                catch (Exception e) when (errorCallback != null && !(e is OperationCanceledException))
                {
                    var error = CreateError(e, statements[i], i, owners[i]);
                    errors.Add(error);
                    errorCallback(error);
                }

                // Report
                Report(createdCallback, completions, errors, i, connection, destinationConnection, tableExistenceBehavior, schemaComposer, startTime);
            }

            // Report
            Report(createdCallback, completions, errors, -1, connection, destinationConnection, tableExistenceBehavior, schemaComposer, startTime);
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
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="CopySchemaExistsBehavior.SkipOnExists"/>. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.DropOnExists"/> permanently deletes the existing table and its data.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopySchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopySchemaResult"/> that describes the schema copy operation.</returns>
        public static Task<CopySchemaResult> CopySchemaToAsync<TEntity>(this IDbConnection connection,
            IDbConnection destinationConnection,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.SkipOnExists,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopySchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            connection.CopySchemaToAsync(ClassMappedNameCache.Get<TEntity>(), destinationConnection, tableExistenceBehavior, commandTimeout, traceKey, trace, transaction, cancellationToken);

        /// <summary>
        /// Copies the schema of the source table of the current connection into the same-named table of the destination connection.
        /// The source table is left untouched.
        /// </summary>
        /// <param name="connection">The source connection.</param>
        /// <param name="tableName">The name of the table to be copied.</param>
        /// <param name="destinationConnection">The connection of the destination database.</param>
        /// <param name="tableExistenceBehavior">Defines what happens when the table already exists in the destination database. The default is <see cref="CopySchemaExistsBehavior.SkipOnExists"/>. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.DropOnExists"/> permanently deletes the existing table and its data.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopySchemaTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopySchemaResult"/> that describes the schema copy operation.</returns>
        public static async Task<CopySchemaResult> CopySchemaToAsync(this IDbConnection connection,
            string tableName,
            IDbConnection destinationConnection,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.SkipOnExists,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopySchemaTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            Validate(connection, tableName, destinationConnection);

            // Copy the table as a set of one table, the callback receives its result
            CopySchemaResult result = null;
            await connection.CopySchemaToAsync(new[] { tableName },
                destinationConnection,
                tableExistenceBehavior,
                createdCallback: r => result = r,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                trace: trace,
                transaction: transaction,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return result;
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
        /// <param name="tableExistenceBehavior">Defines what happens when a table already exists in the destination database. The default is <see cref="CopySchemaExistsBehavior.SkipOnExists"/>. <b>WARNING:</b> <see cref="CopySchemaExistsBehavior.DropOnExists"/> permanently deletes the existing table and its data.</param>
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
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.SkipOnExists,
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
            var statements = schemaComposer.ComposeSchemas(schemas).ToList();
            var completions = GetCompletions(schemas, statements.Count);
            var owners = GetOwners(schemas, statements.Count);
            var errors = new List<CopySchemaError>();

            // Iterate
            for (var i = 0; i < statements.Count; i++)
            {
                try
                {
                    await destinationConnection.ExecuteNonQueryAsync(statements[i],
                        traceKey: traceKey,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e) when (errorCallback != null && !(e is OperationCanceledException))
                {
                    var error = CreateError(e, statements[i], i, owners[i]);
                    errors.Add(error);
                    errorCallback(error);
                }

                // Report
                Report(createdCallback, completions, errors, i, connection, destinationConnection, tableExistenceBehavior, schemaComposer, startTime);
            }

            // Report
            Report(createdCallback, completions, errors, -1, connection, destinationConnection, tableExistenceBehavior, schemaComposer, startTime);
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the schemas that are completed by each statement of the composed script. The composed script has one statement for each table, then one for each index
        /// and then one for each foreign key (see <see cref="ISchemaComposer.ComposeSchemas"/>), so a schema is completed by its last statement.
        /// </summary>
        /// <param name="schemas">The schemas of the tables, in the order that they were composed.</param>
        /// <param name="statementCount">The number of the statements that were composed.</param>
        /// <returns>
        /// The schemas by the index of the statement that completes them. If the script does not have the expected number of statements,
        /// the statements that complete the schemas are not known, so it only has the key <c>-1</c> with all the schemas.
        /// </returns>
        private static ILookup<int, TableSchema> GetCompletions(IList<TableSchema> schemas,
            int statementCount)
        {
            var last = new int[schemas.Count];
            var cursor = 0;
            for (var i = 0; i < schemas.Count; i++)
            {
                last[i] = cursor++;
            }
            for (var i = 0; i < schemas.Count; i++)
            {
                for (var j = 0; j < schemas[i].Indexes.Count; j++)
                {
                    last[i] = cursor++;
                }
            }
            for (var i = 0; i < schemas.Count; i++)
            {
                for (var j = 0; j < schemas[i].ForeignKeys.Count; j++)
                {
                    last[i] = cursor++;
                }
            }
            var known = cursor == statementCount;
            return Enumerable.Range(0, schemas.Count).ToLookup(i => known ? last[i] : -1, i => schemas[i]);
        }

        /// <summary>
        /// Gets the schema that each statement of the composed script belongs to. The composed script has one statement for each table, then one for each index
        /// and then one for each foreign key (see <see cref="ISchemaComposer.ComposeSchemas"/>).
        /// </summary>
        /// <param name="schemas">The schemas of the tables, in the order that they were composed.</param>
        /// <param name="statementCount">The number of the statements that were composed.</param>
        /// <returns>The schema of each statement (<c>null</c> if the script does not have the expected number of statements, so the owners are not known).</returns>
        private static TableSchema[] GetOwners(IList<TableSchema> schemas,
            int statementCount)
        {
            var owners = new List<TableSchema>(schemas);
            owners.AddRange(schemas.SelectMany(schema => schema.Indexes.Select(_ => schema)));
            owners.AddRange(schemas.SelectMany(schema => schema.ForeignKeys.Select(_ => schema)));
            return owners.Count == statementCount
                ? owners.ToArray()
                : new TableSchema[statementCount];
        }

        /// <summary>
        /// Creates the error of a statement that has failed.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="statement"></param>
        /// <param name="statementIndex"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        private static CopySchemaError CreateError(Exception exception,
            string statement,
            int statementIndex,
            TableSchema owner) =>
            new CopySchemaError
            {
                Exception = exception,
                Statement = statement,
                StatementIndex = statementIndex,
                TableName = owner?.Table?.Name,
                SchemaName = owner?.Table?.Schema
            };

        /// <summary>
        /// Calls the callback with the result of each schema that is completed by the statement.
        /// </summary>
        /// <param name="createdCallback"></param>
        /// <param name="completions"></param>
        /// <param name="errors"></param>
        /// <param name="statement"></param>
        /// <param name="connection"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="tableExistenceBehavior"></param>
        /// <param name="schemaComposer"></param>
        /// <param name="startTime"></param>
        private static void Report(Action<CopySchemaResult> createdCallback,
            ILookup<int, TableSchema> completions,
            IList<CopySchemaError> errors,
            int statement,
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

            foreach (var schema in completions[statement])
            {
                var schemaErrors = errors
                    .Where(error => error.TableName == null ||
                        (string.Equals(error.TableName, schema.Table.Name, StringComparison.Ordinal) &&
                        string.Equals(error.SchemaName, schema.Table.Schema, StringComparison.Ordinal)))
                    .ToList();
                var script = schemaComposer.ComposeSchema(schema) ?? Enumerable.Empty<string>();
                
                createdCallback(new CopySchemaResult
                {
                    Action = tableExistenceBehavior,
                    Outcome = schemaErrors.Count > 0 ? CopySchemaOutcome.Failed : CopySchemaOutcome.Created,
                    Errors = schemaErrors,
                    TableName = schema.Table.Name,
                    SourceSchema = schema.Table.Schema,
                    SourceDatabase = connection.Database,
                    SourceServer = (connection as DbConnection)?.DataSource,
                    SourceDatabaseType = connection.GetType().Name,
                    DestinationDatabase = destinationConnection.Database,
                    DestinationServer = (destinationConnection as DbConnection)?.DataSource,
                    DestinationDatabaseType = destinationConnection.GetType().Name,
                    StartTime = startTime,
                    EndTime = DateTime.UtcNow,
                    Script = string.Join(Environment.NewLine, script),
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