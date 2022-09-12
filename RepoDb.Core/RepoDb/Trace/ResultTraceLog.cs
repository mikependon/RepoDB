using System;
using System.Collections.Generic;
using System.Data;

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
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        /// <param name="executionTime"></param>
        /// <param name="result"></param>
        protected internal ResultTraceLog(Guid sessionId,
            string key,
            string statement,
            IEnumerable<IDbDataParameter> parameters = null,
            TimeSpan? executionTime = null,
            TResult result = default)
            : base(sessionId, key, statement, parameters, executionTime)
        {
            Result = result;
        }

        #region Properties

        /// <summary>
        /// Gets the actual result of the operation.
        /// </summary>
        public TResult Result { get; }

        #endregion
    }
}
