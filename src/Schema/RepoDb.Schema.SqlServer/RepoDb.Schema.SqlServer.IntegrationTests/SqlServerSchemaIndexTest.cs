#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.SqlServer.IntegrationTests.Setup;

namespace RepoDb.Schema.SqlServer.IntegrationTests
{
    /// <summary>
    /// Contains the sql server schema index test behavior.
    /// </summary>
    [TestClass]
    public class SqlServerSchemaIndexTest
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
            string indexName)
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                return new SqlServerSchemaReader().GetIndexes(connection,
                    tableName,
                    transaction: null).Single(i => i.Name == indexName);
            }
        }

        #endregion

        #region Reader

        /// <summary>
        /// Verifies sql server schema reader get indexes of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderGetIndexesOfTableWithMultipleIndexes()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader();
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
        /// Verifies sql server schema reader get indexes of unique clustered index.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderGetIndexesOfUniqueClusteredIndex()
        {
            // Act
            var actual = GetIndex("Product",
                "CIX_Product_Code");
            // Assert
            Assert.IsTrue(actual.IsUnique);
            Assert.IsTrue(actual.IsClustered);
            CollectionAssert.AreEqual(new[] { "Code" },
                actual.Columns.ToArray());
            Assert.IsNull(actual.Filter);
        }

        /// <summary>
        /// Verifies sql server schema reader get indexes of unique non clustered index.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderGetIndexesOfUniqueNonClusteredIndex()
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
        /// Verifies sql server schema reader get indexes of multi column index with descending key and included column.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderGetIndexesOfMultiColumnIndexWithDescendingKeyAndIncludedColumn()
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
            CollectionAssert.AreEqual(new[] { "Name" },
                actual.IncludedColumns.ToArray());
        }

        /// <summary>
        /// Verifies sql server schema reader get indexes of filtered index.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderGetIndexesOfFilteredIndex()
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
        /// Verifies sql server schema reader get indexes of index without filter and descending keys.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderGetIndexesOfIndexWithoutFilterAndDescendingKeys()
        {
            // Act
            var actual = GetIndex("Person",
                "IX_Person_Name");
            // Assert
            Assert.IsNull(actual.Filter);
            Assert.AreEqual(0,
                actual.DescendingColumns.Count);
            Assert.IsFalse(actual.IsClustered);
        }

        /// <summary>
        /// Verifies sql server schema reader get primary key of non clustered primary key.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderGetPrimaryKeyOfNonClusteredPrimaryKey()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "Product",
                    transaction: null);
                // Assert
                Assert.AreEqual("PK_Product",
                    actual.Name,
                    StringComparer.Ordinal);
                Assert.IsFalse(actual.IsClustered);
            }
        }

        /// <summary>
        /// Verifies sql server schema reader get primary key of clustered primary key.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaReaderGetPrimaryKeyOfClusteredPrimaryKey()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual.IsClustered);
            }
        }

        /// <summary>
        /// Verifies sql server schema reader get indexes async of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public async Task TestSqlServerSchemaReaderGetIndexesAsyncOfTableWithMultipleIndexes()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader();
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
                Assert.IsTrue(actual.Single(i => i.Name == "CIX_Product_Code").IsClustered);
                CollectionAssert.AreEqual(new[] { "Price" },
                    actual.Single(i => i.Name == "IX_Product_Category_Price").DescendingColumns.ToArray());
            }
        }

        #endregion

        #region Composer

        /// <summary>
        /// Verifies sql server schema composer composed schema of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTableWithMultipleIndexes()
        {
            // Act
            Helper.CopyToTarget("Product");
            // Assert
            Helper.AssertTargetMatchesSource("Product");
        }

        /// <summary>
        /// Verifies sql server schema composer composed schema keeps the non clustered primary key and the clustered index.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaKeepsTheNonClusteredPrimaryKeyAndTheClusteredIndex()
        {
            // Act
            Helper.CopyToTarget("Product");
            // Assert
            var target = Helper.GetTargetSchema("Product");
            Assert.IsFalse(target.PrimaryKey.IsClustered);
            Assert.IsTrue(target.Indexes.Single(i => i.Name == "CIX_Product_Code").IsClustered);
        }

        /// <summary>
        /// Verifies sql server schema composer composed schema keeps the filtered index.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaKeepsTheFilteredIndex()
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
        /// Verifies sql server schema composer composed schema keeps the descending key.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaKeepsTheDescendingKey()
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
        /// Verifies sql server schema composer composed unique index enforces the uniqueness.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaComposerComposedUniqueIndexEnforcesTheUniqueness()
        {
            // Setup
            Helper.CopyToTarget("Product");
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Product] ([Id], [Code], [Sku], [Name], [Category], [Price]) VALUES (1, N'C1', N'S1', N'First', 1, 1.00);");
                // Act/Assert
                Assert.Throws<SqlException>(() => connection.ExecuteNonQuery("INSERT INTO [dbo].[Product] ([Id], [Code], [Sku], [Name], [Category], [Price]) VALUES (2, N'C2', N'S1', N'Second', 1, 1.00);"));
                // Act/Assert
                Assert.Throws<SqlException>(() => connection.ExecuteNonQuery("INSERT INTO [dbo].[Product] ([Id], [Code], [Sku], [Name], [Category], [Price]) VALUES (3, N'C1', N'S3', N'Third', 1, 1.00);"));
            }
        }

        /// <summary>
        /// Verifies sql server schema composer composed each index on its own.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaComposerComposedEachIndexOnItsOwn()
        {
            // Setup
            var source = Helper.GetSourceSchema("Product");
            var composer = new SqlServerSchemaComposer();
            var withoutIndexes = new TableSchema(source.Table.Name,
                source.Table.Schema)
            {
                PrimaryKey = source.PrimaryKey,
                Columns = source.Columns
            };
            // Act
            Helper.ExecuteOnTarget(new[] { composer.ComposeCreateTable(withoutIndexes) });
            Helper.ExecuteOnTarget(source.Indexes.Select(i => composer.ComposeCreateIndex("dbo.Product",
                        i)));
            // Assert
            Helper.AssertTargetMatchesSource("Product");
        }

        /// <summary>
        /// Verifies sql server schema composer composed schema of tables with indexes.
        /// </summary>
        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTablesWithIndexes()
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
