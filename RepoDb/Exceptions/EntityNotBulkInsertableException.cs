using System;

namespace RepoDb.Exceptions
{
    public class EntityNotBulkInsertableException : Exception
    {
        public EntityNotBulkInsertableException(string name)
            : base($"Cannot Insert: {name}") { }
    }
}
