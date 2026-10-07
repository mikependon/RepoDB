#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ahtola.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.Ahtola.IntegrationTests.Setup;

namespace RepoDb.Schema.Ahtola.IntegrationTests
{
    [TestClass]
    public class AhtolaSchemaIndexTest
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

        private static IndexInfo GetIndex(string tableName, string indexName) =>
            GetIndexes(tableName).Single(i => i.Name == indexName);

        private static List<IndexInfo> GetIndexes(string tableName)
        {
            using (var connection = Database.CreateSource())
            {
                return new AhtolaSchemaReader(connection).GetIndexes(tableName).ToList();
            }
        }

        private static UniqueConstraintInfo GetUniqueConstraint(string tableName, string name)
        {
            using (var connection = Database.CreateSource())
            {
                return new AhtolaSchemaReader(connection).GetUniqueConstraints(tableName).Single(c => c.Name == name);
            }
        }

        #endregion

        #region Reader

        [TestMethod]
        public void TestAhtolaSchemaReaderGetIndexesOfTableWithMultipleIndexes()
        {
            using (var connection = Database.CreateSource())
            {
                // Setup
                var reader = new AhtolaSchemaReader(connection);

                // Act
                var actual = reader.GetIndexes("Product").Select(i => i.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray();

                // Assert
                CollectionAssert.AreEqual(
                    new[] { "CIX_Product_Code", "IX_Product_Active_Name", "IX_Product_Category_Price", "UX_Product_Sku" },
                    actual);
            }
        }

        [TestMethod]
        public void TestAhtolaSchemaReaderGetIndexesOfUniqueIndex()
        {
            // Act
            var actual = GetIndex("Product", "CIX_Product_Code");

            // Assert
            Assert.IsTrue(actual.IsUnique);
            Assert.IsFalse(actual.IsClustered);
            CollectionAssert.AreEqual(new[] { "Code" }, actual.Columns.ToArray());
            Assert.IsNull(actual.Filter);
        }

        [TestMethod]
        public void TestAhtolaSchemaReaderGetIndexesOfSecondUniqueIndex()
        {
            // Act
            var actual = GetIndex("Product", "UX_Product_Sku");

            // Assert
            Assert.IsTrue(actual.IsUnique);
            Assert.IsFalse(actual.IsClustered);
            CollectionAssert.AreEqual(new[] { "Sku" }, actual.Columns.ToArray());
        }

        [TestMethod]
        public void TestAhtolaSchemaReaderGetIndexesOfMultiColumnIndexWithDescendingKey()
        {
            // Act
            var actual = GetIndex("Product", "IX_Product_Category_Price");

            // Assert
            Assert.IsFalse(actual.IsUnique);
            Assert.IsFalse(actual.IsClustered);
            CollectionAssert.AreEqual(new[] { "Category", "Price" }, actual.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "Price" }, actual.DescendingColumns.ToArray());
            Assert.AreEqual(0, actual.IncludedColumns.Count);
        }

        [TestMethod]
        public void TestAhtolaSchemaReaderGetIndexesOfFilteredIndex()
        {
            // Act
            var actual = GetIndex("Product", "IX_Product_Active_Name");

            // Assert
            Assert.IsNotNull(actual.Filter);
            StringAssert.Contains(actual.Filter, "IsActive", StringComparison.Ordinal);
            CollectionAssert.AreEqual(new[] { "Name" }, actual.Columns.ToArray());
        }

        [TestMethod]
        public void TestAhtolaSchemaReaderGetIndexesOfIndexWithoutFilterAndWithDescendingKey()
        {
            // Act
            var actual = GetIndex("Person", "IX_Person_Name");

            // Assert
            Assert.IsNull(actual.Filter);
            CollectionAssert.AreEqual(new[] { "Name" }, actual.DescendingColumns.ToArray());
            Assert.IsFalse(actual.IsClustered);
        }

