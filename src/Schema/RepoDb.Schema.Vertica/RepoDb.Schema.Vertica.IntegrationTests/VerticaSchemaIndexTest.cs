#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vertica.Data.VerticaClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.Vertica.IntegrationTests.Setup;

namespace RepoDb.Schema.Vertica.IntegrationTests
{
    /// <summary>
    /// Contains the vertica schema index test behavior.
    /// </summary>
    [TestClass]
    public class VerticaSchemaIndexTest
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
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                return new VerticaSchemaReader().GetIndexes(connection,
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
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                return new VerticaSchemaReader().GetUniqueConstraints(connection,
                    tableName,
                    transaction: null).Single(c => c.Name == name);
            }
        }

        #endregion

        #region Reader

        /// <summary>
        /// Verifies vertica schema reader get indexes of table without indexes.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaReaderGetIndexesOfTableWithoutIndexes()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new VerticaSchemaReader();
                // Act
                var actual = reader.GetIndexes(connection,
                    "Product",
                    transaction: null).ToList();
                // Assert
                Assert.AreEqual(0,
                    actual.Count);
            }
        }

        /// <summary>
        /// Verifies vertica schema reader get unique constraints of product.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaReaderGetUniqueConstraintsOfProduct()
        {
            // Act
            var first = GetUniqueConstraint("Product",
                "CIX_Product_Code");
            var second = GetUniqueConstraint("Product",
                "UX_Product_Sku");
            // Assert
            CollectionAssert.AreEqual(new[] { "Code" },
                first.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "Sku" },
                second.Columns.ToArray());
        }

        /// <summary>
        /// Verifies vertica schema reader get primary key of product.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaReaderGetPrimaryKeyOfProduct()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new VerticaSchemaReader();
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
        /// Verifies vertica schema reader get primary key of clustered primary key.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaReaderGetPrimaryKeyOfClusteredPrimaryKey()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new VerticaSchemaReader();
                // Act
                var actual = reader.GetPrimaryKey(connection,
                    "Person",
                    transaction: null);
                // Assert
                Assert.IsTrue(actual.IsClustered);
            }
        }

        /// <summary>
        /// Verifies vertica schema reader get indexes async of table without indexes.
        /// </summary>
        [TestMethod]
        public async Task TestVerticaSchemaReaderGetIndexesAsyncOfTableWithoutIndexes()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new VerticaSchemaReader();
                // Act
                var actual = (await reader.GetIndexesAsync(connection,
                    "Product",
                    transaction: null)).ToList();
                // Assert
                Assert.AreEqual(0,
                    actual.Count);
            }
        }

        #endregion

        #region Composer

        /// <summary>
        /// Verifies vertica schema composer composed schema of table with multiple indexes.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfTableWithMultipleIndexes()
        {
            // Act
            Helper.CopyToTarget("Product");
            // Assert
            Helper.AssertTargetMatchesSource("Product");
        }

        /// <summary>
        /// Verifies vertica schema composer composed schema keeps the primary key and the unique constraints.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaKeepsThePrimaryKeyAndTheUniqueConstraints()
        {
            // Act
            Helper.CopyToTarget("Product");
            // Assert
            var target = Helper.GetTargetSchema("Product");
            Assert.AreEqual("PK_Product",
                target.PrimaryKey.Name,
                StringComparer.Ordinal);
            CollectionAssert.AreEquivalent(new[] { "CIX_Product_Code", "UX_Product_Sku" },
                target.UniqueConstraints.Select(c => c.Name).ToArray());
        }

        /// <summary>
        /// Verifies vertica schema composer composed schema of tables with indexes.
        /// </summary>
        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfTablesWithIndexes()
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

        /// <summary>
        /// Verifies throw exception on vertica schema composer compose create index as vertica has no index.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaComposerComposeCreateIndexAsVerticaHasNoIndex()
        {
            // Act/Assert
            Assert.Throws<NotSupportedException>(() => new VerticaSchemaComposer().ComposeCreateIndex("Product",
                    new IndexInfo("IX") { Columns = { "Name" } }));
        }
        #endregion
    }
}
