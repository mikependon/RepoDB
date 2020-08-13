using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the primary key is not found from the data entity.
    /// </summary>
    public class PrimaryFieldNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="PrimaryFieldNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public PrimaryFieldNotFoundException(string message)
            : base(message) { }
    }
}
