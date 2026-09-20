#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System;
using System.Text;

namespace RepoDb.PropertyHandlers.ClickHouse
{
    /// <summary>
    /// A property handler that maps the ClickHouse <c>FixedString(N)</c> type into a <see cref="string"/> property. ClickHouse pads a shorter value with trailing <c>NUL</c> (<c>\0</c>) characters up to N bytes; this handler removes that padding when reading, so the property holds the original text.
    /// </summary>
    public class ClickHouseFixedStringToStringPropertyHandler : IPropertyHandler<object, string>
    {
        /// <summary>
        /// Converts the <c>FixedString(N)</c> value, as returned by the driver, into a <see cref="string"/> without the trailing <c>NUL</c> padding.
        /// </summary>
        /// <param name="input">The value of the column, as returned by the driver (a <see cref="string"/> or a UTF-8 <see cref="T:byte[]"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The <see cref="string"/>, or <c>null</c> when the value is <c>null</c>.</returns>
        /// <exception cref="ArgumentException">Thrown when the type of the value is not supported.</exception>
        public string Get(object input,
            PropertyHandlerGetOptions options)
        {
            switch (input)
            {
                case null:
                case DBNull _:
                    return null;
                case string text:
                    return text.TrimEnd('\0');
                case byte[] bytes:
                    return Encoding.UTF8.GetString(bytes).TrimEnd('\0');
                default:
                    throw new ArgumentException($"The type '{input.GetType()}' is not a supported FixedString value.", nameof(input));
            }
        }

        /// <summary>
        /// Passes the <see cref="string"/> to the driver as is; the server pads it up to N bytes and rejects a value that is longer than N bytes.
        /// </summary>
        /// <param name="input">The <see cref="string"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public object Set(string input,
            PropertyHandlerSetOptions options) =>
            input;
    }
}
