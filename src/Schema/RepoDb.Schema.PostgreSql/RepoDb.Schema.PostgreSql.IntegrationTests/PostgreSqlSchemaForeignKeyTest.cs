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
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.PostgreSql.IntegrationTests
{
    [TestClass]
    public class PostgreSqlSchemaForeignKeyTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup() =>
            Database.Cleanup();

        #region Helpers

        private static PostgreSqlSchemaReader CreateReader() =>
            new PostgreSqlSchemaReader(new NpgsqlConnection(Database.ConnectionStringForSource));

        private static void ExecuteOnTarget(string commandText)
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(commandText);
            }
        }

        private static long CountOnTarget(string commandText)
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                return connection.ExecuteScalar<long>(commandText);
            }
        }

        #endregion

        #region Reader

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetForeignKeysOfTableWithMultipleForeignKeys()
        {
            // Act
            var foreignKeys = CreateReader().GetForeignKeys("transfer").OrderBy(x => x.Name).ToList();

            // Assert
            Assert.AreEqual(2, foreignKeys.Count);
            Assert.IsTrue(foreignKeys.All(x => x.ReferencedTable == "public.account"));
            CollectionAssert.AreEqual(new[] { "from_id" }, foreignKeys[0].Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "to_id" }, foreignKeys[1].Columns.ToArray());
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetForeignKeysOfCompositeForeignKey()
        {
            // Act
            var foreignKey = CreateReader().GetForeignKeys("shipment").Single();

            // Assert
            Assert.AreEqual("fk_shipment_region", foreignKey.Name);
            Assert.AreEqual("public.region_code", foreignKey.ReferencedTable);
            CollectionAssert.AreEqual(new[] { "country_code", "region" }, foreignKey.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "country_code", "code" }, foreignKey.ReferencedColumns.ToArray());
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetForeignKeysOfTableWithoutForeignKeys()
        {
            // Assert
            Assert.AreEqual(0, CreateReader().GetForeignKeys("country").Count());
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetForeignKeysDefaultRules()
        {
            // Act
            var foreignKey = CreateReader().GetForeignKeys("chain_b").Single();

            // Assert
            Assert.AreEqual(ForeignKeyRule.NoAction, foreignKey.DeleteRule);
            Assert.AreEqual(ForeignKeyRule.NoAction, foreignKey.UpdateRule);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetForeignKeysWithRestrictAndSetDefaultRules()
        {
            // Act
            var foreignKey = CreateReader().GetForeignKeys("preference").Single(x => x.Name == "fk_preference_color");

            // Assert
            Assert.AreEqual(ForeignKeyRule.SetDefault, foreignKey.DeleteRule);
            Assert.AreEqual(ForeignKeyRule.Restrict, foreignKey.UpdateRule);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetForeignKeysWithSetNullAndCascadeRules()
        {
            // Act
            var foreignKey = CreateReader().GetForeignKeys("preference").Single(x => x.Name == "fk_preference_other");

            // Assert
            Assert.AreEqual(ForeignKeyRule.SetNull, foreignKey.DeleteRule);
            Assert.AreEqual(ForeignKeyRule.Cascade, foreignKey.UpdateRule);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetForeignKeysOfSelfReferencingForeignKey()
        {
            // Act
            var foreignKey = CreateReader().GetForeignKeys("employee").Single();

            // Assert
            Assert.AreEqual("public.employee", foreignKey.ReferencedTable);
            CollectionAssert.AreEqual(new[] { "manager_id" }, foreignKey.Columns.ToArray());
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetForeignKeysAcrossSchemas()
        {
            // Act
            var foreignKeys = CreateReader().GetForeignKeys("item_ref").OrderBy(x => x.ReferencedTable).ToList();

            // Assert
            CollectionAssert.AreEqual(new[] { "public.item", "sales.item" }, foreignKeys.Select(x => x.ReferencedTable).ToArray());
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetForeignKeysOfCircularReferences()
        {
            // Act
            var reader = CreateReader();

            // Assert
            Assert.AreEqual("public.ring_2", reader.GetForeignKeys("ring_1").Single().ReferencedTable);
            Assert.AreEqual("public.ring_3", reader.GetForeignKeys("ring_2").Single().ReferencedTable);
            Assert.AreEqual("public.ring_1", reader.GetForeignKeys("ring_3").Single().ReferencedTable);
        }

        [TestMethod]
        public async Task TestPostgreSqlSchemaReaderGetForeignKeysAsyncOfTableWithMultipleForeignKeys()
        {
            // Act
            var reader = CreateReader();
            var actual = (await reader.GetForeignKeysAsync("transfer")).ToList();

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(1, (await reader.GetForeignKeysAsync("shipment")).Count());
        }

        #endregion

        #region Composer

        [TestMethod]
        public void TestPostgreSqlSchemaComposerComposedSchemaOfSelfReferencingTable()
        {
            // Act
            Helper.CopyToTarget("employee");

            // Assert
            Helper.AssertTargetMatchesSource("employee");
        }

        [TestMethod]
        public void TestPostgreSqlSchemaComposerComposedSchemaOfTableWithCompositeAndMultipleForeignKeys()
        {
            // Act
            Helper.CopyToTarget("region_code", "shipment", "account", "transfer");

            // Assert
            foreach (var table in new[] { "region_code", "shipment", "account", "transfer" })
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        [TestMethod]
        public void TestPostgreSqlSchemaComposerComposedSchemaOfTablesWithForeignKeysAcrossSchemas()
        {
            // Act
            Helper.CopyToTarget("item", "sales.item", "item_ref");

            // Assert
            Helper.AssertTargetMatchesSource("item_ref");
            Helper.AssertTargetMatchesSource("sales.item");
        }

        [TestMethod]
        public void TestPostgreSqlSchemaComposerComposedSchemaOfTableWithRestrictAndSetDefaultRules()
        {
            // Act
            Helper.CopyToTarget("color", "preference");

            // Assert
            Helper.AssertTargetMatchesSource("preference");
        }

        [TestMethod]
        public void TestPostgreSqlSchemaComposerComposedCompositeForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("region_code", "shipment");
            ExecuteOnTarget("INSERT INTO region_code VALUES ('PH', 1);");

            // Act/Assert
            ExecuteOnTarget("INSERT INTO shipment VALUES (1, 'PH', 1);");
            Assert.Throws<PostgresException>(() => ExecuteOnTarget("INSERT INTO shipment VALUES (2, 'PH', 2);"));
        }

        [TestMethod]
        public void TestPostgreSqlSchemaComposerComposedCascadeRuleIsApplied()
        {
            // Setup
            Helper.CopyToTarget("color", "preference");
            ExecuteOnTarget("INSERT INTO color VALUES (0), (1); INSERT INTO preference VALUES (1, 1, 1);");

            // Act (the foreign key of the other_id is on delete set null)
            ExecuteOnTarget("DELETE FROM color WHERE id = 1;");

            // Assert
            Assert.AreEqual(1, CountOnTarget("SELECT COUNT(*) FROM preference WHERE other_id IS NULL AND color_id = 0;"));
        }

        [TestMethod]
        public void TestPostgreSqlSchemaComposerComposedSelfReferencingForeignKeyIsEnforced()
        {
            // Setup
            Helper.CopyToTarget("employee");
            ExecuteOnTarget("INSERT INTO employee VALUES (1, NULL);");

            // Act/Assert
            ExecuteOnTarget("INSERT INTO employee VALUES (2, 1);");
            Assert.Throws<PostgresException>(() => ExecuteOnTarget("INSERT INTO employee VALUES (3, 99);"));
        }

        [TestMethod]
        public void ThrowExceptionOnComposedSchemaIfTheReferencedTableDoesNotExist()
        {
            // Act/Assert
            Assert.Throws<PostgresException>(() => Helper.CopyToTarget("chain_b"));
        }

        [TestMethod]
        public void TestPostgreSqlSchemaComposerComposedSchemaOfTablesInWrongOrderViaComposeSchemas()
        {
            // Act
            Helper.CopyAllToTarget("chain_c", "chain_b", "chain_a");

            // Assert
            foreach (var table in new[] { "chain_a", "chain_b", "chain_c" })
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        #endregion
    }
}
