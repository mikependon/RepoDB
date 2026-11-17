#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.Sqlite.Microsoft.IntegrationTests.Setup;

namespace RepoDb.Schema.Sqlite.Microsoft.IntegrationTests
{
    /// <summary>
    /// Contains the sqlite schema index test behavior.
    /// </summary>
    [TestClass]
    public class SqliteSchemaIndexTest
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
            using (var connection = Database.CreateSource())
            {
                return new SqliteSchemaReader().GetIndexes(connection,
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
            using (var connection = Database.CreateSource())
            {
                return new SqliteSchemaReader().GetUniqueConstraints(connection,
                    tableName,
                    transaction: null).Single(c => c.Name == name);
            }
        }

        #endregion

        #region Reader

        /// <summary>
        /// Verifies sqlite schema reader get indexes of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaReaderGetIndexesOfTableWithMultipleIndexes()
        {
            using (var connection = Database.CreateSource())
            {
                // Setup
                var reader = new SqliteSchemaReader();
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
        /// Verifies sqlite schema reader get indexes of unique index.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaReaderGetIndexesOfUniqueIndex()
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
        /// Verifies sqlite schema reader get indexes of second unique index.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaReaderGetIndexesOfSecondUniqueIndex()
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
        /// Verifies sqlite schema reader get indexes of multi column index with descending key.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaReaderGetIndexesOfMultiColumnIndexWithDescendingKey()
        {
            // Act
            var actual = GetIndex("Product",
                "IX_Product_Category_Price");
            // Assert
            Assert.IsFalse(actual.IsUnique);
            Assert.IsFalse(actual.IsClustered);
            CollectionAssert.AreEqual(new[] { "Category", "Price" },
                actual.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "Price" },
                actual.DescendingColumns.ToArray());
            Assert.AreEqual(0,
                actual.IncludedColumns.Count);
        }

        /// <summary>
        /// Verifies sqlite schema reader get indexes of filtered index.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaReaderGetIndexesOfFilteredIndex()
        {
            // Act
            var actual = GetIndex("Product",
                "IX_Product_Active_Name");
            // Assert
            Assert.IsNotNull(actual.Filter);
            StringAssert.Contains(actual.Filter,
                "IsActive",
                StringComparison.Ordinal);
            CollectionAssert.AreEqual(new[] { "Name" },
                actual.Columns.ToArray());
        }

        /// <summary>
        /// Verifies sqlite schema reader get indexes of index without filter and with descending key.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaReaderGetIndexesOfIndexWithoutFilterAndWithDescendingKey()
        {
            // Act
            var actual = GetIndex("Person",
                "IX_Person_Name");
            // Assert
            Assert.IsNull(actual.Filter);
            CollectionAssert.AreEqual(new[] { "Name" },
                actual.DescendingColumns.ToArray());
            Assert.IsFalse(actual.IsClustered);
        }

        /// <summary>
        /// Verifies sqlite schema reader get primary key of product.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaReaderGetPrimaryKeyOfProduct()
        {
            using (var connection = Database.CreateSource())
            {
                // Setup
                var reader = new SqliteSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "Product",
                    transaction: null);
                // Assert
                Assert.AreEqual("PK_Product",
                    actual.Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Id" },
                    actual.Columns.ToArray());
            }
        }

        /// <summary>
        /// Verifies sqlite schema reader get primary key of clustered primary key.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaReaderGetPrimaryKeyOfClusteredPrimaryKey()
        {
            using (var connection = Database.CreateSource())
            {
                // Setup
                var reader = new SqliteSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual.IsClustered);
            }
        }

        /// <summary>
        /// Verifies sqlite schema reader get indexes async of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public async Task TestSqliteSchemaReaderGetIndexesAsyncOfTableWithMultipleIndexes()
        {
            using (var connection = Database.CreateSource())
            {
                // Setup
                var reader = new SqliteSchemaReader();
                // Act
                var actual = (await reader.GetIndexesAsync(connection,
                    "Product",
                    transaction: null)).ToList();
                // Assert
                Assert.AreEqual(4,
                    actual.Count);
                var filtered = actual.Single(i => i.Name == "IX_Product_Active_Name");
                StringAssert.Contains(filtered.Filter,
                    "IsActive",
                    StringComparison.Ordinal);
                CollectionAssert.AreEqual(new[] { "Price" },
                    actual.Single(i => i.Name == "IX_Product_Category_Price").DescendingColumns.ToArray());
            }
        }

