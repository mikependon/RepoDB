#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Ahtola.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.Sqlite.Ahtola.IntegrationTests.Setup;

namespace RepoDb.Schema.Sqlite.Ahtola.IntegrationTests
{
    /// <summary>
    /// Contains the ahtola schema foreign key test behavior.
    /// </summary>
    [TestClass]
    public class AhtolaSchemaForeignKeyTest
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
            using (var connection = Database.CreateSource())
            {
                return new AhtolaSchemaReader().GetForeignKeys(connection,
                    tableName,
                    transaction: null).Single(f => f.Name == foreignKeyName);
            }
        }

        #endregion

        #region Reader

        /// <summary>
        /// Verifies ahtola schema reader get foreign keys of table with multiple foreign keys.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaReaderGetForeignKeysOfTableWithMultipleForeignKeys()
        {
            using (var connection = Database.CreateSource())
            {
                // Setup
                var reader = new AhtolaSchemaReader();
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
        /// Verifies ahtola schema reader get foreign keys of composite foreign key.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaReaderGetForeignKeysOfCompositeForeignKey()
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
            Assert.AreEqual(ForeignKeyRule.NoAction,
                actual.UpdateRule);
            Assert.AreEqual(ForeignKeyRule.NoAction,
                actual.DeleteRule);
        }

        /// <summary>
        /// Verifies ahtola schema reader get foreign keys with cascade rule.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaReaderGetForeignKeysWithCascadeRule()
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
        /// Verifies ahtola schema reader get foreign keys with set default rules.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaReaderGetForeignKeysWithSetDefaultRules()
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
        /// Verifies ahtola schema reader get foreign keys of self referencing foreign key.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaReaderGetForeignKeysOfSelfReferencingForeignKey()
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
        /// Verifies ahtola schema reader get foreign keys of table in another schema with cascade rule.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaReaderGetForeignKeysOfTableInAnotherSchemaWithCascadeRule()
        {
            // Act
            var actual = GetForeignKey($"{Database.SalesSchema}.InvoiceLine",
                "FK_InvoiceLine_Invoice");
            // Assert
            Assert.AreEqual(new TableInfo("Invoice",
                    Database.SalesSchema),
                actual.ReferencedTable);
            Assert.AreEqual(ForeignKeyRule.Cascade,
                actual.DeleteRule);
        }

        /// <summary>
        /// Verifies ahtola schema reader get foreign keys of table in another schema.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaReaderGetForeignKeysOfTableInAnotherSchema()
        {
            // Act
            var actual = GetForeignKey($"{Database.SalesSchema}.InvoiceLine",
                "FK_InvoiceLine_Invoice");
            // Assert
            Assert.AreEqual(new TableInfo("Invoice",
                    Database.SalesSchema),
                actual.ReferencedTable);
        }

        /// <summary>
        /// Verifies ahtola schema reader get foreign keys of circular references.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaReaderGetForeignKeysOfCircularReferences()
        {
            using (var connection = Database.CreateSource())
            {
                // Setup
                var reader = new AhtolaSchemaReader();
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
        /// Verifies ahtola schema reader get foreign keys async of table with multiple foreign keys.
        /// </summary>
        [TestMethod]
        public async Task TestAhtolaSchemaReaderGetForeignKeysAsyncOfTableWithMultipleForeignKeys()
        {
            using (var connection = Database.CreateSource())
            {
                // Setup
                var reader = new AhtolaSchemaReader();
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
        /// Verifies ahtola schema composer composed schema of self referencing table.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaComposerComposedSchemaOfSelfReferencingTable()
        {
            // Act
            Helper.CopyToTarget("Employee");
            // Assert
            Helper.AssertTargetMatchesSource("Employee");
        }

        /// <summary>
        /// Verifies ahtola schema composer composed schema of table with composite and multiple foreign keys.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaComposerComposedSchemaOfTableWithCompositeAndMultipleForeignKeys()
        {
            // Act
            Helper.CopyToTarget("Country",
                "OrderLine",
                "Shipment");
            // Assert
            Helper.AssertTargetMatchesSource("Shipment");
        }

        /// <summary>
        /// Verifies ahtola schema composer composed schema of tables with foreign keys in another schema.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaComposerComposedSchemaOfTablesWithForeignKeysInAnotherSchema()
        {
            // Act
            Helper.CopyToTarget($"{Database.SalesSchema}.Invoice",
                $"{Database.SalesSchema}.InvoiceLine",
                "Ledger");
            // Assert
            Helper.AssertTargetMatchesSource($"{Database.SalesSchema}.InvoiceLine");
            Helper.AssertTargetMatchesSource("Ledger");
        }

        /// <summary>
        /// Verifies ahtola schema composer composed schema of table with set default rules.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaComposerComposedSchemaOfTableWithSetDefaultRules()
        {
            // Act
            Helper.CopyToTarget("Country",
                "Preference");
            // Assert
            Helper.AssertTargetMatchesSource("Preference");
        }

        /// <summary>
        /// Verifies ahtola schema composer composed composite foreign key is enforced.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaComposerComposedCompositeForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("Country",
                "OrderLine",
                "Shipment");
            using (var connection = Database.CreateTarget())
            {
                connection.ExecuteNonQuery("INSERT INTO [Country] ([Name]) VALUES ('Philippines');");
                connection.ExecuteNonQuery("INSERT INTO [OrderLine] ([OrderId], [LineNumber], [Quantity]) VALUES (1, 1, 5);");
                // Act/Assert
                Assert.Throws<SqliteException>(() => connection.ExecuteNonQuery("INSERT INTO [Shipment] ([Id], [OrderId], [LineNumber], [CountryId]) VALUES (1, 1, 2, 1);"));
                // Act
                connection.ExecuteNonQuery("INSERT INTO [Shipment] ([Id], [OrderId], [LineNumber], [CountryId]) VALUES (1, 1, 1, 1);");
                var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM [Shipment];");
                // Assert
                Assert.AreEqual(1,
                    count);
            }
        }

        /// <summary>
        /// Verifies ahtola schema composer composed cascade rule is applied.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaComposerComposedCascadeRuleIsApplied()
        {
            // Setup
            Helper.CopyToTarget("Country",
                "OrderLine",
                "Shipment");
            using (var connection = Database.CreateTarget())
            {
                connection.ExecuteNonQuery("INSERT INTO [Country] ([Name]) VALUES ('Philippines');");
                connection.ExecuteNonQuery("INSERT INTO [OrderLine] ([OrderId], [LineNumber], [Quantity]) VALUES (1, 1, 5);");
                connection.ExecuteNonQuery("INSERT INTO [Shipment] ([Id], [OrderId], [LineNumber], [CountryId]) VALUES (1, 1, 1, 1);");
                // Act
                connection.ExecuteNonQuery("DELETE FROM [Country];");
                var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM [Shipment];");
                // Assert
                Assert.AreEqual(0,
                    count);
            }
        }

        /// <summary>
        /// Verifies ahtola schema composer composed self referencing foreign key is enforced.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaComposerComposedSelfReferencingForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("Employee");
            using (var connection = Database.CreateTarget())
            {
                // Act/Assert
                Assert.Throws<SqliteException>(() => connection.ExecuteNonQuery("INSERT INTO [Employee] ([Id], [ManagerId]) VALUES (1, 99);"));
                connection.ExecuteNonQuery("INSERT INTO [Employee] ([Id], [ManagerId]) VALUES (1, NULL);");
                connection.ExecuteNonQuery("INSERT INTO [Employee] ([Id], [ManagerId]) VALUES (2, 1);");
            }
        }

        /// <summary>
        /// Verifies ahtola schema composer composed set default rule is applied.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaComposerComposedSetDefaultRuleIsApplied()
        {
            // Setup
            Helper.CopyToTarget("Country",
                "Preference");
            using (var connection = Database.CreateTarget())
            {
                connection.ExecuteNonQuery("INSERT INTO [Country] ([Name]) VALUES ('First'), ('Second');");
                connection.ExecuteNonQuery("INSERT INTO [Preference] ([Id], [CountryId]) VALUES (1, 2);");
                // Act
                connection.ExecuteNonQuery("DELETE FROM [Country] WHERE [Id] = 2;");
                var countryId = connection.ExecuteScalar<int>("SELECT [CountryId] FROM [Preference];");
                // Assert
                Assert.AreEqual(1,
                    countryId);
            }
        }

        /// <summary>
        /// Verifies ahtola schema composer composed schema creates the table even if the referenced table does not exist.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaComposerComposedSchemaCreatesTheTableEvenIfTheReferencedTableDoesNotExist()
        {
            // Act
            Helper.CopyToTarget("Person");
            // Assert
            Assert.IsTrue(Helper.TargetTableExists("Person"));
            Assert.AreEqual(1,
                Helper.GetTargetSchema("Person").ForeignKeys.Count);
        }

        /// <summary>
        /// Verifies ahtola schema composer composed schema of tables in wrong order via compose schemas.
        /// </summary>
        [TestMethod]
        public void TestAhtolaSchemaComposerComposedSchemaOfTablesInWrongOrderViaComposeSchemas()
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
