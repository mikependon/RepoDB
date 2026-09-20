#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

#if !NETSTANDARD2_0

using System;
using System.Numerics;

namespace RepoDb.PropertyHandlers
{
    /// <summary>
    /// A helper that converts the value of a 128-bit integer column (for example, the Firebird <c>INT128</c> type), as returned by the driver, into an <see cref="Int128"/>.
    /// </summary>
    internal static class Int128Converter
    {
        /// <summary>
        /// Converts the value returned by the driver (a <see cref="BigInteger"/>, an <see cref="Int128"/>, or a <see cref="long"/> or <see cref="int"/>) into an <see cref="Int128"/>.
        /// </summary>
        /// <param name="value">The value returned by the driver.</param>
        /// <returns>The <see cref="Int128"/>, or <c>null</c> when the value is <c>null</c> or <see cref="DBNull"/>.</returns>
        /// <exception cref="OverflowException">Thrown when a <see cref="BigInteger"/> is outside of the range of an <see cref="Int128"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the type of the value is not supported.</exception>
        internal static Int128? ToInt128(object value)
        {
            switch (value)
            {
                case null:
                case DBNull _:
                    return null;
                case Int128 number:
                    return number;
                case BigInteger big:
                    return (Int128)big;
                case long number:
                    return number;
                case int number:
                    return number;
                default:
                    throw new ArgumentException($"The type '{value.GetType()}' is not a supported 128-bit integer value.", nameof(value));
            }
        }
    }
}

#endif
