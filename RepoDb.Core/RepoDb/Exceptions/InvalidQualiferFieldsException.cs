using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the qualifier fields passed in the operation are not valid.
    /// </summary>
    public class InvalidQualiferFieldsException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidQualiferFieldsException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public InvalidQualiferFieldsException(string message)
            : base(message) { }
    }
}
