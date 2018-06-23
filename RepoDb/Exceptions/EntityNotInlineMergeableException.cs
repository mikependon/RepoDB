using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the data entity is not inline mergeable.
    /// </summary>
    public class EntityNotInlineMergeableException : Exception
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Exceptions.EntityNotInlineMergeableException</i> object.
        /// </summary>
        /// <param name="name">The type name or the mapped name of the entity.</param>
        public EntityNotInlineMergeableException(string name)
            : base($"Cannot Inline Merge: {name}") { }
    }
}
