using RepoDb.Interfaces;
using System.Reflection;

namespace RepoDb
{
    public sealed class CancellableTraceLog : TraceLog, ICancelableTraceLog
    {
        internal CancellableTraceLog(MethodBase method, string statement, object parameter, object result)
            : base(method, statement, parameter, result, null)
        {
        }

        public bool IsCanceled { get; private set; }

        public bool IsThrowException { get; private set; }

        public void Cancel(bool throwException)
        {
            IsCanceled = true;
            IsThrowException = throwException;
        }
    }
}
