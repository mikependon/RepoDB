#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System.Xml;

namespace RepoDb.PropertyHandlers
{
    /// <summary>
    /// A property handler that maps the SQL Server <c>xml</c> type into an <see cref="XmlDocument"/> property.
    /// </summary>
    public class XmlToXmlDocumentPropertyHandler : IPropertyHandler<string, XmlDocument>
    {
        /// <summary>
        /// Converts the SQL Server <c>xml</c> value, as returned by the database, into an <see cref="XmlDocument"/>.
        /// </summary>
        /// <param name="input">The <c>xml</c> value of the column, as a <see cref="string"/>.</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The loaded <see cref="XmlDocument"/>, or <c>null</c> when the value is <c>null</c> or empty.</returns>
        public XmlDocument Get(string input,
            PropertyHandlerGetOptions options)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            var document = new XmlDocument();
            document.LoadXml(input);
            return document;
        }

        /// <summary>
        /// Converts the <see cref="XmlDocument"/> into its XML text (<see cref="XmlNode.OuterXml"/>), to be written into a SQL Server <c>xml</c> column.
        /// </summary>
        /// <param name="input">The <see cref="XmlDocument"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The XML text, or <c>null</c> when the <see cref="XmlDocument"/> is <c>null</c>.</returns>
        public string Set(XmlDocument input,
            PropertyHandlerSetOptions options) =>
            input?.OuterXml;
    }
}
