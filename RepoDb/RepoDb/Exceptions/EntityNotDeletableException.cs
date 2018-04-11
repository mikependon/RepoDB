using System;

namespace RepoDb.Exceptions
{
    public class EntityNotDeletableException : Exception
    {
        public EntityNotDeletableException(string name)
            : base($"Cannot Delete: {name}") { }
    }
}
