using System;

namespace RepoDb.Exceptions
{
    public class EntityNotInlineUpdateableException : Exception
    {
        public EntityNotInlineUpdateableException(string name)
            : base($"Cannot Inline Update: {name}") { }
    }
}
