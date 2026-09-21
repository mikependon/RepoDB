#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using RepoDb.Resolvers;
using System;

namespace RepoDb.PropertyHandlers.Firebird
{
    /// <summary>
    /// A property handler that maps the Firebird <c>DECFLOAT(16)</c> and <c>DECFLOAT(34)</c> types into a <see cref="decimal"/> property. A <see cref="decimal"/> cannot hold every <c>DECFLOAT</c> value (34 digits, wide exponent, <c>NaN</c> and infinity), so such values are rejected instead of being silently altered.
    /// </summary>
    /// <remarks>
    /// Use <see cref="FirebirdDecFloatToNullableDecimalPropertyHandler"/> for a <see cref="Nullable{T}"/> of <see cref="decimal"/> property.
    /// </remarks>
    public class FirebirdDecFloatToDecimalPropertyHandler : IPropertyHandler<object, decimal>
    {
        private static readonly FirebirdDecFloatToDecimalResolver decFloatToDecimalResolver = new FirebirdDecFloatToDecimalResolver();
        private static readonly DecimalToFirebirdDecFloatResolver decimalToDecFloatResolver = new DecimalToFirebirdDecFloatResolver();

        /// <summary>
        /// Converts the <c>DECFLOAT</c> value, as returned by the driver, into a <see cref="decimal"/>.
        /// </summary>
        /// <param name="input">The <c>DECFLOAT</c> value of the column, as returned by the driver (an <c>FbDecFloat</c> or a <see cref="decimal"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The <see cref="decimal"/>, or <c>0</c> when the value is <c>null</c>.</returns>
        /// <exception cref="OverflowException">Thrown when the value is outside of the range of a <see cref="decimal"/>.</exception>
        /// <exception cref="FormatException">Thrown when the value is <c>NaN</c> or infinity.</exception>
        public decimal Get(object input,
            PropertyHandlerGetOptions options) =>
            decFloatToDecimalResolver.Resolve(input) ?? 0m;

        /// <summary>
        /// Converts the <see cref="decimal"/> into an <c>FbDecFloat</c>, to be written into a <c>DECFLOAT</c> column.
        /// </summary>
        /// <param name="input">The <see cref="decimal"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <c>FbDecFloat</c> (boxed).</returns>
        public object Set(decimal input,
            PropertyHandlerSetOptions options) =>
            decimalToDecFloatResolver.Resolve(input);
    }
}
