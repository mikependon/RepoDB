#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Shares file reading, validation, and tracing between the definition formats.
    /// </summary>
    internal static class SchemaDefinitionFile
    {
        #region Public Methods

        /// <summary>
        /// Reads and traces a definition without opening or querying the connection.
        /// </summary>
        /// <param name="connection">The connection that the definition is read for. It is not opened or queried.</param>
        /// <param name="filename">The name of the definition file.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="parse">The function that parses the content of the file into the definition document.</param>
        /// <param name="validate">Whether the definition is validated after it is read, with <c>ValidateDefinition</c> that throws the first error. The read of a file and the load of a file validate it.</param>
        /// <returns>The definition read from the file, or <c>null</c> if the trace cancelled the read.</returns>
        internal static SchemaDefinitionInfo Read(IDbConnection connection,
            string filename,
            string traceKey,
            ITrace trace,
            Func<string, JsonObject> parse,
            bool validate)
        {
            Validate(connection,
                filename);
            var log = new CancellableTraceLog(Guid.NewGuid(),
                traceKey,
                filename);
            trace?.BeforeExecution(log);
            if (IsCancelled(log))
                return null;
            var watch = Stopwatch.StartNew();
            SchemaDefinitionInfo result;
            try
            {
                using (var reader = Open(log.Statement))
                    result = Parse(reader.ReadToEnd(),
                        parse);
            }
            catch (DecoderFallbackException exception)
            {
                throw new InvalidDataException("The schema definition file contains invalid text encoding.",
                    exception);
            }

            if (validate)
                connection.ValidateDefinition(result,
                    true);
            trace?.AfterExecution(new ResultTraceLog<SchemaDefinitionInfo>(log.SessionId,
                traceKey,
                watch.Elapsed,
                result,
                log));
            return result;
        }

        /// <summary>
        /// Uses asynchronous file reads and trace callbacks, honoring cancellation before and during reading.
        /// </summary>
        /// <param name="connection">The connection that the definition is read for. It is not opened or queried.</param>
        /// <param name="filename">The name of the definition file.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="parse">The function that parses the content of the file into the definition document.</param>
        /// <param name="validate">Whether the definition is validated after it is read, with <c>ValidateDefinition</c> that throws the first error. The read of a file and the load of a file validate it.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the definition read from the file, or <c>null</c> if the trace cancelled the read.</returns>
        internal static async Task<SchemaDefinitionInfo> ReadAsync(IDbConnection connection,
            string filename,
            string traceKey,
            ITrace trace,
            Func<string, JsonObject> parse,
            bool validate,
            CancellationToken cancellationToken)
        {
            Validate(connection,
                filename);
            cancellationToken.ThrowIfCancellationRequested();
            var log = new CancellableTraceLog(Guid.NewGuid(),
                traceKey,
                filename);
            if (trace != null)
                await trace.BeforeExecutionAsync(log,
                    cancellationToken).ConfigureAwait(false);
            if (IsCancelled(log))
                return null;
            cancellationToken.ThrowIfCancellationRequested();
            var watch = Stopwatch.StartNew();
            var content = new StringBuilder();
            var buffer = new char[4096];
            try
            {
                using (var reader = Open(log.Statement,
                    true))
                {
                    int count;
                    while ((count = await reader.ReadAsync(buffer,
                        0,
                        buffer.Length).ConfigureAwait(false)) != 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        content.Append(buffer,
                            0,
                            count);
                    }
                }
            }
            catch (DecoderFallbackException exception)
            {
                throw new InvalidDataException("The schema definition file contains invalid text encoding.",
                    exception);
            }

            cancellationToken.ThrowIfCancellationRequested();
            var result = Parse(content.ToString(),
                parse);
            if (validate)
                await connection.ValidateDefinitionAsync(result,
                    true,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            if (trace != null)
                await trace.AfterExecutionAsync(new ResultTraceLog<SchemaDefinitionInfo>(log.SessionId,
                    traceKey,
                    watch.Elapsed,
                    result,
                    log),
                    cancellationToken).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks the common required arguments before invoking tracing or accessing the file.
        /// </summary>
        /// <param name="connection">The connection that the definition is read for.</param>
        /// <param name="filename">The name of the definition file.</param>
        private static void Validate(IDbConnection connection,
            string filename)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("A filename is required.",
                    nameof(filename));
        }

        /// <summary>
        /// Opens a strict UTF-8 file reader with optional asynchronous file access.
        /// </summary>
        /// <param name="filename">The name of the definition file.</param>
        /// <param name="asynchronous">Whether the file is opened for asynchronous access.</param>
        /// <returns>The reader of the file.</returns>
        private static StreamReader Open(string filename,
            bool asynchronous = false) =>
            new StreamReader(new FileStream(filename,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                4096,
                asynchronous ? FileOptions.Asynchronous | FileOptions.SequentialScan : FileOptions.SequentialScan),
                new UTF8Encoding(false,
                    true),
                true);

        /// <summary>
        /// Applies the existing trace cancellation contract.
        /// </summary>
        /// <param name="log">The trace log that the trace may have cancelled.</param>
        /// <returns><c>true</c> if the trace cancelled the read; otherwise, <c>false</c>.</returns>
        private static bool IsCancelled(CancellableTraceLog log) =>
            SchemaTraceCancellation.IsCancelled(log,
                "The definition read was cancelled by the trace.");

        /// <summary>
        /// Normalizes syntax errors while leaving file access exceptions intact. The definition that is read is not validated.
        /// </summary>
        /// <param name="content">The content of the definition file.</param>
        /// <param name="parse">The function that parses the content into the definition document.</param>
        /// <returns>The definition that is read from the document.</returns>
        private static SchemaDefinitionInfo Parse(string content,
            Func<string, JsonObject> parse)
        {
            try
            {
                return SchemaDefinitionReader.Read(parse(content));
            }
            catch (Exception exception) when (exception is JsonException || exception is XmlException || exception is DecoderFallbackException)
            {
                throw new InvalidDataException("The schema definition file is malformed.",
                    exception);
            }
        }

        #endregion
    }
}
