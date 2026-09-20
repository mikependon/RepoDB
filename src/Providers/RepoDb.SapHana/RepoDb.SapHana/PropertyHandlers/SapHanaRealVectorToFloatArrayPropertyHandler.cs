#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System;

namespace RepoDb.PropertyHandlers.SapHana
{
    /// <summary>
    /// A property handler that maps the SAP HANA <c>REAL_VECTOR</c> type into a <see cref="float"/> array property.
    /// </summary>
    /// <remarks>
    /// The SAP HANA driver reads a <c>REAL_VECTOR</c> as a <see cref="float"/> array (<c>HanaDataReader.GetRealVector</c>) and, depending on how the value is fetched, it can also be surfaced in the binary vector format of SAP HANA. This handler accepts both. The <c>HALF_VECTOR</c> type is not supported by the driver, and therefore not by this handler.
    /// </remarks>
    public class SapHanaRealVectorToFloatArrayPropertyHandler : IPropertyHandler<object, float[]>
    {
        /// <summary>
        /// Converts the <c>REAL_VECTOR</c> value, as returned by the driver, into a <see cref="float"/> array.
        /// </summary>
        /// <param name="input">The <c>REAL_VECTOR</c> value of the column, as returned by the driver (a <see cref="float"/> array, or a <see cref="byte"/> array in the SAP HANA binary vector format: a 4-byte little-endian dimension count followed by the little-endian single-precision values; a <see cref="byte"/> array without the dimension count is also accepted).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>A <see cref="float"/> array with the elements of the vector, or <c>null</c> when the value is <c>null</c>.</returns>
        /// <exception cref="ArgumentException">Thrown when the type or the layout of the value is not supported.</exception>
        public float[] Get(object input,
            PropertyHandlerGetOptions options)
        {
            switch (input)
            {
                case null:
                case DBNull _:
                    return null;
                case float[] values:
                    return values;
                case byte[] bytes:
                    return FromBytes(bytes);
                default:
                    throw new ArgumentException($"The type '{input.GetType()}' is not a supported REAL_VECTOR value.", nameof(input));
            }
        }

        /// <summary>
        /// Passes the <see cref="float"/> array to the driver, which binds it as a <c>REAL_VECTOR</c> value. The parameter must be typed as <c>HanaDbType.RealVector</c> for the value to be written into a <c>REAL_VECTOR</c> column.
        /// </summary>
        /// <param name="input">The <see cref="float"/> array to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="float"/> array itself, or <c>null</c> when the array is <c>null</c>.</returns>
        public object Set(float[] input,
            PropertyHandlerSetOptions options) =>
            input;

        private static float[] FromBytes(byte[] bytes)
        {
            if (bytes.Length % sizeof(float) != 0)
            {
                throw new ArgumentException($"The length of the vector value ({bytes.Length} bytes) is not a multiple of {sizeof(float)} bytes.", nameof(bytes));
            }
            var offset = 0;
            if (bytes.Length >= sizeof(int))
            {
                var dimensions = ReadUInt32(bytes, 0);
                if (dimensions == (bytes.Length - sizeof(int)) / sizeof(float))
                {
                    offset = sizeof(int);
                }
            }
            var result = new float[(bytes.Length - offset) / sizeof(float)];
            var buffer = new byte[sizeof(float)];
            for (var index = 0; index < result.Length; index++)
            {
                Buffer.BlockCopy(bytes, offset + (index * sizeof(float)), buffer, 0, sizeof(float));
                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(buffer);
                }
                result[index] = BitConverter.ToSingle(buffer, 0);
            }
            return result;
        }

        private static uint ReadUInt32(byte[] bytes, int offset) =>
            (uint)(bytes[offset] | (bytes[offset + 1] << 8) | (bytes[offset + 2] << 16) | (bytes[offset + 3] << 24));
    }
}
