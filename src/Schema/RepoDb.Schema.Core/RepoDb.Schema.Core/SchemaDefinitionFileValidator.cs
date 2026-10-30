#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Interfaces;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Validates a definition file: it reads the file without validating it, and validates the definition that was read.
    /// The content problems are reported instead of being thrown.
    /// </summary>
    internal static class SchemaDefinitionFileValidator
    {
        #region Internal Methods

        /// <summary>
        /// Reads the definition file and reports whether its content is valid. Argument and file access errors are still thrown.
        /// </summary>
        /// <param name="connection">The connection that the extension is invoked on. It is not opened or queried.</param>
        /// <param name="filename">The name of the definition file.</param>
        /// <param name="throwOnError">Whether the first problem is thrown, instead of being reported in the outcome. It includes a file that cannot be read as a definition.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="parse">The function that parses the content of the file into the definition document.</param>
        /// <returns>The outcome of the validation, or <c>null</c> if the trace cancelled the read.</returns>
        internal static ValidateDefinitionResult Validate(IDbConnection connection,
            string filename,
            bool throwOnError,
            string traceKey,
            ITrace trace,
            Func<string, JsonObject> parse)
        {
            try
            {
                var definition = SchemaDefinitionFile.Read(connection,
                    filename,
                    traceKey,
                    trace,
                    parse,
                    false);
                return definition == null ? null : connection.ValidateDefinition(definition,
                    throwOnError);
            }
            catch (InvalidDataException exception) when (!throwOnError)
            {
                return CreateInvalidResult(exception);
            }
        }

        /// <summary>
        /// Asynchronously reads the definition file and reports whether its content is valid. Argument and file access errors are still thrown.
        /// </summary>
        /// <param name="connection">The connection that the extension is invoked on. It is not opened or queried.</param>
        /// <param name="filename">The name of the definition file.</param>
        /// <param name="throwOnError">Whether the first problem is thrown, instead of being reported in the outcome. It includes a file that cannot be read as a definition.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="parse">The function that parses the content of the file into the definition document.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the outcome of the validation, or <c>null</c> if the trace cancelled the read.</returns>
        internal static async Task<ValidateDefinitionResult> ValidateAsync(IDbConnection connection,
            string filename,
            bool throwOnError,
            string traceKey,
            ITrace trace,
            Func<string, JsonObject> parse,
            CancellationToken cancellationToken)
        {
            try
            {
                var definition = await SchemaDefinitionFile.ReadAsync(connection,
                    filename,
                    traceKey,
                    trace,
                    parse,
                    false,
                    cancellationToken).ConfigureAwait(false);
                return definition == null ? null : await connection.ValidateDefinitionAsync(definition,
                    throwOnError,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (InvalidDataException exception) when (!throwOnError)
            {
                return CreateInvalidResult(exception);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the outcome of a file that could not be read as a definition.
        /// </summary>
        /// <param name="exception">The exception that describes the problem of the file.</param>
        /// <returns>The invalid outcome, with the messages of the exception and of its underlying causes.</returns>
        private static ValidateDefinitionResult CreateInvalidResult(Exception exception) =>
            new ValidateDefinitionResult { Errors = GetMessages(exception) };

        /// <summary>
        /// Gets the message of the exception, followed by the messages of its underlying causes.
        /// </summary>
        /// <param name="exception">The exception to be described.</param>
        /// <returns>The messages, from the most general to the most specific.</returns>
        private static IList<string> GetMessages(Exception exception)
        {
            var messages = new List<string>();
            for (var current = exception; current != null; current = current.InnerException)
            {
                messages.Add(current.Message);
            }
            return messages;
        }

        #endregion
    }
}
