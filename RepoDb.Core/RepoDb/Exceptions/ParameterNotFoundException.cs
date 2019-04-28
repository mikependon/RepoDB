using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the parameter object is not found.
    /// </summary>
    public class ParameterNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="ParameterNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ParameterNotFoundException(string message)
            : base(message) { }
    }
}
