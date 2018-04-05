using System;

namespace RepoDb.Exceptions
{
    public class EntityNotUpdateableException : Exception
    {
        public EntityNotUpdateableException(string name)
            : base($"Cannot Update: {name}") { }
    }
}
