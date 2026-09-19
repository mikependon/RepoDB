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
    /// A property handler that maps the full-text search <c>tsquery</c> type into a <see cref="string"/> property.
    /// </summary>
    public class PostgreSqlTsQueryToStringPropertyHandler : IPropertyHandler<object, string>
    {
        /// <summary>
        /// Converts the <c>tsquery</c> value, as returned by the driver, into its text form (for example <c>'fat' &amp; 'cat'</c>).
        /// </summary>
        /// <param name="input">The <c>tsquery</c> value of the column, as returned by the driver (an <see cref="NpgsqlTsQuery"/> or a <see cref="string"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The text form of the <c>tsquery</c>, or <c>null</c> when the value is <c>null</c>.</returns>
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
                case NpgsqlTsQuery query:
                    return query.ToString();
                default:
                    throw new ArgumentException($"The type '{input.GetType()}' is not a supported tsquery value.", nameof(input));
            }
        }

#pragma warning disable CS0618 // The client-side parsing is intentional, see the summary of the member.
        /// <summary>
        /// Converts the text form of a <c>tsquery</c> into an <see cref="NpgsqlTsQuery"/>, to be written into a <c>tsquery</c> column. The parsing is performed on the client by the driver, which marks it as unreliable for complex values (it cannot fully duplicate the PostgreSQL logic); prefer simple, well-formed values, or use the server functions (<c>to_tsquery</c>) in the SQL for complex ones.
        /// </summary>
        /// <param name="input">The text form of the <c>tsquery</c> to write (for example <c>'fat' &amp; 'cat'</c>).</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="NpgsqlTsQuery"/> (boxed), or <c>null</c> when the text is <c>null</c>.</returns>
        public object Set(string input,
            PropertyHandlerSetOptions options) =>
            input == null ? null : NpgsqlTsQuery.Parse(input);
#pragma warning restore CS0618
    }
}
