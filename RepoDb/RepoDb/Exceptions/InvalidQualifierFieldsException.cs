using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the qualifier <see cref="Field"/> objects passed in the operation are not valid.
    /// </summary>
    public class InvalidQualifierFieldsException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidQualifierFieldsException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public InvalidQualifierFieldsException(string message)
            : base(message) { }
    }
}
