using System;

namespace RepoDb.Exceptions
{
    public class DuplicateTypeMapException : Exception
    {
        public DuplicateTypeMapException(Type type)
            : base($"Duplicate Type Map: {type.Name} ({type.Namespace ?? type.FullName})") { }
    }
}
