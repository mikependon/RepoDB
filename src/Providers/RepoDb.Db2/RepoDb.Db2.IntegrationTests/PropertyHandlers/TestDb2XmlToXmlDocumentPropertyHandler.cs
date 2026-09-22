#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using IBM.Data.Db2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using RepoDb.PropertyHandlers.Db2;
using System;
using System.Linq;
using System.Xml;

namespace RepoDb.Db2.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestDb2XmlToXmlDocumentPropertyHandler
    {
        private const string Xml = "<root><item id=\"1\">One</item><item id=\"2\">Two</item></root>";

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        private static XmlDocument Create()
        {
            var document = new XmlDocument();
            document.LoadXml(Xml);
            return document;
        }

        private static string ToText(XmlDocument value) => value.OuterXml;

        [TestMethod]
        public void TestDb2XmlToXmlDocumentPropertyHandlerSet()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2XmlToXmlDocumentPropertyHandler();

                // Act
                var result = handler.Set(Create(), null);

                // Assert
                Assert.AreEqual(Xml, result);
            }
        }

        [TestMethod]
        public void TestDb2XmlToXmlDocumentPropertyHandlerSetWithNull()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2XmlToXmlDocumentPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestDb2XmlToXmlDocumentPropertyHandlerGetWithString()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2XmlToXmlDocumentPropertyHandler();

                // Act
                var result = handler.Get(Xml, null);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(Xml, ToText(result), StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestDb2XmlToXmlDocumentPropertyHandlerGetWithNullOrEmpty()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2XmlToXmlDocumentPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);
                var resultOfEmpty = handler.Get("", null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
                Assert.IsNull(resultOfEmpty);
            }
        }

        [TestMethod]
        public void TestDb2XmlToXmlDocumentPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var handler = new Db2XmlToXmlDocumentPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestDb2XmlToXmlDocumentPropertyHandlerInsertAndQuery()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var entity = new Db2XmlDocumentEntity { ColumnXml = Create() };

                // Act
                var id = Convert.ToInt32(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<Db2XmlDocumentEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNotNull(result.ColumnXml);
                Assert.AreEqual(Xml, ToText(result.ColumnXml), StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestDb2XmlToXmlDocumentPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Setup
                var entity = new Db2XmlDocumentEntity { ColumnXml = null };

                // Act
                var id = Convert.ToInt32(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<Db2XmlDocumentEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnXml);
            }
        }
    }
}
