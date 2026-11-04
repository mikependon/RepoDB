#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Xml.Linq;
using System.Xml;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers;

namespace RepoDb.UnitTests.PropertyHandlers
{
    [TestClass]
    public class XmlPropertyHandlerTest
    {
        [TestMethod]
        public void TestXDocumentNull()
        {
            // Setup
            var handler = new XmlToXDocumentPropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestXDocumentEmpty()
        {
            // Setup
            var handler = new XmlToXDocumentPropertyHandler();

            // Act
            var actual = handler.Get(string.Empty, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestXDocumentReadsRoot()
        {
            // Setup
            var handler = new XmlToXDocumentPropertyHandler();

            // Act
            var actual = handler.Get("<root><value>42</value></root>", null);

            // Assert
            Assert.AreEqual("root", actual.Root.Name.LocalName, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestXDocumentWritesXml()
        {
            // Setup
            var handler = new XmlToXDocumentPropertyHandler();
            var input = XDocument.Parse("<root>42</root>");

            // Act
            var actual = handler.Set(input, null);

            // Assert
            Assert.AreEqual("<root>42</root>", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestXDocumentWritesNull()
        {
            // Setup
            var handler = new XmlToXDocumentPropertyHandler();

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestXDocumentRejectsMalformedXml()
        {
            // Setup
            var handler = new XmlToXDocumentPropertyHandler();

            // Act
            Action act = () => handler.Get("<root>", null);

            // Assert
            Assert.ThrowsExactly<XmlException>(act);
        }

        [TestMethod]
        public void TestXElementNull()
        {
            // Setup
            var handler = new XmlToXElementPropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestXElementEmpty()
        {
            // Setup
            var handler = new XmlToXElementPropertyHandler();

            // Act
            var actual = handler.Get(string.Empty, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestXElementReadsRoot()
        {
            // Setup
            var handler = new XmlToXElementPropertyHandler();

            // Act
            var actual = handler.Get("<root><value>42</value></root>", null);

            // Assert
            Assert.AreEqual("root", actual.Name.LocalName, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestXElementWritesXml()
        {
            // Setup
            var handler = new XmlToXElementPropertyHandler();
            var input = XElement.Parse("<root>42</root>");

            // Act
            var actual = handler.Set(input, null);

            // Assert
            Assert.AreEqual("<root>42</root>", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestXElementWritesNull()
        {
            // Setup
            var handler = new XmlToXElementPropertyHandler();

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestXElementRejectsMalformedXml()
        {
            // Setup
            var handler = new XmlToXElementPropertyHandler();

            // Act
            Action act = () => handler.Get("<root>", null);

            // Assert
            Assert.ThrowsExactly<XmlException>(act);
        }

        [TestMethod]
        public void TestXmlDocumentNull()
        {
            // Setup
            var handler = new XmlToXmlDocumentPropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestXmlDocumentEmpty()
        {
            // Setup
            var handler = new XmlToXmlDocumentPropertyHandler();

            // Act
            var actual = handler.Get(string.Empty, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestXmlDocumentReadsRoot()
        {
            // Setup
            var handler = new XmlToXmlDocumentPropertyHandler();

            // Act
            var actual = handler.Get("<root><value>42</value></root>", null);

            // Assert
            Assert.AreEqual("root", actual.DocumentElement.Name, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestXmlDocumentWritesXml()
        {
            // Setup
            var handler = new XmlToXmlDocumentPropertyHandler();
            var input = new XmlDocument();
            input.LoadXml("<root>42</root>");

            // Act
            var actual = handler.Set(input, null);

            // Assert
            Assert.AreEqual("<root>42</root>", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestXmlDocumentWritesNull()
        {
            // Setup
            var handler = new XmlToXmlDocumentPropertyHandler();

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestXmlDocumentRejectsMalformedXml()
        {
            // Setup
            var handler = new XmlToXmlDocumentPropertyHandler();

            // Act
            Action act = () => handler.Get("<root>", null);

            // Assert
            Assert.ThrowsExactly<XmlException>(act);
        }

    }
}
