using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.MDS
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
        public void TestSqLiteConnectionInsertForIdentityReusability()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsCompleteTables(10).AsList();

                // Act
                var insertAllResult = connection.InsertAll<MdsCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, insertAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<MdsCompleteTable>());

                // Setup (3)
                var deleteEntity = tables[2];

                // Act (3)
                var deleteResult = connection.Delete<MdsCompleteTable>(deleteEntity);

                // Assert
                Assert.AreEqual(1, deleteResult);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<MdsCompleteTable>());

                // Setup
                var table = Helper.CreateMdsCompleteTables(1).First();

                // Act (3)
                var fields = FieldCache.Get<MdsCompleteTable>().Where(e => e.Name != "Id");
                var insertResult = connection.Insert<MdsCompleteTable>(table, fields: fields);

                // Assert (3)
                Assert.AreEqual(deleteEntity.Id, insertResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionInsertAsyncForIdentityReusability()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsTables(connection);

                // Setup
                var tables = Helper.CreateMdsCompleteTables(10).AsList();

                // Act
                var insertAllResult = connection.InsertAll<MdsCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, insertAllResult);
                Assert.AreEqual(tables.Count, connection.CountAll<MdsCompleteTable>());

                // Setup (3)
                var deleteEntity = tables[2];

                // Act (3)
                var deleteResult = connection.Delete<MdsCompleteTable>(deleteEntity);

                // Assert
                Assert.AreEqual(1, deleteResult);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<MdsCompleteTable>());

                // Setup
                var table = Helper.CreateMdsCompleteTables(1).First();

                // Act (3)
                var fields = FieldCache.Get<MdsCompleteTable>().Where(e => e.Name != "Id");
                var insertResult = connection.InsertAsync<MdsCompleteTable>(table, fields: fields).Result;

                // Assert (3)
                Assert.AreEqual(deleteEntity.Id, insertResult);
            }
        }
    }
}
