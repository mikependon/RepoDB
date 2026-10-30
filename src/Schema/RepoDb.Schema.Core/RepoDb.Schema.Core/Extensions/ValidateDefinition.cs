#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Diagnostics;
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
        #region Private Variables

        /// <summary>
        /// The message of the exception that is thrown when the trace cancels the validation and asks for an exception.
        /// </summary>
        private const string CancelledMessage = "The definition validation was cancelled by the trace.";

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates a definition that is already loaded, in memory. The definition is valid if its structure is sound (the tables, columns, keys, indexes and foreign keys
        /// are consistent with each other), and it passes the strict checks. This is the validation that the read and the load of a definition file apply as well.
        /// The connection is not opened or queried. Problems are reported in the result instead of being thrown, unless <paramref name="throwOnError"/> is <c>true</c>,
        /// and content that some databases allow is reported as warnings.
        /// </summary>
        /// <param name="connection">The connection that the extension is invoked on; no provider registration is required.</param>
        /// <param name="definition">The definition to be validated.</param>
        /// <param name="throwOnError">Whether the first error that the validation finds is thrown to the caller, instead of being reported in the result. The default is <c>false</c>.
        /// Warnings are never thrown. The exception is thrown before the trace is told that the validation completed.</param>
        /// <param name="traceKey">The tracking key for the validation.</param>
        /// <param name="trace">The trace callbacks. The before log statement describes the definition, and can be changed or cancelled.</param>
        /// <returns>The outcome of the validation, or null if the trace cancels without requesting an exception.</returns>
        /// <exception cref="ArgumentNullException">The connection or the definition is <c>null</c>.</exception>
        /// <exception cref="InvalidDataException"><paramref name="throwOnError"/> is <c>true</c> and the definition has errors. The message is the first error.</exception>
        public static ValidateDefinitionResult ValidateDefinition(this IDbConnection connection,
            SchemaDefinitionInfo definition,
            bool throwOnError = false,
            string traceKey = SchemaTraceKeys.ValidateDefinition,
            ITrace trace = null)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }
            return trace == null ? SchemaDefinitionValidator.Validate(definition,
                throwOnError) : ValidateWithTrace(definition,
                throwOnError,
                traceKey,
                trace);
        }

        /// <summary>
        /// Asynchronously validates a definition that is already loaded, in memory. The definition is valid if its structure is sound (the tables, columns, keys, indexes
        /// and foreign keys are consistent with each other), and it passes the strict checks. This is the validation that the read and the load of a definition file apply as well.
        /// The connection is not opened or queried. Problems are reported in the result instead of being thrown, unless <paramref name="throwOnError"/> is <c>true</c>,
        /// and content that some databases allow is reported as warnings.
        /// </summary>
        /// <param name="connection">The connection that the extension is invoked on; no provider registration is required.</param>
        /// <param name="definition">The definition to be validated.</param>
        /// <param name="throwOnError">Whether the first error that the validation finds is thrown to the caller, instead of being reported in the result. The default is <c>false</c>.
        /// Warnings are never thrown. The exception is thrown before the trace is told that the validation completed.</param>
        /// <param name="traceKey">The tracking key for the validation.</param>
        /// <param name="trace">The asynchronous trace callbacks.</param>
        /// <param name="cancellationToken">The cancellation token for the validation.</param>
        /// <returns>The outcome of the validation, or null if the trace cancels without requesting an exception.</returns>
        /// <exception cref="ArgumentNullException">The connection or the definition is <c>null</c>.</exception>
        /// <exception cref="InvalidDataException"><paramref name="throwOnError"/> is <c>true</c> and the definition has errors. The message is the first error.</exception>
        public static async Task<ValidateDefinitionResult> ValidateDefinitionAsync(this IDbConnection connection,
            SchemaDefinitionInfo definition,
            bool throwOnError = false,
            string traceKey = SchemaTraceKeys.ValidateDefinition,
            ITrace trace = null,
            CancellationToken cancellationToken = default)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }
            cancellationToken.ThrowIfCancellationRequested();
            return trace == null ? SchemaDefinitionValidator.Validate(definition,
                throwOnError) : await ValidateWithTraceAsync(definition,
                throwOnError,
                traceKey,
                trace,
                cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates the definition between the callbacks of the trace.
        /// </summary>
        /// <param name="definition">The definition to be validated.</param>
        /// <param name="throwOnError">Whether the first error is thrown, before the trace is told that the validation completed.</param>
        /// <param name="traceKey">The tracking key for the validation.</param>
        /// <param name="trace">The trace callbacks.</param>
        /// <returns>The outcome of the validation, or <c>null</c> if the trace cancelled it.</returns>
        private static ValidateDefinitionResult ValidateWithTrace(SchemaDefinitionInfo definition,
            bool throwOnError,
            string traceKey,
            ITrace trace)
        {
            var log = new CancellableTraceLog(Guid.NewGuid(),
                traceKey,
                SchemaDefinitionValidator.Describe(definition));
            trace.BeforeExecution(log);
            if (SchemaTraceCancellation.IsCancelled(log,
                CancelledMessage))
            {
                return null;
            }
            var watch = Stopwatch.StartNew();
            var result = SchemaDefinitionValidator.Validate(definition,
                throwOnError);
            trace.AfterExecution(new ResultTraceLog<ValidateDefinitionResult>(log.SessionId,
                traceKey,
                watch.Elapsed,
                result,
                log));
            return result;
        }

        /// <summary>
        /// Validates the definition between the asynchronous callbacks of the trace.
        /// </summary>
        /// <param name="definition">The definition to be validated.</param>
        /// <param name="throwOnError">Whether the first error is thrown, before the trace is told that the validation completed.</param>
        /// <param name="traceKey">The tracking key for the validation.</param>
        /// <param name="trace">The asynchronous trace callbacks.</param>
        /// <param name="cancellationToken">The cancellation token for the validation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the outcome of the validation, or <c>null</c> if the trace cancelled it.</returns>
        private static async Task<ValidateDefinitionResult> ValidateWithTraceAsync(SchemaDefinitionInfo definition,
            bool throwOnError,
            string traceKey,
            ITrace trace,
            CancellationToken cancellationToken)
        {
            var log = new CancellableTraceLog(Guid.NewGuid(),
                traceKey,
                SchemaDefinitionValidator.Describe(definition));
            await trace.BeforeExecutionAsync(log,
                cancellationToken).ConfigureAwait(false);
            if (SchemaTraceCancellation.IsCancelled(log,
                CancelledMessage))
            {
                return null;
            }
            cancellationToken.ThrowIfCancellationRequested();
            var watch = Stopwatch.StartNew();
            var result = SchemaDefinitionValidator.Validate(definition,
                throwOnError);
            await trace.AfterExecutionAsync(new ResultTraceLog<ValidateDefinitionResult>(log.SessionId,
                traceKey,
                watch.Elapsed,
                result,
                log),
                cancellationToken).ConfigureAwait(false);
            return result;
        }

        #endregion
    }
}
