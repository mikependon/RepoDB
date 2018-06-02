using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the data entity is not queryable.
    /// </summary>
    public class EntityNotQueryableException : Exception
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Exceptions.EntityNotQueryableException</i> object.
        /// </summary>
        /// <param name="name">The type name or the mapped name of the entity.</param>
        public EntityNotQueryableException(string name)
            : base($"Cannot Query: {name}") { }
    }
}
