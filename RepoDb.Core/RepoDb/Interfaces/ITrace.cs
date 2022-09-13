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
        /// A method that is being raised after the actual execution of the operation.
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
        /// A method that is being raised after the actual execution of the operation in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="log">The trace log object referenced by the execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
