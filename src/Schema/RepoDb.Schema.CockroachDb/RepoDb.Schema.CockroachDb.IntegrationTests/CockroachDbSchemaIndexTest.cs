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
    /// <summary>
    /// Contains the cockroach db schema index test behavior.
    /// </summary>
    [TestClass]
    public class CockroachDbSchemaIndexTest
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
        private static CockroachDbSchemaReader CreateReader() => new CockroachDbSchemaReader();

        /// <summary>
        /// Performs index.
        /// </summary>
        /// <param name="name">The name.</param>
        private static IndexInfo Index(string name)
        {
            using var operationConnection = new CockroachDbConnection(Database.ConnectionStringForSource);
            return CreateReader().GetIndexes(operationConnection,
                "product").Single(x => x.Name == name);
        }

        #endregion

        #region Reader

        /// <summary>
        /// Verifies cockroach db schema reader get indexes of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaReaderGetIndexesOfTableWithMultipleIndexes()
        {
            using var operationConnection = new CockroachDbConnection(Database.ConnectionStringForSource); // Act
            var indexes = CreateReader().GetIndexes(operationConnection,
                "product").ToList();
            // Assert
            CollectionAssert.AreEquivalent(new[] { "ix_product_name", "ix_product_multi", "ix_product_active", "ix_product_all_desc" },
                indexes.Select(x => x.Name).ToArray());
        }

        /// <summary>
        /// Verifies cockroach db schema reader get indexes of non unique index.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaReaderGetIndexesOfNonUniqueIndex()
        {
            // Act
            var index = Index("ix_product_name");
            // Assert
            Assert.IsFalse(index.IsUnique);
            Assert.IsFalse(index.IsClustered);
            Assert.IsNull(index.Filter);
            CollectionAssert.AreEqual(new[] { "name" },
                index.Columns.ToArray());
            Assert.AreEqual(0,
                index.DescendingColumns.Count);
            Assert.AreEqual(0,
                index.IncludedColumns.Count);
        }

        /// <summary>
        /// Verifies cockroach db schema reader get unique constraints of unique index.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaReaderGetUniqueConstraintsOfUniqueIndex()
        {
            using var operationConnection = new CockroachDbConnection(Database.ConnectionStringForSource); // Act
            var constraint = CreateReader().GetUniqueConstraints(operationConnection,
                "product").Single(x => x.Name == "ux_product_code");
            // Assert
            CollectionAssert.AreEqual(new[] { "code" },
                constraint.Columns.ToArray());
            Assert.IsFalse(CreateReader().GetIndexes(operationConnection,
                    "product").Any(x => x.Name == "ux_product_code"));
        }

        /// <summary>
        /// Verifies cockroach db schema reader get indexes of multi column index with descending key and included column.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaReaderGetIndexesOfMultiColumnIndexWithDescendingKeyAndIncludedColumn()
        {
            // Act
            var index = Index("ix_product_multi");
            // Assert
            CollectionAssert.AreEqual(new[] { "name", "price" },
                index.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "name" },
                index.DescendingColumns.ToArray());
            CollectionAssert.AreEqual(new[] { "code" },
                index.IncludedColumns.ToArray());
        }

        /// <summary>
        /// Verifies cockroach db schema reader get indexes of index with all descending keys.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaReaderGetIndexesOfIndexWithAllDescendingKeys()
        {
            // Act
            var index = Index("ix_product_all_desc");
            // Assert
            CollectionAssert.AreEqual(new[] { "name", "price" },
                index.DescendingColumns.ToArray());
        }

        /// <summary>
        /// Verifies cockroach db schema reader get indexes of filtered index.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaReaderGetIndexesOfFilteredIndex()
        {
            // Act
            var index = Index("ix_product_active");
            // Assert
            Assert.AreEqual("active",
                index.Filter);
        }

        /// <summary>
        /// Verifies cockroach db schema reader get indexes does not include the unique constraints.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaReaderGetIndexesDoesNotIncludeTheUniqueConstraints()
        {
            using var operationConnection = new CockroachDbConnection(Database.ConnectionStringForSource); // Assert
            Assert.AreEqual(0,
                CreateReader().GetIndexes(operationConnection,
                    "country").Count());
        }

        /// <summary>
        /// Verifies cockroach db schema reader get indexes of table without indexes.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaReaderGetIndexesOfTableWithoutIndexes()
        {
            using var operationConnection = new CockroachDbConnection(Database.ConnectionStringForSource); // Assert
            Assert.AreEqual(0,
                CreateReader().GetIndexes(operationConnection,
                    "no_key").Count());
        }

        /// <summary>
        /// Verifies cockroach db schema reader get primary key is never clustered.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaReaderGetPrimaryKeyIsNeverClustered()
        {
            using var operationConnection = new CockroachDbConnection(Database.ConnectionStringForSource); // Act
            var primaryKey = CreateReader().GetPrimaryKey(operationConnection,
                "product");
            // Assert
            Assert.AreEqual("product_pkey",
                primaryKey.Name);
            Assert.IsFalse(primaryKey.IsClustered);
        }

        /// <summary>
        /// Verifies cockroach db schema reader get indexes async of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public async Task TestCockroachDbSchemaReaderGetIndexesAsyncOfTableWithMultipleIndexes()
        {
            using var operationConnection = new CockroachDbConnection(Database.ConnectionStringForSource); // Act
            var indexes = (await CreateReader().GetIndexesAsync(operationConnection,
                "product")).ToList();
            // Assert
            Assert.AreEqual(4,
                indexes.Count);
        }

        #endregion

        #region Composer

        /// <summary>
        /// Verifies cockroach db schema composer composed schema of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaOfTableWithMultipleIndexes()
        {
            // Act
            Helper.CopyToTarget("product");
            // Assert
            Helper.AssertTargetMatchesSource("product");
            Assert.AreEqual(4,
                Helper.GetTargetSchema("product").Indexes.Count);
        }

        /// <summary>
        /// Verifies cockroach db schema composer composed schema keeps the filtered index.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaKeepsTheFilteredIndex()
        {
            // Act
            Helper.CopyToTarget("product");
            // Assert
            Assert.AreEqual("active",
                Helper.GetTargetSchema("product").Indexes.Single(x => x.Name == "ix_product_active").Filter);
        }

        /// <summary>
        /// Verifies cockroach db schema composer composed schema keeps the descending key and the included column.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaKeepsTheDescendingKeyAndTheIncludedColumn()
        {
            // Act
            Helper.CopyToTarget("product");
            // Assert
            var index = Helper.GetTargetSchema("product").Indexes.Single(x => x.Name == "ix_product_multi");
            CollectionAssert.AreEqual(new[] { "name" },
                index.DescendingColumns.ToArray());
            CollectionAssert.AreEqual(new[] { "code" },
                index.IncludedColumns.ToArray());
        }

        /// <summary>
        /// Verifies cockroach db schema composer composed unique index enforces the uniqueness.
        /// </summary>
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

        /// <summary>
        /// Verifies cockroach db schema composer composed each index on its own.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedEachIndexOnItsOwn()
        {
            // Setup
            var composer = new CockroachDbSchemaComposer();
            var schema = Helper.GetSourceSchema("product");
            Helper.ExecuteOnTarget(new[] { composer.ComposeCreateTable(schema) });
            // Act
            Helper.ExecuteOnTarget(schema.Indexes.Select(index => composer.ComposeCreateIndex("product",
                        index)));
            // Assert
            Helper.AssertTargetMatchesSource("product");
        }

        /// <summary>
        /// Verifies cockroach db schema composer composed schema of tables with indexes.
        /// </summary>
        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaOfTablesWithIndexes()
        {
            // Act
            Helper.CopyAllToTarget("product",
                "Person",
                "country");
            // Assert
            foreach (var table in new[]
            {
                "product",
                "Person",
                "country"
            }

            )
            {
                Helper.AssertTargetMatchesSource(table);
            }
        }
        #endregion
    }
}
