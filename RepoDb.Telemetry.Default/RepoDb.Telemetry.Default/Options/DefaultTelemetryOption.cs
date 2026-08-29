#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Telemetry.Core;

namespace RepoDb.Telemetry.Default
{
    /// <summary>
    /// A class that is being used to define the necessary settings to capture the library telemetries.
    /// </summary>
    public class DefaultTelemetryOption : TelemetryOption
    {
        /// <summary>
        /// Creates a new instance of <see cref="DefaultTelemetryOption"/> object.
        /// </summary>
        /// <param name="application">The name of the application that produces the telemetry.</param>

        public DefaultTelemetryOption(
            string application): base(application)
        {
        }
    }
}
