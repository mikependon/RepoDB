#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

#if !NETSTANDARD2_0

using RepoDb.Interfaces;
using RepoDb.Options;
using System.Text.Json;

namespace RepoDb.PropertyHandlers
{
    /// <summary>
    /// A property handler that maps the SQL Server <c>json</c> type into a <see cref="JsonElement"/> property.
    /// </summary>
    public class JsonToJsonElementPropertyHandler : IPropertyHandler<string, JsonElement>
    {
        /// <summary>
        /// Converts the SQL Server <c>json</c> value, as returned by the database, into a <see cref="JsonElement"/> that is independent of the parsed document.
        /// </summary>
        /// <param name="input">The <c>json</c> value of the column, as a <see cref="string"/>.</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The root <see cref="JsonElement"/>, or the default (<see cref="JsonValueKind.Undefined"/>) element when the value is <c>null</c> or empty.</returns>
        public JsonElement Get(string input,
            PropertyHandlerGetOptions options)
        {
            if (string.IsNullOrEmpty(input))
            {
                return default;
            }
            using var document = JsonDocument.Parse(input);
            return document.RootElement.Clone();
        }

        /// <summary>
        /// Converts the <see cref="JsonElement"/> into its raw JSON text, to be written into a SQL Server <c>json</c> column.
        /// </summary>
        /// <param name="input">The <see cref="JsonElement"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The raw JSON text, or <c>null</c> when the element is the default (<see cref="JsonValueKind.Undefined"/>) element.</returns>
        public string Set(JsonElement input,
            PropertyHandlerSetOptions options) =>
            input.ValueKind == JsonValueKind.Undefined ? null : input.GetRawText();
    }
}

#endif
