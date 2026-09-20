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
    /// A property handler that maps the SQL Server <c>json</c> type into a <typeparamref name="TObject"/> property, via <see cref="JsonSerializer"/>.
    /// </summary>
    /// <typeparam name="TObject">The type of the property.</typeparam>
    public class JsonToObjectPropertyHandler<TObject> : IPropertyHandler<string, TObject>
    {
        /// <summary>
        /// Deserializes the SQL Server <c>json</c> value, as returned by the database, into a <typeparamref name="TObject"/>.
        /// </summary>
        /// <param name="input">The <c>json</c> value of the column, as a <see cref="string"/>.</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The deserialized <typeparamref name="TObject"/>, or its default value when the value is <c>null</c> or empty.</returns>
        public TObject Get(string input,
            PropertyHandlerGetOptions options) =>
            string.IsNullOrEmpty(input) ? default : JsonSerializer.Deserialize<TObject>(input);

        /// <summary>
        /// Serializes the <typeparamref name="TObject"/> into JSON text, to be written into a SQL Server <c>json</c> column.
        /// </summary>
        /// <param name="input">The <typeparamref name="TObject"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The JSON text, or <c>null</c> when the <typeparamref name="TObject"/> is <c>null</c>.</returns>
        public string Set(TObject input,
            PropertyHandlerSetOptions options) =>
            input == null ? null : JsonSerializer.Serialize(input);
    }
}

#endif
