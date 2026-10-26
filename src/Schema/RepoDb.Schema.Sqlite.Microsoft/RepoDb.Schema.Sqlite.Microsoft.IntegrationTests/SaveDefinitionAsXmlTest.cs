#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Sqlite.Microsoft.IntegrationTests
{
    [TestClass]
    public class SaveDefinitionAsXmlTest : SaveDefinitionTestBase
    {
        #region Methods

        [TestMethod]
        [DataRow(RelatedTableBehavior.TableOnly, 1)]
        [DataRow(RelatedTableBehavior.Parents, 2)]
        [DataRow(RelatedTableBehavior.Children, 2)]
        [DataRow(RelatedTableBehavior.ParentsAndChildren, 3)]
        public void TestSaveDefinitionAsXmlIncludesRequestedRelatedTables(RelatedTableBehavior behavior,
            int tableCount)
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsXml("Invoice", Filename, behavior);
            var actual = XDocument.Load(Filename);

            // Assert
            Assert.AreEqual(tableCount, actual.Root.Element("tables").Elements("item").Count());
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlReadsNumericPrecision()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsXml("Invoice", Filename);
            var document = XDocument.Load(Filename);
            var actual = document.Root.Element("tables").Elements("item")
                .Single(table => (string)table.Element("table").Element("name") == "Invoice");

            // Assert
            Assert.AreEqual("18", GetColumn(actual, "Amount").Element("type").Element("precision").Value);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlReadsNumericScale()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsXml("Invoice", Filename);
            var document = XDocument.Load(Filename);
            var actual = document.Root.Element("tables").Elements("item")
                .Single(table => (string)table.Element("table").Element("name") == "Invoice");

            // Assert
            Assert.AreEqual("4", GetColumn(actual, "Amount").Element("type").Element("scale").Value);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlReadsUnicodeMetadata()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsXml("Invoice", Filename);
            var document = XDocument.Load(Filename);
            var actual = document.Root.Element("tables").Elements("item")
                .Single(table => (string)table.Element("table").Element("name") == "Invoice");

            // Assert
            Assert.AreEqual("true", GetColumn(actual, "Name").Element("type").Element("unicode").Value);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlReadsCollation()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsXml("Invoice", Filename);
            var document = XDocument.Load(Filename);
            var actual = document.Root.Element("tables").Elements("item")
                .Single(table => (string)table.Element("table").Element("name") == "Invoice");

            // Assert
            Assert.AreEqual("NOCASE", GetColumn(actual, "Name").Element("collation").Value);
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlReadsIndexes()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsXml("Invoice", Filename);
            var document = XDocument.Load(Filename);
            var actual = document.Root.Element("tables").Elements("item")
                .Single(table => (string)table.Element("table").Element("name") == "Invoice");

            // Assert
            Assert.AreEqual(1, actual.Element("indexes").Elements("item").Count());
        }

        [TestMethod]
        public void TestSaveDefinitionAsXmlReadsCheckConstraints()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsXml("Invoice", Filename);
            var document = XDocument.Load(Filename);
            var actual = document.Root.Element("tables").Elements("item")
                .Single(table => (string)table.Element("table").Element("name") == "Invoice");

            // Assert
            Assert.AreEqual(1, actual.Element("checkConstraints").Elements("item").Count());
        }

        #endregion

        #region Helpers

        private static XElement GetColumn(XElement table,
            string name) =>
            table.Element("columns").Elements("item").Single(column => (string)column.Element("name") == name);

        #endregion
    }
}
