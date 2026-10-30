#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Interfaces;

namespace RepoDb.Schema.Core.UnitTests.CustomObjects
{
    /// <summary>
    /// A trace that records the tracking keys of the executions, and can cancel them. It only accepts the calls of the kind (sync or async) that it was created for.
    /// </summary>
    internal sealed class RecordingTrace : ITrace
    {
        #region Private Variables

        /// <summary>
        /// Whether the asynchronous callbacks are the expected ones.
        /// </summary>
        private readonly bool _asynchronous;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="RecordingTrace"/> class.
        /// </summary>
        /// <param name="asynchronous">Whether the trace is used by an asynchronous operation (<c>true</c>) or a synchronous one (<c>false</c>).</param>
        internal RecordingTrace(bool asynchronous)
        {
            _asynchronous = asynchronous;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of executions that the trace was told have completed.
        /// </summary>
        internal int AfterCount { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the executions are cancelled, without throwing an exception.
        /// </summary>
        internal bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a cancelled execution asks for an exception to be thrown.
        /// </summary>
        internal bool CancelWithException { get; set; }

        /// <summary>
        /// Gets the tracking keys of the executions, in the order that they happened.
        /// </summary>
        internal List<string> Keys { get; } = new List<string>();

        /// <summary>
        /// Gets the logs of the executions, in the order that they happened.
        /// </summary>
        internal List<CancellableTraceLog> Logs { get; } = new List<CancellableTraceLog>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Records the tracking key before a synchronous execution, and cancels it if requested.
        /// </summary>
        /// <param name="log">The log of the execution.</param>
        public void BeforeExecution(CancellableTraceLog log)
        {
            Assert.IsFalse(_asynchronous);
            Record(log);
        }

        /// <summary>
        /// Called after a synchronous execution.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="log">The log of the execution.</param>
        public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
        {
            Assert.IsFalse(_asynchronous);
            AfterCount++;
        }

        /// <summary>
        /// Records the tracking key before an asynchronous execution, and cancels it if requested.
        /// </summary>
        /// <param name="log">The log of the execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task BeforeExecutionAsync(CancellableTraceLog log,
            CancellationToken cancellationToken = default)
        {
            Assert.IsTrue(_asynchronous);
            Record(log);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called after an asynchronous execution.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="log">The log of the execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log,
            CancellationToken cancellationToken = default)
        {
            Assert.IsTrue(_asynchronous);
            AfterCount++;
            return Task.CompletedTask;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Records the tracking key of the execution, and cancels it if requested.
        /// </summary>
        /// <param name="log">The log of the execution.</param>
        private void Record(CancellableTraceLog log)
        {
            Keys.Add(log.Key);
            Logs.Add(log);
            if (Cancel || CancelWithException)
            {
                log.Cancel(CancelWithException);
            }
        }

        #endregion
    }
}
