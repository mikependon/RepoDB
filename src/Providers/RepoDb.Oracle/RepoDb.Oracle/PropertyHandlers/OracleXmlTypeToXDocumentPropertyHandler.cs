#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Types;
using RepoDb.Interfaces;
using RepoDb.Options;
using System.Xml.Linq;

namespace RepoDb.PropertyHandlers.Oracle
{
    /// <summary>
    /// A property handler that maps the Oracle <c>XMLType</c> type into an <see cref="XDocument"/> property.
    /// </summary>
    public class OracleXmlTypeToXDocumentPropertyHandler : IPropertyHandler<object, XDocument>
    {
        /// <summary>
        /// Converts the Oracle <c>XMLType</c> value, as returned by the driver, into an <see cref="XDocument"/>.
        /// </summary>
        /// <param name="input">The <c>XMLType</c> value of the column, as returned by the driver (an <see cref="OracleXmlType"/> or a <see cref="string"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The parsed an <see cref="XDocument"/>, or <c>null</c> when the value is <c>null</c> or empty.</returns>
        public XDocument Get(object input,
            PropertyHandlerGetOptions options)
        {
            var text = OracleXmlTypeConverter.ToText(input);
            return text == null ? null : XDocument.Parse(text);
        }

        /// <summary>
        /// Converts an <see cref="XDocument"/> into its unformatted XML text, to be written into an Oracle <c>XMLType</c> column. The text is bound as a string, which Oracle converts implicitly, and is therefore subject to the string parameter size limit.
        /// </summary>
        /// <param name="input">The an <see cref="XDocument"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The unformatted XML text (as a <see cref="string"/>), or <c>null</c> when an <see cref="XDocument"/> is <c>null</c>.</returns>
        public object Set(XDocument input,
            PropertyHandlerSetOptions options) =>
            input?.ToString(SaveOptions.DisableFormatting);
    }
}
