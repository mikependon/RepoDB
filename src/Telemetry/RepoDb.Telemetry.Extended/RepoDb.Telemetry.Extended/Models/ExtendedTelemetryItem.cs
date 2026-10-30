#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using RepoDb.Telemetry.Default;
using Serilog.Events;

namespace RepoDb.Telemetry.Extended
{
    /// <summary>
    /// An extended telemetry event containing tracing, database, and custom metadata.
    /// </summary>
    public class ExtendedTelemetryItem : DefaultTelemetryItem
    {
        #region Properties

        /// <summary>
        /// The command type, when supplied by the caller.
        /// </summary>
        public CommandType? CommandType { get; set; }

        /// <summary>
        /// The connection display name, excluding credentials.
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// The database name, when supplied by the caller.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// The event severity.
        /// </summary>
        public LogEventLevel Level { get; set; } = LogEventLevel.Information;

        /// <summary>
        /// The human-readable event message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The number of command parameters.
        /// </summary>
        public int? ParameterCount { get; set; }

        /// <summary>
        /// The custom event properties. Values must have registered JSON metadata.
        /// </summary>
        public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>(StringComparer.Ordinal);

        /// <summary>
        /// The affected row count for a known write operation.
        /// </summary>
        public long? RowsAffected { get; set; }

        /// <summary>
        /// The returned row count when available without enumerating results.
        /// </summary>
        public long? RowsReturned { get; set; }

        /// <summary>
        /// The current distributed tracing span identifier.
        /// </summary>
        public string SpanId { get; set; }

        /// <summary>
        /// The table name, when supplied by the caller.
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// The managed thread identifier captured before execution.
        /// </summary>
        public int? ThreadId { get; set; }

        /// <summary>
        /// The current distributed tracing trace identifier.
        /// </summary>
        public string TraceId { get; set; }

        #endregion
    }
}

