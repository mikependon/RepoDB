using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// A class that holds the information of the tracing operations.
    /// </summary>
    [Obsolete("Use the 'TraceLog<TResult>' class instead.")]
    public class TraceLog
    {
        /// <summary>
        /// Creates an instance of <see cref="TraceLog"/> class.
        /// </summary>
        /// <param name="sessionId">The session identifier for the current trace object.</param>
        /// <param name="statement">The command text in used.</param>
        /// <param name="parameter">The parameters passed.</param>
        /// <param name="result">The actual result if present.</param>
        /// <param name="executionTime">The elapsed time of the execution.</param>
        protected internal TraceLog(Guid sessionId,
            string statement,
            object parameter,
            object result,
            TimeSpan? executionTime)
        {
            SessionId = sessionId;
            Statement = statement;
            Parameter = parameter;
            Result = result;
            if (executionTime != null)
            {
                ExecutionTime = executionTime.Value;
            }
        }

        #region Properties

        /// <summary>
        /// Gets the session identifier of the current trace.
        /// </summary>
        public Guid SessionId { get; }

        /// <summary>
        /// Gets the actual result of the actual operation execution.
        /// </summary>
        public object Result { get; }

        /// <summary>
        /// Gets or sets the parameter object used on the actual operation execution.
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// Gets or sets the SQL Statement used on the actual operation execution.
        /// </summary>
        public string Statement { get; set; }

        /// <summary>
        /// Gets the actual length of the operation execution.
        /// </summary>
        public TimeSpan ExecutionTime { get; }

        #endregion
    }

    /// <summary>
    /// A class that holds the information of the tracing operations.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class TraceLog<TResult>
    {
        /// <summary>
        /// Creates an instance of <see cref="TraceLog"/> class.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="result"></param>
        /// <param name="executionTime"></param>
        protected internal TraceLog(Guid sessionId,
            TResult result = default,
            TimeSpan? executionTime = null)
        {
            SessionId = sessionId;
            Result = result;
            if (executionTime != null)
            {
                ExecutionTime = executionTime.Value;
            }
        }

        #region Properties

        /// <summary>
        /// Gets the session identifier used by the current trace.
        /// </summary>
        public Guid SessionId { get; }

        /// <summary>
        /// Gets the actual result of the operation.
        /// </summary>
        public TResult Result { get; }

        /// <summary>
        /// Gets the actual length of the operation execution.
        /// </summary>
        public TimeSpan ExecutionTime { get; }

        #endregion
    }
}
