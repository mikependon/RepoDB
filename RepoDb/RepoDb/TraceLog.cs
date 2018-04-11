using RepoDb.Interfaces;
using System.Reflection;

namespace RepoDb
{
    public class TraceLog : ITraceLog
    {
        internal TraceLog(MethodBase method, string statement, object parameter, object result)
        {
            Method = method;
            Statement = statement;
            Parameter = parameter;
            Result = result;
        }

        public MethodBase Method { get; }

        public object Result { get; }

        public object Parameter { get; set; }

        public string Statement { get; set; }
    }
}
