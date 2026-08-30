#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.Enumerations.MySqlConnector;
using RepoDb.IntegrationTests.Setup;
using RepoDb.MySqlConnector.BulkOperations.IntegrationTests.Models;
using System.Linq;

namespace RepoDb.MySqlConnector.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class MySqlConnectionBulkDeleteByKeyOperationsTest
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
        public void TestMySqlConnectionBulkDeleteByKey()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionBulkDeleteByKeyWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionBulkDeleteByKeyViaPhysicalTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKey(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    primaryKeys,
                    pseudoTableType: MySqlConnectorBulkImportPseudoTableType.Physical);

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
        public void TestMySqlConnectionBulkDeleteByKeyAsync()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionBulkDeleteByKeyAsyncWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionBulkDeleteByKeyAsyncViaPhysicalTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKeyAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    primaryKeys,
                    pseudoTableType: MySqlConnectorBulkImportPseudoTableType.Physical).Result;

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
