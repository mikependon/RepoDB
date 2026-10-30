#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
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

        #endregion
    }
}

