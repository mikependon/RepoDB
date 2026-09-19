#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System;

namespace RepoDb.PropertyHandlers
{
    /// <summary>
    /// A property handler that maps a multi-bit database column (for example, the MySQL <c>BIT(n)</c> type) into a <see cref="byte"/> array property, in big-endian order.
    /// </summary>
    public class BitToByteArrayPropertyHandler : IPropertyHandler<object, byte[]>
    {
        /// <summary>
        /// Converts the bit value, as returned by the driver, into a big-endian <see cref="byte"/> array. As the width of the column is not known, the array is always 8 bytes long, with leading zero bytes for the unused high bits.
        /// </summary>
        /// <param name="input">The bit value of the column, as returned by the driver (an <see cref="ulong"/>, a big-endian <see cref="byte"/> array, or a <see cref="bool"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The 8 big-endian bytes of the value, or <c>null</c> when the value is <c>null</c>.</returns>
        public byte[] Get(object input,
            PropertyHandlerGetOptions options)
        {
            var value = BitValueConverter.ToUInt64(input);
            return value == null ? null : BitValueConverter.ToBigEndian(value.Value);
        }

        /// <summary>
        /// Converts the big-endian <see cref="byte"/> array into an <see cref="ulong"/>, to be written into the column.
        /// </summary>
        /// <param name="input">The big-endian <see cref="byte"/> array to write, up to 8 bytes long.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="ulong"/> value (boxed), or <c>null</c> when the array is <c>null</c>.</returns>
        /// <exception cref="ArgumentException">Thrown when the array is longer than 8 bytes.</exception>
        public object Set(byte[] input,
            PropertyHandlerSetOptions options) =>
            input == null ? null : (object)BitValueConverter.FromBigEndian(input);
    }
}
