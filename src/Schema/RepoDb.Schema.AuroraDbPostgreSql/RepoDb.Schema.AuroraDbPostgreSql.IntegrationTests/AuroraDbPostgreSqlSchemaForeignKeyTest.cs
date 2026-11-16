#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests.Setup;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests
{
    /// <summary>
    /// Contains the aurora db postgre sql schema foreign key test behavior.
    /// </summary>
    [TestClass]
    public class AuroraDbPostgreSqlSchemaForeignKeyTest
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
        public void Cleanup() => Database.Cleanup();
        #region Private Methods

        /// <summary>
        /// Performs create reader.
        /// </summary>
        private static AuroraDbPostgreSqlSchemaReader CreateReader() => new AuroraDbPostgreSqlSchemaReader();

        /// <summary>
        /// Performs execute on target.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        private static void ExecuteOnTarget(string commandText)
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(commandText);
            }
        }

        /// <summary>
        /// Performs count on target.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        private static long CountOnTarget(string commandText)
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                return connection.ExecuteScalar<long>(commandText);
            }
        }

        #endregion

        #region Reader

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get foreign keys of table with multiple foreign keys.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetForeignKeysOfTableWithMultipleForeignKeys()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var foreignKeys = CreateReader().GetForeignKeys(operationConnection,
                "transfer").OrderBy(x => x.Name).ToList();
            // Assert
            Assert.AreEqual(2,
                foreignKeys.Count);
            Assert.IsTrue(foreignKeys.All(x => x.ReferencedTable == new TableInfo("account",
                        "public")));
            CollectionAssert.AreEqual(new[] { "from_id" },
                foreignKeys[0].Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "to_id" },
                foreignKeys[1].Columns.ToArray());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get foreign keys of composite foreign key.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetForeignKeysOfCompositeForeignKey()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var foreignKey = CreateReader().GetForeignKeys(operationConnection,
                "shipment").Single();
            // Assert
            Assert.AreEqual("fk_shipment_region",
                foreignKey.Name);
            Assert.AreEqual(new TableInfo("region_code",
                    "public"),
                foreignKey.ReferencedTable);
            CollectionAssert.AreEqual(new[] { "country_code", "region" },
                foreignKey.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "country_code", "code" },
                foreignKey.ReferencedColumns.ToArray());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get foreign keys of table without foreign keys.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetForeignKeysOfTableWithoutForeignKeys()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Assert
            Assert.AreEqual(0,
                CreateReader().GetForeignKeys(operationConnection,
                    "country").Count());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get foreign keys default rules.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetForeignKeysDefaultRules()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var foreignKey = CreateReader().GetForeignKeys(operationConnection,
                "chain_b").Single();
            // Assert
            Assert.AreEqual(ForeignKeyRule.NoAction,
                foreignKey.DeleteRule);
            Assert.AreEqual(ForeignKeyRule.NoAction,
                foreignKey.UpdateRule);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get foreign keys with restrict and set default rules.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetForeignKeysWithRestrictAndSetDefaultRules()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var foreignKey = CreateReader().GetForeignKeys(operationConnection,
                "preference").Single(x => x.Name == "fk_preference_color");
            // Assert
            Assert.AreEqual(ForeignKeyRule.SetDefault,
                foreignKey.DeleteRule);
            Assert.AreEqual(ForeignKeyRule.Restrict,
                foreignKey.UpdateRule);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get foreign keys with set null and cascade rules.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetForeignKeysWithSetNullAndCascadeRules()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var foreignKey = CreateReader().GetForeignKeys(operationConnection,
                "preference").Single(x => x.Name == "fk_preference_other");
            // Assert
            Assert.AreEqual(ForeignKeyRule.SetNull,
                foreignKey.DeleteRule);
            Assert.AreEqual(ForeignKeyRule.Cascade,
                foreignKey.UpdateRule);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get foreign keys of self referencing foreign key.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetForeignKeysOfSelfReferencingForeignKey()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var foreignKey = CreateReader().GetForeignKeys(operationConnection,
                "employee").Single();
            // Assert
            Assert.AreEqual(new TableInfo("employee",
                    "public"),
                foreignKey.ReferencedTable);
            CollectionAssert.AreEqual(new[] { "manager_id" },
                foreignKey.Columns.ToArray());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get foreign keys across schemas.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetForeignKeysAcrossSchemas()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var foreignKeys = CreateReader().GetForeignKeys(operationConnection,
                "item_ref").OrderBy(x => x.ReferencedTable.Schema,
                StringComparer.Ordinal).ToList();
            // Assert
            CollectionAssert.AreEqual(new[] { new TableInfo("item",
                    "public"), new TableInfo("item",
                    "sales") },
                foreignKeys.Select(x => x.ReferencedTable).ToArray());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get foreign keys of circular references.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetForeignKeysOfCircularReferences()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var reader = CreateReader();
            // Assert
            Assert.AreEqual(new TableInfo("ring_2",
                    "public"),
                reader.GetForeignKeys(operationConnection,
                    "ring_1").Single().ReferencedTable);
            Assert.AreEqual(new TableInfo("ring_3",
                    "public"),
                reader.GetForeignKeys(operationConnection,
                    "ring_2").Single().ReferencedTable);
            Assert.AreEqual(new TableInfo("ring_1",
                    "public"),
                reader.GetForeignKeys(operationConnection,
                    "ring_3").Single().ReferencedTable);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get foreign keys async of table with multiple foreign keys.
        /// </summary>
        [TestMethod]
        public async Task TestAuroraDbPostgreSqlSchemaReaderGetForeignKeysAsyncOfTableWithMultipleForeignKeys()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var reader = CreateReader();
            var actual = (await reader.GetForeignKeysAsync(operationConnection,
                "transfer")).ToList();
            // Assert
            Assert.AreEqual(2,
                actual.Count);
            Assert.AreEqual(1,
                (await reader.GetForeignKeysAsync(operationConnection,
                    "shipment")).Count());
        }

        #endregion

        #region Composer

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of self referencing table.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfSelfReferencingTable()
        {
            // Act
            Helper.CopyToTarget("employee");
            // Assert
            Helper.AssertTargetMatchesSource("employee");
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of table with composite and multiple foreign keys.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfTableWithCompositeAndMultipleForeignKeys()
        {
            // Act
            Helper.CopyToTarget("region_code",
                "shipment",
                "account",
                "transfer");
            // Assert
            foreach (var table in new[]
            {
                "region_code",
                "shipment",
                "account",
                "transfer"
            }

            )
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of tables with foreign keys across schemas.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfTablesWithForeignKeysAcrossSchemas()
        {
            // Act
            Helper.CopyToTarget("item",
                "sales.item",
                "item_ref");
            // Assert
            Helper.AssertTargetMatchesSource("item_ref");
            Helper.AssertTargetMatchesSource("sales.item");
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of table with restrict and set default rules.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfTableWithRestrictAndSetDefaultRules()
        {
            // Act
            Helper.CopyToTarget("color",
                "preference");
            // Assert
            Helper.AssertTargetMatchesSource("preference");
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed composite foreign key is enforced.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedCompositeForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("region_code",
                "shipment");
            ExecuteOnTarget("INSERT INTO region_code VALUES ('PH', 1);");
            // Act/Assert
            ExecuteOnTarget("INSERT INTO shipment VALUES (1, 'PH', 1);");
            Assert.Throws<AuroraDbException>(() => ExecuteOnTarget("INSERT INTO shipment VALUES (2, 'PH', 2);"));
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed cascade rule is applied.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedCascadeRuleIsApplied()
        {
            // Setup
            Helper.CopyToTarget("color",
                "preference");
            ExecuteOnTarget("INSERT INTO color VALUES (0), (1); INSERT INTO preference VALUES (1, 1, 1);");
            // Act
            ExecuteOnTarget("DELETE FROM color WHERE id = 1;");
            // Assert
            Assert.AreEqual(1,
                CountOnTarget("SELECT COUNT(*) FROM preference WHERE other_id IS NULL AND color_id = 0;"));
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed self referencing foreign key is enforced.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSelfReferencingForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("employee");
            ExecuteOnTarget("INSERT INTO employee VALUES (1, NULL);");
            // Act/Assert
            ExecuteOnTarget("INSERT INTO employee VALUES (2, 1);");
            Assert.Throws<AuroraDbException>(() => ExecuteOnTarget("INSERT INTO employee VALUES (3, 99);"));
        }

        /// <summary>
        /// Verifies throw exception on composed schema if the referenced table does not exist.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnComposedSchemaIfTheReferencedTableDoesNotExist()
        {
            // Act/Assert
            Assert.Throws<AuroraDbException>(() => Helper.CopyToTarget("chain_b"));
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of tables in wrong order via compose schemas.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfTablesInWrongOrderViaComposeSchemas()
        {
            // Act
            Helper.CopyAllToTarget("chain_c",
                "chain_b",
                "chain_a");
            // Assert
            foreach (var table in new[]
            {
                "chain_a",
                "chain_b",
                "chain_c"
            }

            )
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }
        #endregion
    }
}
