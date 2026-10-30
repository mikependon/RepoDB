#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RepoDb.Telemetry.Extended
{
    /// <summary>
    /// Provides generated JSON metadata for extended events and common custom property values.
    /// </summary>
    [JsonSerializable(typeof(IEnumerable<ExtendedTelemetryItem>))]
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(int))]
    [JsonSerializable(typeof(long))]
    [JsonSerializable(typeof(double))]
    [JsonSerializable(typeof(decimal))]
    [JsonSerializable(typeof(DateTime))]
    [JsonSerializable(typeof(Guid))]
    [JsonSerializable(typeof(JsonElement))]
    internal partial class ExtendedTelemetryJsonSerializerContext : JsonSerializerContext
    {

    }
}

