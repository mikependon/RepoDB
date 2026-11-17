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
    /// <summary>
    /// Contains the sql server schema foreign key test behavior.
    /// </summary>
    [TestClass]
    public class SqlServerSchemaForeignKeyTest
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
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                return new SqlServerSchemaReader().GetForeignKeys(connection,
                    tableName,
                    transaction: null).Single(f => f.Name == foreignKeyName);
            }
        }

        #endregion

        #region Reader

        /// <summary>
        /// Verifies sql server schema reader get foreign keys of table with multiple foreign keys.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysOfTableWithMultipleForeignKeys()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader();
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
        /// Verifies sql server schema reader get foreign keys of composite foreign key.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysOfCompositeForeignKey()
        {
            // Act
            var actual = GetForeignKey("Shipment",
                "FK_Shipment_OrderLine");
            // Assert
            CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" },
                actual.Columns.ToArray());
            Assert.AreEqual(new TableInfo("OrderLine",
                    "dbo"),
                actual.ReferencedTable);
            CollectionAssert.AreEqual(new[] { "OrderId", "LineNumber" },
                actual.ReferencedColumns.ToArray());
            Assert.AreEqual(ForeignKeyRule.NoAction,
                actual.UpdateRule);
            Assert.AreEqual(ForeignKeyRule.NoAction,
                actual.DeleteRule);
        }

        /// <summary>
        /// Verifies sql server schema reader get foreign keys with cascade rule.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysWithCascadeRule()
        {
            // Act
            var actual = GetForeignKey("Shipment",
                "FK_Shipment_Country");
            // Assert
            Assert.AreEqual(ForeignKeyRule.Cascade,
                actual.DeleteRule);
            Assert.AreEqual(ForeignKeyRule.NoAction,
                actual.UpdateRule);
        }

        /// <summary>
        /// Verifies sql server schema reader get foreign keys with set default rules.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysWithSetDefaultRules()
        {
            // Act
            var actual = GetForeignKey("Preference",
                "FK_Preference_Country");
            // Assert
            Assert.AreEqual(ForeignKeyRule.SetDefault,
                actual.DeleteRule);
            Assert.AreEqual(ForeignKeyRule.SetDefault,
                actual.UpdateRule);
        }

        /// <summary>
        /// Verifies sql server schema reader get foreign keys of self referencing foreign key.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysOfSelfReferencingForeignKey()
        {
            // Act
            var actual = GetForeignKey("Employee",
                "FK_Employee_Manager");
            // Assert
            CollectionAssert.AreEqual(new[] { "ManagerId" },
                actual.Columns.ToArray());
            Assert.AreEqual(new TableInfo("Employee",
                    "dbo"),
                actual.ReferencedTable);
            CollectionAssert.AreEqual(new[] { "Id" },
                actual.ReferencedColumns.ToArray());
        }

        /// <summary>
        /// Verifies sql server schema reader get foreign keys across schemas.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysAcrossSchemas()
        {
            // Act
            var actual = GetForeignKey("Ledger",
                "FK_Ledger_Invoice");
            // Assert
            Assert.AreEqual(new TableInfo("Invoice",
                    "Sales"),
                actual.ReferencedTable);
            Assert.AreEqual(ForeignKeyRule.Cascade,
                actual.DeleteRule);
        }

        /// <summary>
        /// Verifies sql server schema reader get foreign keys of table in another schema.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysOfTableInAnotherSchema()
        {
            // Act
            var actual = GetForeignKey("Sales.InvoiceLine",
                "FK_InvoiceLine_Invoice");
            // Assert
            Assert.AreEqual(new TableInfo("Invoice",
                    "Sales"),
                actual.ReferencedTable);
        }

        /// <summary>
        /// Verifies sql server schema reader get foreign keys of circular references.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderGetForeignKeysOfCircularReferences()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader();
                // Act
                var a = reader.GetForeignKeys(connection,
                    "CycleA",
                    transaction: null).Single();
                var b = reader.GetForeignKeys(connection,
                    "CycleB",
                    transaction: null).Single();
                // Assert
                Assert.AreEqual(new TableInfo("CycleB",
                        "dbo"),
                    a.ReferencedTable);
                Assert.AreEqual(new TableInfo("CycleA",
                        "dbo"),
                    b.ReferencedTable);
            }
        }

        /// <summary>
        /// Verifies sql server schema reader get foreign keys async of table with multiple foreign keys.
        /// </summary>
        [TestMethod]
        public async Task TestSqlServerSchemaReaderGetForeignKeysAsyncOfTableWithMultipleForeignKeys()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader();
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
        /// Verifies sql server schema composer composed schema of self referencing table.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfSelfReferencingTable()
        {
            // Act
            Helper.CopyToTarget("Employee");
            // Assert
            Helper.AssertTargetMatchesSource("Employee");
        }

        /// <summary>
        /// Verifies sql server schema composer composed schema of table with composite and multiple foreign keys.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTableWithCompositeAndMultipleForeignKeys()
        {
            // Act
            Helper.CopyToTarget("Country",
                "OrderLine",
                "Shipment");
            // Assert
            Helper.AssertTargetMatchesSource("Shipment");
        }

        /// <summary>
        /// Verifies sql server schema composer composed schema of tables with foreign keys across schemas.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTablesWithForeignKeysAcrossSchemas()
        {
            // Act
            Helper.CopyToTarget("Sales.Invoice",
                "Sales.InvoiceLine",
                "Ledger");
            // Assert
            Helper.AssertTargetMatchesSource("Sales.InvoiceLine");
            Helper.AssertTargetMatchesSource("Ledger");
        }

        /// <summary>
        /// Verifies sql server schema composer composed schema of table with set default rules.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTableWithSetDefaultRules()
        {
            // Act
            Helper.CopyToTarget("Country",
                "Preference");
            // Assert
            Helper.AssertTargetMatchesSource("Preference");
        }

        /// <summary>
        /// Verifies sql server schema composer composed composite foreign key is enforced.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaComposerComposedCompositeForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("Country",
                "OrderLine",
                "Shipment");
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Country] ([Name]) VALUES (N'Philippines');");
                connection.ExecuteNonQuery("INSERT INTO [dbo].[OrderLine] ([OrderId], [LineNumber], [Quantity]) VALUES (1, 1, 5);");
                // Act/Assert
                Assert.Throws<SqlException>(() => connection.ExecuteNonQuery("INSERT INTO [dbo].[Shipment] ([Id], [OrderId], [LineNumber], [CountryId]) VALUES (1, 1, 2, 1);"));
                // Act
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Shipment] ([Id], [OrderId], [LineNumber], [CountryId]) VALUES (1, 1, 1, 1);");
                var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM [dbo].[Shipment];");
                // Assert
                Assert.AreEqual(1,
                    count);
            }
        }

        /// <summary>
        /// Verifies sql server schema composer composed cascade rule is applied.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaComposerComposedCascadeRuleIsApplied()
        {
            // Setup
            Helper.CopyToTarget("Country",
                "OrderLine",
                "Shipment");
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Country] ([Name]) VALUES (N'Philippines');");
                connection.ExecuteNonQuery("INSERT INTO [dbo].[OrderLine] ([OrderId], [LineNumber], [Quantity]) VALUES (1, 1, 5);");
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Shipment] ([Id], [OrderId], [LineNumber], [CountryId]) VALUES (1, 1, 1, 1);");
                // Act
                connection.ExecuteNonQuery("DELETE FROM [dbo].[Country];");
                var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM [dbo].[Shipment];");
                // Assert
                Assert.AreEqual(0,
                    count);
            }
        }

        /// <summary>
        /// Verifies sql server schema composer composed self referencing foreign key is enforced.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSelfReferencingForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("Employee");
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<SqlException>(() => connection.ExecuteNonQuery("INSERT INTO [dbo].[Employee] ([Id], [ManagerId]) VALUES (1, 99);"));
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Employee] ([Id], [ManagerId]) VALUES (1, NULL);");
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Employee] ([Id], [ManagerId]) VALUES (2, 1);");
            }
        }

        /// <summary>
        /// Verifies sql server schema composer composed set default rule is applied.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSetDefaultRuleIsApplied()
        {
            // Setup
            Helper.CopyToTarget("Country",
                "Preference");
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Country] ([Name]) VALUES (N'First'), (N'Second');");
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Preference] ([Id], [CountryId]) VALUES (1, 2);");
                // Act
                connection.ExecuteNonQuery("DELETE FROM [dbo].[Country] WHERE [Id] = 2;");
                var countryId = connection.ExecuteScalar<int>("SELECT [CountryId] FROM [dbo].[Preference];");
                // Assert
                Assert.AreEqual(1,
                    countryId);
            }
        }

        /// <summary>
        /// Verifies throw exception on composed schema if the referenced table does not exist.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnComposedSchemaIfTheReferencedTableDoesNotExist()
        {
            // Act/Assert
            Assert.Throws<SqlException>(() => Helper.CopyToTarget("Person"));
        }

        /// <summary>
        /// Verifies sql server schema composer composed schema of tables in wrong order via compose schemas.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTablesInWrongOrderViaComposeSchemas()
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
