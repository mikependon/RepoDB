using System;

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
        /// <param name="sessionId">The session identifier for the current trace object.</param>
        /// <param name="statement">A SQL statement that was used in the trace operation.</param>
        /// <param name="parameter">An object that was used as a parameter in the operation.</param>
        /// <param name="result">A result of the operation.</param>
        protected internal CancellableTraceLog(Guid sessionId,
            string statement,
            object parameter,
            object result)
            : base(sessionId,
                  statement,
                  parameter,
                  result,
                  null)
        { }

        /// <summary>
        /// Gets a value whether the operation is cancelled.
        /// </summary>
        public bool IsCancelled { get; private set; }

        /// <summary>
        /// Gets a value whether an exception will be thrown after the <see cref="Cancel(bool)"/> method was called.
        /// </summary>
        public bool IsThrowException { get; private set; }

        /// <summary>
        /// Cancel the current executing repository operation.
        /// </summary>
        /// <param name="throwException">If true, an exception will be thrown.</param>
        public void Cancel(bool throwException = true)
        {
            IsCancelled = true;
            IsThrowException = throwException;
        }
    }
}
