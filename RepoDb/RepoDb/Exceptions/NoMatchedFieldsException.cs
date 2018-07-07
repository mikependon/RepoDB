using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the operation extraction of the <i>System.Data.Common.DbDataReader</i> into <i>DataEntity</i> object 
    /// does not matched atleast one of the field from the result set.
    /// </summary>
    public class NoMatchedFieldsException : Exception
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Exceptions.NoMatchedFieldsException</i> object.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public NoMatchedFieldsException(string message)
            : base(message) { }
    }
}
