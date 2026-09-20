#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using RepoDb.PropertyHandlers.Oracle;
using System;
using System.Linq;
using System.Xml;

namespace RepoDb.Oracle.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestOracleXmlTypeToXmlDocumentPropertyHandler
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
        public void TestOracleXmlTypeToXmlDocumentPropertyHandlerSet()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleXmlTypeToXmlDocumentPropertyHandler();

                // Act
                var result = handler.Set(Create(), null);

                // Assert
                Assert.AreEqual(Xml, result);
            }
        }

        [TestMethod]
        public void TestOracleXmlTypeToXmlDocumentPropertyHandlerSetWithNull()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleXmlTypeToXmlDocumentPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestOracleXmlTypeToXmlDocumentPropertyHandlerGetWithString()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleXmlTypeToXmlDocumentPropertyHandler();

                // Act
                var result = handler.Get(Xml, null);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(Xml, ToText(result));
            }
        }

        [TestMethod]
        public void TestOracleXmlTypeToXmlDocumentPropertyHandlerGetWithNullOrEmpty()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleXmlTypeToXmlDocumentPropertyHandler();

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
        public void TestOracleXmlTypeToXmlDocumentPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new OracleXmlTypeToXmlDocumentPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestOracleXmlTypeToXmlDocumentPropertyHandlerInsertAndQuery()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new OracleXmlDocumentEntity { ColumnXml = Create() };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<OracleXmlDocumentEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNotNull(result.ColumnXml);
                Assert.AreEqual(Xml, ToText(result.ColumnXml));
            }
        }

        [TestMethod]
        public void TestOracleXmlTypeToXmlDocumentPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new OracleXmlDocumentEntity { ColumnXml = null };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity));
                var result = connection.Query<OracleXmlDocumentEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnXml);
            }
        }
    }
}
