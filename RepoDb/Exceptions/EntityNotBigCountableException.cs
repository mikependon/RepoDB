using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the data entity is not countable (big).
    /// </summary>
    public class EntityNotBigCountableException : Exception
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Exceptions.EntityNotBigCountableException</i> object.
        /// </summary>
        /// <param name="name">The type name or the mapped name of the entity.</param>
        public EntityNotBigCountableException(string name)
            : base($"Cannot Count (BIG): {name}") { }
    }
}
