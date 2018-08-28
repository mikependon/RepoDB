using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the data entity is not batch queryable.
    /// </summary>
    public class EntityNotBatchQueryableException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="EntityNotBatchQueryableException"/> class.
        /// </summary>
        /// <param name="name">The type name or the mapped name of the entity.</param>
        public EntityNotBatchQueryableException(string name)
            : base($"Cannot BatchQuery: {name}") { }
    }
}
