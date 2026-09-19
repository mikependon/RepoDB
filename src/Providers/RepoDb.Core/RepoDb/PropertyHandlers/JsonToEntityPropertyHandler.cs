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
    /// A property handler that maps the SQL Server <c>json</c> type, which the database returns as a <see cref="string"/>, into a <typeparamref name="TEntity"/> property.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity the JSON text is converted into.</typeparam>
    public class JsonToEntityPropertyHandler<TEntity> : IPropertyHandler<string, TEntity>
    {
        /// <summary>
        /// Converts the SQL Server <c>json</c> value, as returned by the database, from its <see cref="string"/> into a <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="input">The <c>json</c> value of the column, as a <see cref="string"/>.</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The <typeparamref name="TEntity"/> deserialized from the JSON text, or its default value when the value is <c>null</c> or empty.</returns>
        public TEntity Get(string input,
            PropertyHandlerGetOptions options) =>
            string.IsNullOrEmpty(input) ? default : JsonSerializer.Deserialize<TEntity>(input);

        /// <summary>
        /// Converts the <typeparamref name="TEntity"/> into its JSON <see cref="string"/>, to be written into a SQL Server <c>json</c> column.
        /// </summary>
        /// <param name="input">The <typeparamref name="TEntity"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The JSON text, or <c>null</c> when the <typeparamref name="TEntity"/> is <c>null</c>.</returns>
        public string Set(TEntity input,
            PropertyHandlerSetOptions options) =>
            input == null ? null : JsonSerializer.Serialize(input);
    }
}

#endif
