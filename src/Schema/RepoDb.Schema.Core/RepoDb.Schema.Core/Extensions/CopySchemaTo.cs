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
    /// Contains the extension methods of <see cref="IDbConnection"/> object for copying the schema of a table between databases.
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

            var result = new CopySchemaResult
            {
                Action = tableExistenceBehavior,
                StartTime = DateTime.UtcNow,
                TableName = tableName,
                SourceDatabase = connection.Database,
                SourceServer = (connection as DbConnection)?.DataSource,
                SourceDatabaseType = connection.GetType().Name,
                DestinationDatabase = destinationConnection.Database,
                DestinationServer = (destinationConnection as DbConnection)?.DataSource,
                DestinationDatabaseType = destinationConnection.GetType().Name
            };

            // Get the schema reader based on the type of the source connection
            var schemaReader = SchemaReaderMapper.Get(connection) ??
                throw new MissingMappingException($"There is no schema reader mapping found for '{connection.GetType().FullName}'. Make sure to register one via the '{nameof(SchemaReaderMapper)}'.");

            // Get the schema composer based on the type of the destination connection
            var schemaComposer = SchemaComposerMapper.Get(destinationConnection) ??
                throw new MissingMappingException($"There is no schema composer mapping found for '{destinationConnection.GetType().FullName}'. Make sure to register one via the '{nameof(SchemaComposerMapper)}'.");

            // Read the schema of the table and compose the statements for the destination
            var schema = schemaReader.GetTableSchema(tableName);
            var statements = schemaComposer.ComposeSchema(schema).ToList();

            // Execute the composed statements, in order, on the destination connection
            foreach (var statement in statements)
            {
                destinationConnection.ExecuteNonQuery(statement,
                    traceKey: traceKey,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace);
            }

            Fill(result, schema, statements);
            result.Outcome = CopySchemaOutcome.Created;
            result.EndTime = DateTime.UtcNow;
            return result;
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

            var result = new CopySchemaResult
            {
                Action = tableExistenceBehavior,
                StartTime = DateTime.UtcNow,
                TableName = tableName,
                SourceDatabase = connection.Database,
                SourceServer = (connection as DbConnection)?.DataSource,
                SourceDatabaseType = connection.GetType().Name,
                DestinationDatabase = destinationConnection.Database,
                DestinationServer = (destinationConnection as DbConnection)?.DataSource,
                DestinationDatabaseType = destinationConnection.GetType().Name
            };

            // Get the schema reader based on the type of the source connection
            var schemaReader = SchemaReaderMapper.Get(connection) ??
                throw new MissingMappingException($"There is no schema reader mapping found for '{connection.GetType().FullName}'. Make sure to register one via the '{nameof(SchemaReaderMapper)}'.");

            // Get the schema composer based on the type of the destination connection
            var schemaComposer = SchemaComposerMapper.Get(destinationConnection) ??
                throw new MissingMappingException($"There is no schema composer mapping found for '{destinationConnection.GetType().FullName}'. Make sure to register one via the '{nameof(SchemaComposerMapper)}'.");

            // Read the schema of the table and compose the statements for the destination
            var schema = await schemaReader.GetTableSchemaAsync(tableName, cancellationToken).ConfigureAwait(false);
            var statements = schemaComposer.ComposeSchema(schema).ToList();

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

            Fill(result, schema, statements);
            result.Outcome = CopySchemaOutcome.Created;
            result.EndTime = DateTime.UtcNow;
            return result;
        }

        #endregion

        #endregion

        #region Helpers

        private static void Fill(CopySchemaResult result,
            TableSchema schema,
            IList<string> statements)
        {
            result.SourceSchema = schema.SchemaName;
            result.Script = string.Join(Environment.NewLine, statements);
            result.ColumnCount = schema.Columns.Count;
            result.IndexCount = schema.Indexes.Count;
            result.ForeignKeyCount = schema.ForeignKeys.Count;
            result.UniqueConstraintCount = schema.UniqueConstraints.Count;
            result.CheckConstraintCount = schema.CheckConstraints.Count;
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

        #endregion
    }
}
