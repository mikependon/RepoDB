#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClickHouse.Driver;
using ClickHouse.Driver.ADO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.ClickHouse.IntegrationTests.Setup;

namespace RepoDb.Schema.ClickHouse.IntegrationTests
{
    [TestClass]
    public class ClickHouseSchemaIndexTest
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
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                return new ClickHouseSchemaReader(connection).GetIndexes(tableName).ToList();
            }
        }

        private static UniqueConstraintInfo GetUniqueConstraint(string tableName, string name)
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                return new ClickHouseSchemaReader(connection).GetUniqueConstraints(tableName).Single(c => c.Name == name);
            }
        }

        #endregion

        #region Reader

        [TestMethod]
        public void TestClickHouseSchemaReaderGetIndexesOfTableWithMultipleIndexes()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetIndexes("Product").Select(i => i.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray();

                // Assert
                CollectionAssert.AreEqual(
                    new[] { "IX_Product_Active_Name", "IX_Product_Category_Price" },
                    actual);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetIndexesOfSingleColumnIndex()
        {
            // Act
            var actual = GetIndex("Product", "IX_Product_Active_Name");

            // Assert
            Assert.IsNull(actual.Filter);
            CollectionAssert.AreEqual(new[] { "Name" }, actual.Columns.ToArray());
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetPrimaryKeyOfProduct()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("Product");

                // Assert
                Assert.AreEqual("PRIMARY", actual.Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Id" }, actual.Columns.ToArray());
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaReaderGetPrimaryKeyOfClusteredPrimaryKey()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("Person");

                // Assert
                Assert.IsTrue(actual.IsClustered);
            }
        }

        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetIndexesAsyncOfTableWithMultipleIndexes()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader(connection);

                // Act
                var actual = (await reader.GetIndexesAsync("Product")).ToList();

                // Assert
                Assert.AreEqual(2, actual.Count);
                Assert.IsNull(actual.Single(i => i.Name == "IX_Product_Active_Name").Filter);
                CollectionAssert.AreEqual(new[] { "Category", "Price" }, actual.Single(i => i.Name == "IX_Product_Category_Price").Columns.ToArray());
            }
        }

        #endregion

        #region Composer

        [TestMethod]
        public void TestClickHouseSchemaComposerComposedSchemaOfTableWithMultipleIndexes()
        {
            // Act
            Helper.CopyToTarget("Product");

            // Assert
            Helper.AssertTargetMatchesSource("Product");
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposedSchemaKeepsTheSingleColumnIndex()
        {
            // Act
            Helper.CopyToTarget("Product");

            // Assert
            var actual = Helper.GetTargetSchema("Product").Indexes.Single(i => i.Name == "IX_Product_Active_Name");
            CollectionAssert.AreEqual(new[] { "Name" }, actual.Columns.ToArray());
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposedEachIndexOnItsOwn()
        {
            // Setup
            var source = Helper.GetSourceSchema("Product");
            var composer = new ClickHouseSchemaComposer();
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
        public void TestClickHouseSchemaComposerComposedSchemaOfTablesWithIndexes()
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
