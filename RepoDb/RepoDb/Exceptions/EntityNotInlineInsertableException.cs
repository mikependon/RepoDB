using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the data entity is not inline insertable.
    /// </summary>
    public class EntityNotInlineInsertableException : Exception
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Exceptions.EntityNotInlineInsertableException</i> object.
        /// </summary>
        /// <param name="name">The type name or the mapped name of the entity.</param>
        public EntityNotInlineInsertableException(string name)
            : base($"Cannot Inline Insert: {name}") { }
    }
}
