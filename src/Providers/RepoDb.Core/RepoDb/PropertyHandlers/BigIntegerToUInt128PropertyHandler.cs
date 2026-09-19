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
    /// A property handler that maps an unsigned 128-bit integer column (for example, the ClickHouse <c>UInt128</c> type, which drivers expose as a <see cref="BigInteger"/>) into a <see cref="UInt128"/> property.
    /// </summary>
    /// <remarks>
    /// Use <see cref="BigIntegerToNullableUInt128PropertyHandler"/> for a <see cref="Nullable{T}"/> of <see cref="UInt128"/> property.
    /// </remarks>
    public class BigIntegerToUInt128PropertyHandler : IPropertyHandler<object, UInt128>
    {
        /// <summary>
        /// Converts the unsigned 128-bit integer value, as returned by the driver, into a <see cref="UInt128"/>.
        /// </summary>
        /// <param name="input">The value of the column, as returned by the driver (a <see cref="BigInteger"/>, a <see cref="UInt128"/>, or another integral type).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The <see cref="UInt128"/>, or <c>0</c> when the value is <c>null</c>.</returns>
        /// <exception cref="OverflowException">Thrown when the value is negative or outside of the range of a <see cref="UInt128"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the type of the value is not supported.</exception>
        public UInt128 Get(object input,
            PropertyHandlerGetOptions options) =>
            UInt128Converter.ToUInt128(input) ?? UInt128.Zero;

        /// <summary>
        /// Converts the <see cref="UInt128"/> into a <see cref="BigInteger"/>, to be written into the column.
        /// </summary>
        /// <param name="input">The <see cref="UInt128"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="BigInteger"/> (boxed).</returns>
        public object Set(UInt128 input,
            PropertyHandlerSetOptions options) =>
            (BigInteger)input;
    }
}

#endif
