using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the duplicate type mapping is found.
    /// </summary>
    public class DuplicateTypeMapException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuplicateTypeMapException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public DuplicateTypeMapException(string message)
            : base(message) { }
    }
}
