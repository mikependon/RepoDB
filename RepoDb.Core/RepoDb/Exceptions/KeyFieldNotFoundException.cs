using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the primary key and identity key is not found from the data entity.
    /// </summary>
    public class KeyFieldNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="KeyFieldNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public KeyFieldNotFoundException(string message)
            : base(message) { }
    }
}
