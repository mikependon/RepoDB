#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using System.IO;
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
        /// Validates a RepoDB YAML definition file. The file is valid if it can be read and passes the strict checks: its keys, indexes and foreign keys must be consistent with its columns and tables.
        /// The connection is not opened or queried. Problems in the content are reported in the result instead of being thrown, unless <paramref name="throwOnError"/> is <c>true</c>, and content that some databases allow is reported as warnings.
        /// YAML files must use the representation emitted by RepoDB's save operation.
        /// </summary>
        /// <param name="connection">The connection that the extension is invoked on; no provider registration is required.</param>
        /// <param name="filename">The existing definition filename.</param>
        /// <param name="throwOnError">Whether the first problem is thrown to the caller as an <see cref="InvalidDataException"/>, instead of being reported in the result.
        /// It includes a file that cannot be read as a definition (it is malformed, has an unsupported version or an invalid text encoding). The default is <c>false</c>.
        /// Warnings are never thrown, and a file that does not exist is always thrown.</param>
        /// <param name="traceKey">The tracking key for the file read.</param>
        /// <param name="trace">The trace callbacks. The before log statement contains the filename and can be changed or cancelled.</param>
        /// <returns>The outcome of the validation, or null if the trace cancels without requesting an exception.</returns>
        /// <exception cref="InvalidDataException"><paramref name="throwOnError"/> is <c>true</c> and the file is not a valid definition. The message is the first problem.</exception>
        public static ValidateDefinitionResult ValidateDefinitionFromYaml(this IDbConnection connection,
            string filename,
            bool throwOnError = false,
            string traceKey = SchemaTraceKeys.ValidateDefinitionFromYaml,
            ITrace trace = null) =>
            SchemaDefinitionFileValidator.Validate(connection,
                filename,
                throwOnError,
                traceKey,
                trace,
                SchemaDefinitionYamlReader.Parse);

        /// <summary>
        /// Asynchronously validates a RepoDB YAML definition file. The file is valid if it can be read and passes the strict checks: its keys, indexes and foreign keys must be consistent with its columns and tables.
        /// The connection is not opened or queried. Problems in the content are reported in the result instead of being thrown, unless <paramref name="throwOnError"/> is <c>true</c>, and content that some databases allow is reported as warnings.
        /// YAML files must use the representation emitted by RepoDB's save operation.
        /// </summary>
        /// <param name="connection">The connection that the extension is invoked on; no provider registration is required.</param>
        /// <param name="filename">The existing definition filename.</param>
        /// <param name="throwOnError">Whether the first problem is thrown to the caller as an <see cref="InvalidDataException"/>, instead of being reported in the result.
        /// It includes a file that cannot be read as a definition (it is malformed, has an unsupported version or an invalid text encoding). The default is <c>false</c>.
        /// Warnings are never thrown, and a file that does not exist is always thrown.</param>
        /// <param name="traceKey">The tracking key for the file read.</param>
        /// <param name="trace">The asynchronous trace callbacks.</param>
        /// <param name="cancellationToken">The cancellation token for reading the file.</param>
        /// <returns>The outcome of the validation, or null if the trace cancels without requesting an exception.</returns>
        /// <exception cref="InvalidDataException"><paramref name="throwOnError"/> is <c>true</c> and the file is not a valid definition. The message is the first problem.</exception>
        public static Task<ValidateDefinitionResult> ValidateDefinitionFromYamlAsync(this IDbConnection connection,
            string filename,
            bool throwOnError = false,
            string traceKey = SchemaTraceKeys.ValidateDefinitionFromYaml,
            ITrace trace = null,
            CancellationToken cancellationToken = default) =>
            SchemaDefinitionFileValidator.ValidateAsync(connection,
                filename,
                throwOnError,
                traceKey,
                trace,
                SchemaDefinitionYamlReader.Parse,
                cancellationToken);

        #endregion
    }
}
