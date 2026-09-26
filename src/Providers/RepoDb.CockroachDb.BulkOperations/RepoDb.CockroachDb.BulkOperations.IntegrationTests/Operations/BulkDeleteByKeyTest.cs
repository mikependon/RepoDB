#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.CockroachDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations.CockroachDb;
using RepoDb.IntegrationTests.Setup;
using RepoDb.CockroachDb.BulkOperations.IntegrationTests.Models;
using System.Linq;

namespace RepoDb.CockroachDb.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class CockroachDbConnectionBulkDeleteByKeyOperationsTest
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
        public void TestCockroachDbConnectionBulkDeleteByKey()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
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
        public void TestCockroachDbConnectionBulkDeleteByKeyWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
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
        public void TestCockroachDbConnectionBulkDeleteByKeyViaPhysicalTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKey(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    primaryKeys,
                    pseudoTableType: CockroachDbBulkImportPseudoTableType.Physical);

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
        public void TestCockroachDbConnectionBulkDeleteByKeyAsync()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
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
        public void TestCockroachDbConnectionBulkDeleteByKeyAsyncWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
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
        public void TestCockroachDbConnectionBulkDeleteByKeyAsyncViaPhysicalTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKeyAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    primaryKeys,
                    pseudoTableType: CockroachDbBulkImportPseudoTableType.Physical).Result;

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
