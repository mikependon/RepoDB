using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the target item is not found from the collection.
    /// </summary>
    public class ItemNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="ItemNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ItemNotFoundException(string message)
            : base(message) { }
    }
}
