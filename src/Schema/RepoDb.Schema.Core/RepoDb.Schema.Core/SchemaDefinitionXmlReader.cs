#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Linq;

namespace RepoDb.Schema
{
    /// <summary>
    /// Reads the typed XML representation produced by SaveDefinitionAsXml.
    /// </summary>
    internal static class SchemaDefinitionXmlReader
    {
        #region Public Methods

        /// <summary>
        /// Reads an XML document without permitting external entities or document type declarations.
        /// </summary>
        /// <param name="content">The XML content of the definition file.</param>
        /// <returns>The definition document.</returns>
        internal static JsonObject Parse(string content)
        {
            using (var text = new StringReader(content))
            using (var reader = XmlReader.Create(text,
                new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null }))
            {
                var root = XDocument.Load(reader).Root;
                if (root?.Name != "repoDbSchema")
                    throw new InvalidDataException("Invalid schema XML root.");
                return Node(root,
                    0) as JsonObject ?? throw new InvalidDataException("The schema XML root must be an object.");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Restores the explicit scalar and collection types used by the writer.
        /// </summary>
        /// <param name="element">The XML element to be restored.</param>
        /// <param name="depth">The nesting depth of the element, limited to prevent excessive recursion.</param>
        /// <returns>The node that the element represents.</returns>
        private static JsonNode Node(XElement element,
            int depth)
        {
            if (depth > 64)
                throw new InvalidDataException("The definition nesting limit was exceeded.");
            var type = (string)element.Attribute("type");
            if ((string.Equals(type,
                "object",
                StringComparison.Ordinal) || string.Equals(type,
                    "array",
                    StringComparison.Ordinal)) && element.Nodes().OfType<XText>().Any(t => !string.IsNullOrWhiteSpace(t.Value)))
                throw new InvalidDataException("An XML collection contains scalar text.");
            if (string.Equals(type,
                "object",
                StringComparison.Ordinal))
            {
                return ReadObject(element,
                    depth);
            }

            if (string.Equals(type,
                "array",
                StringComparison.Ordinal))
            {
                return ReadArray(element,
                    depth);
            }

            if (element.Elements().Any())
                throw new InvalidDataException("An XML scalar contains elements.");
            switch (type)
            {
                case "null":
                    if (element.Value.Length != 0)
                        throw new InvalidDataException("Invalid XML null.");
                    return null;
                case "string":
                    return JsonValue.Create(element.Value);
                case "boolean":
                    if (!string.Equals(element.Value,
                        "true",
                        StringComparison.Ordinal) && !string.Equals(element.Value,
                            "false",
                            StringComparison.Ordinal))
                        throw new InvalidDataException("Invalid XML boolean.");
                    return JsonValue.Create(string.Equals(element.Value,
                        "true",
                        StringComparison.Ordinal));
                case "integer":
                    var number = JsonNode.Parse(element.Value);
                    if (!(number is JsonValue value) || !value.TryGetValue<long>(out _))
                        throw new InvalidDataException("Invalid XML integer.");
                    return number;
                default:
                    throw new InvalidDataException("Unknown XML value type.");
            }
        }

        /// <summary>
        /// Reads and validates XML object properties recursively.
        /// </summary>
        /// <param name="element">The XML element to be restored.</param>
        /// <param name="depth">The nesting depth of the element, limited to prevent excessive recursion.</param>
        /// <returns>The reconstructed JSON collection.</returns>
        private static JsonNode ReadObject(XElement element,
            int depth)
        {
            var result = new JsonObject();
            foreach (var child in element.Elements())
            {
                if (child.Name.Namespace != XNamespace.None || result.ContainsKey(child.Name.LocalName))
                    throw new InvalidDataException("Invalid or duplicate XML property.");
                result.Add(child.Name.LocalName,
                    Node(child,
                        depth + 1));
            }

            return result;
        }

        /// <summary>
        /// Reads and validates XML array items recursively.
        /// </summary>
        /// <param name="element">The XML element to be restored.</param>
        /// <param name="depth">The nesting depth of the element, limited to prevent excessive recursion.</param>
        /// <returns>The reconstructed JSON collection.</returns>
        private static JsonNode ReadArray(XElement element,
            int depth)
        {
            var result = new JsonArray();
            foreach (var child in element.Elements())
            {
                if (child.Name != "item")
                    throw new InvalidDataException("Invalid XML array item.");
                result.Add(Node(child,
                    depth + 1));
            }

            return result;
        }

        #endregion
    }
}
