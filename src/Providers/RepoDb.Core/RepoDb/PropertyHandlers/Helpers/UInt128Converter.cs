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
    /// A helper that converts the value of an unsigned 128-bit integer column (for example, the ClickHouse <c>UInt128</c> type), as returned by the driver, into a <see cref="UInt128"/>.
    /// </summary>
    internal static class UInt128Converter
    {
        /// <summary>
        /// Converts the value returned by the driver (a <see cref="BigInteger"/>, a <see cref="UInt128"/>, or any other integral type) into a <see cref="UInt128"/>.
        /// </summary>
        /// <param name="value">The value returned by the driver.</param>
        /// <returns>The <see cref="UInt128"/>, or <c>null</c> when the value is <c>null</c> or <see cref="DBNull"/>.</returns>
        /// <exception cref="OverflowException">Thrown when the value is negative or outside of the range of a <see cref="UInt128"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the type of the value is not supported.</exception>
        internal static UInt128? ToUInt128(object value)
        {
            switch (value)
            {
                case null:
                case DBNull _:
                    return null;
                case UInt128 number:
                    return number;
                case BigInteger big:
                    return checked((UInt128)big);
                case ulong number:
                    return number;
                case uint number:
                    return number;
                case long number:
                    return checked((UInt128)number);
                case int number:
                    return checked((UInt128)number);
                default:
                    throw new ArgumentException($"The type '{value.GetType()}' is not a supported unsigned 128-bit integer value.", nameof(value));
            }
        }
    }
}

#endif
