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
    /// Contains the extension methods of <see cref="IDbConnection"/> object for copying the schema of multiple tables between databases.
    /// </summary>
    public static class CopySchemasToExtension
    {
        #region Public Methods

        #region CopySchemasTo (IDbConnection, Sync)

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
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopySchemasTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <returns>The <see cref="CopySchemasResult"/> that describes the schema copy operation.</returns>
        public static CopySchemasResult CopySchemasTo(this IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.SkipOnExists,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopySchemasTo,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            var names = Validate(connection, tableNames, destinationConnection);
            var result = Create(connection, destinationConnection, tableExistenceBehavior);

            // If there are no tables to copy, return the result immediately
            if (names.Count == 0)
            {
                result.EndTime = DateTime.UtcNow;
                return result;
            }

            // Get the schema reader based on the type of the source connection
            var schemaReader = SchemaReaderMapper.Get(connection) ??
                throw new MissingMappingException($"There is no schema reader mapping found for '{connection.GetType().FullName}'. Make sure to register one via the '{nameof(SchemaReaderMapper)}'.");

            // Get the schema composer based on the type of the destination connection
            var schemaComposer = SchemaComposerMapper.Get(destinationConnection) ??
                throw new MissingMappingException($"There is no schema composer mapping found for '{destinationConnection.GetType().FullName}'. Make sure to register one via the '{nameof(SchemaComposerMapper)}'.");

            // Read the schemas of the tables (the tables that are referenced by the other tables come first) and compose the statements for the destination
            var schemas = schemaReader.GetDependencyOrder(names).Select(r => r.Table).ToList();
            var statements = schemaComposer.ComposeSchemas(schemas).ToList();

            // Execute the composed statements, in order, on the destination connection
            foreach (var statement in statements)
            {
                destinationConnection.ExecuteNonQuery(statement,
                    traceKey: traceKey,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace);
            }

            Fill(result, schemaComposer, schemas, statements);
            return result;
        }

        #endregion

        #region CopySchemasTo (IDbConnection, Async)

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
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CopySchemasTo"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used on the destination connection. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="CopySchemasResult"/> that describes the schema copy operation.</returns>
        public static async Task<CopySchemasResult> CopySchemasToAsync(this IDbConnection connection,
            IEnumerable<string> tableNames,
            IDbConnection destinationConnection,
            CopySchemaExistsBehavior tableExistenceBehavior = CopySchemaExistsBehavior.SkipOnExists,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CopySchemasTo,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var names = Validate(connection, tableNames, destinationConnection);
            var result = Create(connection, destinationConnection, tableExistenceBehavior);

            // If there are no tables to copy, return the result immediately
            if (names.Count == 0)
            {
                result.EndTime = DateTime.UtcNow;
                return result;
            }

            // Get the schema reader based on the type of the source connection
            var schemaReader = SchemaReaderMapper.Get(connection) ??
                throw new MissingMappingException($"There is no schema reader mapping found for '{connection.GetType().FullName}'. Make sure to register one via the '{nameof(SchemaReaderMapper)}'.");

            // Get the schema composer based on the type of the destination connection
            var schemaComposer = SchemaComposerMapper.Get(destinationConnection) ??
                throw new MissingMappingException($"There is no schema composer mapping found for '{destinationConnection.GetType().FullName}'. Make sure to register one via the '{nameof(SchemaComposerMapper)}'.");

            // Read the schemas of the tables (the tables that are referenced by the other tables come first) and compose the statements for the destination
            var relationships = await schemaReader.GetDependencyOrderAsync(names, cancellationToken).ConfigureAwait(false);
            var schemas = relationships.Select(r => r.Table).ToList();
            var statements = schemaComposer.ComposeSchemas(schemas).ToList();

            // Execute the composed statements, in order, on the destination connection
            foreach (var statement in statements)
            {
                await destinationConnection.ExecuteNonQueryAsync(statement,
                    traceKey: traceKey,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            Fill(result, schemaComposer, schemas, statements);
            return result;
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="destinationConnection"></param>
        /// <param name="tableExistenceBehavior"></param>
        /// <returns></returns>
        private static CopySchemasResult Create(IDbConnection connection,
            IDbConnection destinationConnection,
            CopySchemaExistsBehavior tableExistenceBehavior) =>
            new CopySchemasResult
            {
                Action = tableExistenceBehavior,
                StartTime = DateTime.UtcNow,
                SourceDatabase = connection.Database,
                SourceServer = (connection as DbConnection)?.DataSource,
                SourceDatabaseType = connection.GetType().Name,
                DestinationDatabase = destinationConnection.Database,
                DestinationServer = (destinationConnection as DbConnection)?.DataSource,
                DestinationDatabaseType = destinationConnection.GetType().Name
            };

        /// <summary>
        ///
        /// </summary>
        /// <param name="result"></param>
        /// <param name="schemaComposer"></param>
        /// <param name="schemas"></param>
        /// <param name="statements"></param>
        private static void Fill(CopySchemasResult result,
            ISchemaComposer schemaComposer,
            IList<TableSchema> schemas,
            IList<string> statements)
        {
            result.Script = string.Join(Environment.NewLine, statements);
            foreach (var schema in schemas)
            {
                var script = schemaComposer.ComposeSchema(schema) ?? Enumerable.Empty<string>();
                result.Tables.Add(new CopySchemaResult
                {
                    Action = result.Action,
                    Outcome = CopySchemaOutcome.Created,
                    TableName = schema.TableName,
                    SourceSchema = schema.SchemaName,
                    SourceDatabase = result.SourceDatabase,
                    SourceServer = result.SourceServer,
                    SourceDatabaseType = result.SourceDatabaseType,
                    DestinationDatabase = result.DestinationDatabase,
                    DestinationServer = result.DestinationServer,
                    DestinationDatabaseType = result.DestinationDatabaseType,
                    StartTime = result.StartTime,
                    Script = string.Join(Environment.NewLine, script),
                    ColumnCount = schema.Columns.Count,
                    IndexCount = schema.Indexes.Count,
                    ForeignKeyCount = schema.ForeignKeys.Count,
                    UniqueConstraintCount = schema.UniqueConstraints.Count,
                    CheckConstraintCount = schema.CheckConstraints.Count
                });
            }

            result.EndTime = DateTime.UtcNow;
            foreach (var table in result.Tables)
            {
                table.EndTime = result.EndTime;
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
