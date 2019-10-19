using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the parameter is not valid.
    /// </summary>
    public class InvalidParameterException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidParameterException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public InvalidParameterException(string message)
            : base(message) { }
    }
}
