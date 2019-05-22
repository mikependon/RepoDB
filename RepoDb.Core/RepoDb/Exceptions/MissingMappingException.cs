using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the mapping is missing.
    /// </summary>
    public class MissingMappingException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="MissingMappingException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public MissingMappingException(string message)
            : base(message) { }
    }
}
