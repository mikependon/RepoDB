using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vertica.Data.VerticaClient;
using RepoDb.Enumerations.Vertica;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Vertica.BulkOperations;
using RepoDb.Vertica.BulkOperations.IntegrationTests.Models;
using System.Linq;

namespace RepoDb.Vertica.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class VerticaConnectionBulkDeleteByKeyOperationsTest
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
        public void TestVerticaConnectionBulkDeleteByKey()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
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
        public void TestVerticaConnectionBulkDeleteByKeyWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
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
        public void TestVerticaConnectionBulkDeleteByKeyViaPhysicalTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKey(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    primaryKeys,
                    pseudoTableType: VerticaBulkImportPseudoTableType.Physical);

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
        public void TestVerticaConnectionBulkDeleteByKeyAsync()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
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
        public void TestVerticaConnectionBulkDeleteByKeyAsyncWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
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
        public void TestVerticaConnectionBulkDeleteByKeyAsyncViaPhysicalTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKeyAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    primaryKeys,
                    pseudoTableType: VerticaBulkImportPseudoTableType.Physical).Result;

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
