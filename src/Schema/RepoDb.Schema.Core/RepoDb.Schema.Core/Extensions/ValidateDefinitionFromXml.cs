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
    /// Contains the extension methods for validating saved schema definitions.
    /// </summary>
    public static partial class ValidateDefinitionExtension
    {
        #region Public Methods

        /// <summary>
        /// Validates a RepoDB XML definition file. The file is valid if it can be read and passes the strict checks: its keys, indexes and foreign keys must be consistent with its columns and tables.
        /// The connection is not opened or queried. Problems in the content are reported in the result instead of being thrown, and content that some databases allow is reported as warnings.
        /// </summary>
        /// <param name="connection">The connection that the extension is invoked on; no provider registration is required.</param>
        /// <param name="filename">The existing definition filename.</param>
        /// <param name="traceKey">The tracking key for the file read.</param>
        /// <param name="trace">The trace callbacks. The before log statement contains the filename and can be changed or cancelled.</param>
        /// <returns>The outcome of the validation, or null if the trace cancels without requesting an exception.</returns>
        public static ValidateDefinitionResult ValidateDefinitionFromXml(this IDbConnection connection,
            string filename,
            string traceKey = SchemaTraceKeys.ValidateDefinitionFromXml,
            ITrace trace = null) =>
            SchemaDefinitionValidator.Validate(connection,
                filename,
                traceKey,
                trace,
                SchemaDefinitionXmlReader.Parse);

        /// <summary>
        /// Asynchronously validates a RepoDB XML definition file. The file is valid if it can be read and passes the strict checks: its keys, indexes and foreign keys must be consistent with its columns and tables.
        /// The connection is not opened or queried. Problems in the content are reported in the result instead of being thrown, and content that some databases allow is reported as warnings.
        /// </summary>
        /// <param name="connection">The connection that the extension is invoked on; no provider registration is required.</param>
        /// <param name="filename">The existing definition filename.</param>
        /// <param name="traceKey">The tracking key for the file read.</param>
        /// <param name="trace">The asynchronous trace callbacks.</param>
        /// <param name="cancellationToken">The cancellation token for reading the file.</param>
        /// <returns>The outcome of the validation, or null if the trace cancels without requesting an exception.</returns>
        public static Task<ValidateDefinitionResult> ValidateDefinitionFromXmlAsync(this IDbConnection connection,
            string filename,
            string traceKey = SchemaTraceKeys.ValidateDefinitionFromXml,
            ITrace trace = null,
            CancellationToken cancellationToken = default) =>
            SchemaDefinitionValidator.ValidateAsync(connection,
                filename,
                traceKey,
                trace,
                SchemaDefinitionXmlReader.Parse,
                cancellationToken);

        #endregion
    }
}
