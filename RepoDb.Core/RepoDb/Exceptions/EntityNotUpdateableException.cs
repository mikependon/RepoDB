using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the data entity is not updateable.
    /// </summary>
    public class EntityNotUpdateableException : Exception
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Exceptions.EntityNotUpdateableException</i> object.
        /// </summary>
        /// <param name="name">The type name or the mapped name of the entity.</param>
        public EntityNotUpdateableException(string name)
            : base($"Cannot Update: {name}") { }
    }
}
