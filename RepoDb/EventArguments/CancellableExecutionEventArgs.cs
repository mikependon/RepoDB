using System;

namespace RepoDb.EventArguments
{
    [Obsolete]
    internal class CancellableExecutionEventArgs : ExecutionEventArgs
    {
        public CancellableExecutionEventArgs(string statement, object parameter)
            : base(statement, parameter)
        {
            IsCancelled = false;
        }

        public void Cancel(bool throwException = false)
        {
            IsCancelled = true;
            IsThrowException = throwException;
        }

        public bool IsCancelled { get; private set; }

        internal bool IsThrowException { get; private set; }
    }
}
