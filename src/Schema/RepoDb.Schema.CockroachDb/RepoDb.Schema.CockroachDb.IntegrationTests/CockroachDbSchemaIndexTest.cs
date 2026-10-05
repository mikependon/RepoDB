#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;
using RepoDb.Schema.Models;
using RepoDb.Schema.CockroachDb.IntegrationTests.Setup;

namespace RepoDb.Schema.CockroachDb.IntegrationTests
{
    [TestClass]
    public class CockroachDbSchemaIndexTest
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

        private static CockroachDbSchemaReader CreateReader() =>
            new CockroachDbSchemaReader(new CockroachDbConnection(Database.ConnectionStringForSource));

        private static IndexInfo Index(string name) =>
            CreateReader().GetIndexes("product").Single(x => x.Name == name);

        #endregion

        #region Reader

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetIndexesOfTableWithMultipleIndexes()
        {
            // Act
            var indexes = CreateReader().GetIndexes("product").ToList();

            // Assert
            CollectionAssert.AreEquivalent(new[] { "ix_product_name", "ix_product_multi", "ix_product_active", "ix_product_all_desc" }, indexes.Select(x => x.Name).ToArray());
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetIndexesOfNonUniqueIndex()
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
        public void TestCockroachDbSchemaReaderGetUniqueConstraintsOfUniqueIndex()
        {
            // Act
            var constraint = CreateReader().GetUniqueConstraints("product").Single(x => x.Name == "ux_product_code");

            // Assert
            CollectionAssert.AreEqual(new[] { "code" }, constraint.Columns.ToArray());
            Assert.IsFalse(CreateReader().GetIndexes("product").Any(x => x.Name == "ux_product_code"));
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetIndexesOfMultiColumnIndexWithDescendingKeyAndIncludedColumn()
        {
            // Act
            var index = Index("ix_product_multi");

            // Assert
            CollectionAssert.AreEqual(new[] { "name", "price" }, index.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "name" }, index.DescendingColumns.ToArray());
            CollectionAssert.AreEqual(new[] { "code" }, index.IncludedColumns.ToArray());
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetIndexesOfIndexWithAllDescendingKeys()
        {
            // Act
            var index = Index("ix_product_all_desc");

            // Assert
            CollectionAssert.AreEqual(new[] { "name", "price" }, index.DescendingColumns.ToArray());
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetIndexesOfFilteredIndex()
        {
            // Act
            var index = Index("ix_product_active");

            // Assert
            Assert.AreEqual("active", index.Filter);
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetIndexesDoesNotIncludeTheUniqueConstraints()
        {
            // Assert
            Assert.AreEqual(0, CreateReader().GetIndexes("country").Count());
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetIndexesOfTableWithoutIndexes()
        {
            // Assert
            Assert.AreEqual(0, CreateReader().GetIndexes("no_key").Count());
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetPrimaryKeyIsNeverClustered()
        {
            // Act
            var primaryKey = CreateReader().GetPrimaryKey("product");

            // Assert
            Assert.AreEqual("product_pkey", primaryKey.Name);
            Assert.IsFalse(primaryKey.IsClustered);
        }

        [TestMethod]
        public async Task TestCockroachDbSchemaReaderGetIndexesAsyncOfTableWithMultipleIndexes()
        {
            // Act
            var indexes = (await CreateReader().GetIndexesAsync("product")).ToList();

            // Assert
            Assert.AreEqual(4, indexes.Count);
        }

        #endregion

        #region Composer

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaOfTableWithMultipleIndexes()
        {
            // Act
            Helper.CopyToTarget("product");

            // Assert
            Helper.AssertTargetMatchesSource("product");
            Assert.AreEqual(4, Helper.GetTargetSchema("product").Indexes.Count);
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaKeepsTheFilteredIndex()
        {
            // Act
            Helper.CopyToTarget("product");

            // Assert
            Assert.AreEqual("active", Helper.GetTargetSchema("product").Indexes.Single(x => x.Name == "ix_product_active").Filter);
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaKeepsTheDescendingKeyAndTheIncludedColumn()
        {
            // Act
            Helper.CopyToTarget("product");

            // Assert
            var index = Helper.GetTargetSchema("product").Indexes.Single(x => x.Name == "ix_product_multi");
            CollectionAssert.AreEqual(new[] { "name" }, index.DescendingColumns.ToArray());
            CollectionAssert.AreEqual(new[] { "code" }, index.IncludedColumns.ToArray());
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedUniqueIndexEnforcesTheUniqueness()
        {
            // Setup
            Helper.CopyToTarget("product");
            using (var connection = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO product (id, code) VALUES (1, 'A');");

                // Act/Assert
                Assert.Throws<CockroachDbException>(() => connection.ExecuteNonQuery("INSERT INTO product (id, code) VALUES (2, 'A');"));
            }
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedEachIndexOnItsOwn()
        {
            // Setup
            var composer = new CockroachDbSchemaComposer();
            var schema = Helper.GetSourceSchema("product");
            Helper.ExecuteOnTarget(new[] { composer.ComposeCreateTable(schema) });

            // Act
            Helper.ExecuteOnTarget(schema.Indexes.Select(index => composer.ComposeCreateIndex("product", index)));

            // Assert
            Helper.AssertTargetMatchesSource("product");
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaOfTablesWithIndexes()
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
