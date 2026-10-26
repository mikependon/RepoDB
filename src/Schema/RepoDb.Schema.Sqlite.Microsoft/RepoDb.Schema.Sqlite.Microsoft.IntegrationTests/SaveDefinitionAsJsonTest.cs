#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Sqlite.Microsoft.IntegrationTests
{
    [TestClass]
    public class SaveDefinitionAsJsonTest : SaveDefinitionTestBase
    {
        #region Methods

        [TestMethod]
        [DataRow(RelatedTableBehavior.TableOnly, 1)]
        [DataRow(RelatedTableBehavior.Parents, 2)]
        [DataRow(RelatedTableBehavior.Children, 2)]
        [DataRow(RelatedTableBehavior.ParentsAndChildren, 3)]
        public void TestSaveDefinitionAsJsonIncludesRequestedRelatedTables(RelatedTableBehavior behavior,
            int tableCount)
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsJson("Invoice", Filename, behavior);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual(tableCount, actual["tables"].AsArray().Count);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonReadsNumericPrecision()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsJson("Invoice", Filename);
            var document = JsonNode.Parse(File.ReadAllText(Filename));
            var actual = document["tables"].AsArray().Single(table => (string)table["table"]["name"] == "Invoice");

            // Assert
            Assert.AreEqual(18, (int)actual["columns"].AsArray().Single(column => (string)column["name"] == "Amount")["type"]["precision"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonReadsNumericScale()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsJson("Invoice", Filename);
            var document = JsonNode.Parse(File.ReadAllText(Filename));
            var actual = document["tables"].AsArray().Single(table => (string)table["table"]["name"] == "Invoice");

            // Assert
            Assert.AreEqual(4, (int)actual["columns"].AsArray().Single(column => (string)column["name"] == "Amount")["type"]["scale"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonReadsUnicodeMetadata()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsJson("Invoice", Filename);
            var document = JsonNode.Parse(File.ReadAllText(Filename));
            var actual = document["tables"].AsArray().Single(table => (string)table["table"]["name"] == "Invoice");

            // Assert
            Assert.IsTrue((bool)actual["columns"].AsArray().Single(column => (string)column["name"] == "Name")["type"]["unicode"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonReadsCollation()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsJson("Invoice", Filename);
            var document = JsonNode.Parse(File.ReadAllText(Filename));
            var actual = document["tables"].AsArray().Single(table => (string)table["table"]["name"] == "Invoice");

            // Assert
            Assert.AreEqual("NOCASE", (string)actual["columns"].AsArray().Single(column => (string)column["name"] == "Name")["collation"]);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonReadsIndexes()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsJson("Invoice", Filename);
            var document = JsonNode.Parse(File.ReadAllText(Filename));
            var actual = document["tables"].AsArray().Single(table => (string)table["table"]["name"] == "Invoice");

            // Assert
            Assert.AreEqual(1, actual["indexes"].AsArray().Count);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonReadsCheckConstraints()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsJson("Invoice", Filename);
            var document = JsonNode.Parse(File.ReadAllText(Filename));
            var actual = document["tables"].AsArray().Single(table => (string)table["table"]["name"] == "Invoice");

            // Assert
            Assert.AreEqual(1, actual["checkConstraints"].AsArray().Count);
        }

        [TestMethod]
        [DataRow(RelatedTableBehavior.TableOnly, 0)]
        [DataRow(RelatedTableBehavior.Parents, 1)]
        [DataRow(RelatedTableBehavior.Children, 1)]
        [DataRow(RelatedTableBehavior.ParentsAndChildren, 1)]
        public void TestSaveDefinitionAsJsonIncludesForeignKeysForRelationships(RelatedTableBehavior behavior,
            int expectedCount)
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsJson("Invoice", Filename, behavior);
            var document = JsonNode.Parse(File.ReadAllText(Filename));
            var actual = document["tables"].AsArray().Single(table => (string)table["table"]["name"] == "Invoice");

            // Assert
            Assert.AreEqual(expectedCount, actual["foreignKeys"].AsArray().Count);
        }

        [TestMethod]
        [DataRow(RelatedTableBehavior.TableOnly, 0)]
        [DataRow(RelatedTableBehavior.Parents, 0)]
        [DataRow(RelatedTableBehavior.Children, 1)]
        [DataRow(RelatedTableBehavior.ParentsAndChildren, 0)]
        public void TestSaveDefinitionAsJsonListsExternalDependencies(RelatedTableBehavior behavior,
            int expectedCount)
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsJson("Invoice", Filename, behavior);
            var actual = JsonNode.Parse(File.ReadAllText(Filename));

            // Assert
            Assert.AreEqual(expectedCount, actual["externalDependencies"].AsArray().Count);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonTracesCatalogQueries()
        {
            // Setup
            var connection = Connection;
            var trace = new CatalogTrace();

            // Act
            connection.SaveDefinitionAsJson("Invoice", Filename,
                commandTimeout: 42,
                traceKey: "live-export",
                trace: trace);

            // Assert
            Assert.IsTrue(trace.BeforeKeys.Count > 0);
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPassesTraceKeyBeforeQueries()
        {
            // Setup
            var connection = Connection;
            var trace = new CatalogTrace();

            // Act
            connection.SaveDefinitionAsJson("Invoice", Filename,
                commandTimeout: 42,
                traceKey: "live-export",
                trace: trace);

            // Assert
            Assert.IsTrue(trace.BeforeKeys.Count > 0 && trace.BeforeKeys.All(key => key == "live-export"));
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPassesTraceKeyAfterQueries()
        {
            // Setup
            var connection = Connection;
            var trace = new CatalogTrace();

            // Act
            connection.SaveDefinitionAsJson("Invoice", Filename,
                commandTimeout: 42,
                traceKey: "live-export",
                trace: trace);

            // Assert
            Assert.IsTrue(trace.AfterKeys.Count > 0 && trace.AfterKeys.All(key => key == "live-export"));
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonPassesTimeoutToCatalogQueries()
        {
            // Setup
            var connection = Connection;
            var trace = new CatalogTrace();

            // Act
            connection.SaveDefinitionAsJson("Invoice", Filename,
                commandTimeout: 42,
                traceKey: "live-export",
                trace: trace);

            // Assert
            Assert.IsTrue(trace.Timeouts.Count > 0 && trace.Timeouts.All(timeout => timeout == 42));
        }

        [TestMethod]
        public void TestSaveDefinitionAsJsonRestoresContextAfterSaving()
        {
            // Setup
            var connection = Connection;
            var trace = new CatalogTrace();

            // Act
            connection.SaveDefinitionAsJson("Invoice", Filename,
                commandTimeout: 42,
                traceKey: "live-export",
                trace: trace);

            // Assert
            Assert.IsNull(SchemaReadContext.Current);
        }

        #endregion
    }
}
