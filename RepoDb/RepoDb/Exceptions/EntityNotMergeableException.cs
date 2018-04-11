using System;

namespace RepoDb.Exceptions
{
    public class EntityNotMergeableException : Exception
    {
        public EntityNotMergeableException(string name)
            : base($"Cannot Merge: {name}") { }
    }
}
