#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Interfaces;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Contains the extension methods for reading saved schema definitions.
    /// </summary>
    public static partial class ReadDefinitionExtension
    {
        #region Public Methods

        /// <summary>
        /// Reads a RepoDB JSON definition, including all tables and extraction metadata.
        /// The connection is not opened or queried. Only metadata present in the file can be restored.
        /// </summary>
        /// <param name="connection">The connection that the extension is invoked on; no provider registration is required.</param>
        /// <param name="filename">The existing definition filename.</param>
        /// <param name="traceKey">The tracking key for the file read.</param>
        /// <param name="trace">The trace callbacks. The before log statement contains the filename and can be changed or cancelled.</param>
        /// <returns>The definition, or null if the trace cancels without requesting an exception.</returns>
        public static SchemaDefinitionInfo ReadDefinitionFromJson(this IDbConnection connection,
            string filename,
            string traceKey = SchemaTraceKeys.ReadDefinitionFromJson,
            ITrace trace = null) =>
            SchemaDefinitionFile.Read(connection,
                filename,
                traceKey,
                trace,
                SchemaDefinitionJsonReader.Parse);

        /// <summary>
        /// Asynchronously reads a RepoDB JSON definition, including all tables and extraction metadata.
        /// The connection is not opened or queried. Only metadata present in the file can be restored.
        /// </summary>
        /// <param name="connection">The connection that the extension is invoked on; no provider registration is required.</param>
        /// <param name="filename">The existing definition filename.</param>
        /// <param name="traceKey">The tracking key for the file read.</param>
        /// <param name="trace">The asynchronous trace callbacks.</param>
        /// <param name="cancellationToken">The cancellation token for reading the file.</param>
        /// <returns>The definition, or null if the trace cancels without requesting an exception.</returns>
        public static Task<SchemaDefinitionInfo> ReadDefinitionFromJsonAsync(this IDbConnection connection,
            string filename,
            string traceKey = SchemaTraceKeys.ReadDefinitionFromJson,
            ITrace trace = null,
            CancellationToken cancellationToken = default) =>
            SchemaDefinitionFile.ReadAsync(connection,
                filename,
                traceKey,
                trace,
                SchemaDefinitionJsonReader.Parse,
                cancellationToken);

        #endregion
    }
}
