#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using IBM.Data.DB2Types;
using System;

namespace RepoDb.PropertyHandlers.Db2
{
    /// <summary>
    /// A helper that converts the value of a Db2 <c>XML</c> column, as returned by the driver, into XML text.
    /// </summary>
    internal static class Db2XmlConverter
    {
        /// <summary>
        /// Converts the value returned by the driver (a <see cref="DB2Xml"/> or a <see cref="string"/>) into XML text.
        /// </summary>
        /// <param name="value">The value returned by the driver.</param>
        /// <returns>The XML text, or <c>null</c> when the value is <c>null</c>, <see cref="DBNull"/>, empty or a null <see cref="DB2Xml"/>.</returns>
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
                case DB2Xml xml:
                    text = xml.IsNull ? null : xml.GetString();
                    break;
                default:
                    throw new ArgumentException($"The type '{value.GetType()}' is not a supported XML value.", nameof(value));
            }
            return string.IsNullOrEmpty(text) ? null : text;
        }
    }
}
