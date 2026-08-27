using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations.Firebird;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
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

        [TestMethod]
        public void TestFirebirdConnectionBulkDeleteByKey()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            using var connection = new FbConnection(Database.ConnectionString);
            connection.BulkInsert(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);
            var keysToDelete = tables.Take(6).Select(t => t.Id);

            // Act
            var deleteResult = connection.BulkDeleteByKey<BulkOperationIdentityTable, long>(keysToDelete);

            // Assert
            Assert.AreEqual(6, deleteResult);
            Assert.AreEqual(4, connection.CountAll<BulkOperationIdentityTable>());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestFirebirdConnectionBulkDeleteByKeyAsync()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            using var connection = new FbConnection(Database.ConnectionString);
            await connection.BulkInsertAsync(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);
            var keysToDelete = tables.Take(6).Select(t => t.Id);

            // Act
            var deleteResult = await connection.BulkDeleteByKeyAsync<BulkOperationIdentityTable, long>(keysToDelete);

            // Assert
            Assert.AreEqual(6, deleteResult);
        }
    }
}
