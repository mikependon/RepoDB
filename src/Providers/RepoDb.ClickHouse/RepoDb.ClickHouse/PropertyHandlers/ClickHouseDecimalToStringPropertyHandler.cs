#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System;

namespace RepoDb.PropertyHandlers.ClickHouse
{
    /// <summary>
    /// A property handler that maps the ClickHouse <c>Decimal</c> types (<c>Decimal32</c>, <c>Decimal64</c>, <c>Decimal128</c> and <c>Decimal256</c>) into a <see cref="string"/> property. The string keeps every digit and the scale of the value, which a <see cref="decimal"/> cannot do for the wide <c>Decimal128</c> and <c>Decimal256</c> types.
    /// </summary>
    public class ClickHouseDecimalToStringPropertyHandler : IPropertyHandler<object, string>
    {
        /// <summary>
        /// Converts the <c>Decimal</c> value, as returned by the driver, into a culture-invariant <see cref="string"/>.
        /// </summary>
        /// <param name="input">The value of the column, as returned by the driver (a <c>ClickHouseDecimal</c> or a <see cref="decimal"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The <see cref="string"/>, or <c>null</c> when the value is <c>null</c>.</returns>
        public string Get(object input,
            PropertyHandlerGetOptions options) =>
            ClickHouseDecimalConverter.ToInvariantString(input);

        /// <summary>
        /// Parses the culture-invariant <see cref="string"/> into a <c>ClickHouseDecimal</c>, to be written into a <c>Decimal</c> column.
        /// </summary>
        /// <param name="input">The numeric <see cref="string"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <c>ClickHouseDecimal</c> (boxed), or <c>null</c> when the string is <c>null</c> or empty.</returns>
        /// <exception cref="FormatException">Thrown when the string is not a valid number.</exception>
        public object Set(string input,
            PropertyHandlerSetOptions options) =>
            ClickHouseDecimalConverter.FromString(input);
    }
}
