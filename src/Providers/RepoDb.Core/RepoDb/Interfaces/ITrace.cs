#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface that is used to mark a class to be a trace object. A trace object is being used to provide an auditing and debugging capability when executing the actual operation.
    /// The caller can modify the SQL statement and/or the parameters passed prior the actual execution, or even cancel the operation.
    /// </summary>
    public interface ITrace
    {
        #region Sync

        /// <summary>
        /// A method that is being raised before the actual execution of the operation.
        /// </summary>
        /// <param name="log">The cancellable trace log object referenced by the execution.</param>
        void BeforeExecution(CancellableTraceLog log);

        /// <summary>
        /// Reports completion after success, failure, or cancellation of a traced command.
        /// Execution failures are available in the log's Error property and are rethrown to the caller.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="log">The trace log object referenced by the execution.</param>
        void AfterExecution<TResult>(ResultTraceLog<TResult> log);

        #endregion

        #region Async

        /// <summary>
        /// A method that is being raised before the actual execution of the operation in an asynchronous way.
        /// </summary>
        /// <param name="log">The cancellable trace log object referenced by the execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        Task BeforeExecutionAsync(CancellableTraceLog log,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Reports completion asynchronously after success, failure, or cancellation of a traced command.
        /// Execution failures are available in the log's Error property and are rethrown to the caller.
        /// Failure notifications receive an uncancelled token so cancellation does not prevent recording the error.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="log">The trace log object referenced by the execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
