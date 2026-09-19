#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Types;
using RepoDb.Interfaces;
using RepoDb.Options;
using System.Xml;

namespace RepoDb.PropertyHandlers.Oracle
{
    /// <summary>
    /// A property handler that maps the Oracle <c>XMLType</c> type into an <see cref="XmlDocument"/> property.
    /// </summary>
    public class OracleXmlTypeToXmlDocumentPropertyHandler : IPropertyHandler<object, XmlDocument>
    {
        /// <summary>
        /// Converts the Oracle <c>XMLType</c> value, as returned by the driver, into an <see cref="XmlDocument"/>.
        /// </summary>
        /// <param name="input">The <c>XMLType</c> value of the column, as returned by the driver (an <see cref="OracleXmlType"/> or a <see cref="string"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The parsed an <see cref="XmlDocument"/>, or <c>null</c> when the value is <c>null</c> or empty.</returns>
        public XmlDocument Get(object input,
            PropertyHandlerGetOptions options)
        {
            var text = OracleXmlTypeConverter.ToText(input);
            if (text == null)
            {
                return null;
            }
            var document = new XmlDocument();
            document.LoadXml(text);
            return document;
        }

        /// <summary>
        /// Converts an <see cref="XmlDocument"/> into its unformatted XML text, to be written into an Oracle <c>XMLType</c> column. The text is bound as a string, which Oracle converts implicitly, and is therefore subject to the string parameter size limit.
        /// </summary>
        /// <param name="input">The an <see cref="XmlDocument"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The unformatted XML text (as a <see cref="string"/>), or <c>null</c> when an <see cref="XmlDocument"/> is <c>null</c>.</returns>
        public object Set(XmlDocument input,
            PropertyHandlerSetOptions options) =>
            input?.OuterXml;
    }
}
