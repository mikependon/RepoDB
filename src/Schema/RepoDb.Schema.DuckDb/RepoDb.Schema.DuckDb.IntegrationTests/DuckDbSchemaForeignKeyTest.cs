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
    /// <summary>
    /// Contains the duck db schema foreign key test behavior.
    /// </summary>
    [TestClass]
    public class DuckDbSchemaForeignKeyTest
    {
        /// <summary>
        /// Performs initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        /// <summary>
        /// Performs cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region Private Methods

        /// <summary>
        /// Performs get foreign key.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="foreignKeyName">The foreign key name.</param>
        private static ForeignKeyInfo GetForeignKey(string tableName,
            string foreignKeyName)
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                return new DuckDbSchemaReader().GetForeignKeys(connection,
                    tableName,
                    transaction: null).Single(f => f.Name == foreignKeyName);
            }
        }

        #endregion

        #region Reader

        /// <summary>
        /// Verifies duck db schema reader get foreign keys of table with multiple foreign keys.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetForeignKeysOfTableWithMultipleForeignKeys()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.GetForeignKeys(connection,
                    "Shipment",
                    transaction: null).Select(f => f.Name).OrderBy(n => n,
                    StringComparer.Ordinal).ToArray();
                // Assert
                CollectionAssert.AreEqual(new[] { "Shipment_countryid_id_fkey", "Shipment_orderid_linenumber_orderid_linenumber_fkey" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get foreign keys of composite foreign key.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetForeignKeysOfCompositeForeignKey()
        {
            // Act
            var actual = GetForeignKey("Shipment",
                "Shipment_orderid_linenumber_orderid_linenumber_fkey");
            // Assert
            CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" },
                actual.Columns.ToArray());
            Assert.AreEqual(new TableInfo("OrderLine",
                    null),
                actual.ReferencedTable);
            CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" },
                actual.ReferencedColumns.ToArray());
            Assert.AreEqual(ForeignKeyRule.NoAction,
                actual.UpdateRule);
            Assert.AreEqual(ForeignKeyRule.NoAction,
                actual.DeleteRule);
        }

        /// <summary>
        /// Verifies duck db schema reader get foreign keys with no action rules.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetForeignKeysWithNoActionRules()
        {
            // Act
            var actual = GetForeignKey("Preference",
                "Preference_countryid_id_fkey");
            // Assert
            Assert.AreEqual(ForeignKeyRule.NoAction,
                actual.DeleteRule);
            Assert.AreEqual(ForeignKeyRule.NoAction,
                actual.UpdateRule);
        }

        /// <summary>
        /// Verifies duck db schema reader get foreign keys of self referencing foreign key.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetForeignKeysOfSelfReferencingForeignKey()
        {
            // Act
            var actual = GetForeignKey("Employee",
                "Employee_managerid_id_fkey");
            // Assert
            CollectionAssert.AreEqual(new[] { "ManagerId" },
                actual.Columns.ToArray());
            Assert.AreEqual(new TableInfo("Employee",
                    null),
                actual.ReferencedTable);
            CollectionAssert.AreEqual(new[] { "Id" },
                actual.ReferencedColumns.ToArray());
        }

        /// <summary>
        /// Verifies duck db schema reader get foreign keys of table in another schema.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetForeignKeysOfTableInAnotherSchema()
        {
            // Act
            var actual = GetForeignKey($"{Database.SourceSalesName}.InvoiceLine",
                "InvoiceLine_invoiceid_id_fkey");
            // Assert
            Assert.AreEqual(new TableInfo("Invoice",
                    Database.SourceSalesName),
                actual.ReferencedTable);
        }

        /// <summary>
        /// Verifies duck db schema reader get foreign keys async of table with multiple foreign keys.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderGetForeignKeysAsyncOfTableWithMultipleForeignKeys()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = (await reader.GetForeignKeysAsync(connection,
                    "Shipment",
                    transaction: null)).ToList();
                // Assert
                Assert.AreEqual(2,
                    actual.Count);
                CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" },
                    actual.Single(f => f.Name == "Shipment_orderid_linenumber_orderid_linenumber_fkey").Columns.ToArray());
            }
        }

        #endregion

        #region Composer

        /// <summary>
        /// Verifies duck db schema composer composed schema of self referencing table.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaComposerComposedSchemaOfSelfReferencingTable()
        {
            // Act
            Helper.CopyToTarget("Employee");
            // Assert
            Helper.AssertTargetMatchesSource("Employee");
        }

        /// <summary>
        /// Verifies duck db schema composer composed schema of table with composite and multiple foreign keys.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaComposerComposedSchemaOfTableWithCompositeAndMultipleForeignKeys()
        {
            // Act
            Helper.CopyToTarget("Country",
                "OrderLine",
                "Shipment");
            // Assert
            Helper.AssertTargetMatchesSource("Shipment");
        }

        /// <summary>
        /// Verifies duck db schema composer composed schema of tables with foreign keys in another schema.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaComposerComposedSchemaOfTablesWithForeignKeysInAnotherSchema()
        {
            // Act
            Helper.CopyToTarget($"{Database.SourceSalesName}.Invoice",
                $"{Database.SourceSalesName}.InvoiceLine",
                "Ledger");
            // Assert
            Helper.AssertTargetMatchesSource($"{Database.SourceSalesName}.InvoiceLine");
            Helper.AssertTargetMatchesSource("Ledger");
        }

        /// <summary>
        /// Verifies duck db schema composer composed schema of table with no action rules.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaComposerComposedSchemaOfTableWithNoActionRules()
        {
            // Act
            Helper.CopyToTarget("Country",
                "Preference");
            // Assert
            Helper.AssertTargetMatchesSource("Preference");
        }

        /// <summary>
        /// Verifies duck db schema composer composed composite foreign key is enforced.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaComposerComposedCompositeForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("Country",
                "OrderLine",
                "Shipment");
            using (var connection = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO \"Country\" (\"Name\") VALUES ('Philippines')");
                connection.ExecuteNonQuery("INSERT INTO \"OrderLine\" (\"OrderId\", \"LineNumber\", \"Quantity\") VALUES (1, 1, 5)");
                // Act/Assert
                Assert.Throws<DuckDBException>(() => connection.ExecuteNonQuery("INSERT INTO \"Shipment\" (\"Id\", \"OrderId\", \"LineNumber\", \"CountryId\") VALUES (1, 1, 2, 1)"));
                // Act
                connection.ExecuteNonQuery("INSERT INTO \"Shipment\" (\"Id\", \"OrderId\", \"LineNumber\", \"CountryId\") VALUES (1, 1, 1, 1)");
                var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM \"Shipment\"");
                // Assert
                Assert.AreEqual(1,
                    count);
            }
        }

        /// <summary>
        /// Verifies duck db schema composer composed self referencing foreign key is enforced.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaComposerComposedSelfReferencingForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("Employee");
            using (var connection = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<DuckDBException>(() => connection.ExecuteNonQuery("INSERT INTO \"Employee\" (\"Id\", \"ManagerId\") VALUES (1, 99)"));
                connection.ExecuteNonQuery("INSERT INTO \"Employee\" (\"Id\", \"ManagerId\") VALUES (1, NULL)");
                connection.ExecuteNonQuery("INSERT INTO \"Employee\" (\"Id\", \"ManagerId\") VALUES (2, 1)");
            }
        }

        /// <summary>
        /// Verifies duck db schema composer composed no action rule is applied.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaComposerComposedNoActionRuleIsApplied()
        {
            // Setup
            Helper.CopyToTarget("Country",
                "Preference");
            using (var connection = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO \"Country\" (\"Name\") VALUES ('First'), ('Second')");
                connection.ExecuteNonQuery("INSERT INTO \"Preference\" (\"Id\", \"CountryId\") VALUES (1, 2)");
                // Act/Assert
                Assert.Throws<DuckDBException>(() => connection.ExecuteNonQuery("DELETE FROM \"Country\" WHERE \"Id\" = 2"));
                Assert.AreEqual(1,
                    connection.ExecuteScalar<int>("SELECT COUNT(*) FROM \"Preference\""));
            }
        }

        /// <summary>
        /// Verifies throw exception on composed schema if the referenced table does not exist.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnComposedSchemaIfTheReferencedTableDoesNotExist()
        {
            // Act/Assert
            Assert.Throws<DuckDBException>(() => Helper.CopyToTarget("Person"));
        }

        /// <summary>
        /// Verifies duck db schema composer composed schema of tables in wrong order via compose schemas.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaComposerComposedSchemaOfTablesInWrongOrderViaComposeSchemas()
        {
            // Act
            Helper.CopyAllToTarget("Person",
                "Country");
            // Assert
            Helper.AssertTargetMatchesSource("Country");
            Helper.AssertTargetMatchesSource("Person");
        }
        #endregion
    }
}
