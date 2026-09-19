#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System.Xml.Linq;

namespace RepoDb.PropertyHandlers
{
    /// <summary>
    /// A property handler that maps the SQL Server <c>xml</c> type into an <see cref="XDocument"/> property.
    /// </summary>
    public class XmlToXDocumentPropertyHandler : IPropertyHandler<string, XDocument>
    {
        /// <summary>
        /// Converts the SQL Server <c>xml</c> value, as returned by the database, into an <see cref="XDocument"/>.
        /// </summary>
        /// <param name="input">The <c>xml</c> value of the column, as a <see cref="string"/>.</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The parsed <see cref="XDocument"/>, or <c>null</c> when the value is <c>null</c> or empty.</returns>
        public XDocument Get(string input,
            PropertyHandlerGetOptions options) =>
            string.IsNullOrEmpty(input) ? null : XDocument.Parse(input);

        /// <summary>
        /// Converts the <see cref="XDocument"/> into its unformatted XML text, to be written into a SQL Server <c>xml</c> column.
        /// </summary>
        /// <param name="input">The <see cref="XDocument"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The unformatted XML text, or <c>null</c> when the <see cref="XDocument"/> is <c>null</c>.</returns>
        public string Set(XDocument input,
            PropertyHandlerSetOptions options) =>
            input?.ToString(SaveOptions.DisableFormatting);
    }
}
