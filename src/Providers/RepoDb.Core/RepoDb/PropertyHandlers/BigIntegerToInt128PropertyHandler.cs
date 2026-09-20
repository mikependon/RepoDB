#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

#if !NETSTANDARD2_0

using RepoDb.Interfaces;
using RepoDb.Options;
using System;
using System.Numerics;

namespace RepoDb.PropertyHandlers
{
    /// <summary>
    /// A property handler that maps a 128-bit integer column (for example, the Firebird <c>INT128</c> type, which drivers expose as a <see cref="BigInteger"/>) into an <see cref="Int128"/> property.
    /// </summary>
    /// <remarks>
    /// Use <see cref="BigIntegerToNullableInt128PropertyHandler"/> for a <see cref="Nullable{T}"/> of <see cref="Int128"/> property.
    /// </remarks>
    public class BigIntegerToInt128PropertyHandler : IPropertyHandler<object, Int128>
    {
        /// <summary>
        /// Converts the 128-bit integer value, as returned by the driver, into an <see cref="Int128"/>.
        /// </summary>
        /// <param name="input">The value of the column, as returned by the driver (a <see cref="BigInteger"/>, an <see cref="Int128"/>, or a <see cref="long"/> or <see cref="int"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The <see cref="Int128"/>, or <c>0</c> when the value is <c>null</c>.</returns>
        /// <exception cref="OverflowException">Thrown when the value is outside of the range of an <see cref="Int128"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the type of the value is not supported.</exception>
        public Int128 Get(object input,
            PropertyHandlerGetOptions options) =>
            Int128Converter.ToInt128(input) ?? Int128.Zero;

        /// <summary>
        /// Converts the <see cref="Int128"/> into a <see cref="BigInteger"/>, to be written into the column.
        /// </summary>
        /// <param name="input">The <see cref="Int128"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="BigInteger"/> (boxed).</returns>
        public object Set(Int128 input,
            PropertyHandlerSetOptions options) =>
            (BigInteger)input;
    }
}

#endif
