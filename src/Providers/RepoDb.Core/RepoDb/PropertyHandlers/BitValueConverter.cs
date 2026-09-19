#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.PropertyHandlers
{
    /// <summary>
    /// A helper that converts the value of a multi-bit database column (for example, the MySQL <c>BIT(n)</c> type) into and from an <see cref="ulong"/>.
    /// </summary>
    internal static class BitValueConverter
    {
        /// <summary>
        /// Converts the value returned by the driver (an <see cref="ulong"/>, a big-endian <see cref="byte"/> array, a <see cref="bool"/>, or any other value convertible to an <see cref="ulong"/>) into an <see cref="ulong"/>.
        /// </summary>
        /// <param name="value">The value returned by the driver.</param>
        /// <returns>The <see cref="ulong"/> value, or <c>null</c> when the value is <c>null</c> or <see cref="DBNull"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when a <see cref="byte"/> array is longer than 8 bytes.</exception>
        internal static ulong? ToUInt64(object value)
        {
            switch (value)
            {
                case null:
                case DBNull _:
                    return null;
                case ulong number:
                    return number;
                case bool flag:
                    return flag ? 1UL : 0UL;
                case byte[] bytes:
                    return FromBigEndian(bytes);
                default:
                    return Convert.ToUInt64(value, System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Converts a big-endian <see cref="byte"/> array (the binary form of the MySQL <c>BIT(n)</c> type) into an <see cref="ulong"/>.
        /// </summary>
        /// <param name="bytes">The big-endian bytes, up to 8 bytes long.</param>
        /// <returns>The <see cref="ulong"/> value.</returns>
        /// <exception cref="ArgumentException">Thrown when the array is longer than 8 bytes.</exception>
        internal static ulong FromBigEndian(byte[] bytes)
        {
            if (bytes.Length > sizeof(ulong))
            {
                throw new ArgumentException($"The bit value ({bytes.Length} bytes) does not fit into a 64-bit value.", nameof(bytes));
            }
            var result = 0UL;
            foreach (var item in bytes)
            {
                result = (result << 8) | item;
            }
            return result;
        }

        /// <summary>
        /// Converts an <see cref="ulong"/> into its 8-byte big-endian representation.
        /// </summary>
        /// <param name="value">The <see cref="ulong"/> value.</param>
        /// <returns>The 8 big-endian bytes.</returns>
        internal static byte[] ToBigEndian(ulong value)
        {
            var result = new byte[sizeof(ulong)];
            for (var index = result.Length - 1; index >= 0; index--)
            {
                result[index] = (byte)value;
                value >>= 8;
            }
            return result;
        }
    }
}
