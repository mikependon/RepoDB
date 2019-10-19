using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the mapping is being added to the existing one without overriding it.
    /// </summary>
    public class MappingExistsException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="MappingExistsException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public MappingExistsException(string message)
            : base(message) { }
    }
}
