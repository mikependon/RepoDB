#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Interfaces;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Contains extension methods for applying saved schema definitions to a database.
    /// </summary>
    public static partial class LoadDefinitionExtension
    {
        #region Public Methods

        /// <summary>
        /// Reads a RepoDB JSON definition and applies all included tables to the destination.
        /// Table schemas and relationship selection come from the file; external dependencies are not loaded.
        /// </summary>
        /// <param name="connection">The destination connection with a registered schema composer.</param>
        /// <param name="filename">The existing definition filename.</param>
        /// <param name="tableExistenceBehavior">The behavior for existing tables. Drop permanently deletes their data. The default is Skip.</param>
        /// <param name="createdCallback">The callback invoked for each completed table, including skipped or aligned tables, as in CopyTableSchemaTo.</param>
        /// <param name="commandTimeout">The destination command timeout in seconds; null uses the provider default.</param>
        /// <param name="traceKey">The tracking key for file reads and destination commands.</param>
        /// <param name="trace">The trace for file reads and destination commands.</param>
        /// <param name="transaction">An optional transaction belonging to the destination connection. The caller commits or rolls it back.</param>
        /// <returns>The loaded definition and creation outcomes, or null if the file read is cancelled by Cancel(false).</returns>
        public static LoadDefinitionResult LoadDefinitionFromJson(this IDbConnection connection,
            string filename,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            Action<CopyTableSchemaResultItem> createdCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.LoadDefinitionFromJson,
            ITrace trace = null,
            IDbTransaction transaction = null)
        {
            SchemaDefinitionLoader.Validate(connection,
                filename,
                tableExistenceBehavior,
                commandTimeout,
                transaction);
            var definition = connection.ReadDefinitionFromJson(filename,
                traceKey,
                trace);
            return SchemaDefinitionLoader.Apply(connection,
                definition,
                tableExistenceBehavior,
                createdCallback,
                commandTimeout,
                traceKey,
                trace,
                transaction);
        }

        /// <summary>
        /// Asynchronously reads a RepoDB JSON definition and applies all included tables to the destination.
        /// Cancellation or command failure does not roll back earlier statements; use a provider-supported transaction when required.
        /// </summary>
        /// <param name="connection">The destination connection with a registered schema composer.</param>
        /// <param name="filename">The existing definition filename.</param>
        /// <param name="tableExistenceBehavior">The behavior for existing tables. Drop permanently deletes their data. The default is Skip.</param>
        /// <param name="createdCallback">The callback invoked for each completed table, as in CopyTableSchemaTo.</param>
        /// <param name="commandTimeout">The destination command timeout in seconds; null uses the provider default.</param>
        /// <param name="traceKey">The tracking key for file reads and destination commands.</param>
        /// <param name="trace">The asynchronous trace for file reads and destination commands.</param>
        /// <param name="transaction">An optional destination transaction, managed by the caller.</param>
        /// <param name="cancellationToken">The cancellation token for file reads and destination commands.</param>
        /// <returns>The loaded definition and creation outcomes, or null if the file read is cancelled by Cancel(false).</returns>
        public static async Task<LoadDefinitionResult> LoadDefinitionFromJsonAsync(this IDbConnection connection,
            string filename,
            TableExistsBehavior tableExistenceBehavior = TableExistsBehavior.Skip,
            Action<CopyTableSchemaResultItem> createdCallback = null,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.LoadDefinitionFromJson,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            SchemaDefinitionLoader.Validate(connection,
                filename,
                tableExistenceBehavior,
                commandTimeout,
                transaction);
            var definition = await connection.ReadDefinitionFromJsonAsync(filename,
                traceKey,
                trace,
                cancellationToken).ConfigureAwait(false);
            return await SchemaDefinitionLoader.ApplyAsync(connection,
                definition,
                tableExistenceBehavior,
                createdCallback,
                commandTimeout,
                traceKey,
                trace,
                transaction,
                cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
