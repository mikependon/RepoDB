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
    [TestClass]
    public class SqlServerSchemaIndexTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region Helpers

        private static IndexInfo GetIndex(string tableName, string indexName)
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                return new SqlServerSchemaReader(connection).GetIndexes(tableName).Single(i => i.Name == indexName);
            }
        }

        #endregion

        #region Reader

        [TestMethod]
        public void TestSqlServerSchemaReaderGetIndexesOfTableWithMultipleIndexes()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetIndexes("Product").Select(i => i.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray();

                // Assert
                CollectionAssert.AreEqual(
                    new[] { "CIX_Product_Code", "IX_Product_Active_Name", "IX_Product_Category_Price", "UX_Product_Sku" },
                    actual);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetIndexesOfUniqueClusteredIndex()
        {
            // Act
            var actual = GetIndex("Product", "CIX_Product_Code");

            // Assert
            Assert.IsTrue(actual.IsUnique);
            Assert.IsTrue(actual.IsClustered);
            CollectionAssert.AreEqual(new[] { "Code" }, actual.Columns.ToArray());
            Assert.IsNull(actual.Filter);
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetIndexesOfUniqueNonClusteredIndex()
        {
            // Act
            var actual = GetIndex("Product", "UX_Product_Sku");

            // Assert
            Assert.IsTrue(actual.IsUnique);
            Assert.IsFalse(actual.IsClustered);
            CollectionAssert.AreEqual(new[] { "Sku" }, actual.Columns.ToArray());
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetIndexesOfMultiColumnIndexWithDescendingKeyAndIncludedColumn()
        {
            // Act
            var actual = GetIndex("Product", "IX_Product_Category_Price");

            // Assert
            Assert.IsFalse(actual.IsUnique);
            Assert.IsFalse(actual.IsClustered);
            CollectionAssert.AreEqual(new[] { "Category", "Price" }, actual.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "Price" }, actual.DescendingColumns.ToArray());
            CollectionAssert.AreEqual(new[] { "Name" }, actual.IncludedColumns.ToArray());
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetIndexesOfFilteredIndex()
        {
            // Act
            var actual = GetIndex("Product", "IX_Product_Active_Name");

            // Assert
            Assert.IsNotNull(actual.Filter);
            StringAssert.Contains(actual.Filter, "IsActive", StringComparison.Ordinal);
            CollectionAssert.AreEqual(new[] { "Name" }, actual.Columns.ToArray());
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetIndexesOfIndexWithoutFilterAndDescendingKeys()
        {
            // Act
            var actual = GetIndex("Person", "IX_Person_Name");

            // Assert
            Assert.IsNull(actual.Filter);
            Assert.AreEqual(0, actual.DescendingColumns.Count);
            Assert.IsFalse(actual.IsClustered);
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetPrimaryKeyOfNonClusteredPrimaryKey()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("Product");

                // Assert
                Assert.AreEqual("PK_Product", actual.Name, StringComparer.Ordinal);
                Assert.IsFalse(actual.IsClustered);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaReaderGetPrimaryKeyOfClusteredPrimaryKey()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("Person");

                // Assert
                Assert.IsTrue(actual.IsClustered);
            }
        }

        [TestMethod]
        public async Task TestSqlServerSchemaReaderGetIndexesAsyncOfTableWithMultipleIndexes()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new SqlServerSchemaReader(connection);

                // Act
                var actual = (await reader.GetIndexesAsync("Product")).ToList();

                // Assert
                Assert.AreEqual(4, actual.Count);
                var filtered = actual.Single(i => i.Name == "IX_Product_Active_Name");
                StringAssert.Contains(filtered.Filter, "IsActive", StringComparison.Ordinal);
                Assert.IsTrue(actual.Single(i => i.Name == "CIX_Product_Code").IsClustered);
                CollectionAssert.AreEqual(new[] { "Price" }, actual.Single(i => i.Name == "IX_Product_Category_Price").DescendingColumns.ToArray());
            }
        }

        #endregion

        #region Composer

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTableWithMultipleIndexes()
        {
            // Act
            Helper.CopyToTarget("Product");

            // Assert
            Helper.AssertTargetMatchesSource("Product");
        }

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

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaKeepsTheFilteredIndex()
        {
            // Act
            Helper.CopyToTarget("Product");

            // Assert
            var actual = Helper.GetTargetSchema("Product").Indexes.Single(i => i.Name == "IX_Product_Active_Name");
            Assert.AreEqual(GetIndex("Product", "IX_Product_Active_Name").Filter, actual.Filter, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaKeepsTheDescendingKey()
        {
            // Act
            Helper.CopyToTarget("Product");

            // Assert
            var actual = Helper.GetTargetSchema("Product").Indexes.Single(i => i.Name == "IX_Product_Category_Price");
            CollectionAssert.AreEqual(new[] { "Category", "Price" }, actual.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "Price" }, actual.DescendingColumns.ToArray());
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedUniqueIndexEnforcesTheUniqueness()
        {
            // Setup
            Helper.CopyToTarget("Product");

            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Product] ([Id], [Code], [Sku], [Name], [Category], [Price]) VALUES (1, N'C1', N'S1', N'First', 1, 1.00);");

                // Act/Assert (the unique nonclustered index)
                Assert.Throws<SqlException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO [dbo].[Product] ([Id], [Code], [Sku], [Name], [Category], [Price]) VALUES (2, N'C2', N'S1', N'Second', 1, 1.00);"));

                // Act/Assert (the unique clustered index)
                Assert.Throws<SqlException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO [dbo].[Product] ([Id], [Code], [Sku], [Name], [Category], [Price]) VALUES (3, N'C1', N'S3', N'Third', 1, 1.00);"));
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedEachIndexOnItsOwn()
        {
            // Setup
            var source = Helper.GetSourceSchema("Product");
            var composer = new SqlServerSchemaComposer();
            var withoutIndexes = new TableSchema(source.Table.Name, source.Table.Schema)
            {
                PrimaryKey = source.PrimaryKey,
                Columns = source.Columns
            };

            // Act
            Helper.ExecuteOnTarget(new[] { composer.ComposeCreateTable(withoutIndexes) });
            Helper.ExecuteOnTarget(source.Indexes.Select(i => composer.ComposeCreateIndex("dbo.Product", i)));

            // Assert
            Helper.AssertTargetMatchesSource("Product");
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTablesWithIndexes()
        {
            // Act
            Helper.CopyAllToTarget("Product", "Person", "Country");

            // Assert
            Helper.AssertTargetMatchesSource("Product");
            Helper.AssertTargetMatchesSource("Person");
            Helper.AssertTargetMatchesSource("Country");
        }

        #endregion
    }
}
