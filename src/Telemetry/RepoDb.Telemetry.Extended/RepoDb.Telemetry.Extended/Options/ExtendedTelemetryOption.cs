#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using RepoDb.Telemetry.Default;

namespace RepoDb.Telemetry.Extended
{
    /// <summary>
    /// Defines the settings and enrichment callback for extended telemetry.
    /// </summary>
    public class ExtendedTelemetryOption : DefaultTelemetryOption
    {
        #region Constructors

        /// <summary>
        /// Creates the extended telemetry settings.
        /// </summary>
        /// <param name="application">The application producing telemetry.</param>
        public ExtendedTelemetryOption(string application)
            : base(application)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the callback that supplies metadata unavailable in the trace log.
        /// The callback runs before execution and should not throw.
        /// </summary>
        public Action<ExtendedTelemetryItem, CancellableTraceLog> Enrich { get; set; }

        /// <summary>
        /// Whether parameter values are captured in addition to metadata.
        /// </summary>
        public bool CaptureParameterValues { get; set; } = true;

        /// <summary>
        /// Whether the number of results (rows) of the operation is captured in addition to summaries.
        /// </summary>
        public bool CaptureResultValues { get; set; } = true;

        /// <summary>
        /// Supplies JSON snapshots for application types. Return null for unsupported values.
        /// Deferred sequences and readers are excluded before this callback is invoked.
        /// </summary>
        public Func<object, JsonElement?> CaptureValue { get; set; }

        /// <summary>
        /// Gets or sets generated metadata for application value types.
        /// </summary>
        public JsonSerializerContext ValueSerializerContext { get; set; }

        #endregion
    }
}
