using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the target property is not found.
    /// </summary>
    public class PropertyNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="PropertyNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public PropertyNotFoundException(string message)
            : base(message) { }
    }
}
