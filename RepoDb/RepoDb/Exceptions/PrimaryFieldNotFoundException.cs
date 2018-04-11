using System;

namespace RepoDb.Exceptions
{
    public class PrimaryFieldNotFoundException : Exception
    {
        public PrimaryFieldNotFoundException(string message)
            : base(message) { }
    }
}
