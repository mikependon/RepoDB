#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Types;
using System;

namespace RepoDb.PropertyHandlers.Oracle
{
    /// <summary>
    /// A helper that converts the value of an Oracle <c>XMLType</c> column, as returned by the driver, into XML text.
    /// </summary>
    internal static class OracleXmlTypeConverter
    {
        /// <summary>
        /// Converts the value returned by the driver (an <see cref="OracleXmlType"/> or a <see cref="string"/>) into XML text.
        /// </summary>
        /// <param name="value">The value returned by the driver.</param>
        /// <returns>The XML text, or <c>null</c> when the value is <c>null</c>, <see cref="DBNull"/>, empty or a null <see cref="OracleXmlType"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the type of the value is not supported.</exception>
        internal static string ToText(object value)
        {
            string text;
            switch (value)
            {
                case null:
                case DBNull _:
                    return null;
                case string content:
                    text = content;
                    break;
                case OracleXmlType xml:
                    text = xml.IsNull ? null : xml.Value;
                    break;
                default:
                    throw new ArgumentException($"The type '{value.GetType()}' is not a supported XMLType value.", nameof(value));
            }
            return string.IsNullOrEmpty(text) ? null : text;
        }
    }
}
