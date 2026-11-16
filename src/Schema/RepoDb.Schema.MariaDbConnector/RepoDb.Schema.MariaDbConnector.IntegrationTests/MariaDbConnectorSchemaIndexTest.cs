#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepoDb.Connector.MariaDbConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.MariaDbConnector.IntegrationTests.Setup;

namespace RepoDb.Schema.MariaDbConnector.IntegrationTests
{
    /// <summary>
    /// Contains the maria db connector schema index test behavior.
    /// </summary>
    [TestClass]
    public class MariaDbConnectorSchemaIndexTest
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
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                return new MariaDbConnectorSchemaReader().GetIndexes(connection,
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
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                return new MariaDbConnectorSchemaReader().GetUniqueConstraints(connection,
                    tableName,
                    transaction: null).Single(c => c.Name == name);
            }
        }

        #endregion

        #region Reader

        /// <summary>
        /// Verifies maria db connector schema reader get indexes of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetIndexesOfTableWithMultipleIndexes()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get unique constraints of unique index.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetUniqueConstraintsOfUniqueIndex()
        {
            // Act
            var actual = GetUniqueConstraint("Product",
                "CIX_Product_Code");
            // Assert
            CollectionAssert.AreEqual(new[] { "Code" },
                actual.Columns.ToArray());
            Assert.IsFalse(GetIndexes("Product").Any(i => i.Name == "CIX_Product_Code"));
        }

        /// <summary>
        /// Verifies maria db connector schema reader get unique constraints of second unique index.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetUniqueConstraintsOfSecondUniqueIndex()
        {
            // Act
            var actual = GetUniqueConstraint("Product",
                "UX_Product_Sku");
            // Assert
            CollectionAssert.AreEqual(new[] { "Sku" },
                actual.Columns.ToArray());
            Assert.IsFalse(GetIndexes("Product").Any(i => i.Name == "UX_Product_Sku"));
        }

        /// <summary>
        /// Verifies maria db connector schema reader get indexes of multi column index with descending key.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetIndexesOfMultiColumnIndexWithDescendingKey()
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
        /// Verifies maria db connector schema reader get indexes of single column index.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetIndexesOfSingleColumnIndex()
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
        /// Verifies maria db connector schema reader get indexes of index without filter and with descending key.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetIndexesOfIndexWithoutFilterAndWithDescendingKey()
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
        /// Verifies maria db connector schema reader get primary key of product.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetPrimaryKeyOfProduct()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
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
        /// Verifies maria db connector schema reader get primary key of clustered primary key.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaReaderGetPrimaryKeyOfClusteredPrimaryKey()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual.IsClustered);
            }
        }

        /// <summary>
        /// Verifies maria db connector schema reader get indexes async of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public async Task TestMariaDbConnectorSchemaReaderGetIndexesAsyncOfTableWithMultipleIndexes()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new MariaDbConnectorSchemaReader();
                // Act
                var actual = (await reader.GetIndexesAsync(connection,
                    "Product",
                    transaction: null)).ToList();
                // Assert
                Assert.AreEqual(2,
                    actual.Count);
                Assert.IsNull(actual.Single(i => i.Name == "IX_Product_Active_Name").Filter);
                CollectionAssert.AreEqual(new[] { "Price" },
                    actual.Single(i => i.Name == "IX_Product_Category_Price").DescendingColumns.ToArray());
            }
        }

        #endregion

        #region Composer

        /// <summary>
        /// Verifies maria db connector schema composer composed schema of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaComposerComposedSchemaOfTableWithMultipleIndexes()
        {
            // Act
            Helper.CopyToTarget("Product");
            // Assert
            Helper.AssertTargetMatchesSource("Product");
        }

        /// <summary>
        /// Verifies maria db connector schema composer composed schema keeps the primary key and the unique indexes.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaComposerComposedSchemaKeepsThePrimaryKeyAndTheUniqueIndexes()
        {
            // Act
            Helper.CopyToTarget("Product");
            // Assert
            var target = Helper.GetTargetSchema("Product");
            Assert.AreEqual("PRIMARY",
                target.PrimaryKey.Name,
                StringComparer.Ordinal);
            CollectionAssert.AreEquivalent(new[] { "CIX_Product_Code", "UX_Product_Sku" },
                target.UniqueConstraints.Select(c => c.Name).ToArray());
        }

        /// <summary>
        /// Verifies maria db connector schema composer composed schema keeps the single column index.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaComposerComposedSchemaKeepsTheSingleColumnIndex()
        {
            // Act
            Helper.CopyToTarget("Product");
            // Assert
            var actual = Helper.GetTargetSchema("Product").Indexes.Single(i => i.Name == "IX_Product_Active_Name");
            CollectionAssert.AreEqual(new[] { "Name" },
                actual.Columns.ToArray());
        }

        /// <summary>
        /// Verifies maria db connector schema composer composed schema keeps the descending key.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaComposerComposedSchemaKeepsTheDescendingKey()
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
        /// Verifies maria db connector schema composer composed unique index enforces the uniqueness.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaComposerComposedUniqueIndexEnforcesTheUniqueness()
        {
            // Setup
            Helper.CopyToTarget("Product");
            using (var connection = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO `Product` (`Id`, `Code`, `Sku`, `Name`, `Category`, `Price`) VALUES (1, 'C1', 'S1', 'First', 1, 1.00);");
                // Act/Assert
                Assert.Throws<MariaDbException>(() => connection.ExecuteNonQuery("INSERT INTO `Product` (`Id`, `Code`, `Sku`, `Name`, `Category`, `Price`) VALUES (2, 'C2', 'S1', 'Second', 1, 1.00);"));
                // Act/Assert
                Assert.Throws<MariaDbException>(() => connection.ExecuteNonQuery("INSERT INTO `Product` (`Id`, `Code`, `Sku`, `Name`, `Category`, `Price`) VALUES (3, 'C1', 'S3', 'Third', 1, 1.00);"));
            }
        }

        /// <summary>
        /// Verifies maria db connector schema composer composed each index on its own.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaComposerComposedEachIndexOnItsOwn()
        {
            // Setup
            var source = Helper.GetSourceSchema("Product");
            var composer = new MariaDbConnectorSchemaComposer();
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
        /// Verifies maria db connector schema composer composed schema of tables with indexes.
        /// </summary>
        [TestMethod]
        public void TestMariaDbConnectorSchemaComposerComposedSchemaOfTablesWithIndexes()
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
