#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DuckDB.NET.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.DuckDb.IntegrationTests.Setup;

namespace RepoDb.Schema.DuckDb.IntegrationTests
{
    /// <summary>
    /// Contains the duck db schema index test behavior.
    /// </summary>
    [TestClass]
    public class DuckDbSchemaIndexTest
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
        /// Performs get index.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="indexName">The index name.</param>
        private static IndexInfo GetIndex(string tableName,
            string indexName) => GetIndexes(tableName).Single(i => i.Name == indexName);

        /// <summary>
        /// Performs get indexes.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        private static List<IndexInfo> GetIndexes(string tableName)
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                return new DuckDbSchemaReader().GetIndexes(connection,
                    tableName,
                    transaction: null).ToList();
            }
        }

        /// <summary>
        /// Performs get unique constraint.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="name">The name.</param>
        private static UniqueConstraintInfo GetUniqueConstraint(string tableName,
            string name)
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                return new DuckDbSchemaReader().GetUniqueConstraints(connection,
                    tableName,
                    transaction: null).Single(c => c.Name == name);
            }
        }

        #endregion

        #region Reader

        /// <summary>
        /// Verifies duck db schema reader get indexes of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetIndexesOfTableWithMultipleIndexes()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.GetIndexes(connection,
                    "Product",
                    transaction: null).Select(i => i.Name).OrderBy(n => n,
                    StringComparer.Ordinal).ToArray();
                // Assert
                CollectionAssert.AreEqual(new[] { "CIX_Product_Code", "IX_Product_Active_Name", "IX_Product_Category_Price", "UX_Product_Sku" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get indexes of unique index.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetIndexesOfUniqueIndex()
        {
            // Act
            var actual = GetIndex("Product",
                "CIX_Product_Code");
            // Assert
            Assert.IsTrue(actual.IsUnique);
            Assert.IsFalse(actual.IsClustered);
            CollectionAssert.AreEqual(new[] { "Code" },
                actual.Columns.ToArray());
            Assert.IsNull(actual.Filter);
        }

        /// <summary>
        /// Verifies duck db schema reader get indexes of second unique index.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetIndexesOfSecondUniqueIndex()
        {
            // Act
            var actual = GetIndex("Product",
                "UX_Product_Sku");
            // Assert
            Assert.IsTrue(actual.IsUnique);
            Assert.IsFalse(actual.IsClustered);
            CollectionAssert.AreEqual(new[] { "Sku" },
                actual.Columns.ToArray());
        }

        /// <summary>
        /// Verifies duck db schema reader get indexes of single column index.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetIndexesOfSingleColumnIndex()
        {
            // Act
            var actual = GetIndex("Product",
                "IX_Product_Active_Name");
            // Assert
            Assert.IsNull(actual.Filter);
            CollectionAssert.AreEqual(new[] { "Name" },
                actual.Columns.ToArray());
        }

        /// <summary>
        /// Verifies duck db schema reader get primary key of product.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetPrimaryKeyOfProduct()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "Product",
                    transaction: null);
                // Assert
                Assert.AreEqual("Product_id_pkey",
                    actual.Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Id" },
                    actual.Columns.ToArray());
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get primary key of clustered primary key.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaReaderGetPrimaryKeyOfClusteredPrimaryKey()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual.IsClustered);
            }
        }

        /// <summary>
        /// Verifies duck db schema reader get indexes async of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public async Task TestDuckDbSchemaReaderGetIndexesAsyncOfTableWithMultipleIndexes()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new DuckDbSchemaReader();
                // Act
                var actual = (await reader.GetIndexesAsync(connection,
                    "Product",
                    transaction: null)).ToList();
                // Assert
                Assert.AreEqual(4,
                    actual.Count);
                Assert.IsNull(actual.Single(i => i.Name == "IX_Product_Active_Name").Filter);
            }
        }

        #endregion

        #region Composer

        /// <summary>
        /// Verifies duck db schema composer composed schema of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaComposerComposedSchemaOfTableWithMultipleIndexes()
        {
            // Act
            Helper.CopyToTarget("Product");
            // Assert
            Helper.AssertTargetMatchesSource("Product");
        }

        /// <summary>
        /// Verifies duck db schema composer composed schema keeps the primary key and the unique indexes.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaComposerComposedSchemaKeepsThePrimaryKeyAndTheUniqueIndexes()
        {
            // Act
            Helper.CopyToTarget("Product");
            // Assert
            var target = Helper.GetTargetSchema("Product");
            Assert.AreEqual("Product_id_pkey",
                target.PrimaryKey.Name,
                StringComparer.Ordinal);
            CollectionAssert.AreEquivalent(new[] { "CIX_Product_Code", "UX_Product_Sku" },
                target.Indexes.Where(i => i.IsUnique).Select(i => i.Name).ToArray());
        }

        /// <summary>
        /// Verifies duck db schema composer composed schema keeps the single column index.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaComposerComposedSchemaKeepsTheSingleColumnIndex()
        {
            // Act
            Helper.CopyToTarget("Product");
            // Assert
            var actual = Helper.GetTargetSchema("Product").Indexes.Single(i => i.Name == "IX_Product_Active_Name");
            CollectionAssert.AreEqual(new[] { "Name" },
                actual.Columns.ToArray());
        }

        /// <summary>
        /// Verifies duck db schema composer composed unique index enforces the uniqueness.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaComposerComposedUniqueIndexEnforcesTheUniqueness()
        {
            // Setup
            Helper.CopyToTarget("Product");
            using (var connection = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO \"Product\" (\"Id\", \"Code\", \"Sku\", \"Name\", \"Category\", \"Price\") VALUES (1, 'C1', 'S1', 'First', 1, 1.00)");
                // Act/Assert
                Assert.Throws<DuckDBException>(() => connection.ExecuteNonQuery("INSERT INTO \"Product\" (\"Id\", \"Code\", \"Sku\", \"Name\", \"Category\", \"Price\") VALUES (2, 'C2', 'S1', 'Second', 1, 1.00)"));
                // Act/Assert
                Assert.Throws<DuckDBException>(() => connection.ExecuteNonQuery("INSERT INTO \"Product\" (\"Id\", \"Code\", \"Sku\", \"Name\", \"Category\", \"Price\") VALUES (3, 'C1', 'S3', 'Third', 1, 1.00)"));
            }
        }

        /// <summary>
        /// Verifies duck db schema composer composed each index on its own.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaComposerComposedEachIndexOnItsOwn()
        {
            // Setup
            var source = Helper.GetSourceSchema("Product");
            var composer = new DuckDbSchemaComposer();
            var withoutIndexes = new TableSchema(source.Table.Name,
                source.Table.Schema)
            {
                PrimaryKey = source.PrimaryKey,
                Columns = source.Columns
            };
            // Act
            Helper.ExecuteOnTarget(new[] { composer.ComposeCreateTable(withoutIndexes) });
            Helper.ExecuteOnTarget(source.Indexes.Select(i => composer.ComposeCreateIndex("Product",
                        i)));
            // Assert
            Helper.AssertTargetMatchesSource("Product");
        }

        /// <summary>
        /// Verifies duck db schema composer composed schema of tables with indexes.
        /// </summary>
        [TestMethod]
        public void TestDuckDbSchemaComposerComposedSchemaOfTablesWithIndexes()
        {
            // Act
            Helper.CopyAllToTarget("Product",
                "Person",
                "Country");
            // Assert
            Helper.AssertTargetMatchesSource("Product");
            Helper.AssertTargetMatchesSource("Person");
            Helper.AssertTargetMatchesSource("Country");
        }
        #endregion
    }
}
