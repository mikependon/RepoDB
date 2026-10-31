#region Copyright Attributions

// Copyright (c) 2022 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Diagnostics;

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
            string key,
            TimeSpan? executionTime = null,
            TResult result = default,
            CancellableTraceLog beforeExecutionLog = null)
            : this(sessionId, key, executionTime, result, beforeExecutionLog, null)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="ResultTraceLog{TResult}"/> class for an execution, which may have failed.
        /// </summary>
        /// <param name="sessionId">The session identifier used by the current trace.</param>
        /// <param name="key">The actual tracing key used by the operation.</param>
        /// <param name="executionTime">The actual length of the actual execution.</param>
        /// <param name="result">The actual result of the actual execution.</param>
        /// <param name="beforeExecutionLog">The log that was written before the execution. The context of the execution is taken from it.</param>
        /// <param name="error">The exception that made the execution fail, or <c>null</c> if it succeeded.</param>
        internal ResultTraceLog(Guid sessionId,
            string key,
            TimeSpan? executionTime,
            TResult result,
            CancellableTraceLog beforeExecutionLog,
            Exception error)
            : base(sessionId, key, beforeExecutionLog?.Context, GetLevel(executionTime, error))
        {
            Result = result;
            BeforeExecutionLog = beforeExecutionLog;
            Error = error;
            if (executionTime != null)
            {
                ExecutionTime = executionTime.Value;
            }
        }

        private static TraceLevel GetLevel(TimeSpan? executionTime, Exception error)
        {
            if (error != null)
            {
                return TraceLevel.Error;
            }
            var threshold = GlobalConfiguration.Options.TraceWarningThreshold;
            return threshold.HasValue && executionTime.HasValue && executionTime.Value >= threshold.Value
                ? TraceLevel.Warning
                : TraceLevel.Info;
        }

        #region Properties

        /// <summary>
        /// Gets the exception that made the execution fail, or <c>null</c> if it succeeded. The same exception is thrown back to the caller of the operation.
        /// When it is set, the <see cref="Result"/> is not a result of the execution: it is the default value.
        /// </summary>
        public Exception Error { get; }

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
