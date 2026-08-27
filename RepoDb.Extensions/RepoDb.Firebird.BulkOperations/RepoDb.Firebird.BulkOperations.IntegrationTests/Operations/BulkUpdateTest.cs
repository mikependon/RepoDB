using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Firebird.BulkOperations.IntegrationTests.Models;
using System.Linq;

namespace RepoDb.Firebird.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class FirebirdConnectionBulkUpdateOperationsTest
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
        public void TestFirebirdConnectionBulkUpdateForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            using var connection = new FbConnection(Database.ConnectionString);
            connection.BulkInsert(tables, identityBehavior: RepoDb.Enumerations.Firebird.FirebirdBulkImportIdentityBehavior.ReturnIdentity);

            Helper.UpdateBulkOperationIdentityTables(tables);

            // Act
            var updateResult = connection.BulkUpdate(tables);

            // Assert
            Assert.AreEqual(tables.Count, updateResult);

            // Act
            var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

            // Assert
            tables.ForEach(t =>
            {
                var item = queryResult.First(e => e.Id == t.Id);
                Helper.AssertPropertiesEquality(t, item);
            });
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkUpdateForEntitiesWithExplicitQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            using var connection = new FbConnection(Database.ConnectionString);
            connection.BulkInsert(tables);

            Helper.UpdateBulkOperationNonIdentityTables(tables);

            // Act
            var updateResult = connection.BulkUpdate(tables, qualifiers: Field.From(nameof(BulkOperationNonIdentityTable.Id)));

            // Assert
            Assert.AreEqual(tables.Count, updateResult);

            // Act
            var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

            // Assert
            tables.ForEach(t =>
            {
                var item = queryResult.First(e => e.Id == t.Id);
                Helper.AssertPropertiesEquality(t, item);
            });
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestFirebirdConnectionBulkUpdateForEntitiesAsync()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            using var connection = new FbConnection(Database.ConnectionString);
            await connection.BulkInsertAsync(tables, identityBehavior: RepoDb.Enumerations.Firebird.FirebirdBulkImportIdentityBehavior.ReturnIdentity);

            Helper.UpdateBulkOperationIdentityTables(tables);

            // Act
            var updateResult = await connection.BulkUpdateAsync(tables);

            // Assert
            Assert.AreEqual(tables.Count, updateResult);
        }
    }
}
