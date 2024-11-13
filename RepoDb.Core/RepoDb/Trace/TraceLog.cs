#nullable enable
using System;

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
        protected internal TraceLog(Guid sessionId,
            string? key)
        {
            SessionId = sessionId;
            Key = key;
        }

        #region Properties

        /// <summary>
        /// Gets the session identifier used by the current trace.
        /// </summary>
        public Guid SessionId { get; }

        /// <summary>
        /// Gets the actual tracing key used by the operation.
        /// </summary>
        public string? Key { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the string that represents the current object.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public override string ToString() => $"SessiontId: {SessionId}, Key: {Key}";

        #endregion
    }
}
