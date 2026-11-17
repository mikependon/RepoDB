#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Sap.Data.Hana;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.SapHana.IntegrationTests.Setup;

namespace RepoDb.Schema.SapHana.IntegrationTests
{
    /// <summary>
    /// Contains the sap hana schema foreign key test behavior.
    /// </summary>
    [TestClass]
    public class SapHanaSchemaForeignKeyTest
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
            using (var connection = new HanaConnection(Database.ConnectionStringForSource))
            {
                return new SapHanaSchemaReader().GetForeignKeys(connection,
                    tableName,
                    transaction: null).Single(f => f.Name == foreignKeyName);
            }
        }

        #endregion

        #region Reader

        /// <summary>
        /// Verifies sap hana schema reader get foreign keys of table with multiple foreign keys.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaReaderGetForeignKeysOfTableWithMultipleForeignKeys()
        {
            using (var connection = new HanaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SapHanaSchemaReader();
                // Act
                var actual = reader.GetForeignKeys(connection,
                    "Shipment",
                    transaction: null).Select(f => f.Name).OrderBy(n => n,
                    StringComparer.Ordinal).ToArray();
                // Assert
                CollectionAssert.AreEqual(new[] { "FK_Shipment_Country", "FK_Shipment_OrderLine" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies sap hana schema reader get foreign keys of composite foreign key.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaReaderGetForeignKeysOfCompositeForeignKey()
        {
            // Act
            var actual = GetForeignKey("Shipment",
                "FK_Shipment_OrderLine");
            // Assert
            CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" },
                actual.Columns.ToArray());
            Assert.AreEqual(new TableInfo("OrderLine",
                    null),
                actual.ReferencedTable);
            CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" },
                actual.ReferencedColumns.ToArray());
            Assert.AreEqual(ForeignKeyRule.Restrict,
                actual.UpdateRule);
            Assert.AreEqual(ForeignKeyRule.Restrict,
                actual.DeleteRule);
        }

        /// <summary>
        /// Verifies sap hana schema reader get foreign keys with cascade rule.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaReaderGetForeignKeysWithCascadeRule()
        {
            // Act
            var actual = GetForeignKey("Shipment",
                "FK_Shipment_Country");
            // Assert
            Assert.AreEqual(ForeignKeyRule.Cascade,
                actual.DeleteRule);
            Assert.AreEqual(ForeignKeyRule.Restrict,
                actual.UpdateRule);
        }

        /// <summary>
        /// Verifies sap hana schema reader get foreign keys with restrict rules.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaReaderGetForeignKeysWithRestrictRules()
        {
            // Act
            var actual = GetForeignKey("Preference",
                "FK_Preference_Country");
            // Assert
            Assert.AreEqual(ForeignKeyRule.Restrict,
                actual.DeleteRule);
            Assert.AreEqual(ForeignKeyRule.Restrict,
                actual.UpdateRule);
        }

        /// <summary>
        /// Verifies sap hana schema reader get foreign keys of self referencing foreign key.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaReaderGetForeignKeysOfSelfReferencingForeignKey()
        {
            // Act
            var actual = GetForeignKey("Employee",
                "FK_Employee_Manager");
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
        /// Verifies sap hana schema reader get foreign keys of table in another schema with cascade rule.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaReaderGetForeignKeysOfTableInAnotherSchemaWithCascadeRule()
        {
            // Act
            var actual = GetForeignKey($"{Database.SourceSalesName}.InvoiceLine",
                "FK_InvoiceLine_Invoice");
            // Assert
            Assert.AreEqual(new TableInfo("Invoice",
                    Database.SourceSalesName),
                actual.ReferencedTable);
            Assert.AreEqual(ForeignKeyRule.Cascade,
                actual.DeleteRule);
        }

        /// <summary>
        /// Verifies sap hana schema reader get foreign keys of table in another schema.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaReaderGetForeignKeysOfTableInAnotherSchema()
        {
            // Act
            var actual = GetForeignKey($"{Database.SourceSalesName}.InvoiceLine",
                "FK_InvoiceLine_Invoice");
            // Assert
            Assert.AreEqual(new TableInfo("Invoice",
                    Database.SourceSalesName),
                actual.ReferencedTable);
        }

        /// <summary>
        /// Verifies sap hana schema reader get foreign keys of circular references.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaReaderGetForeignKeysOfCircularReferences()
        {
            using (var connection = new HanaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SapHanaSchemaReader();
                // Act
                var a = reader.GetForeignKeys(connection,
                    "CycleA",
                    transaction: null).Single();
                var b = reader.GetForeignKeys(connection,
                    "CycleB",
                    transaction: null).Single();
                // Assert
                Assert.AreEqual(new TableInfo("CycleB",
                        null),
                    a.ReferencedTable);
                Assert.AreEqual(new TableInfo("CycleA",
                        null),
                    b.ReferencedTable);
            }
        }

        /// <summary>
        /// Verifies sap hana schema reader get foreign keys async of table with multiple foreign keys.
        /// </summary>
        [TestMethod]
        public async Task TestSapHanaSchemaReaderGetForeignKeysAsyncOfTableWithMultipleForeignKeys()
        {
            using (var connection = new HanaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SapHanaSchemaReader();
                // Act
                var actual = (await reader.GetForeignKeysAsync(connection,
                    "Shipment",
                    transaction: null)).ToList();
                // Assert
                Assert.AreEqual(2,
                    actual.Count);
                CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" },
                    actual.Single(f => f.Name == "FK_Shipment_OrderLine").Columns.ToArray());
            }
        }

        #endregion

        #region Composer

        /// <summary>
        /// Verifies sap hana schema composer composed schema of self referencing table.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaComposerComposedSchemaOfSelfReferencingTable()
        {
            // Act
            Helper.CopyToTarget("Employee");
            // Assert
            Helper.AssertTargetMatchesSource("Employee");
        }

        /// <summary>
        /// Verifies sap hana schema composer composed schema of table with composite and multiple foreign keys.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaComposerComposedSchemaOfTableWithCompositeAndMultipleForeignKeys()
        {
            // Act
            Helper.CopyToTarget("Country",
                "OrderLine",
                "Shipment");
            // Assert
            Helper.AssertTargetMatchesSource("Shipment");
        }

        /// <summary>
        /// Verifies sap hana schema composer composed schema of tables with foreign keys in another schema.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaComposerComposedSchemaOfTablesWithForeignKeysInAnotherSchema()
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
        /// Verifies sap hana schema composer composed schema of table with restrict rules.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaComposerComposedSchemaOfTableWithRestrictRules()
        {
            // Act
            Helper.CopyToTarget("Country",
                "Preference");
            // Assert
            Helper.AssertTargetMatchesSource("Preference");
        }

        /// <summary>
        /// Verifies sap hana schema composer composed composite foreign key is enforced.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaComposerComposedCompositeForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("Country",
                "OrderLine",
                "Shipment");
            using (var connection = new HanaConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO \"Country\" (\"Name\") VALUES ('Philippines')");
                connection.ExecuteNonQuery("INSERT INTO \"OrderLine\" (\"OrderId\", \"LineNumber\", \"Quantity\") VALUES (1, 1, 5)");
                // Act/Assert
                Assert.Throws<HanaException>(() => connection.ExecuteNonQuery("INSERT INTO \"Shipment\" (\"Id\", \"OrderId\", \"LineNumber\", \"CountryId\") VALUES (1, 1, 2, 1)"));
                // Act
                connection.ExecuteNonQuery("INSERT INTO \"Shipment\" (\"Id\", \"OrderId\", \"LineNumber\", \"CountryId\") VALUES (1, 1, 1, 1)");
                var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM \"Shipment\"");
                // Assert
                Assert.AreEqual(1,
                    count);
            }
        }

        /// <summary>
        /// Verifies sap hana schema composer composed cascade rule is applied.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaComposerComposedCascadeRuleIsApplied()
        {
            // Setup
            Helper.CopyToTarget("Country",
                "OrderLine",
                "Shipment");
            using (var connection = new HanaConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO \"Country\" (\"Name\") VALUES ('Philippines')");
                connection.ExecuteNonQuery("INSERT INTO \"OrderLine\" (\"OrderId\", \"LineNumber\", \"Quantity\") VALUES (1, 1, 5)");
                connection.ExecuteNonQuery("INSERT INTO \"Shipment\" (\"Id\", \"OrderId\", \"LineNumber\", \"CountryId\") VALUES (1, 1, 1, 1)");
                // Act
                connection.ExecuteNonQuery("DELETE FROM \"Country\"");
                var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM \"Shipment\"");
                // Assert
                Assert.AreEqual(0,
                    count);
            }
        }

        /// <summary>
        /// Verifies sap hana schema composer composed self referencing foreign key is enforced.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaComposerComposedSelfReferencingForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("Employee");
            using (var connection = new HanaConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<HanaException>(() => connection.ExecuteNonQuery("INSERT INTO \"Employee\" (\"Id\", \"ManagerId\") VALUES (1, 99)"));
                connection.ExecuteNonQuery("INSERT INTO \"Employee\" (\"Id\", \"ManagerId\") VALUES (1, NULL)");
                connection.ExecuteNonQuery("INSERT INTO \"Employee\" (\"Id\", \"ManagerId\") VALUES (2, 1)");
            }
        }

        /// <summary>
        /// Verifies sap hana schema composer composed restrict rule is applied.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaComposerComposedRestrictRuleIsApplied()
        {
            // Setup
            Helper.CopyToTarget("Country",
                "Preference");
            using (var connection = new HanaConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO \"Country\" (\"Name\") VALUES ('First')");
                connection.ExecuteNonQuery("INSERT INTO \"Country\" (\"Name\") VALUES ('Second')");
                connection.ExecuteNonQuery("INSERT INTO \"Preference\" (\"Id\", \"CountryId\") VALUES (1, 2)");
                // Act/Assert
                Assert.Throws<HanaException>(() => connection.ExecuteNonQuery("DELETE FROM \"Country\" WHERE \"Id\" = 2"));
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
            Assert.Throws<HanaException>(() => Helper.CopyToTarget("Person"));
        }

        /// <summary>
        /// Verifies sap hana schema composer composed schema of tables in wrong order via compose schemas.
        /// </summary>
        [TestMethod]
        public void TestSapHanaSchemaComposerComposedSchemaOfTablesInWrongOrderViaComposeSchemas()
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
