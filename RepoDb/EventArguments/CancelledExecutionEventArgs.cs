namespace RepoDb.EventArguments
{
    public class CancelledExecutionEventArgs : ExecutionEventArgs
    {
        public CancelledExecutionEventArgs(string statement, object parameter)
            : base(statement, parameter)
        {
        }
    }
}
