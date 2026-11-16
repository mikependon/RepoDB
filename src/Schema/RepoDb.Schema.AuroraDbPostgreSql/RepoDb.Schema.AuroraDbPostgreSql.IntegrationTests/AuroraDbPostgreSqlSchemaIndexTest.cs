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
    /// <summary>
    /// Contains the aurora db postgre sql schema index test behavior.
    /// </summary>
    [TestClass]
    public class AuroraDbPostgreSqlSchemaIndexTest
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
        /// Performs index.
        /// </summary>
        /// <param name="name">The name.</param>
        private static IndexInfo Index(string name)
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource);
            return CreateReader().GetIndexes(operationConnection,
                "product").Single(x => x.Name == name);
        }

        #endregion

        #region Reader

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get indexes of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetIndexesOfTableWithMultipleIndexes()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var indexes = CreateReader().GetIndexes(operationConnection,
                "product").ToList();
            // Assert
            CollectionAssert.AreEquivalent(new[] { "ix_product_name", "ux_product_code", "ix_product_multi", "ix_product_active", "ix_product_all_desc" },
                indexes.Select(x => x.Name).ToArray());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get indexes of non unique index.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetIndexesOfNonUniqueIndex()
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
        /// Verifies aurora db postgre sql schema reader get indexes of unique index.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetIndexesOfUniqueIndex()
        {
            // Act
            var index = Index("ux_product_code");
            // Assert
            Assert.IsTrue(index.IsUnique);
            CollectionAssert.AreEqual(new[] { "code" },
                index.Columns.ToArray());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get indexes of multi column index with descending key and included column.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetIndexesOfMultiColumnIndexWithDescendingKeyAndIncludedColumn()
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
        /// Verifies aurora db postgre sql schema reader get indexes of index with all descending keys.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetIndexesOfIndexWithAllDescendingKeys()
        {
            // Act
            var index = Index("ix_product_all_desc");
            // Assert
            CollectionAssert.AreEqual(new[] { "name", "price" },
                index.DescendingColumns.ToArray());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get indexes of filtered index.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetIndexesOfFilteredIndex()
        {
            // Act
            var index = Index("ix_product_active");
            // Assert
            Assert.AreEqual("active",
                index.Filter);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get indexes does not include the unique constraints.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetIndexesDoesNotIncludeTheUniqueConstraints()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Assert
            Assert.AreEqual(0,
                CreateReader().GetIndexes(operationConnection,
                    "country").Count());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get indexes of table without indexes.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetIndexesOfTableWithoutIndexes()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Assert
            Assert.AreEqual(0,
                CreateReader().GetIndexes(operationConnection,
                    "no_key").Count());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get primary key is never clustered.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetPrimaryKeyIsNeverClustered()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var primaryKey = CreateReader().GetPrimaryKey(operationConnection,
                "product");
            // Assert
            Assert.AreEqual("product_pkey",
                primaryKey.Name);
            Assert.IsFalse(primaryKey.IsClustered);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get indexes async of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public async Task TestAuroraDbPostgreSqlSchemaReaderGetIndexesAsyncOfTableWithMultipleIndexes()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var indexes = (await CreateReader().GetIndexesAsync(operationConnection,
                "product")).ToList();
            // Assert
            Assert.AreEqual(5,
                indexes.Count);
        }

        #endregion

        #region Composer

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfTableWithMultipleIndexes()
        {
            // Act
            Helper.CopyToTarget("product");
            // Assert
            Helper.AssertTargetMatchesSource("product");
            Assert.AreEqual(5,
                Helper.GetTargetSchema("product").Indexes.Count);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema keeps the filtered index.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaKeepsTheFilteredIndex()
        {
            // Act
            Helper.CopyToTarget("product");
            // Assert
            Assert.AreEqual("active",
                Helper.GetTargetSchema("product").Indexes.Single(x => x.Name == "ix_product_active").Filter);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema keeps the descending key and the included column.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaKeepsTheDescendingKeyAndTheIncludedColumn()
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
        /// Verifies aurora db postgre sql schema composer composed unique index enforces the uniqueness.
        /// </summary>
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

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed each index on its own.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedEachIndexOnItsOwn()
        {
            // Setup
            var composer = new AuroraDbPostgreSqlSchemaComposer();
            var schema = Helper.GetSourceSchema("product");
            Helper.ExecuteOnTarget(new[] { composer.ComposeCreateTable(schema) });
            // Act
            Helper.ExecuteOnTarget(schema.Indexes.Select(index => composer.ComposeCreateIndex("product",
                        index)));
            // Assert
            Helper.AssertTargetMatchesSource("product");
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of tables with indexes.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfTablesWithIndexes()
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
