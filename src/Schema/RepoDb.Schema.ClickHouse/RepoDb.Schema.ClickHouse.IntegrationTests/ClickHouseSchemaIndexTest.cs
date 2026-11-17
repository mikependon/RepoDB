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
    /// <summary>
    /// Contains the click house schema index test behavior.
    /// </summary>
    [TestClass]
    public class ClickHouseSchemaIndexTest
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
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                return new ClickHouseSchemaReader().GetIndexes(connection,
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
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                return new ClickHouseSchemaReader().GetUniqueConstraints(connection,
                    tableName,
                    transaction: null).Single(c => c.Name == name);
            }
        }

        #endregion

        #region Reader

        /// <summary>
        /// Verifies click house schema reader get indexes of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetIndexesOfTableWithMultipleIndexes()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetIndexes(connection,
                    "Product",
                    transaction: null).Select(i => i.Name).OrderBy(n => n,
                    StringComparer.Ordinal).ToArray();
                // Assert
                CollectionAssert.AreEqual(new[] { "IX_Product_Active_Name", "IX_Product_Category_Price" },
                    actual);
            }
        }

        /// <summary>
        /// Verifies click house schema reader get indexes of single column index.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetIndexesOfSingleColumnIndex()
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
        /// Verifies click house schema reader get primary key of product.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetPrimaryKeyOfProduct()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "Product",
                    transaction: null);
                // Assert
                Assert.AreEqual("PRIMARY",
                    actual.Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Id" },
                    actual.Columns.ToArray());
            }
        }

        /// <summary>
        /// Verifies click house schema reader get primary key of clustered primary key.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaReaderGetPrimaryKeyOfClusteredPrimaryKey()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual.IsClustered);
            }
        }

        /// <summary>
        /// Verifies click house schema reader get indexes async of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public async Task TestClickHouseSchemaReaderGetIndexesAsyncOfTableWithMultipleIndexes()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new ClickHouseSchemaReader();
                // Act
                var actual = (await reader.GetIndexesAsync(connection,
                    "Product",
                    transaction: null)).ToList();
                // Assert
                Assert.AreEqual(2,
                    actual.Count);
                Assert.IsNull(actual.Single(i => i.Name == "IX_Product_Active_Name").Filter);
                CollectionAssert.AreEqual(new[] { "Category", "Price" },
                    actual.Single(i => i.Name == "IX_Product_Category_Price").Columns.ToArray());
            }
        }

        #endregion

        #region Composer

        /// <summary>
        /// Verifies click house schema composer composed schema of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaComposerComposedSchemaOfTableWithMultipleIndexes()
        {
            // Act
            Helper.CopyToTarget("Product");
            // Assert
            Helper.AssertTargetMatchesSource("Product");
        }

        /// <summary>
        /// Verifies click house schema composer composed schema keeps the single column index.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaComposerComposedSchemaKeepsTheSingleColumnIndex()
        {
            // Act
            Helper.CopyToTarget("Product");
            // Assert
            var actual = Helper.GetTargetSchema("Product").Indexes.Single(i => i.Name == "IX_Product_Active_Name");
            CollectionAssert.AreEqual(new[] { "Name" },
                actual.Columns.ToArray());
        }

        /// <summary>
        /// Verifies click house schema composer composed each index on its own.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaComposerComposedEachIndexOnItsOwn()
        {
            // Setup
            var source = Helper.GetSourceSchema("Product");
            var composer = new ClickHouseSchemaComposer();
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
        /// Verifies click house schema composer composed schema of tables with indexes.
        /// </summary>
        [TestMethod]
        public void TestClickHouseSchemaComposerComposedSchemaOfTablesWithIndexes()
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
