using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the type is not valid.
    /// </summary>
    public class InvalidTypeException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidTypeException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public InvalidTypeException(string message)
            : base(message) { }
    }
}