        #endregion

        #region Composer

        /// <summary>
        /// Verifies sqlite schema composer composed schema of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaComposerComposedSchemaOfTableWithMultipleIndexes()
        {
            // Act
            Helper.CopyToTarget("Product");
            // Assert
            Helper.AssertTargetMatchesSource("Product");
        }

        /// <summary>
        /// Verifies sqlite schema composer composed schema keeps the primary key and the unique indexes.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaComposerComposedSchemaKeepsThePrimaryKeyAndTheUniqueIndexes()
        {
            // Act
            Helper.CopyToTarget("Product");
            // Assert
            var target = Helper.GetTargetSchema("Product");
            Assert.AreEqual("PK_Product",
                target.PrimaryKey.Name,
                StringComparer.Ordinal);
            CollectionAssert.AreEquivalent(new[] { "CIX_Product_Code", "UX_Product_Sku" },
                target.Indexes.Where(i => i.IsUnique).Select(i => i.Name).ToArray());
        }

        /// <summary>
        /// Verifies sqlite schema composer composed schema keeps the filtered index.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaComposerComposedSchemaKeepsTheFilteredIndex()
        {
            // Act
            Helper.CopyToTarget("Product");
            // Assert
            var actual = Helper.GetTargetSchema("Product").Indexes.Single(i => i.Name == "IX_Product_Active_Name");
            Assert.AreEqual(GetIndex("Product",
                    "IX_Product_Active_Name").Filter,
                actual.Filter,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Verifies sqlite schema composer composed schema keeps the descending key.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaComposerComposedSchemaKeepsTheDescendingKey()
        {
            // Act
            Helper.CopyToTarget("Product");
            // Assert
            var actual = Helper.GetTargetSchema("Product").Indexes.Single(i => i.Name == "IX_Product_Category_Price");
            CollectionAssert.AreEqual(new[] { "Category", "Price" },
                actual.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "Price" },
                actual.DescendingColumns.ToArray());
        }

        /// <summary>
        /// Verifies sqlite schema composer composed unique index enforces the uniqueness.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaComposerComposedUniqueIndexEnforcesTheUniqueness()
        {
            // Setup
            Helper.CopyToTarget("Product");
            using (var connection = Database.CreateTarget())
            {
                connection.ExecuteNonQuery("INSERT INTO [Product] ([Id], [Code], [Sku], [Name], [Category], [Price]) VALUES (1, 'C1', 'S1', 'First', 1, 1.00);");
                // Act/Assert
                Assert.Throws<SqliteException>(() => connection.ExecuteNonQuery("INSERT INTO [Product] ([Id], [Code], [Sku], [Name], [Category], [Price]) VALUES (2, 'C2', 'S1', 'Second', 1, 1.00);"));
                // Act/Assert
                Assert.Throws<SqliteException>(() => connection.ExecuteNonQuery("INSERT INTO [Product] ([Id], [Code], [Sku], [Name], [Category], [Price]) VALUES (3, 'C1', 'S3', 'Third', 1, 1.00);"));
            }
        }

        /// <summary>
        /// Verifies sqlite schema composer composed each index on its own.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaComposerComposedEachIndexOnItsOwn()
        {
            // Setup
            var source = Helper.GetSourceSchema("Product");
            var composer = new SqliteSchemaComposer();
            var withoutIndexes = new TableSchema(source.Table.Name,
                source.Table.Schema)
            {
                PrimaryKey = source.PrimaryKey,
                Columns = source.Columns,
                UniqueConstraints = source.UniqueConstraints
            };
            // Act
            Helper.ExecuteOnTarget(new[] { composer.ComposeCreateTable(withoutIndexes) });
            Helper.ExecuteOnTarget(source.Indexes.Select(i => composer.ComposeCreateIndex("Product",
                        i)));
            // Assert
            Helper.AssertTargetMatchesSource("Product");
        }

        /// <summary>
        /// Verifies sqlite schema composer composed schema of tables with indexes.
        /// </summary>
        [TestMethod]
        public void TestSqliteSchemaComposerComposedSchemaOfTablesWithIndexes()
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
