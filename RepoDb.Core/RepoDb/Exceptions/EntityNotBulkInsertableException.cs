using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the data entity is not bulk insertable.
    /// </summary>
    public class EntityNotBulkInsertableException : Exception
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Exceptions.EntityNotBulkInsertableException</i> object.
        /// </summary>
        /// <param name="name">The type name or the mapped name of the entity.</param>
        public EntityNotBulkInsertableException(string name)
            : base($"Cannot Insert: {name}") { }
    }
}
