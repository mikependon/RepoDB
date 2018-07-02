using RepoDb.Interfaces;
using System;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A tracing log object used in the tracing operations.
    /// </summary>
    public class TraceLog
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

        /// <summary>
        /// Gets the method that triggers the actual operation execution.
        /// </summary>
        public MethodBase Method { get; }

        /// <summary>
        /// Gets the actual result of the actual operation execution.
        /// </summary>
        public object Result { get; }

        /// <summary>
        /// Gets or sets the parameter object used on the actual operation execution.
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// Gets or sets the SQL Statement used on the actual operation execution.
        /// </summary>
        public string Statement { get; set; }

        /// <summary>
        /// Gets the actual length of the operation execution.
        /// </summary>
        public TimeSpan ExecutionTime { get; }
    }
}
