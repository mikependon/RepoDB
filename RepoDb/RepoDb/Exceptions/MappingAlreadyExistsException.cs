using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the mapping is being added to the existing one without overriding it.
    /// </summary>
    public class MappingAlreadyExistsException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="MappingAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public MappingAlreadyExistsException(string message)
            : base(message) { }
    }
}
