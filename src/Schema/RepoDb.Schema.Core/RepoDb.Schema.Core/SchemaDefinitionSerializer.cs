#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Linq;

namespace RepoDb.Schema
{
    /// <summary>
    /// Serializes a RepoDB schema definition as JSON, YAML, or XML.
    /// </summary>
    internal static class SchemaDefinitionSerializer
    {
        #region Public Methods

        /// <summary>
        /// Serializes a schema definition using the requested format.
        /// </summary>
        /// <param name="definition">The schema definition to serialize.</param>
        /// <param name="format">The output format: json, yaml, or xml.</param>
        /// <returns>The serialized schema definition.</returns>
        internal static string Serialize(JsonObject definition,
            string format)
        {
            if (string.Equals(format,
                "json",
                StringComparison.Ordinal))
                return Json(definition,
                    true) + "\n";
            if (string.Equals(format,
                "yaml",
                StringComparison.Ordinal))
            {
                var builder = new StringBuilder();
                WriteYaml(builder,
                    definition,
                    0);
                return builder.ToString();
            }

            if (!string.Equals(format,
                "xml",
                StringComparison.Ordinal))
                throw new ArgumentOutOfRangeException(nameof(format));
            var document = new XDocument(new XDeclaration("1.0",
                "utf-8",
                null),
                Xml("repoDbSchema",
                    definition));
            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream,
                    new XmlWriterSettings { Encoding = new UTF8Encoding(false), Indent = true }))
                    document.Save(writer);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Writes a JSON node directly without reflection-based object serialization.
        /// </summary>
        /// <param name="node">The definition node, or null.</param>
        /// <param name="indented">Whether to indent the JSON output.</param>
        /// <returns>The serialized JSON value.</returns>
        private static string Json(JsonNode node,
            bool indented)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream,
                    new JsonWriterOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, Indented = indented }))
                {
                    if (node == null)
                        writer.WriteNullValue();
                    else
                        node.WriteTo(writer);
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// Writes a YAML collection, quoting scalar strings to avoid implicit typing and SQL escaping issues.
        /// </summary>
        /// <param name="builder">The output builder.</param>
        /// <param name="token">The collection to serialize.</param>
        /// <param name="indent">The indentation width in spaces.</param>
        private static void WriteYaml(StringBuilder builder,
            JsonNode token,
            int indent)
        {
            if (token is JsonObject obj)
            {
                foreach (var property in obj)
                {
                    builder.Append(' ',
                        indent).Append(property.Key).Append(':');
                    WriteYamlValue(builder,
                        property.Value,
                        indent);
                }
            }
            else if (token is JsonArray array)
            {
                foreach (var item in array)
                {
                    builder.Append(' ',
                        indent).Append('-');
                    WriteYamlValue(builder,
                        item,
                        indent);
                }
            }
        }

        /// <summary>
        /// Writes a YAML value and recursively expands nonempty collections.
        /// </summary>
        /// <param name="builder">The output builder.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="indent">The indentation width of the containing collection in spaces.</param>
        private static void WriteYamlValue(StringBuilder builder,
            JsonNode value,
            int indent)
        {
            if ((value is JsonObject obj && obj.Count > 0) || (value is JsonArray array && array.Count > 0))
            {
                builder.Append('\n');
                WriteYaml(builder,
                    value,
                    indent + 2);
            }
            else
                builder.Append(' ').Append(Json(value,
                    false)).Append('\n');
        }

        /// <summary>
        /// Creates an XML element that preserves the value and type of a definition token.
        /// </summary>
        /// <param name="name">The element name.</param>
        /// <param name="token">The definition token to serialize.</param>
        /// <returns>The XML element representing the token.</returns>
        private static XElement Xml(string name,
            JsonNode token)
        {
            var element = new XElement(name);
            var kind = token?.GetValueKind() ?? JsonValueKind.Null;
            var type = kind == JsonValueKind.Number ? "integer" : kind == JsonValueKind.True || kind == JsonValueKind.False ? "boolean" : kind.ToString().ToLowerInvariant();
            // Type tags preserve null, empty arrays, booleans and integers across formats.
            element.SetAttributeValue("type",
                type);
            if (token is JsonObject obj)
                foreach (var property in obj)
                    element.Add(Xml(property.Key,
                        property.Value));
            else if (token is JsonArray array)
                foreach (var item in array)
                    element.Add(Xml("item",
                        item));
            else if (kind != JsonValueKind.Null)
                element.Value = kind == JsonValueKind.String ? token.GetValue<string>() : Json(token,
                    false);
            return element;
        }

        #endregion
    }
}
