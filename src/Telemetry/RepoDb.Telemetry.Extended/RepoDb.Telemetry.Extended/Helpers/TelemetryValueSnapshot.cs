#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text.Json;
using Serilog;

namespace RepoDb.Telemetry.Extended
{
    /// <summary>
    /// Captures values without reflection or deferred enumeration.
    /// </summary>
    internal static class TelemetryValueSnapshot
    {
        #region Public Methods

        /// <summary>
        /// Captures an independent JSON snapshot.
        /// </summary>
        /// <param name="value">The value to capture.</param>
        /// <param name="option">The capture settings.</param>
        /// <param name="logger">The diagnostic logger.</param>
        /// <returns>The snapshot, or null when unavailable.</returns>
        public static JsonElement? Capture(object value,
            ExtendedTelemetryOption option,
            ILogger logger)
        {
            if (IsDeferred(value))
            {
                return null;
            }

            try
            {
                return CaptureValue(value,
                    option);
            }
            catch (Exception error)
            {
                logger?.Warning(error,
                    "Unable to snapshot a telemetry value of type {ValueType}.",
                    value?.GetType().FullName);
                return null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Serializes a supported value or invokes custom capture.
        /// </summary>
        /// <param name="value">The value to capture.</param>
        /// <param name="option">The capture settings.</param>
        /// <returns>The captured value.</returns>
        private static JsonElement? CaptureValue(object value,
            ExtendedTelemetryOption option)
        {
            if (value != null && !(value is DBNull) && option.CaptureValue != null)
            {
                return option.CaptureValue(value)?.Clone();
            }

            using var output = new MemoryStream();
            using (var writer = new Utf8JsonWriter(output))
            {
                WriteValue(writer,
                    value,
                    option,
                    0);
            }

            using var document = JsonDocument.Parse(output.ToArray());
            return document.RootElement.Clone();
        }

        /// <summary>
        /// Identifies values that must remain unconsumed.
        /// </summary>
        /// <param name="value">The inspected value.</param>
        /// <returns>Whether the value is deferred.</returns>
        private static bool IsDeferred(object value) => value is IDataReader || value is Stream || (value is IEnumerable && !(value is string) && !(value is ICollection));

        /// <summary>
        /// Writes materialized collection elements.
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="values">The materialized collection.</param>
        /// <param name="option">The capture settings.</param>
        /// <param name="depth">The current nesting depth.</param>
        private static void WriteCollection(Utf8JsonWriter writer,
            ICollection values,
            ExtendedTelemetryOption option,
            int depth)
        {
            writer.WriteStartArray();
            foreach (var value in values)
            {
                WriteValue(writer,
                    value,
                    option,
                    depth + 1);
            }

            writer.WriteEndArray();
        }

        /// <summary>
        /// Writes dictionary entries.
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="values">The dictionary.</param>
        /// <param name="option">The capture settings.</param>
        /// <param name="depth">The current nesting depth.</param>
        private static void WriteDictionary(Utf8JsonWriter writer,
            IDictionary values,
            ExtendedTelemetryOption option,
            int depth)
        {
            writer.WriteStartObject();
            foreach (DictionaryEntry entry in values)
            {
                writer.WritePropertyName(Convert.ToString(entry.Key,
                    System.Globalization.CultureInfo.InvariantCulture));
                WriteValue(writer,
                    entry.Value,
                    option,
                    depth + 1);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Writes a value using generated metadata or collection traversal.
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="option">The capture settings.</param>
        /// <param name="depth">The current nesting depth.</param>
        private static void WriteValue(Utf8JsonWriter writer,
            object value,
            ExtendedTelemetryOption option,
            int depth)
        {
            if (value == null || value is DBNull || IsDeferred(value) || depth >= 32)
            {
                writer.WriteNullValue();
                return;
            }

            var metadata = option.ValueSerializerContext?.GetTypeInfo(value.GetType()) ?? ExtendedTelemetryJsonSerializerContext.Default.GetTypeInfo(value.GetType());
            if (metadata != null)
            {
                JsonSerializer.Serialize(writer,
                    value,
                    metadata);
                return;
            }

            WriteMaterializedValue(writer,
                value,
                option,
                depth);
        }

        /// <summary>
        /// Writes values without registered scalar metadata.
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="option">The capture settings.</param>
        /// <param name="depth">The current nesting depth.</param>
        private static void WriteMaterializedValue(Utf8JsonWriter writer,
            object value,
            ExtendedTelemetryOption option,
            int depth)
        {
            if (value is IDictionary dictionary)
            {
                WriteDictionary(writer,
                    dictionary,
                    option,
                    depth);
            }
            else if (value is ICollection collection)
            {
                WriteCollection(writer,
                    collection,
                    option,
                    depth);
            }
            else
            {
                writer.WriteNullValue();
            }
        }

        #endregion
    }
}
