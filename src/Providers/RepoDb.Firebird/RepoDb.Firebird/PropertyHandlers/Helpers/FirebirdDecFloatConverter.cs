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
                    return ToPlainText(decFloat.ToString());
                case decimal number:
                    return number.ToString(CultureInfo.InvariantCulture);
                default:
                    throw new ArgumentException($"The type '{value.GetType()}' is not a supported DECFLOAT value.", nameof(value));
            }
        }

        /// <summary>
        /// Converts the scientific text form the driver produces for a <c>DECFLOAT</c> with a negative exponent (for example <c>12345E-2</c>) into its plain form (<c>123.45</c>). The text is returned as is when it is not such a value, or when it has more digits than a <see cref="decimal"/> can hold, so that the precision is never altered.
        /// </summary>
        private static string ToPlainText(string text)
        {
            var index = text.IndexOf("E-", StringComparison.OrdinalIgnoreCase);
            if (index <= 0)
            {
                return text;
            }
            var digits = 0;
            for (var i = 0; i < index; i++)
            {
                if (char.IsDigit(text[i]))
                {
                    digits++;
                }
            }
            if (digits > 28 ||
                decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) == false)
            {
                return text;
            }
            return number.ToString(CultureInfo.InvariantCulture);
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

        /// <summary>
        /// Converts the text form of a finite <c>DECFLOAT</c> value (for example <c>123.45</c> or <c>1.5E+20</c>) into an <see cref="FbDecFloat"/>, keeping its coefficient and scale.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>The <see cref="FbDecFloat"/> (boxed), or the text itself when it is not a finite number (for example <c>NaN</c>), to be converted by the server.</returns>
        internal static object FromText(string text)
        {
            var match = System.Text.RegularExpressions.Regex.Match(text ?? string.Empty,
                @"^\s*([+-]?)(\d*)(?:\.(\d*))?(?:[eE]([+-]?\d+))?\s*$",
                System.Text.RegularExpressions.RegexOptions.CultureInvariant);
            var digits = match.Success ? match.Groups[2].Value + match.Groups[3].Value : string.Empty;
            if (digits.Length == 0 ||
                (match.Groups[4].Success && int.TryParse(match.Groups[4].Value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out _) == false))
            {
                return text;
            }
            var coefficient = BigInteger.Parse(digits, CultureInfo.InvariantCulture);
            if (string.Equals(match.Groups[1].Value, "-", StringComparison.Ordinal))
            {
                coefficient = -coefficient;
            }
            var exponent = match.Groups[4].Success ? int.Parse(match.Groups[4].Value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture) : 0;
            return new FbDecFloat(coefficient, exponent - match.Groups[3].Value.Length);
        }
    }
}
