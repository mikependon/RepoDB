#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using ClickHouse.Driver.Numerics;
using System;
using System.Globalization;

namespace RepoDb.PropertyHandlers.ClickHouse
{
    /// <summary>
    /// A helper that converts the value of a ClickHouse <c>Decimal</c> column (<c>Decimal32</c>, <c>Decimal64</c>, <c>Decimal128</c> and <c>Decimal256</c>), as returned by the driver, into .NET values, and back.
    /// </summary>
    internal static class ClickHouseDecimalConverter
    {
        /// <summary>
        /// Converts the value returned by the driver into a <see cref="decimal"/>.
        /// </summary>
        /// <param name="value">The value returned by the driver (a <see cref="ClickHouseDecimal"/>, a <see cref="decimal"/>, or any other <see cref="IConvertible"/> numeric value).</param>
        /// <returns>The <see cref="decimal"/>, or <c>null</c> when the value is <c>null</c> or <see cref="DBNull"/>.</returns>
        /// <exception cref="OverflowException">Thrown when the value is outside of the range of a <see cref="decimal"/> (possible with <c>Decimal128</c> and <c>Decimal256</c>).</exception>
        internal static decimal? ToDecimal(object value)
        {
            switch (value)
            {
                case null:
                case DBNull _:
                    return null;
                case decimal number:
                    return number;
                case ClickHouseDecimal number:
                    return (decimal)number;
                default:
                    return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Converts the value returned by the driver into its exact, culture-invariant string representation.
        /// </summary>
        /// <param name="value">The value returned by the driver (a <see cref="ClickHouseDecimal"/>, a <see cref="decimal"/>, or any other <see cref="IConvertible"/> numeric value).</param>
        /// <returns>The string, or <c>null</c> when the value is <c>null</c> or <see cref="DBNull"/>.</returns>
        internal static string ToInvariantString(object value)
        {
            switch (value)
            {
                case null:
                case DBNull _:
                    return null;
                case ClickHouseDecimal number:
                    return number.ToString(CultureInfo.InvariantCulture);
                case decimal number:
                    return number.ToString(CultureInfo.InvariantCulture);
                default:
                    return Convert.ToString(value, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Converts the <see cref="decimal"/> into a <see cref="ClickHouseDecimal"/>.
        /// </summary>
        /// <param name="value">The <see cref="decimal"/> to convert.</param>
        /// <returns>The <see cref="ClickHouseDecimal"/> (boxed).</returns>
        internal static object FromDecimal(decimal value) =>
            new ClickHouseDecimal(value);

        /// <summary>
        /// Parses the string into a <see cref="ClickHouseDecimal"/>.
        /// </summary>
        /// <param name="value">The culture-invariant numeric string to parse.</param>
        /// <returns>The <see cref="ClickHouseDecimal"/> (boxed), or <c>null</c> when the value is <c>null</c> or empty.</returns>
        /// <exception cref="FormatException">Thrown when the string is not a valid number.</exception>
        internal static object FromString(string value) =>
            string.IsNullOrWhiteSpace(value) ? null : (object)ClickHouseDecimal.Parse(value.Trim(), CultureInfo.InvariantCulture);
    }
}
