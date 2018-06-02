using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the data entity is not inline updateable.
    /// </summary>
    public class EntityNotInlineUpdateableException : Exception
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Exceptions.EntityNotInlineUpdateableException</i> object.
        /// </summary>
        /// <param name="name">The type name or the mapped name of the entity.</param>
        public EntityNotInlineUpdateableException(string name)
            : base($"Cannot Inline Update: {name}") { }
    }
}
