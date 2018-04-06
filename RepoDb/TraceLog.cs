using RepoDb.Interfaces;

namespace RepoDb
{
    public class TraceLog : ITraceLog
    {
        internal TraceLog(string statement, object parameter, object result)
        {
            Statement = statement;
            Parameter = parameter;
            Result = result;
        }

        public object Parameter { get; }

        public string Statement { get; }

        public object Result { get; }
    }
}
