#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations.Firebird;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Firebird.BulkOperations;
using RepoDb.Firebird.BulkOperations.IntegrationTests.Models;
using System.Linq;

namespace RepoDb.Firebird.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class FirebirdConnectionBulkDeleteByKeyOperationsTest
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

        #region Sync

        [TestMethod]
        public void TestFirebirdConnectionBulkDeleteByKey()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKey(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkDeleteByKeyWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKey(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    primaryKeys,
                    batchSize: 3);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkDeleteByKeyViaPhysicalTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKey(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    primaryKeys,
                    pseudoTableType: FirebirdBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestFirebirdConnectionBulkDeleteByKeyAsync()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKeyAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    primaryKeys).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkDeleteByKeyAsyncWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKeyAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    primaryKeys,
                    batchSize: 3).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkDeleteByKeyAsyncViaPhysicalTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKeyAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    primaryKeys,
                    pseudoTableType: FirebirdBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion
    }
}
