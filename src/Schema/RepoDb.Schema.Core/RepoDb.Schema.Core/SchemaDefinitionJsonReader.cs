#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.IO;
using System.Text.Json.Nodes;

namespace RepoDb.Schema
{
    /// <summary>
    /// Reads the JSON representation produced by SaveDefinitionAsJson.
    /// </summary>
    internal static class SchemaDefinitionJsonReader
    {
        #region Internal Methods

        /// <summary>
        /// Parses a JSON document, requiring its root to be an object.
        /// </summary>
        /// <param name="content">The JSON content of the definition file.</param>
        /// <returns>The definition document.</returns>
        internal static JsonObject Parse(string content) =>
            JsonNode.Parse(content) as JsonObject ?? throw new InvalidDataException("The definition root must be an object.");

        #endregion
    }
}
