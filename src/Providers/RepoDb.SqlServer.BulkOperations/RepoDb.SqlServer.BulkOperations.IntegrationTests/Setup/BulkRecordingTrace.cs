#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Interfaces;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Setup
{
    /// <summary>
    /// Records correlated bulk and nested SQL notifications.
    /// </summary>
    public sealed class BulkRecordingTrace : ITrace
    {
        #region Properties

        /// <summary>
        /// Gets or sets whether the operation is silently cancelled.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets the completion count for each execution identifier.
        /// </summary>
        public IDictionary<Guid, int> Completions { get; } = new Dictionary<Guid, int>();

        /// <summary>
        /// Gets the original error for each execution identifier.
        /// </summary>
        public IDictionary<Guid, Exception> Errors { get; } = new Dictionary<Guid, Exception>();

        /// <summary>
        /// Gets the notifications received before execution.
        /// </summary>
        public IList<CancellableTraceLog> Starts { get; } = new List<CancellableTraceLog>();

        /// <summary>
        /// Gets or sets whether failure notifications throw a secondary observer error.
        /// </summary>
        public bool ThrowOnError { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Records the beginning of an execution and optionally cancels it.
        /// </summary>
        /// <param name="log">The command being started.</param>
        public void BeforeExecution(CancellableTraceLog log)
        {
            Starts.Add(log);
            if (Cancel)
            {
                log.Cancel(false);
            }
        }

        /// <summary>
        /// Records completion and optionally throws a secondary observer error.
        /// </summary>
        /// <typeparam name="TResult">The execution result type.</typeparam>
        /// <param name="log">The completed command.</param>
        public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
        {
            Completions.TryGetValue(log.SessionId,
                out var count);
            Completions[log.SessionId] = count + 1;
            Errors[log.SessionId] = log.Error;
            if (ThrowOnError && log.Error != null)
            {
                throw new NotSupportedException("The observer failed while recording the original error.");
            }
        }

        /// <summary>
        /// Records the beginning of an asynchronous execution.
        /// </summary>
        /// <param name="log">The command being started.</param>
        /// <param name="cancellationToken">The notification cancellation token.</param>
        /// <returns>A completed notification task.</returns>
        public Task BeforeExecutionAsync(CancellableTraceLog log,
            CancellationToken cancellationToken = default)
        {
            BeforeExecution(log);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Records completion of an asynchronous execution.
        /// </summary>
        /// <typeparam name="TResult">The execution result type.</typeparam>
        /// <param name="log">The completed command.</param>
        /// <param name="cancellationToken">The notification cancellation token.</param>
        /// <returns>A completed notification task.</returns>
        public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log,
            CancellationToken cancellationToken = default)
        {
            AfterExecution(log);
            return Task.CompletedTask;
        }

        #endregion
    }
}

