#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System;
using System.Collections;

namespace RepoDb.PropertyHandlers
{
    /// <summary>
    /// A property handler that maps a multi-bit database column (for example, the MySQL <c>BIT(n)</c> type) into a <see cref="BitArray"/> property.
    /// </summary>
    public class BitToBitArrayPropertyHandler : IPropertyHandler<object, BitArray>
    {
        private const int Length = sizeof(ulong) * 8;

        /// <summary>
        /// Converts the bit value, as returned by the driver, into a <see cref="BitArray"/> of 64 bits, where the index 0 is the least significant bit. As the width of the column is not known, unused high bits are <c>false</c>.
        /// </summary>
        /// <param name="input">The bit value of the column, as returned by the driver (an <see cref="ulong"/>, a big-endian <see cref="byte"/> array, or a <see cref="bool"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>A <see cref="BitArray"/> with 64 bits, or <c>null</c> when the value is <c>null</c>.</returns>
        public BitArray Get(object input,
            PropertyHandlerGetOptions options)
        {
            var value = BitValueConverter.ToUInt64(input);
            if (value == null)
            {
                return null;
            }
            var result = new BitArray(Length);
            for (var index = 0; index < Length; index++)
            {
                result[index] = ((value.Value >> index) & 1UL) != 0;
            }
            return result;
        }

        /// <summary>
        /// Converts the <see cref="BitArray"/> into an <see cref="ulong"/>, where the index 0 is the least significant bit, to be written into the column.
        /// </summary>
        /// <param name="input">The <see cref="BitArray"/> to write, up to 64 bits long.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="ulong"/> value (boxed), or <c>null</c> when the <see cref="BitArray"/> is <c>null</c>.</returns>
        /// <exception cref="ArgumentException">Thrown when the <see cref="BitArray"/> is longer than 64 bits.</exception>
        public object Set(BitArray input,
            PropertyHandlerSetOptions options)
        {
            if (input == null)
            {
                return null;
            }
            if (input.Length > Length)
            {
                throw new ArgumentException($"The bit array ({input.Length} bits) does not fit into a 64-bit value.", nameof(input));
            }
            var result = 0UL;
            for (var index = 0; index < input.Length; index++)
            {
                if (input[index])
                {
                    result |= 1UL << index;
                }
            }
            return result;
        }
    }
}
