using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the operation extraction of the <see cref="System.Data.Common.DbDataReader"/> into data entity object 
    /// does not matched at least one of the field from the result set.
    /// </summary>
    public class MissingFieldsException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="MissingFieldsException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public MissingFieldsException(string message)
            : base(message) { }
    }
}
