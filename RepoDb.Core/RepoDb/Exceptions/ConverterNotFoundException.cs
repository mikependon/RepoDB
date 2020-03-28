using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the converter is not found.
    /// </summary>
    public class ConverterNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="ConverterNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ConverterNotFoundException(string message)
            : base(message) { }
    }
}
