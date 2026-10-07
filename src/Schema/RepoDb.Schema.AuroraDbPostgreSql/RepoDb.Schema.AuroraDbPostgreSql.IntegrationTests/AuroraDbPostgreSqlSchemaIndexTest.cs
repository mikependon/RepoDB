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
using RepoDb.Schema.Models;
using RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests
{
    [TestClass]
    public class AuroraDbPostgreSqlSchemaIndexTest
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

        private static AuroraDbPostgreSqlSchemaReader CreateReader() =>
            new AuroraDbPostgreSqlSchemaReader(new AuroraDbConnection(Database.ConnectionStringForSource));

        private static IndexInfo Index(string name) =>
            CreateReader().GetIndexes("product").Single(x => x.Name == name);

        #endregion

        #region Reader

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetIndexesOfTableWithMultipleIndexes()
        {
            // Act
            var indexes = CreateReader().GetIndexes("product").ToList();

            // Assert
            CollectionAssert.AreEquivalent(new[] { "ix_product_name", "ux_product_code", "ix_product_multi", "ix_product_active", "ix_product_all_desc" }, indexes.Select(x => x.Name).ToArray());
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetIndexesOfNonUniqueIndex()
        {
            // Act
            var index = Index("ix_product_name");

            // Assert
            Assert.IsFalse(index.IsUnique);
            Assert.IsFalse(index.IsClustered);
            Assert.IsNull(index.Filter);
            CollectionAssert.AreEqual(new[] { "name" }, index.Columns.ToArray());
            Assert.AreEqual(0, index.DescendingColumns.Count);
            Assert.AreEqual(0, index.IncludedColumns.Count);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetIndexesOfUniqueIndex()
        {
            // Act
            var index = Index("ux_product_code");

            // Assert
            Assert.IsTrue(index.IsUnique);
            CollectionAssert.AreEqual(new[] { "code" }, index.Columns.ToArray());
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetIndexesOfMultiColumnIndexWithDescendingKeyAndIncludedColumn()
        {
            // Act
            var index = Index("ix_product_multi");

            // Assert
            CollectionAssert.AreEqual(new[] { "name", "price" }, index.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "name" }, index.DescendingColumns.ToArray());
            CollectionAssert.AreEqual(new[] { "code" }, index.IncludedColumns.ToArray());
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetIndexesOfIndexWithAllDescendingKeys()
        {
            // Act
            var index = Index("ix_product_all_desc");

            // Assert
            CollectionAssert.AreEqual(new[] { "name", "price" }, index.DescendingColumns.ToArray());
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetIndexesOfFilteredIndex()
        {
            // Act
            var index = Index("ix_product_active");

            // Assert
            Assert.AreEqual("active", index.Filter);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetIndexesDoesNotIncludeTheUniqueConstraints()
        {
            // Assert
            Assert.AreEqual(0, CreateReader().GetIndexes("country").Count());
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetIndexesOfTableWithoutIndexes()
        {
            // Assert
            Assert.AreEqual(0, CreateReader().GetIndexes("no_key").Count());
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetPrimaryKeyIsNeverClustered()
        {
            // Act
            var primaryKey = CreateReader().GetPrimaryKey("product");

            // Assert
            Assert.AreEqual("product_pkey", primaryKey.Name);
            Assert.IsFalse(primaryKey.IsClustered);
        }

        [TestMethod]
        public async Task TestAuroraDbPostgreSqlSchemaReaderGetIndexesAsyncOfTableWithMultipleIndexes()
        {
            // Act
            var indexes = (await CreateReader().GetIndexesAsync("product")).ToList();

            // Assert
            Assert.AreEqual(5, indexes.Count);
        }

        #endregion

        #region Composer

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfTableWithMultipleIndexes()
        {
            // Act
            Helper.CopyToTarget("product");

            // Assert
            Helper.AssertTargetMatchesSource("product");
            Assert.AreEqual(5, Helper.GetTargetSchema("product").Indexes.Count);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaKeepsTheFilteredIndex()
        {
            // Act
            Helper.CopyToTarget("product");

            // Assert
            Assert.AreEqual("active", Helper.GetTargetSchema("product").Indexes.Single(x => x.Name == "ix_product_active").Filter);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaKeepsTheDescendingKeyAndTheIncludedColumn()
        {
            // Act
            Helper.CopyToTarget("product");

            // Assert
            var index = Helper.GetTargetSchema("product").Indexes.Single(x => x.Name == "ix_product_multi");
            CollectionAssert.AreEqual(new[] { "name" }, index.DescendingColumns.ToArray());
            CollectionAssert.AreEqual(new[] { "code" }, index.IncludedColumns.ToArray());
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedUniqueIndexEnforcesTheUniqueness()
        {
            // Setup
            Helper.CopyToTarget("product");
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO product (id, code) VALUES (1, 'A');");

                // Act/Assert
                Assert.Throws<AuroraDbException>(() => connection.ExecuteNonQuery("INSERT INTO product (id, code) VALUES (2, 'A');"));
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedEachIndexOnItsOwn()
        {
            // Setup
            var composer = new AuroraDbPostgreSqlSchemaComposer();
            var schema = Helper.GetSourceSchema("product");
            Helper.ExecuteOnTarget(new[] { composer.ComposeCreateTable(schema) });

            // Act
            Helper.ExecuteOnTarget(schema.Indexes.Select(index => composer.ComposeCreateIndex("product", index)));

            // Assert
            Helper.AssertTargetMatchesSource("product");
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfTablesWithIndexes()
        {
            // Act
            Helper.CopyAllToTarget("product", "Person", "country");

            // Assert
            foreach (var table in new[] { "product", "Person", "country" })
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }

        #endregion
    }
}
