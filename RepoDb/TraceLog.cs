using RepoDb.Interfaces;
using System;
using System.Reflection;

namespace RepoDb
{
    public class TraceLog : ITraceLog
    {
        internal TraceLog(MethodBase method, string statement, object parameter, object result, TimeSpan? executionTime)
        {
            Method = method;
            Statement = statement;
            Parameter = parameter;
            Result = result;
            if (executionTime != null && executionTime.HasValue)
            {
                ExecutionTime = executionTime.Value;
            }
        }

        public MethodBase Method { get; }

        public object Result { get; }

        public object Parameter { get; set; }

        public string Statement { get; set; }

        public TimeSpan ExecutionTime { get; }
    }
}
