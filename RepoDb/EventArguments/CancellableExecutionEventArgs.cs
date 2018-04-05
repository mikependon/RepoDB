namespace RepoDb.EventArguments
{
    public class CancellableExecutionEventArgs : ExecutionEventArgs
    {
        public CancellableExecutionEventArgs(string statement, object parameter)
            : base(statement, parameter)
        {
            IsCancelled = false;
        }

        public bool IsCancelled { get; set; }
    }
}
