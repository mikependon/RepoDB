using System;

namespace RepoDb
{
    /// <summary>
    /// A class that holds the result of the tracing.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class ResultTraceLog<TResult> : TraceLog
    {
        /// <summary>
        /// Creates an instance of <see cref="TraceLog"/> class.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="key"></param>
        /// <param name="executionTime"></param>
        /// <param name="result"></param>
        /// <param name="beforeExecutionLog"></param>
        protected internal ResultTraceLog(Guid sessionId,
            string? key,
            TimeSpan? executionTime = null,
            TResult result = default,
            CancellableTraceLog beforeExecutionLog = null)
            : base(sessionId, key)
        {
            Result = result;
            BeforeExecutionLog = beforeExecutionLog;
            if (executionTime != null)
            {
                ExecutionTime = executionTime.Value;
            }
        }

        #region Properties

        /// <summary>
        /// Gets the actual length of the actual execution.
        /// </summary>
        public TimeSpan ExecutionTime { get; }

        /// <summary>
        /// Gets the actual result of the actual execution.
        /// </summary>
        public TResult Result { get; }

        /// <summary>
        /// Gets the associated <see cref="CancellableTraceLog"/> object of the actual execution.
        /// </summary>
        public CancellableTraceLog BeforeExecutionLog { get; }

        #endregion
    }
}
