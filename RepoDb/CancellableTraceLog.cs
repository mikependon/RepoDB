using RepoDb.Interfaces;
using System.Reflection;

namespace RepoDb
{
    public sealed class CancellableTraceLog : TraceLog, ICancellableTraceLog
    {
        internal CancellableTraceLog(MethodBase method, string statement, object parameter, object result)
            : base(method, statement, parameter, result)
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
