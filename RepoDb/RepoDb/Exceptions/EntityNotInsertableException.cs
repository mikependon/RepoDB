using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the data entity is not insertable.
    /// </summary>
    public class EntityNotInsertableException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="EntityNotInsertableException"/> class.
        /// </summary>
        /// <param name="name">The type name or the mapped name of the entity.</param>
        public EntityNotInsertableException(string name)
            : base($"Cannot Insert: {name}") { }
    }
}
