#region Copyright Attributions

// Copyright (c) 2022 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Dispatches paired execution notifications and preserves operation failures.
    /// </summary>
    internal static class Tracer
    {
        #region Public Methods

        /// <summary>
        /// Notifies observers before execution and reports failures before returning control.
        /// </summary>
        /// <param name="key">The operation name.</param>
        /// <param name="trace">The local execution observer.</param>
        /// <param name="command">The command being executed.</param>
        /// <returns>The correlated execution state, or null when tracing is disabled.</returns>
        public static TraceResult InvokeBeforeExecution(string key,
            ITrace trace,
            DbCommand command)
        {
            if (string.IsNullOrEmpty(key) || (trace == null && !GlobalConfiguration.Options.UseRegisteredGlobalTraces))
            {
                return null;
            }

            var result = TraceResult.Create(key,
                command);

            try
            {
                trace?.BeforeExecution(result.CancellableTraceLog);

                if (GlobalConfiguration.Options.UseRegisteredGlobalTraces)
                {
                    foreach (var globalTrace in GlobalTraceRegistration.GetTracers())
                    {
                        globalTrace.BeforeExecution(result.CancellableTraceLog);
                    }
                }

                ValidateCancellation(key,
                    result.CancellableTraceLog);
            }
            catch (Exception exception)
            {
                InvokeAfterExecution<object>(result,
                    trace,
                    null,
                    exception);
                throw;
            }
            return result;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Notifies observers before execution and reports failures before returning control.
        /// </summary>
        /// <param name="key">The operation name.</param>
        /// <param name="trace">The local execution observer.</param>
        /// <param name="command">The command being executed.</param>
        /// <param name="cancellationToken">The cancellation token for notification.</param>
        /// <returns>The correlated execution state, or null when tracing is disabled.</returns>
        public static async Task<TraceResult> InvokeBeforeExecutionAsync(string key,
            ITrace trace,
            DbCommand command,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(key) || (trace == null && !GlobalConfiguration.Options.UseRegisteredGlobalTraces))
            {
                return null;
            }

            var result = TraceResult.Create(key,
                command);

            try
            {
                if (trace != null)
                {
                    await trace.BeforeExecutionAsync(result.CancellableTraceLog,
                        cancellationToken).ConfigureAwait(false);
                }

                if (GlobalConfiguration.Options.UseRegisteredGlobalTraces)
                {
                    foreach (var globalTrace in GlobalTraceRegistration.GetTracers())
                    {
                        await globalTrace.BeforeExecutionAsync(result.CancellableTraceLog,
                            cancellationToken).ConfigureAwait(false);
                    }
                }

                ValidateCancellation(key,
                    result.CancellableTraceLog);
            }
            catch (Exception exception)
            {
                await InvokeAfterExecutionAsync<object>(result,
                    trace,
                    null,
                    CancellationToken.None,
                    exception).ConfigureAwait(false);
                throw;
            }

            return result;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tells the traces that the execution has completed, whether it succeeded or failed.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="result">The result of the tracing that was started before the execution.</param>
        /// <param name="trace">The trace object to be told.</param>
        /// <param name="value">The actual result of the execution (the default value if it failed).</param>
        /// <param name="error">The exception that made the execution fail, or <c>null</c> if it succeeded.</param>
        public static void InvokeAfterExecution<TResult>(TraceResult result,
            ITrace trace,
            TResult value,
            Exception error = null)
        {
            if (result == null)
            {
                return;
            }

            var globalTraces = GlobalConfiguration.Options.UseRegisteredGlobalTraces
                ? GlobalTraceRegistration.GetTracers()
                : new List<ITrace>();
            if (trace == null && globalTraces.Count == 0)
            {
                return;
            }

            var log = new ResultTraceLog<TResult>(result.SessionId,
                result.CancellableTraceLog.Key,
                result.Elapsed,
                error == null ? value : default,
                result.CancellableTraceLog,
                error);

            var observerError = NotifyAfterExecution(trace,
                log);

            foreach (var globalTrace in globalTraces)
            {
                var globalError = NotifyAfterExecution(globalTrace,
                    log);
                observerError ??= globalError;
            }

            RethrowObserverError(observerError);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tells the traces in an asynchronous way that the execution has completed, whether it succeeded or failed.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="result">The result of the tracing that was started before the execution.</param>
        /// <param name="trace">The trace object to be told.</param>
        /// <param name="value">The actual result of the execution (the default value if it failed).</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to be given to the traces.</param>
        /// <param name="error">The exception that made the execution fail, or <c>null</c> if it succeeded.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task InvokeAfterExecutionAsync<TResult>(TraceResult result,
            ITrace trace,
            TResult value,
            CancellationToken cancellationToken = default,
            Exception error = null)
        {
            if (result == null)
            {
                return;
            }

            var globalTraces = GlobalConfiguration.Options.UseRegisteredGlobalTraces
                ? GlobalTraceRegistration.GetTracers()
                : new List<ITrace>();
            if (trace == null && globalTraces.Count == 0)
            {
                return;
            }

            var log = new ResultTraceLog<TResult>(result.SessionId,
                result.CancellableTraceLog.Key,
                result.Elapsed,
                error == null ? value : default,
                result.CancellableTraceLog,
                error);

            var observerError = await NotifyAfterExecutionAsync(trace,
                log,
                error == null ? cancellationToken : CancellationToken.None).ConfigureAwait(false);

            foreach (var globalTrace in globalTraces)
            {
                var globalError = await NotifyAfterExecutionAsync(globalTrace,
                    log,
                    error == null ? cancellationToken : CancellationToken.None).ConfigureAwait(false);
                observerError ??= globalError;
            }

            RethrowObserverError(observerError);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Tells a trace that the execution has completed. If the execution has failed, a trace that fails as well is ignored,
        /// so it can never replace the error that is reported to the caller.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="trace">The trace to be told, or <c>null</c>.</param>
        /// <param name="log">The log of the completed execution.</param>
        /// <returns>The observer failure, or null when it succeeded or an execution error takes precedence.</returns>
        private static Exception NotifyAfterExecution<TResult>(ITrace trace,
            ResultTraceLog<TResult> log)
        {
            if (trace == null)
            {
                return null;
            }

            try
            {
                trace.AfterExecution(log);
            }
            catch (Exception exception)
            {
                return log.Error == null ? exception : null;
            }

            return null;
        }

        /// <summary>
        /// Tells a trace in an asynchronous way that the execution has completed. If the execution has failed, a trace that fails as well is ignored,
        /// so it can never replace the error that is reported to the caller.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="trace">The trace to be told, or <c>null</c>.</param>
        /// <param name="log">The log of the completed execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to be given to the trace.</param>
        /// <returns>The observer failure, or null when it succeeded or an execution error takes precedence.</returns>
        private static async Task<Exception> NotifyAfterExecutionAsync<TResult>(ITrace trace,
            ResultTraceLog<TResult> log,
            CancellationToken cancellationToken)
        {
            if (trace == null)
            {
                return null;
            }

            try
            {
                await trace.AfterExecutionAsync(log,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                return log.Error == null ? exception : null;
            }

            return null;
        }

        /// <summary>
        /// Rethrows the first observer failure after all observers have been notified.
        /// </summary>
        /// <param name="error">The observer error, or null when all notifications succeeded.</param>
        private static void RethrowObserverError(Exception error)
        {
            if (error != null)
            {
                ExceptionDispatchInfo.Capture(error).Throw();
            }
        }

        /// <summary>
        /// Throws when an observer requests cancellation with an exception.
        /// </summary>
        /// <param name="key">The operation name.</param>
        /// <param name="log">The cancellable command notification.</param>
        /// <exception cref="CancelledExecutionException">The observer requested throwing cancellation.</exception>
        private static void ValidateCancellation(string key,
            CancellableTraceLog log)
        {
            if (log?.IsCancelled == true && log?.IsThrowException == true)
            {
                throw new CancelledExecutionException($"The execution has been cancelled for {key}.");
            }
        }

        #endregion
    }
}
