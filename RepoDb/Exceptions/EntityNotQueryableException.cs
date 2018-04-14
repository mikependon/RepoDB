using System;

namespace RepoDb.Exceptions
{
    public class EntityNotBatchQueryableException : Exception
    {
        public EntityNotBatchQueryableException(string name)
            : base($"Cannot BatchQuery: {name}") { }
    }
}
