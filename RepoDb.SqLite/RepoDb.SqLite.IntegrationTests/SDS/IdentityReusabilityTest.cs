using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.SDS
{
    [TestClass]
    public class IdentityReusabilityTest
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
        public void TestSQLiteConnectionInsertForIdentityReusability()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10).AsList();

                // Act
                var insertAllResult = connection.InsertAll<SdsCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, insertAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());

                // Setup (3)
                var deleteEntity = tables[2];

                // Act (3)
                var deleteResult = connection.Delete<SdsCompleteTable>(deleteEntity);

                // Assert
                Assert.AreEqual(1, deleteResult);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<SdsCompleteTable>());

                // Setup
                var table = Helper.CreateSdsCompleteTables(1).First();

                // Act (3)
                var fields = FieldCache.Get<SdsCompleteTable>().Where(e => e.Name != "Id");
                var insertResult = connection.Insert<SdsCompleteTable>(table, fields: fields);

                // Assert (3)
                Assert.AreEqual(deleteEntity.Id, insertResult);
            }
        }

        [TestMethod]
        public void TestSQLiteConnectionInsertAsyncForIdentityReusability()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsTables(connection);

                // Setup
                var tables = Helper.CreateSdsCompleteTables(10).AsList();

                // Act
                var insertAllResult = connection.InsertAll<SdsCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, insertAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<SdsCompleteTable>());

                // Setup (3)
                var deleteEntity = tables[2];

                // Act (3)
                var deleteResult = connection.Delete<SdsCompleteTable>(deleteEntity);

                // Assert
                Assert.AreEqual(1, deleteResult);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<SdsCompleteTable>());

                // Setup
                var table = Helper.CreateSdsCompleteTables(1).First();

                // Act (3)
                var fields = FieldCache.Get<SdsCompleteTable>().Where(e => e.Name != "Id");
                var insertResult = connection.InsertAsync<SdsCompleteTable>(table, fields: fields).Result;

                // Assert (3)
                Assert.AreEqual(deleteEntity.Id, insertResult);
            }
        }
    }
}
