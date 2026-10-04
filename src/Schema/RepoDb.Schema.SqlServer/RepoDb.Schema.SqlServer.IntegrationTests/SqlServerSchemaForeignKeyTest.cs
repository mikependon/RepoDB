#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.SqlServer.IntegrationTests.Setup;

namespace RepoDb.Schema.SqlServer.IntegrationTests
{
    [TestClass]
    public class SqlServerSchemaForeignKeyTest
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
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                return new SqlServerSchemaReader(connection).GetForeignKeys(tableName).Single(f => f.Name == foreignKeyName);
            }
        }

        #endregion

        #region Reader

        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysOfTableWithMultipleForeignKeys()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetForeignKeys("Shipment").Select(f => f.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray();

                // Assert
                CollectionAssert.AreEqual(new[] { "FK_Shipment_Country", "FK_Shipment_OrderLine" }, actual);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysOfCompositeForeignKey()
        {
            // Act
            var actual = GetForeignKey("Shipment", "FK_Shipment_OrderLine");

            // Assert
            CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" }, actual.Columns.ToArray());
            Assert.AreEqual("dbo.OrderLine", actual.ReferencedTable, StringComparer.Ordinal);
            CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" }, actual.ReferencedColumns.ToArray());
            Assert.AreEqual(ForeignKeyRule.NoAction, actual.UpdateRule);
            Assert.AreEqual(ForeignKeyRule.NoAction, actual.DeleteRule);
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysWithCascadeRule()
        {
            // Act
            var actual = GetForeignKey("Shipment", "FK_Shipment_Country");

            // Assert
            Assert.AreEqual(ForeignKeyRule.Cascade, actual.DeleteRule);
            Assert.AreEqual(ForeignKeyRule.NoAction, actual.UpdateRule);
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysWithSetDefaultRules()
        {
            // Act
            var actual = GetForeignKey("Preference", "FK_Preference_Country");

            // Assert
            Assert.AreEqual(ForeignKeyRule.SetDefault, actual.DeleteRule);
            Assert.AreEqual(ForeignKeyRule.SetDefault, actual.UpdateRule);
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysOfSelfReferencingForeignKey()
        {
            // Act
            var actual = GetForeignKey("Employee", "FK_Employee_Manager");

            // Assert
            CollectionAssert.AreEqual(new[] { "ManagerId" }, actual.Columns.ToArray());
            Assert.AreEqual("dbo.Employee", actual.ReferencedTable, StringComparer.Ordinal);
            CollectionAssert.AreEqual(new[] { "Id" }, actual.ReferencedColumns.ToArray());
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysAcrossSchemas()
        {
            // Act
            var actual = GetForeignKey("Ledger", "FK_Ledger_Invoice");

            // Assert
            Assert.AreEqual("Sales.Invoice", actual.ReferencedTable, StringComparer.Ordinal);
            Assert.AreEqual(ForeignKeyRule.Cascade, actual.DeleteRule);
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysOfTableInAnotherSchema()
        {
            // Act
            var actual = GetForeignKey("Sales.InvoiceLine", "FK_InvoiceLine_Invoice");

            // Assert
            Assert.AreEqual("Sales.Invoice", actual.ReferencedTable, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysOfCircularReferences()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var a = reader.GetForeignKeys("CycleA").Single();
                var b = reader.GetForeignKeys("CycleB").Single();

                // Assert
                Assert.AreEqual("dbo.CycleB", a.ReferencedTable, StringComparer.Ordinal);
                Assert.AreEqual("dbo.CycleA", b.ReferencedTable, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public async Task TestSqlServerSchemaReaderGetForeignKeysAsyncOfTableWithMultipleForeignKeys()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

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
        public void TestSqlServerSchemaComposerComposedSchemaOfSelfReferencingTable()
        {
            // Act (the table references itself, so it can be created on its own)
            Helper.CopyToTarget("Employee");

            // Assert
            Helper.AssertTargetMatchesSource("Employee");
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTableWithCompositeAndMultipleForeignKeys()
        {
            // Act
            Helper.CopyToTarget("Country", "OrderLine", "Shipment");

            // Assert
            Helper.AssertTargetMatchesSource("Shipment");
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTablesWithForeignKeysAcrossSchemas()
        {
            // Act
            Helper.CopyToTarget("Sales.Invoice", "Sales.InvoiceLine", "Ledger");

            // Assert
            Helper.AssertTargetMatchesSource("Sales.InvoiceLine");
            Helper.AssertTargetMatchesSource("Ledger");
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTableWithSetDefaultRules()
        {
            // Act
            Helper.CopyToTarget("Country", "Preference");

            // Assert
            Helper.AssertTargetMatchesSource("Preference");
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedCompositeForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("Country", "OrderLine", "Shipment");

            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Country] ([Name]) VALUES (N'Philippines');");
                connection.ExecuteNonQuery("INSERT INTO [dbo].[OrderLine] ([OrderId], [LineNumber], [Quantity]) VALUES (1, 1, 5);");

                // Act/Assert (the second column of the key does not match)
                Assert.Throws<SqlException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO [dbo].[Shipment] ([Id], [OrderId], [LineNumber], [CountryId]) VALUES (1, 1, 2, 1);"));

                // Act (a matching row)
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Shipment] ([Id], [OrderId], [LineNumber], [CountryId]) VALUES (1, 1, 1, 1);");
                var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM [dbo].[Shipment];");

                // Assert
                Assert.AreEqual(1, count);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedCascadeRuleIsApplied()
        {
            // Setup
            Helper.CopyToTarget("Country", "OrderLine", "Shipment");

            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Country] ([Name]) VALUES (N'Philippines');");
                connection.ExecuteNonQuery("INSERT INTO [dbo].[OrderLine] ([OrderId], [LineNumber], [Quantity]) VALUES (1, 1, 5);");
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Shipment] ([Id], [OrderId], [LineNumber], [CountryId]) VALUES (1, 1, 1, 1);");

                // Act
                connection.ExecuteNonQuery("DELETE FROM [dbo].[Country];");
                var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM [dbo].[Shipment];");

                // Assert
                Assert.AreEqual(0, count);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSelfReferencingForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("Employee");

            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<SqlException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO [dbo].[Employee] ([Id], [ManagerId]) VALUES (1, 99);"));

                connection.ExecuteNonQuery("INSERT INTO [dbo].[Employee] ([Id], [ManagerId]) VALUES (1, NULL);");
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Employee] ([Id], [ManagerId]) VALUES (2, 1);");
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSetDefaultRuleIsApplied()
        {
            // Setup
            Helper.CopyToTarget("Country", "Preference");

            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Country] ([Name]) VALUES (N'First'), (N'Second');");
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Preference] ([Id], [CountryId]) VALUES (1, 2);");

                // Act
                connection.ExecuteNonQuery("DELETE FROM [dbo].[Country] WHERE [Id] = 2;");
                var countryId = connection.ExecuteScalar<int>("SELECT [CountryId] FROM [dbo].[Preference];");

                // Assert (the default of the column is 1)
                Assert.AreEqual(1, countryId);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnComposedSchemaIfTheReferencedTableDoesNotExist()
        {
            // Act/Assert (a table is composed on its own, so the table that it references must already exist)
            Assert.Throws<SqlException>(() => Helper.CopyToTarget("Person"));
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTablesInWrongOrderViaComposeSchemas()
        {
            // Act (the referencing table is given before the referenced table)
            Helper.CopyAllToTarget("Person", "Country");

            // Assert
            Helper.AssertTargetMatchesSource("Country");
            Helper.AssertTargetMatchesSource("Person");
        }

        #endregion
    }
}
