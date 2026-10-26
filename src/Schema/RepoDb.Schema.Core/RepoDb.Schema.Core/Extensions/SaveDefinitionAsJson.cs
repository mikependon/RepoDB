#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using RepoDb.Interfaces;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema
{
    /// <summary>
    /// Contains the extension method for saving table definitions as JSON.
    /// </summary>
    public static partial class SaveDefinitionExtension
    {
        #region Public Methods

        /// <summary>
        /// Saves the table definition as a UTF-8 JSON file.
        /// </summary>
        /// <param name="connection">The source connection with a registered schema reader.</param>
        /// <param name="tableName">The source table, optionally schema-qualified.</param>
        /// <param name="filename">The output filename. An existing file is replaced only after serialization succeeds.</param>
        /// <param name="relationshipBehavior">The related tables to include. TableOnly omits foreign keys.</param>
        /// <param name="commandTimeout">The catalog query timeout in seconds; null uses the provider default.</param>
        /// <param name="traceKey">The tracking key for catalog queries.</param>
        /// <param name="trace">The catalog query trace.</param>
        public static void SaveDefinitionAsJson(this IDbConnection connection,
            string tableName,
            string filename,
            RelatedTableBehavior relationshipBehavior = RelatedTableBehavior.TableOnly,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.SaveDefinitionAsJson,
            ITrace trace = null) =>
            DbConnectionExtension.SaveDefinition(connection,
                tableName,
                filename,
                relationshipBehavior,
                commandTimeout,
                traceKey,
                trace,
                "json");

        #endregion
    }
}
