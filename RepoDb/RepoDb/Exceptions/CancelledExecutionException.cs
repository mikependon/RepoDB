using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the operation has been cancelled during the tracing.
    /// </summary>
    public class CancelledExecutionException : Exception
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Exceptions.CancelledExecutionException</i> object.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public CancelledExecutionException(string message)
            : base(message) { }
    }
}
