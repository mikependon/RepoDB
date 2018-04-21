using System;

namespace RepoDb.Exceptions
{
    public class EntityNotCountableException : Exception
    {
        public EntityNotCountableException(string name)
            : base($"Cannot Count: {name}") { }
    }
}
