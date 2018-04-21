using System;

namespace RepoDb.Exceptions
{
    public class EntityNotBigCountableException : Exception
    {
        public EntityNotBigCountableException(string name)
            : base($"Cannot Count (BIG): {name}") { }
    }
}
