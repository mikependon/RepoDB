using RepoDb.Connector.MariaDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations.MariaDb;
using RepoDb.IntegrationTests.Setup;
using RepoDb.MariaDb.BulkOperations.IntegrationTests.Models;
using System.Linq;

namespace RepoDb.MariaDb.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class MariaDbConnectionBulkDeleteByKeyOperationsTest
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
        public void TestMariaDbConnectionBulkDeleteByKey()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
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
        public void TestMariaDbConnectionBulkDeleteByKeyWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
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
        public void TestMariaDbConnectionBulkDeleteByKeyViaPhysicalTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKey(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    primaryKeys,
                    pseudoTableType: MariaDbBulkImportPseudoTableType.Physical);

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
        public void TestMariaDbConnectionBulkDeleteByKeyAsync()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
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
        public void TestMariaDbConnectionBulkDeleteByKeyAsyncWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
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
        public void TestMariaDbConnectionBulkDeleteByKeyAsyncViaPhysicalTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKeyAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    primaryKeys,
                    pseudoTableType: MariaDbBulkImportPseudoTableType.Physical).Result;

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
