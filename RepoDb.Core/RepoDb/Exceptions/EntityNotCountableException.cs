using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the data entity is not countable.
    /// </summary>
    public class EntityNotCountableException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="EntityNotCountableException"/> class.
        /// </summary>
        /// <param name="name">The type name or the mapped name of the entity.</param>
        public EntityNotCountableException(string name)
            : base($"Cannot Count: {name}") { }
    }
}
