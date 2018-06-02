using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown of the validation for primary key has been called and the primary key is not
    /// found from the data entity.
    /// </summary>
    public class PrimaryFieldNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Exceptions.PrimaryFieldNotFoundException</i> object.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public PrimaryFieldNotFoundException(string message)
            : base(message) { }
    }
}