        [TestMethod]
        public void TestAhtolaSchemaReaderGetPrimaryKeyOfProduct()
        {
            using (var connection = Database.CreateSource())
            {
                // Setup
                var reader = new AhtolaSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("Product");

                // Assert
                Assert.AreEqual("PK_Product", actual.Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Id" }, actual.Columns.ToArray());
            }
        }

        [TestMethod]
        public void TestAhtolaSchemaReaderGetPrimaryKeyOfClusteredPrimaryKey()
        {
            using (var connection = Database.CreateSource())
            {
                // Setup
                var reader = new AhtolaSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("Person");

                // Assert
                Assert.IsTrue(actual.IsClustered);
            }
        }

        [TestMethod]
        public async Task TestAhtolaSchemaReaderGetIndexesAsyncOfTableWithMultipleIndexes()
        {
            using (var connection = Database.CreateSource())
            {
                // Setup
                var reader = new AhtolaSchemaReader(connection);

                // Act
                var actual = (await reader.GetIndexesAsync("Product")).ToList();

                // Assert
                Assert.AreEqual(4, actual.Count);
                var filtered = actual.Single(i => i.Name == "IX_Product_Active_Name");
                StringAssert.Contains(filtered.Filter, "IsActive", StringComparison.Ordinal);
                CollectionAssert.AreEqual(new[] { "Price" }, actual.Single(i => i.Name == "IX_Product_Category_Price").DescendingColumns.ToArray());
            }
        }

        #endregion

        #region Composer

        [TestMethod]
        public void TestAhtolaSchemaComposerComposedSchemaOfTableWithMultipleIndexes()
        {
            // Act
            Helper.CopyToTarget("Product");

            // Assert
            Helper.AssertTargetMatchesSource("Product");
        }

        [TestMethod]
        public void TestAhtolaSchemaComposerComposedSchemaKeepsThePrimaryKeyAndTheUniqueIndexes()
        {
            // Act
            Helper.CopyToTarget("Product");

            // Assert
            var target = Helper.GetTargetSchema("Product");
            Assert.AreEqual("PK_Product", target.PrimaryKey.Name, StringComparer.Ordinal);
            CollectionAssert.AreEquivalent(new[] { "CIX_Product_Code", "UX_Product_Sku" }, target.Indexes.Where(i => i.IsUnique).Select(i => i.Name).ToArray());
        }

        [TestMethod]
        public void TestAhtolaSchemaComposerComposedSchemaKeepsTheFilteredIndex()
        {
            // Act
            Helper.CopyToTarget("Product");

            // Assert
            var actual = Helper.GetTargetSchema("Product").Indexes.Single(i => i.Name == "IX_Product_Active_Name");
            Assert.AreEqual(GetIndex("Product", "IX_Product_Active_Name").Filter, actual.Filter, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAhtolaSchemaComposerComposedSchemaKeepsTheDescendingKey()
        {
            // Act
            Helper.CopyToTarget("Product");

            // Assert
            var actual = Helper.GetTargetSchema("Product").Indexes.Single(i => i.Name == "IX_Product_Category_Price");
            CollectionAssert.AreEqual(new[] { "Category", "Price" }, actual.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "Price" }, actual.DescendingColumns.ToArray());
        }

        [TestMethod]
        public void TestAhtolaSchemaComposerComposedUniqueIndexEnforcesTheUniqueness()
        {
            // Setup
            Helper.CopyToTarget("Product");

            using (var connection = Database.CreateTarget())
            {
                connection.ExecuteNonQuery("INSERT INTO [Product] ([Id], [Code], [Sku], [Name], [Category], [Price]) VALUES (1, 'C1', 'S1', 'First', 1, 1.00);");

                // Act/Assert
                Assert.Throws<SqliteException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO [Product] ([Id], [Code], [Sku], [Name], [Category], [Price]) VALUES (2, 'C2', 'S1', 'Second', 1, 1.00);"));

                // Act/Assert
                Assert.Throws<SqliteException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO [Product] ([Id], [Code], [Sku], [Name], [Category], [Price]) VALUES (3, 'C1', 'S3', 'Third', 1, 1.00);"));
            }
        }

        [TestMethod]
        public void TestAhtolaSchemaComposerComposedEachIndexOnItsOwn()
        {
            // Setup
            var source = Helper.GetSourceSchema("Product");
            var composer = new AhtolaSchemaComposer();
            var withoutIndexes = new TableSchema(source.Table.Name, source.Table.Schema)
            {
                PrimaryKey = source.PrimaryKey,
                Columns = source.Columns,
                UniqueConstraints = source.UniqueConstraints
            };

            // Act
            Helper.ExecuteOnTarget(new[] { composer.ComposeCreateTable(withoutIndexes) });
            Helper.ExecuteOnTarget(source.Indexes.Select(i => composer.ComposeCreateIndex("Product", i)));

            // Assert
            Helper.AssertTargetMatchesSource("Product");
        }

        [TestMethod]
        public void TestAhtolaSchemaComposerComposedSchemaOfTablesWithIndexes()
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
