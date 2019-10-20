using System;

namespace RepoDb
{
    /// <summary>
    /// A tracing log object used in the tracing operations.
    /// </summary>
    public class TraceLog
    {
        /// <summary>
        /// Creates an instance of <see cref="TraceLog"/> class.
        /// </summary>
        /// <param name="statement">The command text in used.</param>
        /// <param name="parameter">The parameters passed.</param>
        /// <param name="result">The actual result if present.</param>
        /// <param name="executionTime">The elapsed time of the execution.</param>
        internal TraceLog(string statement,
            object parameter,
            object result,
            TimeSpan? executionTime)
        {
            Statement = statement;
            Parameter = parameter;
            Result = result;
            if (executionTime != null && executionTime.HasValue)
            {
                ExecutionTime = executionTime.Value;
            }
        }

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
