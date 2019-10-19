using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the qualifier <see cref="Field"/> objects passed in the operation are not valid.
    /// </summary>
    public class InvalidQualifiersException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidQualifiersException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public InvalidQualifiersException(string message)
            : base(message) { }
    }
}
