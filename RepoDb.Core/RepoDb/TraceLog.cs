using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class that holds the information of the tracing operations.
    /// </summary>
    public class TraceLog
    {
        /// <summary>
        /// Creates an instance of <see cref="TraceLog"/> class.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="key"></param>
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        /// <param name="executionTime"></param>
        protected internal TraceLog(Guid sessionId,
            string key,
            string statement,
            IEnumerable<IDbDataParameter> parameters = null,
            TimeSpan? executionTime = null)
        {
            SessionId = sessionId;
            Key = key;
            Statement = statement;
            Parameters = parameters;
            if (executionTime != null)
            {
                ExecutionTime = executionTime.Value;
            }
        }

        #region Properties

        /// <summary>
        /// Gets the session identifier used by the current trace.
        /// </summary>
        public Guid SessionId { get; }

        /// <summary>
        /// Gets the actual tracing key used by the operation.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets or sets the SQL Statement used on the actual operation execution.
        /// </summary>
        public string Statement { get; set; }

        /// <summary>
        /// Gets the parameters used on the actual operation.
        /// </summary>
        public IEnumerable<IDbDataParameter> Parameters { get; }

        /// <summary>
        /// Gets the actual length of the operation execution.
        /// </summary>
        public TimeSpan ExecutionTime { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the manipulated string based from the value of the <see cref="Statement"/> and <see cref="Parameters"/> properties.
        /// The value is the representation of the current object.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public string Stringify() =>
            $"SessiontId: {SessionId}\n" +
            $"Key: {Key}\n" +
            $"Statement: {Statement}\n" +
            $"ExecutionTime (Milliseconds): {ExecutionTime.TotalMilliseconds}\n" +
            $"Parameters: {(Parameters?.Any() == true ? string.Join(", ", Parameters.ToArray().Select(param => $"({param.ParameterName}={param.Value})")) : "No Parameters")}";

        /// <summary>
        /// Returns the string that represents the current object.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public override string ToString() => $"SessiontId: {SessionId}, Key: {Key}";

        #endregion
    }
}
