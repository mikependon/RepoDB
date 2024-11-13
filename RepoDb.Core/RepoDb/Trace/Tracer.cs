#nullable enable
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Exceptions;
using RepoDb.Interfaces;

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
        public static TraceResult? InvokeBeforeExecution(string? key,
            ITrace? trace,
            DbCommand command)
        {
            if (string.IsNullOrEmpty(key) || trace == null)
            {
                return null;
            }

            var result = TraceResult.Create(key, command);

            trace.BeforeExecution(result.CancellableTraceLog);

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
        public static async Task<TraceResult?> InvokeBeforeExecutionAsync(string? key,
            ITrace? trace,
            DbCommand command,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(key) || trace == null)
            {
                return null;
            }

            var result = TraceResult.Create(key, command);

            await trace.BeforeExecutionAsync(result.CancellableTraceLog, cancellationToken);

            ValidateCancellation(key, result.CancellableTraceLog);

            return result;
        }

        #endregion

        #region AfterExecution

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result"></param>
        /// <param name="trace"></param>
        /// <param name="value"></param>
        public static void InvokeAfterExecution<TResult>(TraceResult? result,
            ITrace? trace,
            TResult value)
        {
            if (trace == null || result == null)
            {
                return;
            }

            if (result.CancellableTraceLog.IsCancelled == true)
            {
                return;
            }

            var log = new ResultTraceLog<TResult>(result.SessionId,
                result.CancellableTraceLog.Key,
                DateTime.UtcNow.TimeOfDay.Subtract(result.StartTime.TimeOfDay),
                value,
                result.CancellableTraceLog);

            trace.AfterExecution(log);
        }

        #endregion

        #region AfterExecution

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result"></param>
        /// <param name="trace"></param>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task InvokeAfterExecutionAsync<TResult>(TraceResult? result,
            ITrace? trace,
            TResult value,
            CancellationToken cancellationToken = default)
        {
            if (trace == null || result == null)
            {
                return;
            }

            var log = new ResultTraceLog<TResult>(result.SessionId,
                result.CancellableTraceLog.Key,
                DateTime.UtcNow.TimeOfDay.Subtract(result.StartTime.TimeOfDay),
                value,
                result.CancellableTraceLog);

            await trace.AfterExecutionAsync(log, cancellationToken);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="log"></param>
        /// <exception cref="CancelledExecutionException"></exception>
        private static void ValidateCancellation(string? key,
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
