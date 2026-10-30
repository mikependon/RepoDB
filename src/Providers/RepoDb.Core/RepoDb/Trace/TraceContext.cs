#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data.Common;
using System.Diagnostics;

namespace RepoDb
{
    /// <summary>
    /// Holds the information about where and in what a traced operation was executed. It is captured once, when the execution is about to start,
    /// and it is shared by the log that is written before the execution and the log that is written after it.
    /// </summary>
    internal sealed class TraceContext
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="TraceContext"/> class.
        /// </summary>
        /// <param name="threadId">The identifier of the managed thread that started the execution.</param>
        /// <param name="machineName">The name of the machine.</param>
        /// <param name="ipAddress">The IP address of the machine.</param>
        /// <param name="traceId">The identifier of the trace that the active <see cref="Activity"/> belongs to.</param>
        /// <param name="spanId">The identifier of the span of the active <see cref="Activity"/>.</param>
        /// <param name="connectionType">The name of the type of the connection.</param>
        /// <param name="database">The name of the database of the connection.</param>
        internal TraceContext(int threadId,
            string machineName,
            string ipAddress,
            string traceId,
            string spanId,
            string connectionType,
            string database)
        {
            ThreadId = threadId;
            MachineName = machineName;
            IpAddress = ipAddress;
            TraceId = traceId;
            SpanId = spanId;
            ConnectionType = connectionType;
            Database = database;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the context that has no information. It is used by the logs that are not created for an execution.
        /// </summary>
        public static TraceContext Empty { get; } = new TraceContext(0, null, null, null, null, null, null);

        /// <summary>
        /// Gets the name of the type of the connection that the command was created from (i.e.: <c>SqlConnection</c>).
        /// </summary>
        public string ConnectionType { get; }

        /// <summary>
        /// Gets the name of the database of the connection.
        /// </summary>
        public string Database { get; }

        /// <summary>
        /// Gets the IP address of the machine.
        /// </summary>
        public string IpAddress { get; }

        /// <summary>
        /// Gets the name of the machine.
        /// </summary>
        public string MachineName { get; }

        /// <summary>
        /// Gets the identifier of the span of the active <see cref="Activity"/>, or <c>null</c> if there is none.
        /// </summary>
        public string SpanId { get; }

        /// <summary>
        /// Gets the identifier of the managed thread that started the execution.
        /// </summary>
        public int ThreadId { get; }

        /// <summary>
        /// Gets the identifier of the trace that the active <see cref="Activity"/> belongs to, or <c>null</c> if there is none.
        /// </summary>
        public string TraceId { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Captures the context of the execution of a command. The trace and the span come from the <see cref="Activity"/> that is active: a span is not created for the execution,
        /// so the operations that run within the same activity share them. Only the identifiers of the W3C format are captured.
        /// </summary>
        /// <param name="command">The command that is about to be executed.</param>
        /// <returns>The captured context.</returns>
        public static TraceContext Capture(DbCommand command)
        {
            var activity = Activity.Current;
            var isW3C = activity?.IdFormat == ActivityIdFormat.W3C;
            var connection = command?.Connection;
            return new TraceContext(Environment.CurrentManagedThreadId,
                TraceEnvironment.MachineName,
                TraceEnvironment.IpAddress,
                isW3C ? activity.TraceId.ToString() : null,
                isW3C ? activity.SpanId.ToString() : null,
                connection?.GetType().Name,
                GetDatabase(connection));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the name of the database of the connection. A provider that cannot tell it (i.e.: because the connection is not usable) gives <c>null</c>.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>The name of the database, or <c>null</c> if it cannot be read.</returns>
        private static string GetDatabase(DbConnection connection)
        {
            try
            {
                return connection?.Database;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}
