#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using NpgsqlTypes;
using RepoDb.Interfaces;
using RepoDb.Options;
using System;

namespace RepoDb.PropertyHandlers.PostgreSql
{
    /// <summary>
    /// A property handler that maps the full-text search <c>tsvector</c> type into a <see cref="string"/> property.
    /// </summary>
    public class PostgreSqlTsVectorToStringPropertyHandler : IPropertyHandler<object, string>
    {
        /// <summary>
        /// Converts the <c>tsvector</c> value, as returned by the driver, into its text form (for example <c>'cat':3 'fat':2</c>).
        /// </summary>
        /// <param name="input">The <c>tsvector</c> value of the column, as returned by the driver (an <see cref="NpgsqlTsVector"/> or a <see cref="string"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The text form of the <c>tsvector</c>, or <c>null</c> when the value is <c>null</c>.</returns>
        public string Get(object input,
            PropertyHandlerGetOptions options)
        {
            switch (input)
            {
                case null:
                case DBNull _:
                    return null;
                case string text:
                    return text;
                case NpgsqlTsVector vector:
                    return vector.ToString();
                default:
                    throw new ArgumentException($"The type '{input.GetType()}' is not a supported tsvector value.", nameof(input));
            }
        }

#pragma warning disable CS0618 // The client-side parsing is intentional, see the summary of the member.
        /// <summary>
        /// Converts the text form of a <c>tsvector</c> into an <see cref="NpgsqlTsVector"/>, to be written into a <c>tsvector</c> column. The parsing is performed on the client by the driver, which marks it as unreliable for complex values (it cannot fully duplicate the PostgreSQL logic); prefer simple, well-formed values, or use the server functions (<c>to_tsvector</c>) in the SQL for complex ones.
        /// </summary>
        /// <param name="input">The text form of the <c>tsvector</c> to write (for example <c>'cat':3 'fat':2</c>).</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="NpgsqlTsVector"/> (boxed), or <c>null</c> when the text is <c>null</c>.</returns>
        public object Set(string input,
            PropertyHandlerSetOptions options) =>
            input == null ? null : NpgsqlTsVector.Parse(input);
#pragma warning restore CS0618
    }
}
