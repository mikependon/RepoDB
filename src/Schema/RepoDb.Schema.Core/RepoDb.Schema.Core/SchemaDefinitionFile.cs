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
        #region Methods

        /// <summary>
        /// Reads and traces a definition without opening or querying the connection.
        /// </summary>
        internal static SchemaDefinitionResult Read(IDbConnection connection, string filename,
            string traceKey, ITrace trace, Func<string, JsonObject> parse)
        {
            Validate(connection, filename);
            var log = new CancellableTraceLog(Guid.NewGuid(), traceKey, filename);
            trace?.BeforeExecution(log);
            if (Cancelled(log)) return null;
            var watch = Stopwatch.StartNew();
            SchemaDefinitionResult result;
            try
            {
                using (var reader = Open(log.Statement)) result = Parse(reader.ReadToEnd(), parse);
            }
            catch (DecoderFallbackException exception)
            { throw new InvalidDataException("The schema definition file contains invalid text encoding.", exception); }
            trace?.AfterExecution(new ResultTraceLog<SchemaDefinitionResult>(log.SessionId, traceKey, watch.Elapsed, result, log));
            return result;
        }

        /// <summary>
        /// Uses asynchronous file reads and trace callbacks, honoring cancellation before and during reading.
        /// </summary>
        internal static async Task<SchemaDefinitionResult> ReadAsync(IDbConnection connection, string filename,
            string traceKey, ITrace trace, Func<string, JsonObject> parse, CancellationToken cancellationToken)
        {
            Validate(connection, filename);
            cancellationToken.ThrowIfCancellationRequested();
            var log = new CancellableTraceLog(Guid.NewGuid(), traceKey, filename);
            if (trace != null) await trace.BeforeExecutionAsync(log, cancellationToken).ConfigureAwait(false);
            if (Cancelled(log)) return null;
            cancellationToken.ThrowIfCancellationRequested();
            var watch = Stopwatch.StartNew();
            var content = new StringBuilder();
            var buffer = new char[4096];
            try
            {
                using (var reader = Open(log.Statement, true))
                {
                    int count;
                    while ((count = await reader.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) != 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        content.Append(buffer, 0, count);
                    }
                }
            }
            catch (DecoderFallbackException exception)
            { throw new InvalidDataException("The schema definition file contains invalid text encoding.", exception); }
            cancellationToken.ThrowIfCancellationRequested();
            var result = Parse(content.ToString(), parse);
            cancellationToken.ThrowIfCancellationRequested();
            if (trace != null)
                await trace.AfterExecutionAsync(new ResultTraceLog<SchemaDefinitionResult>(log.SessionId, traceKey, watch.Elapsed, result, log), cancellationToken).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Checks the common required arguments before invoking tracing or accessing the file.
        /// </summary>
        private static void Validate(IDbConnection connection, string filename)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentException("A filename is required.", nameof(filename));
        }

        /// <summary>
        /// Opens a strict UTF-8 file reader with optional asynchronous file access.
        /// </summary>
        private static StreamReader Open(string filename, bool asynchronous = false) =>
            new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
                asynchronous ? FileOptions.Asynchronous | FileOptions.SequentialScan : FileOptions.SequentialScan),
                new UTF8Encoding(false, true), true);

        /// <summary>
        /// Applies the existing trace cancellation contract.
        /// </summary>
        private static bool Cancelled(CancellableTraceLog log)
        {
            if (log.IsCancelled && log.IsThrowException) throw new CancelledExecutionException("The definition read was cancelled by the trace.");
            return log.IsCancelled;
        }

        /// <summary>
        /// Normalizes syntax errors while leaving file access exceptions intact.
        /// </summary>
        private static SchemaDefinitionResult Parse(string content, Func<string, JsonObject> parse)
        {
            try { return SchemaDefinitionReader.Read(parse(content)); }
            catch (Exception exception) when (exception is JsonException || exception is XmlException || exception is DecoderFallbackException)
            { throw new InvalidDataException("The schema definition file is malformed.", exception); }
        }

        #endregion
    }
}
