#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.Types;
using System;
using System.Globalization;
using System.Numerics;

namespace RepoDb.PropertyHandlers.Firebird
{
    /// <summary>
    /// A helper that converts the value of a Firebird <c>DECFLOAT</c> column, as returned by the driver, into text and <see cref="decimal"/> values, and back.
    /// </summary>
    internal static class FirebirdDecFloatConverter
    {
        /// <summary>
        /// Converts the value returned by the driver (an <see cref="FbDecFloat"/>, a <see cref="decimal"/> or a <see cref="string"/>) into text.
        /// </summary>
        /// <param name="value">The value returned by the driver.</param>
        /// <returns>The text form of the value, or <c>null</c> when the value is <c>null</c> or <see cref="DBNull"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the type of the value is not supported.</exception>
        internal static string ToText(object value)
        {
            switch (value)
            {
                case null:
                case DBNull _:
                    return null;
                case string text:
                    return text;
                case FbDecFloat decFloat:
                    return decFloat.ToString();
                case decimal number:
                    return number.ToString(CultureInfo.InvariantCulture);
                default:
                    throw new ArgumentException($"The type '{value.GetType()}' is not a supported DECFLOAT value.", nameof(value));
            }
        }

        /// <summary>
        /// Converts the value returned by the driver (an <see cref="FbDecFloat"/>, a <see cref="decimal"/> or a <see cref="string"/>) into a <see cref="decimal"/>.
        /// </summary>
        /// <param name="value">The value returned by the driver.</param>
        /// <returns>The <see cref="decimal"/>, or <c>null</c> when the value is <c>null</c> or <see cref="DBNull"/>.</returns>
        /// <exception cref="OverflowException">Thrown when the value is outside of the range of a <see cref="decimal"/>.</exception>
        /// <exception cref="FormatException">Thrown when the value is not a finite number (<c>NaN</c> or infinity).</exception>
        internal static decimal? ToDecimal(object value)
        {
            if (value is decimal number)
            {
                return number;
            }
            var text = ToText(value);
            return text == null ? (decimal?)null : decimal.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a <see cref="decimal"/> into an <see cref="FbDecFloat"/>, keeping its coefficient and scale.
        /// </summary>
        /// <param name="value">The <see cref="decimal"/> to convert.</param>
        /// <returns>The <see cref="FbDecFloat"/> (boxed).</returns>
        internal static object FromDecimal(decimal value)
        {
            var bits = decimal.GetBits(value);
            var coefficient = (new BigInteger((uint)bits[2]) << 64) | (new BigInteger((uint)bits[1]) << 32) | new BigInteger((uint)bits[0]);
            if (bits[3] < 0)
            {
                coefficient = -coefficient;
            }
            var scale = (bits[3] >> 16) & 0xFF;
            return new FbDecFloat(coefficient, -scale);
        }
    }
}
