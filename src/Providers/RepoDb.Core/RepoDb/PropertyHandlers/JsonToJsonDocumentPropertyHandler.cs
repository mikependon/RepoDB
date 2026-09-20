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
    /// A property handler that maps a database <c>json</c> column (for example, the MySQL <c>JSON</c> type) into a <see cref="JsonDocument"/> property.
    /// </summary>
    /// <remarks>
    /// <see cref="JsonDocument"/> is <see cref="System.IDisposable"/>. The document created by this handler is owned by the entity, and it is the responsibility of the caller to dispose it.
    /// </remarks>
    public class JsonToJsonDocumentPropertyHandler : IPropertyHandler<string, JsonDocument>
    {
        /// <summary>
        /// Parses the <c>json</c> value, as returned by the database, into a <see cref="JsonDocument"/>.
        /// </summary>
        /// <param name="input">The <c>json</c> value of the column, as a <see cref="string"/>.</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The parsed <see cref="JsonDocument"/>, or <c>null</c> when the value is <c>null</c> or empty.</returns>
        public JsonDocument Get(string input,
            PropertyHandlerGetOptions options) =>
            string.IsNullOrEmpty(input) ? null : JsonDocument.Parse(input);

        /// <summary>
        /// Converts the <see cref="JsonDocument"/> into its raw JSON text, to be written into a <c>json</c> column.
        /// </summary>
        /// <param name="input">The <see cref="JsonDocument"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The raw JSON text of the root element, or <c>null</c> when the <see cref="JsonDocument"/> is <c>null</c>.</returns>
        public string Set(JsonDocument input,
            PropertyHandlerSetOptions options) =>
            input?.RootElement.GetRawText();
    }
}

#endif
