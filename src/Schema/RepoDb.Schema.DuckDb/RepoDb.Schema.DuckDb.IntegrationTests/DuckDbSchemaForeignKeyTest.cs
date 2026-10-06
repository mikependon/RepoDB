#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using DuckDB.NET.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.DuckDb.IntegrationTests.Setup;

namespace RepoDb.Schema.DuckDb.IntegrationTests
{
    [TestClass]
    public class DuckDbSchemaForeignKeyTest
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
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                return new DuckDbSchemaReader(connection).GetForeignKeys(tableName).Single(f => f.Name == foreignKeyName);
            }
        }

        #endregion

        #region Reader

        [TestMethod]
        public void TestDuckDbSchemaReaderGetForeignKeysOfTableWithMultipleForeignKeys()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader(connection);

                // Act
                var actual = reader.GetForeignKeys("Shipment").Select(f => f.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray();

                // Assert
                CollectionAssert.AreEqual(new[] { "Shipment_countryid_id_fkey", "Shipment_orderid_linenumber_orderid_linenumber_fkey" }, actual);
            }
        }

        [TestMethod]
        public void TestDuckDbSchemaReaderGetForeignKeysOfCompositeForeignKey()
        {
            // Act
            var actual = GetForeignKey("Shipment", "Shipment_orderid_linenumber_orderid_linenumber_fkey");

            // Assert
            CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" }, actual.Columns.ToArray());
            Assert.AreEqual(new TableInfo("OrderLine", null), actual.ReferencedTable);
            CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" }, actual.ReferencedColumns.ToArray());
            Assert.AreEqual(CopySchemaForeignKeyRule.NoAction, actual.UpdateRule);
            Assert.AreEqual(CopySchemaForeignKeyRule.NoAction, actual.DeleteRule);
        }

        [TestMethod]
        public void TestDuckDbSchemaReaderGetForeignKeysWithNoActionRules()
        {
            // Act
            var actual = GetForeignKey("Preference", "Preference_countryid_id_fkey");

            // Assert
            Assert.AreEqual(CopySchemaForeignKeyRule.NoAction, actual.DeleteRule);
            Assert.AreEqual(CopySchemaForeignKeyRule.NoAction, actual.UpdateRule);
        }

        [TestMethod]
        public void TestDuckDbSchemaReaderGetForeignKeysOfSelfReferencingForeignKey()
        {
            // Act
            var actual = GetForeignKey("Employee", "Employee_managerid_id_fkey");

            // Assert
            CollectionAssert.AreEqual(new[] { "ManagerId" }, actual.Columns.ToArray());
            Assert.AreEqual(new TableInfo("Employee", null), actual.ReferencedTable);
            CollectionAssert.AreEqual(new[] { "Id" }, actual.ReferencedColumns.ToArray());
        }

        [TestMethod]
        public void TestDuckDbSchemaReaderGetForeignKeysOfTableInAnotherSchema()
        {
            // Act
            var actual = GetForeignKey($"{Database.SourceSalesName}.InvoiceLine", "InvoiceLine_invoiceid_id_fkey");

            // Assert
            Assert.AreEqual(new TableInfo("Invoice", Database.SourceSalesName), actual.ReferencedTable);
        }

        [TestMethod]
        public async Task TestDuckDbSchemaReaderGetForeignKeysAsyncOfTableWithMultipleForeignKeys()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader(connection);

                // Act
                var actual = (await reader.GetForeignKeysAsync("Shipment")).ToList();

                // Assert
                Assert.AreEqual(2, actual.Count);
                CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" }, actual.Single(f => f.Name == "Shipment_orderid_linenumber_orderid_linenumber_fkey").Columns.ToArray());
            }
        }

        #endregion

        #region Composer

        [TestMethod]
        public void TestDuckDbSchemaComposerComposedSchemaOfSelfReferencingTable()
        {
            // Act
            Helper.CopyToTarget("Employee");

            // Assert
            Helper.AssertTargetMatchesSource("Employee");
        }

        [TestMethod]
        public void TestDuckDbSchemaComposerComposedSchemaOfTableWithCompositeAndMultipleForeignKeys()
        {
            // Act
            Helper.CopyToTarget("Country", "OrderLine", "Shipment");

            // Assert
            Helper.AssertTargetMatchesSource("Shipment");
        }

        [TestMethod]
        public void TestDuckDbSchemaComposerComposedSchemaOfTablesWithForeignKeysInAnotherSchema()
        {
            // Act
            Helper.CopyToTarget($"{Database.SourceSalesName}.Invoice", $"{Database.SourceSalesName}.InvoiceLine", "Ledger");

            // Assert
            Helper.AssertTargetMatchesSource($"{Database.SourceSalesName}.InvoiceLine");
            Helper.AssertTargetMatchesSource("Ledger");
        }

        [TestMethod]
        public void TestDuckDbSchemaComposerComposedSchemaOfTableWithNoActionRules()
        {
            // Act
            Helper.CopyToTarget("Country", "Preference");

            // Assert
            Helper.AssertTargetMatchesSource("Preference");
        }

        [TestMethod]
        public void TestDuckDbSchemaComposerComposedCompositeForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("Country", "OrderLine", "Shipment");

            using (var connection = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO \"Country\" (\"Name\") VALUES ('Philippines')");
                connection.ExecuteNonQuery("INSERT INTO \"OrderLine\" (\"OrderId\", \"LineNumber\", \"Quantity\") VALUES (1, 1, 5)");

                // Act/Assert
                Assert.Throws<DuckDBException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO \"Shipment\" (\"Id\", \"OrderId\", \"LineNumber\", \"CountryId\") VALUES (1, 1, 2, 1)"));

                // Act
                connection.ExecuteNonQuery("INSERT INTO \"Shipment\" (\"Id\", \"OrderId\", \"LineNumber\", \"CountryId\") VALUES (1, 1, 1, 1)");
                var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM \"Shipment\"");

                // Assert
                Assert.AreEqual(1, count);
            }
        }

        [TestMethod]
        public void TestDuckDbSchemaComposerComposedSelfReferencingForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("Employee");

            using (var connection = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<DuckDBException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO \"Employee\" (\"Id\", \"ManagerId\") VALUES (1, 99)"));

                connection.ExecuteNonQuery("INSERT INTO \"Employee\" (\"Id\", \"ManagerId\") VALUES (1, NULL)");
                connection.ExecuteNonQuery("INSERT INTO \"Employee\" (\"Id\", \"ManagerId\") VALUES (2, 1)");
            }
        }

        [TestMethod]
        public void TestDuckDbSchemaComposerComposedNoActionRuleIsApplied()
        {
            // Setup
            Helper.CopyToTarget("Country", "Preference");

            using (var connection = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO \"Country\" (\"Name\") VALUES ('First'), ('Second')");
                connection.ExecuteNonQuery("INSERT INTO \"Preference\" (\"Id\", \"CountryId\") VALUES (1, 2)");

                // Act/Assert
                Assert.Throws<DuckDBException>(() =>
                    connection.ExecuteNonQuery("DELETE FROM \"Country\" WHERE \"Id\" = 2"));
                Assert.AreEqual(1, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM \"Preference\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnComposedSchemaIfTheReferencedTableDoesNotExist()
        {
            // Act/Assert
            Assert.Throws<DuckDBException>(() => Helper.CopyToTarget("Person"));
        }

        [TestMethod]
        public void TestDuckDbSchemaComposerComposedSchemaOfTablesInWrongOrderViaComposeSchemas()
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
