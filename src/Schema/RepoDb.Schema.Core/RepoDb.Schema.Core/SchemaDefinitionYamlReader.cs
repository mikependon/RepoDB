#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;

namespace RepoDb.Schema
{
    /// <summary>
    /// Reads the YAML subset emitted by RepoDB: indented collections and JSON-quoted scalar values.
    /// Arbitrary YAML tags, anchors, aliases, and implicit scalar conversions are not supported.
    /// </summary>
    internal static class SchemaDefinitionYamlReader
    {
        #region Public Methods

        /// <summary>
        /// Parses one definition using the writer's two-space indentation and scalar conventions.
        /// </summary>
        /// <param name="content">The YAML content of the definition file.</param>
        /// <returns>The definition document.</returns>
        internal static JsonObject Parse(string content)
        {
            var lines = content.Replace("\r\n",
                "\n").Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            if (lines.Length == 0)
                throw new InvalidDataException("The YAML definition is empty.");
            var position = 0;
            var result = Collection(lines,
                ref position,
                0);
            if (position != lines.Length || !(result is JsonObject root))
                throw new InvalidDataException("Invalid YAML definition root.");
            return root;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads a block collection, rejecting mixed collection types and duplicate mapping keys.
        /// </summary>
        /// <param name="lines">The nonempty lines of the document.</param>
        /// <param name="position">The index of the line that is read first. It is advanced past the lines of the collection.</param>
        /// <param name="indent">The indentation of the collection, in spaces.</param>
        /// <returns>The array or object that the collection represents.</returns>
        private static JsonNode Collection(string[] lines,
            ref int position,
            int indent)
        {
            if (indent > 128)
                throw new InvalidDataException("The definition nesting limit was exceeded.");
            var isArray = lines[position].TrimStart().StartsWith("-",
                StringComparison.Ordinal);
            JsonNode result = isArray ? (JsonNode)new JsonArray() : new JsonObject();
            while (position < lines.Length)
            {
                var line = lines[position];
                var actualIndent = line.TakeWhile(c => c == ' ').Count();
                if (actualIndent < indent)
                    break;
                if (actualIndent != indent)
                    throw new InvalidDataException("Invalid YAML indentation.");
                var entry = line.Substring(indent);
                var value = ReadEntry(entry,
                    isArray,
                    result,
                    out var name);
                position++;
                JsonNode node;
                if (value.Length == 0)
                {
                    if (position == lines.Length || lines[position].TakeWhile(c => c == ' ').Count() != indent + 2)
                        throw new InvalidDataException("Missing YAML collection.");
                    node = Collection(lines,
                        ref position,
                        indent + 2);
                }
                else
                    node = JsonNode.Parse(value);
                if (isArray)
                    ((JsonArray)result).Add(node);
                else
                    ((JsonObject)result).Add(name,
                        node);
            }

            return result;
        }

        /// <summary>
        /// Reads a YAML sequence value or validates and separates a mapping key and value.
        /// </summary>
        /// <param name="entry">The entry with indentation removed.</param>
        /// <param name="isArray">Whether the containing collection is a sequence.</param>
        /// <param name="result">The current collection used to detect duplicate property names.</param>
        /// <param name="name">Receives the mapping key, or null for a sequence item.</param>
        /// <returns>The trimmed scalar or collection marker.</returns>
        private static string ReadEntry(string entry,
            bool isArray,
            JsonNode result,
            out string name)
        {
            name = null;
            string value;
            if (isArray)
            {
                if (!string.Equals(entry,
                    "-",
                    StringComparison.Ordinal) && !entry.StartsWith("- ",
                        StringComparison.Ordinal))
                    throw new InvalidDataException("Invalid YAML sequence item.");
                value = entry.Substring(1).Trim();
            }
            else
            {
                var separator = entry.IndexOf(':');
                if (separator <= 0)
                    throw new InvalidDataException("Invalid YAML mapping entry.");
                name = entry.Substring(0,
                    separator);
                if (name.Any(c => !char.IsLetterOrDigit(c)) || ((JsonObject)result).ContainsKey(name))
                    throw new InvalidDataException("Invalid or duplicate YAML property.");
                value = entry.Substring(separator + 1).Trim();
            }

            return value;
        }

        #endregion
    }
}
