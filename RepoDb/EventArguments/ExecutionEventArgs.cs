using System;

namespace RepoDb.EventArguments
{
    public class ExecutionEventArgs : EventArgs
    {
        public ExecutionEventArgs(string statement, object parameter)
        {
            Statement = statement;
            Parameter = parameter;
        }

        public string Statement { get; }

        public object Parameter { get; }
    }
}
