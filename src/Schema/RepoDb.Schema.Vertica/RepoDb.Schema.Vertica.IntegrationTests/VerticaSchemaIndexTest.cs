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
    [TestClass]
    public class VerticaSchemaIndexTest
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
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                return new VerticaSchemaReader(connection).GetIndexes(tableName).ToList();
            }
        }

        private static UniqueConstraintInfo GetUniqueConstraint(string tableName, string name)
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                return new VerticaSchemaReader(connection).GetUniqueConstraints(tableName).Single(c => c.Name == name);
            }
        }

        #endregion

        #region Reader

        [TestMethod]
        public void TestVerticaSchemaReaderGetIndexesOfTableWithoutIndexes()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new VerticaSchemaReader(connection);

                // Act
                var actual = reader.GetIndexes("Product").ToList();

                // Assert
                Assert.AreEqual(0, actual.Count);
            }
        }

        [TestMethod]
        public void TestVerticaSchemaReaderGetUniqueConstraintsOfProduct()
        {
            // Act
            var first = GetUniqueConstraint("Product", "CIX_Product_Code");
            var second = GetUniqueConstraint("Product", "UX_Product_Sku");

            // Assert
            CollectionAssert.AreEqual(new[] { "Code" }, first.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "Sku" }, second.Columns.ToArray());
        }

        [TestMethod]
        public void TestVerticaSchemaReaderGetPrimaryKeyOfProduct()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new VerticaSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("Product");

                // Assert
                Assert.AreEqual("PK_Product", actual.Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "Id" }, actual.Columns.ToArray());
            }
        }

        [TestMethod]
        public void TestVerticaSchemaReaderGetPrimaryKeyOfClusteredPrimaryKey()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new VerticaSchemaReader(connection);

                // Act
                var actual = reader.GetPrimaryKey("Person");

                // Assert
                Assert.IsTrue(actual.IsClustered);
            }
        }

        [TestMethod]
        public async Task TestVerticaSchemaReaderGetIndexesAsyncOfTableWithoutIndexes()
        {
            using (var connection = new VerticaConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var reader = new VerticaSchemaReader(connection);

                // Act
                var actual = (await reader.GetIndexesAsync("Product")).ToList();

                // Assert
                Assert.AreEqual(0, actual.Count);
            }
        }

        #endregion

        #region Composer

        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfTableWithMultipleIndexes()
        {
            // Act
            Helper.CopyToTarget("Product");

            // Assert
            Helper.AssertTargetMatchesSource("Product");
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaKeepsThePrimaryKeyAndTheUniqueConstraints()
        {
            // Act
            Helper.CopyToTarget("Product");

            // Assert
            var target = Helper.GetTargetSchema("Product");
            Assert.AreEqual("PK_Product", target.PrimaryKey.Name, StringComparer.Ordinal);
            CollectionAssert.AreEquivalent(new[] { "CIX_Product_Code", "UX_Product_Sku" }, target.UniqueConstraints.Select(c => c.Name).ToArray());
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfTablesWithIndexes()
        {
            // Act
            Helper.CopyAllToTarget("Product", "Person", "Country");

            // Assert
            Helper.AssertTargetMatchesSource("Product");
            Helper.AssertTargetMatchesSource("Person");
            Helper.AssertTargetMatchesSource("Country");
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaComposerComposeCreateIndexAsVerticaHasNoIndex()
        {
            // Act/Assert
            Assert.Throws<NotSupportedException>(() => new VerticaSchemaComposer().ComposeCreateIndex("Product", new IndexInfo("IX") { Columns = { "Name" } }));
        }

        #endregion
    }
}
