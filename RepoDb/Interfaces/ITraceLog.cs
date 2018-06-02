using System;
using System.Reflection;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class object to be a traceable log object for tracing operations.
    /// </summary>
    public interface ITraceLog
    {
        /// <summary>
        /// Gets the method that triggers the actual operation execution.
        /// </summary>
        MethodBase Method { get; }

        /// <summary>
        /// Gets the actual result of the actual operation execution.
        /// </summary>
        object Result { get; }

        /// <summary>
        /// Gets the parameter object used on the actual operation execution.
        /// </summary>
        object Parameter { get; set; }

        /// <summary>
        /// Gets the SQL Statement used on the actual operation execution.
        /// </summary>
        string Statement { get; set; }

        /// <summary>
        /// Gets the actual length of the operation execution.
        /// </summary>
        TimeSpan ExecutionTime { get; }
    }
}