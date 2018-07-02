using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A cancellable tracing log object used in the tracing operations. This class holds the cancellable operations for all tracing logs.
    /// </summary>
    public sealed class CancellableTraceLog : TraceLog
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.CancellableTraceLog</i> object.
        /// </summary>
        /// <param name="method">A method that will instantiate this trace log object.</param>
        /// <param name="statement">A SQL statement that was used in the trace operation.</param>
        /// <param name="parameter">An object that was used as a parameter in the operation.</param>
        /// <param name="result">A result of the operation.</param>
        internal CancellableTraceLog(MethodBase method, string statement, object parameter, object result)
            : base(method, statement, parameter, result, null)
        {
        }

        /// <summary>
        /// Gets a value whether the operation is cancelled.
        /// </summary>
        public bool IsCancelled { get; private set; }

        /// <summary>
        /// Gets a value whether an exception will be thrown after the <i>Cancel</i> method was called.
        /// </summary>
        public bool IsThrowException { get; private set; }

        /// <summary>
        /// Cancel the current executing repository operation.
        /// </summary>
        /// <param name="throwException">If true, an exception will be thrown.</param>
        public void Cancel(bool throwException)
        {
            IsCancelled = true;
            IsThrowException = throwException;
        }
    }
}
