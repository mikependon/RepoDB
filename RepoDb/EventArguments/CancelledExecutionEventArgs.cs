using System;

namespace RepoDb.EventArguments
{
    [Obsolete]
    internal class CancelledExecutionEventArgs : ExecutionEventArgs
    {
        public CancelledExecutionEventArgs(string statement, object parameter)
            : base(statement, parameter)
        {
        }
    }
}
