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
    /// A property handler that maps the ClickHouse <c>Decimal</c> types (<c>Decimal32</c>, <c>Decimal64</c>, <c>Decimal128</c> and <c>Decimal256</c>) into a <see cref="decimal"/> property. Values that are outside of the range of a <see cref="decimal"/> are rejected instead of being silently altered.
    /// </summary>
    /// <remarks>
    /// Use <see cref="ClickHouseDecimalToNullableDecimalPropertyHandler"/> for a <see cref="Nullable{T}"/> of <see cref="decimal"/> property, or <see cref="ClickHouseDecimalToStringPropertyHandler"/> to keep every digit of a wide <c>Decimal</c>.
    /// </remarks>
    public class ClickHouseDecimalToDecimalPropertyHandler : IPropertyHandler<object, decimal>
    {
        /// <summary>
        /// Converts the <c>Decimal</c> value, as returned by the driver, into a <see cref="decimal"/>.
        /// </summary>
        /// <param name="input">The value of the column, as returned by the driver (a <c>ClickHouseDecimal</c> or a <see cref="decimal"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The <see cref="decimal"/>, or <c>0</c> when the value is <c>null</c>.</returns>
        /// <exception cref="OverflowException">Thrown when the value is outside of the range of a <see cref="decimal"/>.</exception>
        public decimal Get(object input,
            PropertyHandlerGetOptions options) =>
            ClickHouseDecimalConverter.ToDecimal(input) ?? 0m;

        /// <summary>
        /// Converts the <see cref="decimal"/> into a <c>ClickHouseDecimal</c>, to be written into a <c>Decimal</c> column.
        /// </summary>
        /// <param name="input">The <see cref="decimal"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <c>ClickHouseDecimal</c> (boxed).</returns>
        public object Set(decimal input,
            PropertyHandlerSetOptions options) =>
            ClickHouseDecimalConverter.FromDecimal(input);
    }
}
