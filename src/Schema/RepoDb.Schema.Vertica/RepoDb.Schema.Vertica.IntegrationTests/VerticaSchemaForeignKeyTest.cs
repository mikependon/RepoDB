#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Vertica.Data.VerticaClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.Vertica.IntegrationTests.Setup;

namespace RepoDb.Schema.Vertica.IntegrationTests
{
    [TestClass]
    public class VerticaSchemaForeignKeyTest
    {
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

        #region Helpers

        private static ForeignKeyInfo GetForeignKey(string tableName, string foreignKeyName)
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                return new VerticaSchemaReader(connection).GetForeignKeys(tableName).Single(f => f.Name == foreignKeyName);
            }
        }

        #endregion

        #region Reader

        [TestMethod]
        public void TestVerticaSchemaReaderGetForeignKeysOfTableWithMultipleForeignKeys()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new VerticaSchemaReader(connection);

                // Act
                var actual = reader.GetForeignKeys("Shipment").Select(f => f.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray();

                // Assert
                CollectionAssert.AreEqual(new[] { "FK_Shipment_Country", "FK_Shipment_OrderLine" }, actual);
            }
        }

        [TestMethod]
        public void TestVerticaSchemaReaderGetForeignKeysOfCompositeForeignKey()
        {
            // Act
            var actual = GetForeignKey("Shipment", "FK_Shipment_OrderLine");

            // Assert
            CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" }, actual.Columns.ToArray());
            Assert.AreEqual(new TableInfo("OrderLine", null), actual.ReferencedTable);
            CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" }, actual.ReferencedColumns.ToArray());
            Assert.AreEqual(CopySchemaForeignKeyRule.NoAction, actual.UpdateRule);
            Assert.AreEqual(CopySchemaForeignKeyRule.NoAction, actual.DeleteRule);
        }

        [TestMethod]
        public void TestVerticaSchemaReaderGetForeignKeysWithNoActionRules()
        {
            // Act
            var actual = GetForeignKey("Preference", "FK_Preference_Country");

            // Assert
            Assert.AreEqual(CopySchemaForeignKeyRule.NoAction, actual.DeleteRule);
            Assert.AreEqual(CopySchemaForeignKeyRule.NoAction, actual.UpdateRule);
        }

        [TestMethod]
        public void TestVerticaSchemaReaderGetForeignKeysOfSelfReferencingForeignKey()
        {
            // Act
            var actual = GetForeignKey("Employee", "FK_Employee_Manager");

            // Assert
            CollectionAssert.AreEqual(new[] { "ManagerId" }, actual.Columns.ToArray());
            Assert.AreEqual(new TableInfo("Employee", null), actual.ReferencedTable);
            CollectionAssert.AreEqual(new[] { "Id" }, actual.ReferencedColumns.ToArray());
        }

        [TestMethod]
        public void TestVerticaSchemaReaderGetForeignKeysOfTableInAnotherSchema()
        {
            // Act
            var actual = GetForeignKey($"{Database.SourceSalesName}.InvoiceLine", "FK_InvoiceLine_Invoice");

            // Assert
            Assert.AreEqual(new TableInfo("Invoice", Database.SourceSalesName), actual.ReferencedTable);
        }

        [TestMethod]
        public void TestVerticaSchemaReaderGetForeignKeysOfCircularReferences()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new VerticaSchemaReader(connection);

                // Act
                var a = reader.GetForeignKeys("CycleA").Single();
                var b = reader.GetForeignKeys("CycleB").Single();

                // Assert
                Assert.AreEqual(new TableInfo("CycleB", null), a.ReferencedTable);
                Assert.AreEqual(new TableInfo("CycleA", null), b.ReferencedTable);
            }
        }

        [TestMethod]
        public async Task TestVerticaSchemaReaderGetForeignKeysAsyncOfTableWithMultipleForeignKeys()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new VerticaSchemaReader(connection);

                // Act
                var actual = (await reader.GetForeignKeysAsync("Shipment")).ToList();

                // Assert
                Assert.AreEqual(2, actual.Count);
                CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" }, actual.Single(f => f.Name == "FK_Shipment_OrderLine").Columns.ToArray());
            }
        }

        #endregion

        #region Composer

        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfSelfReferencingTable()
        {
            // Act
            Helper.CopyToTarget("Employee");

            // Assert
            Helper.AssertTargetMatchesSource("Employee");
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfTableWithCompositeAndMultipleForeignKeys()
        {
            // Act
            Helper.CopyToTarget("Country", "OrderLine", "Shipment");

            // Assert
            Helper.AssertTargetMatchesSource("Shipment");
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfTablesWithForeignKeysInAnotherSchema()
        {
            // Act
            Helper.CopyToTarget($"{Database.SourceSalesName}.Invoice", $"{Database.SourceSalesName}.InvoiceLine", "Ledger");

            // Assert
            Helper.AssertTargetMatchesSource($"{Database.SourceSalesName}.InvoiceLine");
            Helper.AssertTargetMatchesSource("Ledger");
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfTableWithNoActionRules()
        {
            // Act
            Helper.CopyToTarget("Country", "Preference");

            // Assert
            Helper.AssertTargetMatchesSource("Preference");
        }

        [TestMethod]
        public void ThrowExceptionOnComposedSchemaIfTheReferencedTableDoesNotExist()
        {
            // Act/Assert
            Assert.Throws<VerticaException>(() => Helper.CopyToTarget("Person"));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfTablesInWrongOrderViaComposeSchemas()
        {
            // Act
            Helper.CopyAllToTarget("Person", "Country");

            // Assert
            Helper.AssertTargetMatchesSource("Country");
            Helper.AssertTargetMatchesSource("Person");
        }

        #endregion
    }
}
