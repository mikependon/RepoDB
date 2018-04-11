using System;

namespace RepoDb.Exceptions
{
    public class EntityNotQueryableException : Exception
    {
        public EntityNotQueryableException(string name)
            : base($"Cannot Query: {name}") { }
    }
}
