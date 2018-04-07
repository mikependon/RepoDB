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


        public object Result { get; }

        public object Parameter { get; set; }

        public string Statement { get; set; }
    }
}
