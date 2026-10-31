#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Diagnostics;
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
        protected internal TraceLog(Guid sessionId,
            string key)
            : this(sessionId, key, TraceContext.Empty, TraceLevel.Info)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="TraceLog"/> class for an execution.
        /// </summary>
        /// <param name="sessionId">The session identifier used by the current trace.</param>
        /// <param name="key">The actual tracing key used by the operation.</param>
        /// <param name="context">The context in which the operation is executed.</param>
        /// <param name="level">The level of the log.</param>
        internal TraceLog(Guid sessionId,
            string key,
            TraceContext context,
            TraceLevel level)
        {
            SessionId = sessionId;
            Key = key;
            Context = context ?? TraceContext.Empty;
            Level = level;
        }

        #region Properties

        /// <summary>
        /// Gets the name of the type of the connection that the operation was executed with (i.e.: <c>SqlConnection</c>).
        /// </summary>
        public string ConnectionType => Context.ConnectionType;

        /// <summary>
        /// Gets the context in which the operation is executed.
        /// </summary>
        internal TraceContext Context { get; }

        /// <summary>
        /// Gets the name of the database that the operation was executed against.
        /// </summary>
        public string Database => Context.Database;

        /// <summary>
        /// Gets the IP address of the machine that executed the operation, or <c>null</c> if it cannot be resolved.
        /// </summary>
        public string IpAddress => Context.IpAddress;

        /// <summary>
        /// Gets the actual tracing key used by the operation.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the level of the log: <see cref="TraceLevel.Info"/> normally, and <see cref="TraceLevel.Error"/> if the operation has failed.
        /// </summary>
        public TraceLevel Level { get; }

        /// <summary>
        /// Gets the name of the machine that executed the operation.
        /// </summary>
        public string MachineName => Context.MachineName;

        /// <summary>
        /// Gets the session identifier used by the current trace.
        /// </summary>
        public Guid SessionId { get; }

        /// <summary>
        /// Gets the identifier of the span of the <see cref="System.Diagnostics.Activity"/> that was active when the operation started, or <c>null</c> if there was none.
        /// The span identifies one step of the whole trace. A span is not created for the operation: the operations that are executed within the same activity
        /// share both the <see cref="TraceId"/> and the span. Use the <see cref="SessionId"/> to tell the operations apart.
        /// </summary>
        public string SpanId => Context.SpanId;

        /// <summary>
        /// Gets the identifier of the managed thread that started the operation.
        /// </summary>
        public int ThreadId => Context.ThreadId;

        /// <summary>
        /// Gets the identifier of the trace of the <see cref="System.Diagnostics.Activity"/> that was active when the operation started, or <c>null</c> if there was none.
        /// The trace identifies the whole operation across the services, and all of its steps share it.
        /// </summary>
        public string TraceId => Context.TraceId;

        #endregion

        #region Methods

        /// <summary>
        /// Returns the string that represents the current object.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public override string ToString()
        {
            return $"SessiontId: {SessionId}, Key: {Key}";
        }

        #endregion
    }
}
