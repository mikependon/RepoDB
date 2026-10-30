#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests.Setup
{
    /// <summary>
    /// A trace that records the logs and the cancellation tokens that it receives, and that can be told to fail when it is told that an execution has completed
    /// or to run an action when an execution is about to start.
    /// </summary>
    public class RecordingTrace : ITrace
    {
        #region Properties

        /// <summary>
        /// Gets the logs that were received after an execution, in the order that they were received.
        /// </summary>
        public List<object> AfterLogs { get; } = new List<object>();

        /// <summary>
        /// Gets the cancellation tokens that were received after an execution (the asynchronous ones only).
        /// </summary>
        public List<CancellationToken> AfterTokens { get; } = new List<CancellationToken>();

        /// <summary>
        /// Gets the logs that were received before an execution, in the order that they were received.
        /// </summary>
        public List<CancellableTraceLog> BeforeLogs { get; } = new List<CancellableTraceLog>();

        /// <summary>
        /// Gets or sets the action that is called when the trace is told that an execution is about to start.
        /// </summary>
        public Action OnBefore { get; set; }

        /// <summary>
        /// Gets or sets the exception that is thrown when the trace is told that an execution has completed, or <c>null</c> to not throw.
        /// </summary>
        public Exception ThrowOnAfter { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the only log that was received after an execution.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the execution.</typeparam>
        /// <returns>The log.</returns>
        public ResultTraceLog<TResult> GetAfterLog<TResult>() =>
            (ResultTraceLog<TResult>)AfterLogs.Single();

        /// <summary>
        /// Records the log before an execution.
        /// </summary>
        /// <param name="log">The log of the execution.</param>
        public void BeforeExecution(CancellableTraceLog log)
        {
            BeforeLogs.Add(log);
            OnBefore?.Invoke();
        }

        /// <summary>
        /// Records the log after an execution, and throws if requested.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="log">The log of the execution.</param>
        public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
        {
            AfterLogs.Add(log);
            if (ThrowOnAfter != null)
            {
                throw ThrowOnAfter;
            }
        }

        /// <summary>
        /// Records the log before an execution.
        /// </summary>
        /// <param name="log">The log of the execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task BeforeExecutionAsync(CancellableTraceLog log,
            CancellationToken cancellationToken = default)
        {
            BeforeExecution(log);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Records the log and the token after an execution, and fails if requested.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="log">The log of the execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log,
            CancellationToken cancellationToken = default)
        {
            AfterLogs.Add(log);
            AfterTokens.Add(cancellationToken);
            return ThrowOnAfter == null ? Task.CompletedTask : Task.FromException(ThrowOnAfter);
        }

        #endregion
    }
}
