#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A cancellable tracing log object that is used in the tracing operations. This class holds the cancellable operations for all tracing logs.
    /// </summary>
    public class CancellableTraceLog : TraceLog
    {
        /// <summary>
        /// Creates a new instance of <see cref="CancellableTraceLog"/> object.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="key"></param>
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        protected internal CancellableTraceLog(Guid sessionId,
            string key,
            string statement,
            IEnumerable<IDbDataParameter> parameters = null)
            : this(sessionId, key, statement, parameters, TraceContext.Empty)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="CancellableTraceLog"/> object for an execution.
        /// </summary>
        /// <param name="sessionId">The session identifier used by the current trace.</param>
        /// <param name="key">The actual tracing key used by the operation.</param>
        /// <param name="statement">The SQL Statement used on the actual operation execution.</param>
        /// <param name="parameters">The parameters used on the actual operation.</param>
        /// <param name="context">The context in which the operation is executed.</param>
        internal CancellableTraceLog(Guid sessionId,
            string key,
            string statement,
            IEnumerable<IDbDataParameter> parameters,
            TraceContext context)
            : base(sessionId, key, context, TraceLevel.Info)
        {
            Statement = statement;
            Parameters = parameters;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the SQL Statement used on the actual operation execution.
        /// </summary>
        public string Statement { get; set; }

        /// <summary>
        /// Gets the parameters used on the actual operation.
        /// </summary>
        public IEnumerable<IDbDataParameter> Parameters { get; }

        /// <summary>
        /// Gets the actual date/time value of when the actual execution has started. The value is set when the log is created, and it does not change.
        /// </summary>
        public DateTime StartTime { get; } = DateTime.UtcNow;

        /// <summary>
        /// Gets a value whether the operation is cancelled.
        /// </summary>
        public bool IsCancelled { get; private set; }

        /// <summary>
        /// Gets a value whether an exception will be thrown after the <see cref="Cancel(bool)"/> method was called.
        /// </summary>
        public bool IsThrowException { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Cancel the current executing repository operation.
        /// </summary>
        /// <param name="throwException">If true, an exception will be thrown.</param>
        public void Cancel(bool throwException = true)
        {
            IsCancelled = true;
            IsThrowException = throwException;
        }

        /// <summary>
        /// Returns the manipulated string based from the value of the <see cref="Statement"/> and <see cref="Parameters"/> properties.
        /// The value is the representation of the current object.
        /// </summary>
        /// <returns>The string representation of the current object.</returns>
        public override string ToString()
        {
            return $"SessionId: {SessionId}\n" +
            $"Key: {Key}\n" +
            $"Statement: {Statement}\n" +
            $"StartTime (Ticks): {StartTime.Ticks}\n" +
            $"Parameters: {(Parameters?.Any() == true ? string.Join(", ", Parameters.ToArray().Select(param => $"({param.ParameterName}={(param.Value is DBNull ? "DBNull" : param.Value)})")) : "No Parameters")}";
        }

        #endregion
    }
}
