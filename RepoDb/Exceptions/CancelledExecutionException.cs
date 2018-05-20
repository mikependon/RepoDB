using System;

namespace RepoDb.Exceptions
{
    public class CancelledExecutionException : Exception
    {
        public CancelledExecutionException(string message)
            : base(message) { }
    }
}
