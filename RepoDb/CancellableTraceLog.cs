using System;
using RepoDb.Interfaces;

namespace RepoDb
{
    public sealed class CancellableTraceLog : TraceLog, ICancellableTraceLog
    {
        internal CancellableTraceLog(string statement, object parameter, object result)
            : base(statement, parameter, result)
        {
        }

        public bool IsCancelled { get; private set; }

        public bool IsThrowException { get; private set; }

        public void Cancel(bool throwException)
        {
            IsCancelled = true;
            IsThrowException = throwException;
        }
    }
}
