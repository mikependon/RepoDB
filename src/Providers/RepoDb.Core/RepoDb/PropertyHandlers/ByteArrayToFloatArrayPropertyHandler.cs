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
    /// A property handler that maps a binary-encoded vector column (for example, the MySQL <c>VECTOR</c> type, which is transferred as little-endian IEEE 754 single-precision values) into a <see cref="float"/> array property.
    /// </summary>
    public class ByteArrayToFloatArrayPropertyHandler : IPropertyHandler<byte[], float[]>
    {
        /// <summary>
        /// Converts the binary vector value, as returned by the database, into a <see cref="float"/> array.
        /// </summary>
        /// <param name="input">The binary value of the column, made of consecutive 4-byte little-endian single-precision values.</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>A <see cref="float"/> array with the elements of the vector, or <c>null</c> when the value is <c>null</c>.</returns>
        /// <exception cref="ArgumentException">Thrown when the length of the value is not a multiple of 4 bytes.</exception>
        public float[] Get(byte[] input,
            PropertyHandlerGetOptions options)
        {
            if (input == null)
            {
                return null;
            }
            if (input.Length % sizeof(float) != 0)
            {
                throw new ArgumentException($"The length of the vector value ({input.Length} bytes) is not a multiple of {sizeof(float)} bytes.", nameof(input));
            }
            var result = new float[input.Length / sizeof(float)];
            var buffer = new byte[sizeof(float)];
            for (var index = 0; index < result.Length; index++)
            {
                Buffer.BlockCopy(input, index * sizeof(float), buffer, 0, sizeof(float));
                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(buffer);
                }
                result[index] = BitConverter.ToSingle(buffer, 0);
            }
            return result;
        }

        /// <summary>
        /// Converts the <see cref="float"/> array into the binary vector value, to be written into the column.
        /// </summary>
        /// <param name="input">The <see cref="float"/> array to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The binary value made of consecutive 4-byte little-endian single-precision values, or <c>null</c> when the array is <c>null</c>.</returns>
        public byte[] Set(float[] input,
            PropertyHandlerSetOptions options)
        {
            if (input == null)
            {
                return null;
            }
            var result = new byte[input.Length * sizeof(float)];
            for (var index = 0; index < input.Length; index++)
            {
                var bytes = BitConverter.GetBytes(input[index]);
                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(bytes);
                }
                Buffer.BlockCopy(bytes, 0, result, index * sizeof(float), sizeof(float));
            }
            return result;
        }
    }
}
