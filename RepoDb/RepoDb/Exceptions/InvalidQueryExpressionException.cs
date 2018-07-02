using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the query expression passed is not valid.
    /// </summary>
    public class InvalidQueryExpressionException : Exception
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Exceptions.InvalidQueryExpressionException</i> object.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public InvalidQueryExpressionException(string message)
            : base(message) { }
    }
}
