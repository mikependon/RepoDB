#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;
using RepoDb.Connector.AuroraDb.MySqlConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.AuroraDbMySqlConnector.IntegrationTests.Setup;

namespace RepoDb.Schema.AuroraDbMySqlConnector.IntegrationTests
{
    [TestClass]
    public class AuroraDbMySqlConnectorSchemaIndexTest
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
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                return new AuroraDbMySqlConnectorSchemaReader(connection).GetIndexes(tableName).ToList();
            }
        }

        private static UniqueConstraintInfo GetUniqueConstraint(string tableName, string name)
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                return new AuroraDbMySqlConnectorSchemaReader(connection).GetUniqueConstraints(tableName).Single(c => c.Name == name);
            }
        }

        #endregion

        #region Reader

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetIndexesOfTableWithMultipleIndexes()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetIndexes("Product").Select(i => i.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray();

                // Assert
                CollectionAssert.AreEqual(
                    new[] { "IX_Product_Active_Name", "IX_Product_Category_Price" },
                    actual);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetUniqueConstraintsOfUniqueIndex()
        {
            // Act
            var actual = GetUniqueConstraint("Product", "CIX_Product_Code");

            // Assert
            CollectionAssert.AreEqual(new[] { "Code" }, actual.Columns.ToArray());
            Assert.IsFalse(GetIndexes("Product").Any(i => i.Name == "CIX_Product_Code"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetUniqueConstraintsOfSecondUniqueIndex()
        {
            // Act
            var actual = GetUniqueConstraint("Product", "UX_Product_Sku");

            // Assert
            CollectionAssert.AreEqual(new[] { "Sku" }, actual.Columns.ToArray());
            Assert.IsFalse(GetIndexes("Product").Any(i => i.Name == "UX_Product_Sku"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetIndexesOfMultiColumnIndexWithDescendingKey()
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
        public void TestAuroraDbMySqlConnectorSchemaReaderGetIndexesOfSingleColumnIndex()
        {
            // Act
            var actual = GetIndex("Product", "IX_Product_Active_Name");

            // Assert
            Assert.IsNull(actual.Filter);
            CollectionAssert.AreEqual(new[] { "Name" }, actual.Columns.ToArray());
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetIndexesOfIndexWithoutFilterAndWithDescendingKey()
        {
            // Act
            var actual = GetIndex("Person", "IX_Person_Name");

            // Assert
            Assert.IsNull(actual.Filter);
            CollectionAssert.AreEqual(new[] { "Name" }, actual.DescendingColumns.ToArray());
            Assert.IsFalse(actual.IsClustered);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetPrimaryKeyOfProduct()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("Product");

                // Assert
                Assert.AreEqual("PRIMARY", actual.Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Id" }, actual.Columns.ToArray());
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaReaderGetPrimaryKeyOfClusteredPrimaryKey()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("Person");

                // Assert
                Assert.IsTrue(actual.IsClustered);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbMySqlConnectorSchemaReaderGetIndexesAsyncOfTableWithMultipleIndexes()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new AuroraDbMySqlConnectorSchemaReader(connection);

                // Act
                var actual = (await reader.GetIndexesAsync("Product")).ToList();

                // Assert
                Assert.AreEqual(2, actual.Count);
                Assert.IsNull(actual.Single(i => i.Name == "IX_Product_Active_Name").Filter);
                CollectionAssert.AreEqual(new[] { "Price" }, actual.Single(i => i.Name == "IX_Product_Category_Price").DescendingColumns.ToArray());
            }
        }

        #endregion

        #region Composer

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedSchemaOfTableWithMultipleIndexes()
        {
            // Act
            Helper.CopyToTarget("Product");

            // Assert
            Helper.AssertTargetMatchesSource("Product");
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedSchemaKeepsThePrimaryKeyAndTheUniqueIndexes()
        {
            // Act
            Helper.CopyToTarget("Product");

            // Assert
            var target = Helper.GetTargetSchema("Product");
            Assert.AreEqual("PRIMARY", target.PrimaryKey.Name, StringComparer.Ordinal);
            CollectionAssert.AreEquivalent(new[] { "CIX_Product_Code", "UX_Product_Sku" }, target.UniqueConstraints.Select(c => c.Name).ToArray());
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedSchemaKeepsTheSingleColumnIndex()
        {
            // Act
            Helper.CopyToTarget("Product");

            // Assert
            var actual = Helper.GetTargetSchema("Product").Indexes.Single(i => i.Name == "IX_Product_Active_Name");
            CollectionAssert.AreEqual(new[] { "Name" }, actual.Columns.ToArray());
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedSchemaKeepsTheDescendingKey()
        {
            // Act
            Helper.CopyToTarget("Product");

            // Assert
            var actual = Helper.GetTargetSchema("Product").Indexes.Single(i => i.Name == "IX_Product_Category_Price");
            CollectionAssert.AreEqual(new[] { "Category", "Price" }, actual.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "Price" }, actual.DescendingColumns.ToArray());
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedUniqueIndexEnforcesTheUniqueness()
        {
            // Setup
            Helper.CopyToTarget("Product");

            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO `Product` (`Id`, `Code`, `Sku`, `Name`, `Category`, `Price`) VALUES (1, 'C1', 'S1', 'First', 1, 1.00);");

                // Act/Assert
                Assert.Throws<AuroraDbException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO `Product` (`Id`, `Code`, `Sku`, `Name`, `Category`, `Price`) VALUES (2, 'C2', 'S1', 'Second', 1, 1.00);"));

                // Act/Assert
                Assert.Throws<AuroraDbException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO `Product` (`Id`, `Code`, `Sku`, `Name`, `Category`, `Price`) VALUES (3, 'C1', 'S3', 'Third', 1, 1.00);"));
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedEachIndexOnItsOwn()
        {
            // Setup
            var source = Helper.GetSourceSchema("Product");
            var composer = new AuroraDbMySqlConnectorSchemaComposer();
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
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedSchemaOfTablesWithIndexes()
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
