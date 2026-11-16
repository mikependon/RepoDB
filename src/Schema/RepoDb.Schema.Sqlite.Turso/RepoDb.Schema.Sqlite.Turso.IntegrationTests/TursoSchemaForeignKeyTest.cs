#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Turso.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.Sqlite.Turso.IntegrationTests.Setup;

namespace RepoDb.Schema.Sqlite.Turso.IntegrationTests
{
    /// <summary>
    /// Contains the turso schema foreign key test behavior.
    /// </summary>
    [TestClass]
    public class TursoSchemaForeignKeyTest
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
                return new TursoSchemaReader().GetForeignKeys(connection,
                    tableName,
                    transaction: null).Single(f => f.Name == foreignKeyName);
            }
        }

        #endregion

        #region Reader

        /// <summary>
        /// Verifies turso schema reader get foreign keys of table with multiple foreign keys.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaReaderGetForeignKeysOfTableWithMultipleForeignKeys()
        {
            using (var connection = Database.CreateSource())
            {
                // Setup
                var reader = new TursoSchemaReader();
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
        /// Verifies turso schema reader get foreign keys of composite foreign key.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaReaderGetForeignKeysOfCompositeForeignKey()
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
        /// Verifies turso schema reader get foreign keys with cascade rule.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaReaderGetForeignKeysWithCascadeRule()
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
        /// Verifies turso schema reader get foreign keys with set default rules.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaReaderGetForeignKeysWithSetDefaultRules()
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
        /// Verifies turso schema reader get foreign keys of self referencing foreign key.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaReaderGetForeignKeysOfSelfReferencingForeignKey()
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
        /// Verifies turso schema reader get foreign keys of circular references.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaReaderGetForeignKeysOfCircularReferences()
        {
            using (var connection = Database.CreateSource())
            {
                // Setup
                var reader = new TursoSchemaReader();
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
        /// Verifies turso schema reader get foreign keys async of table with multiple foreign keys.
        /// </summary>
        [TestMethod]
        public async Task TestTursoSchemaReaderGetForeignKeysAsyncOfTableWithMultipleForeignKeys()
        {
            using (var connection = Database.CreateSource())
            {
                // Setup
                var reader = new TursoSchemaReader();
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
        /// Verifies turso schema composer composed schema of self referencing table.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaComposerComposedSchemaOfSelfReferencingTable()
        {
            // Act
            Helper.CopyToTarget("Employee");
            // Assert
            Helper.AssertTargetMatchesSource("Employee");
        }

        /// <summary>
        /// Verifies turso schema composer composed schema of table with composite and multiple foreign keys.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaComposerComposedSchemaOfTableWithCompositeAndMultipleForeignKeys()
        {
            // Act
            Helper.CopyToTarget("Country",
                "OrderLine",
                "Shipment");
            // Assert
            Helper.AssertTargetMatchesSource("Shipment");
        }

        /// <summary>
        /// Verifies turso schema composer composed schema of table with set default rules.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaComposerComposedSchemaOfTableWithSetDefaultRules()
        {
            // Act
            Helper.CopyToTarget("Country",
                "Preference");
            // Assert
            Helper.AssertTargetMatchesSource("Preference");
        }

        /// <summary>
        /// Verifies turso schema composer composed composite foreign key is enforced.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaComposerComposedCompositeForeignKeyIsEnforced()
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
        /// Verifies turso schema composer composed cascade rule is applied.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaComposerComposedCascadeRuleIsApplied()
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
        /// Verifies turso schema composer composed self referencing foreign key is enforced.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaComposerComposedSelfReferencingForeignKeyIsEnforced()
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
        /// Verifies turso schema composer composed set default rule is applied.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaComposerComposedSetDefaultRuleIsApplied()
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
        /// Verifies turso schema composer composed schema creates the table even if the referenced table does not exist.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaComposerComposedSchemaCreatesTheTableEvenIfTheReferencedTableDoesNotExist()
        {
            // Act
            Helper.CopyToTarget("Person");
            // Assert
            Assert.IsTrue(Helper.TargetTableExists("Person"));
            Assert.AreEqual(1,
                Helper.GetTargetSchema("Person").ForeignKeys.Count);
        }

        /// <summary>
        /// Verifies turso schema composer composed schema of tables in wrong order via compose schemas.
        /// </summary>
        [TestMethod]
        public void TestTursoSchemaComposerComposedSchemaOfTablesInWrongOrderViaComposeSchemas()
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
