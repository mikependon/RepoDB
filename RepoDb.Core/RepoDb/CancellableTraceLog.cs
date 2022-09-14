using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// A cancellable tracing log object that is used in the tracing operations. This class holds the cancellable operations for all tracing logs.
    /// </summary>
    public class CancellableTraceLog : TraceLog
    {
        /// <summary>
        /// Creates a new instance of <see cref="CancellableTraceLog"/> object.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="key"></param>
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        /// <param name="executionTime"></param>
        protected internal CancellableTraceLog(Guid sessionId,
            string key,
            string statement,
            IEnumerable<IDbDataParameter> parameters = null,
            TimeSpan? executionTime = null)
            : base(sessionId, key, statement, parameters, executionTime)
        { }

        #region Properties

        /// <summary>
        /// Gets a value whether the operation is cancelled.
        /// </summary>
        public bool IsCancelled { get; private set; }

        /// <summary>
        /// Gets a value whether an exception will be thrown after the <see cref="Cancel(bool)"/> method was called.
        /// </summary>
        public bool IsThrowException { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Cancel the current executing repository operation.
        /// </summary>
        /// <param name="throwException">If true, an exception will be thrown.</param>
        public void Cancel(bool throwException = true)
        {
            IsCancelled = true;
            IsThrowException = throwException;
        }

        #endregion
    }
}
