#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

#if !NETSTANDARD2_0

using RepoDb.Interfaces;
using RepoDb.Options;
using System.Text.Json.Nodes;

namespace RepoDb.PropertyHandlers
{
    /// <summary>
    /// A property handler that maps the SQL Server <c>json</c> type into a <see cref="JsonNode"/> property.
    /// </summary>
    public class JsonToJsonNodePropertyHandler : IPropertyHandler<string, JsonNode>
    {
        /// <summary>
        /// Converts the SQL Server <c>json</c> value, as returned by the database, into a <see cref="JsonNode"/>.
        /// </summary>
        /// <param name="input">The <c>json</c> value of the column, as a <see cref="string"/>.</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The parsed <see cref="JsonNode"/>, or <c>null</c> when the value is <c>null</c> or empty.</returns>
        public JsonNode Get(string input,
            PropertyHandlerGetOptions options) =>
            string.IsNullOrEmpty(input) ? null : JsonNode.Parse(input);

        /// <summary>
        /// Converts the <see cref="JsonNode"/> into its compact JSON text, to be written into a SQL Server <c>json</c> column.
        /// </summary>
        /// <param name="input">The <see cref="JsonNode"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The compact JSON text, or <c>null</c> when the <see cref="JsonNode"/> is <c>null</c>.</returns>
        public string Set(JsonNode input,
            PropertyHandlerSetOptions options) =>
            input?.ToJsonString();
    }
}

#endif
