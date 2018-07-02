using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the duplicate type mapping is found.
    /// </summary>
    public class DuplicateTypeMapException : Exception
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Exceptions.DuplicateTypeMapException</i> object.
        /// </summary>
        /// <param name="type">The type being mapped.</param>
        public DuplicateTypeMapException(Type type)
            : base($"Duplicate Type Map: {type.Name} ({type.Namespace ?? type.FullName})") { }
    }
}
