#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using IBM.Data.DB2Types;
using RepoDb.Interfaces;
using RepoDb.Options;
using System.Xml.Linq;

namespace RepoDb.PropertyHandlers.Db2
{
    /// <summary>
    /// A property handler that maps the Db2 <c>XML</c> type into an <see cref="XDocument"/> property.
    /// </summary>
    public class Db2XmlToXDocumentPropertyHandler : IPropertyHandler<object, XDocument>
    {
        /// <summary>
        /// Converts the Db2 <c>XML</c> value, as returned by the driver, into an <see cref="XDocument"/>.
        /// </summary>
        /// <param name="input">The <c>XML</c> value of the column, as returned by the driver (a <see cref="DB2Xml"/> or a <see cref="string"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The parsed an <see cref="XDocument"/>, or <c>null</c> when the value is <c>null</c> or empty.</returns>
        public XDocument Get(object input,
            PropertyHandlerGetOptions options)
        {
            var text = Db2XmlConverter.ToText(input);
            return text == null ? null : XDocument.Parse(text);
        }

        /// <summary>
        /// Converts an <see cref="XDocument"/> into its unformatted XML text, to be written into a Db2 <c>XML</c> column. The text is bound as a string, so the parameter has to be accepted by Db2 for the <c>XML</c> column (for example by typing it as <c>DB2Type.Xml</c>).
        /// </summary>
        /// <param name="input">The an <see cref="XDocument"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The unformatted XML text (as a <see cref="string"/>), or <c>null</c> when an <see cref="XDocument"/> is <c>null</c>.</returns>
        public object Set(XDocument input,
            PropertyHandlerSetOptions options) =>
            input?.ToString(SaveOptions.DisableFormatting);
    }
}
