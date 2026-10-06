#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.MySql.IntegrationTests.Setup;

namespace RepoDb.Schema.MySql.IntegrationTests
{
    [TestClass]
    public class MySqlSchemaForeignKeyTest
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
            using (var connection = new MySqlConnection(Database.ConnectionStringForSource))
            {
                return new MySqlSchemaReader(connection).GetForeignKeys(tableName).Single(f => f.Name == foreignKeyName);
            }
        }

        #endregion

        #region Reader

        [TestMethod]
        public void TestMySqlSchemaReaderGetForeignKeysOfTableWithMultipleForeignKeys()
        {
            using (var connection = new MySqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MySqlSchemaReader(connection);

                // Act
                var actual = reader.GetForeignKeys("Shipment").Select(f => f.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray();

                // Assert
                CollectionAssert.AreEqual(new[] { "FK_Shipment_Country", "FK_Shipment_OrderLine" }, actual);
            }
        }

        [TestMethod]
        public void TestMySqlSchemaReaderGetForeignKeysOfCompositeForeignKey()
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
        public void TestMySqlSchemaReaderGetForeignKeysWithCascadeRule()
        {
            // Act
            var actual = GetForeignKey("Shipment", "FK_Shipment_Country");

            // Assert
            Assert.AreEqual(CopySchemaForeignKeyRule.Cascade, actual.DeleteRule);
            Assert.AreEqual(CopySchemaForeignKeyRule.NoAction, actual.UpdateRule);
        }

        [TestMethod]
        public void TestMySqlSchemaReaderGetForeignKeysWithRestrictRules()
        {
            // Act
            var actual = GetForeignKey("Preference", "FK_Preference_Country");

            // Assert
            Assert.AreEqual(CopySchemaForeignKeyRule.Restrict, actual.DeleteRule);
            Assert.AreEqual(CopySchemaForeignKeyRule.Restrict, actual.UpdateRule);
        }

        [TestMethod]
        public void TestMySqlSchemaReaderGetForeignKeysOfSelfReferencingForeignKey()
        {
            // Act
            var actual = GetForeignKey("Employee", "FK_Employee_Manager");

            // Assert
            CollectionAssert.AreEqual(new[] { "ManagerId" }, actual.Columns.ToArray());
            Assert.AreEqual(new TableInfo("Employee", null), actual.ReferencedTable);
            CollectionAssert.AreEqual(new[] { "Id" }, actual.ReferencedColumns.ToArray());
        }

        [TestMethod]
        public void TestMySqlSchemaReaderGetForeignKeysAcrossSchemas()
        {
            // Act
            var actual = GetForeignKey("Ledger", "FK_Ledger_Invoice");

            // Assert
            Assert.AreEqual(new TableInfo("Invoice", Database.SourceSalesName), actual.ReferencedTable);
            Assert.AreEqual(CopySchemaForeignKeyRule.Cascade, actual.DeleteRule);
        }

        [TestMethod]
        public void TestMySqlSchemaReaderGetForeignKeysOfTableInAnotherSchema()
        {
            // Act
            var actual = GetForeignKey($"{Database.SourceSalesName}.InvoiceLine", "FK_InvoiceLine_Invoice");

            // Assert
            Assert.AreEqual(new TableInfo("Invoice", Database.SourceSalesName), actual.ReferencedTable);
        }

        [TestMethod]
        public void TestMySqlSchemaReaderGetForeignKeysOfCircularReferences()
        {
            using (var connection = new MySqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MySqlSchemaReader(connection);

                // Act
                var a = reader.GetForeignKeys("CycleA").Single();
                var b = reader.GetForeignKeys("CycleB").Single();

                // Assert
                Assert.AreEqual(new TableInfo("CycleB", null), a.ReferencedTable);
                Assert.AreEqual(new TableInfo("CycleA", null), b.ReferencedTable);
            }
        }

        [TestMethod]
        public async Task TestMySqlSchemaReaderGetForeignKeysAsyncOfTableWithMultipleForeignKeys()
        {
            using (var connection = new MySqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MySqlSchemaReader(connection);

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
        public void TestMySqlSchemaComposerComposedSchemaOfSelfReferencingTable()
        {
            // Act
            Helper.CopyToTarget("Employee");

            // Assert
            Helper.AssertTargetMatchesSource("Employee");
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedSchemaOfTableWithCompositeAndMultipleForeignKeys()
        {
            // Act
            Helper.CopyToTarget("Country", "OrderLine", "Shipment");

            // Assert
            Helper.AssertTargetMatchesSource("Shipment");
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedSchemaOfTablesWithForeignKeysAcrossSchemas()
        {
            // Act
            Helper.CopyToTarget($"{Database.SourceSalesName}.Invoice", $"{Database.SourceSalesName}.InvoiceLine", "Ledger");

            // Assert
            Helper.AssertTargetMatchesSource($"{Database.SourceSalesName}.InvoiceLine");
            Helper.AssertTargetMatchesSource("Ledger");
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedSchemaOfTableWithRestrictRules()
        {
            // Act
            Helper.CopyToTarget("Country", "Preference");

            // Assert
            Helper.AssertTargetMatchesSource("Preference");
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedCompositeForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("Country", "OrderLine", "Shipment");

            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO `Country` (`Name`) VALUES ('Philippines');");
                connection.ExecuteNonQuery("INSERT INTO `OrderLine` (`OrderId`, `LineNumber`, `Quantity`) VALUES (1, 1, 5);");

                // Act/Assert
                Assert.Throws<MySqlException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO `Shipment` (`Id`, `OrderId`, `LineNumber`, `CountryId`) VALUES (1, 1, 2, 1);"));

                // Act
                connection.ExecuteNonQuery("INSERT INTO `Shipment` (`Id`, `OrderId`, `LineNumber`, `CountryId`) VALUES (1, 1, 1, 1);");
                var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM `Shipment`;");

                // Assert
                Assert.AreEqual(1, count);
            }
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedCascadeRuleIsApplied()
        {
            // Setup
            Helper.CopyToTarget("Country", "OrderLine", "Shipment");

            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO `Country` (`Name`) VALUES ('Philippines');");
                connection.ExecuteNonQuery("INSERT INTO `OrderLine` (`OrderId`, `LineNumber`, `Quantity`) VALUES (1, 1, 5);");
                connection.ExecuteNonQuery("INSERT INTO `Shipment` (`Id`, `OrderId`, `LineNumber`, `CountryId`) VALUES (1, 1, 1, 1);");

                // Act
                connection.ExecuteNonQuery("DELETE FROM `Country`;");
                var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM `Shipment`;");

                // Assert
                Assert.AreEqual(0, count);
            }
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedSelfReferencingForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("Employee");

            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MySqlException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO `Employee` (`Id`, `ManagerId`) VALUES (1, 99);"));

                connection.ExecuteNonQuery("INSERT INTO `Employee` (`Id`, `ManagerId`) VALUES (1, NULL);");
                connection.ExecuteNonQuery("INSERT INTO `Employee` (`Id`, `ManagerId`) VALUES (2, 1);");
            }
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedRestrictRuleIsApplied()
        {
            // Setup
            Helper.CopyToTarget("Country", "Preference");

            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO `Country` (`Name`) VALUES ('First'), ('Second');");
                connection.ExecuteNonQuery("INSERT INTO `Preference` (`Id`, `CountryId`) VALUES (1, 2);");

                // Act/Assert
                Assert.Throws<MySqlException>(() =>
                    connection.ExecuteNonQuery("DELETE FROM `Country` WHERE `Id` = 2;"));
                Assert.AreEqual(1, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM `Preference`;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnComposedSchemaIfTheReferencedTableDoesNotExist()
        {
            // Act/Assert
            Assert.Throws<MySqlException>(() => Helper.CopyToTarget("Person"));
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedSchemaOfTablesInWrongOrderViaComposeSchemas()
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
