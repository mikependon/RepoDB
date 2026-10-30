#region Copyright Attributions

// Copyright (c) 2022 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Tracer
    {
        #region BeforeExecution

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="trace"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static TraceResult InvokeBeforeExecution(string key,
            ITrace trace,
            DbCommand command)
        {
            if (string.IsNullOrEmpty(key) || (trace == null && !GlobalConfiguration.Options.UseRegisteredGlobalTraces))
            {
                return null;
            }

            var result = TraceResult.Create(key, command);

            trace?.BeforeExecution(result.CancellableTraceLog);

            if (GlobalConfiguration.Options.UseRegisteredGlobalTraces)
            {
                foreach (var globalTrace in GlobalTraceRegistration.GetTracers())
                {
                    globalTrace.BeforeExecution(result.CancellableTraceLog);
                }
            }

            ValidateCancellation(key, result.CancellableTraceLog);
            return result;
        }

        #endregion

        #region BeforeExecutionAsync

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="trace"></param>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<TraceResult> InvokeBeforeExecutionAsync(string key,
            ITrace trace,
            DbCommand command,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(key) || (trace == null && !GlobalConfiguration.Options.UseRegisteredGlobalTraces))
            {
                return null;
            }

            var result = TraceResult.Create(key, command);

            if (trace != null)
            {
                await trace.BeforeExecutionAsync(result.CancellableTraceLog, cancellationToken).ConfigureAwait(false);
            }

            if (GlobalConfiguration.Options.UseRegisteredGlobalTraces)
            {
                foreach (var globalTrace in GlobalTraceRegistration.GetTracers())
                {
                    await globalTrace.BeforeExecutionAsync(result.CancellableTraceLog, cancellationToken).ConfigureAwait(false);
                }
            }

            ValidateCancellation(key, result.CancellableTraceLog);

            return result;
        }

        #endregion

        #region AfterExecution

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

            var globalTraces = GlobalTraceRegistration.GetTracers();
            if (trace == null &&
                (
!GlobalConfiguration.Options.UseRegisteredGlobalTraces ||
                    globalTraces.Count == 0)
                )
            {
                return;
            }

            var log = new ResultTraceLog<TResult>(result.SessionId,
                result.CancellableTraceLog.Key,
                result.Elapsed,
                value,
                result.CancellableTraceLog,
                error);

            NotifyAfterExecution(trace, log);

            foreach (var globalTrace in globalTraces)
            {
                NotifyAfterExecution(globalTrace, log);
            }
        }

        #endregion

        #region AfterExecutionAsync

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

            var globalTraces = GlobalTraceRegistration.GetTracers();
            if (trace == null &&
                (
!GlobalConfiguration.Options.UseRegisteredGlobalTraces ||
                    globalTraces.Count == 0)
                )
            {
                return;
            }

            var log = new ResultTraceLog<TResult>(result.SessionId,
                result.CancellableTraceLog.Key,
                result.Elapsed,
                value,
                result.CancellableTraceLog,
                error);

            await NotifyAfterExecutionAsync(trace, log, cancellationToken).ConfigureAwait(false);

            foreach (var globalTrace in globalTraces)
            {
                await NotifyAfterExecutionAsync(globalTrace, log, cancellationToken).ConfigureAwait(false);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Tells a trace that the execution has completed. If the execution has failed, a trace that fails as well is ignored,
        /// so it can never replace the error that is reported to the caller.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="trace">The trace to be told, or <c>null</c>.</param>
        /// <param name="log">The log of the completed execution.</param>
        private static void NotifyAfterExecution<TResult>(ITrace trace,
            ResultTraceLog<TResult> log)
        {
            if (trace == null)
            {
                return;
            }

            try
            {
                trace.AfterExecution(log);
            }
            catch (Exception) when (log.Error != null)
            {
                // The error of the execution is more important than the error of the trace.
            }
        }

        /// <summary>
        /// Tells a trace in an asynchronous way that the execution has completed. If the execution has failed, a trace that fails as well is ignored,
        /// so it can never replace the error that is reported to the caller.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="trace">The trace to be told, or <c>null</c>.</param>
        /// <param name="log">The log of the completed execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to be given to the trace.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private static async Task NotifyAfterExecutionAsync<TResult>(ITrace trace,
            ResultTraceLog<TResult> log,
            CancellationToken cancellationToken)
        {
            if (trace == null)
            {
                return;
            }

            try
            {
                await trace.AfterExecutionAsync(log, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception) when (log.Error != null)
            {
                // The error of the execution is more important than the error of the trace.
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="log"></param>
        /// <exception cref="CancelledExecutionException"></exception>
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
