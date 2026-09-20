#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System;
using System.Net;

namespace RepoDb.PropertyHandlers
{
    /// <summary>
    /// A property handler that maps a text-based network address column (for example, the MariaDB <c>INET4</c> and <c>INET6</c> types) into an <see cref="IPAddress"/> property.
    /// </summary>
    public class StringToIPAddressPropertyHandler : IPropertyHandler<object, IPAddress>
    {
        /// <summary>
        /// Converts the network address value, as returned by the driver, into an <see cref="IPAddress"/>.
        /// </summary>
        /// <param name="input">The value of the column, as returned by the driver (a <see cref="string"/> such as <c>192.168.0.1</c> or <c>::1</c>, an <see cref="IPAddress"/>, or a 4-byte or 16-byte <see cref="byte"/> array).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The <see cref="IPAddress"/>, or <c>null</c> when the value is <c>null</c> or empty.</returns>
        /// <exception cref="ArgumentException">Thrown when the type of the value is not supported.</exception>
        /// <exception cref="FormatException">Thrown when the text is not a valid IP address.</exception>
        public IPAddress Get(object input,
            PropertyHandlerGetOptions options)
        {
            switch (input)
            {
                case null:
                case DBNull _:
                    return null;
                case IPAddress address:
                    return address;
                case string text:
                    return text.Length == 0 ? null : IPAddress.Parse(text);
                case byte[] bytes:
                    return bytes.Length == 0 ? null : new IPAddress(bytes);
                default:
                    throw new ArgumentException($"The type '{input.GetType()}' is not a supported network address value.", nameof(input));
            }
        }

        /// <summary>
        /// Converts the <see cref="IPAddress"/> into its text form (for example <c>192.168.0.1</c> or <c>::1</c>), to be written into the column.
        /// </summary>
        /// <param name="input">The <see cref="IPAddress"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The text form of the address (as a <see cref="string"/>), or <c>null</c> when the <see cref="IPAddress"/> is <c>null</c>.</returns>
        public object Set(IPAddress input,
            PropertyHandlerSetOptions options) =>
            input?.ToString();
    }
}
