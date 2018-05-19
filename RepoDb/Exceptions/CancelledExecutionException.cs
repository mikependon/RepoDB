using System;

namespace RepoDb.Exceptions
{
    public class CanceledExecutionException : Exception
    {
        public CanceledExecutionException(string message)
            : base(message) { }
    }
}
