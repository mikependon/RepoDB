#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using IBM.Data.DB2Types;
using System;
using System.Globalization;

namespace RepoDb.PropertyHandlers.Db2
{
    /// <summary>
    /// A helper that converts the value of a Db2 <c>DECFLOAT</c> column, as returned by the driver, into a <see cref="DB2DecimalFloat"/>.
    /// </summary>
    internal static class Db2DecimalFloatConverter
    {
        /// <summary>
        /// Converts the value returned by the driver (a <see cref="DB2DecimalFloat"/>, a <see cref="decimal"/>, a <see cref="double"/>, or a <see cref="string"/>) into a <see cref="DB2DecimalFloat"/>.
        /// </summary>
        /// <param name="value">The value returned by the driver.</param>
        /// <returns>The <see cref="DB2DecimalFloat"/>, or <see cref="DB2DecimalFloat.Null"/> when the value is <c>null</c>, <see cref="DBNull"/> or empty.</returns>
        /// <exception cref="ArgumentException">Thrown when the type of the value is not supported.</exception>
        internal static DB2DecimalFloat ToDecimalFloat(object value)
        {
            switch (value)
            {
                case null:
                case DBNull _:
                    return DB2DecimalFloat.Null;
                case DB2DecimalFloat decimalFloat:
                    return decimalFloat;
                case decimal number:
                    return new DB2DecimalFloat(number);
                case double number:
                    return new DB2DecimalFloat(number);
                case string text:
                    return text.Length == 0 ? DB2DecimalFloat.Null : DB2DecimalFloat.Parse(text);
                default:
                    throw new ArgumentException($"The type '{value.GetType()}' is not a supported DECFLOAT value.", nameof(value));
            }
        }
    }
}
