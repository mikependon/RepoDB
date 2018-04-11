using System;

namespace RepoDb.Exceptions
{
    public class EntityNotInsertableException : Exception
    {
        public EntityNotInsertableException(string name)
            : base($"Cannot Insert: {name}") { }
    }
}
