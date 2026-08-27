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
    public class FirebirdConnectionBulkDeleteOperationsTest
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
        public void TestFirebirdConnectionBulkDeleteForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            using var connection = new FbConnection(Database.ConnectionString);
            connection.BulkInsert(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

            // Act
            var deleteResult = connection.BulkDelete(tables);

            // Assert
            Assert.AreEqual(tables.Count, deleteResult);
            Assert.AreEqual(0, connection.CountAll<BulkOperationIdentityTable>());
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkDeleteForEntitiesWithExplicitQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            using var connection = new FbConnection(Database.ConnectionString);
            connection.BulkInsert(tables);

            // Act
            var deleteResult = connection.BulkDelete(tables, qualifiers: Field.From(nameof(BulkOperationNonIdentityTable.Id)));

            // Assert
            Assert.AreEqual(tables.Count, deleteResult);
            Assert.AreEqual(0, connection.CountAll<BulkOperationNonIdentityTable>());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestFirebirdConnectionBulkDeleteForEntitiesAsync()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            using var connection = new FbConnection(Database.ConnectionString);
            await connection.BulkInsertAsync(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

            // Act
            var deleteResult = await connection.BulkDeleteAsync(tables);

            // Assert
            Assert.AreEqual(tables.Count, deleteResult);
        }
    }
}
