#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepoDb.Telemetry.Core
{
    /// <summary>
    /// The source generated JSON serialization metadata of the telemetry items (compatible with the trimming and NativeAOT).
    /// </summary>
    [JsonSerializable(typeof(IEnumerable<TelemetryItem>))]
    internal partial class TelemetryJsonSerializerContext : JsonSerializerContext
    {
    }
}
