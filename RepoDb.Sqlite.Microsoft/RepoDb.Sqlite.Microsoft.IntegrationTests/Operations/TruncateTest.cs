using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;

namespace RepoDb.SqLite.IntegrationTests.Operations.MDS
{
    [TestClass]
    public class TruncateTest
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

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionTruncate()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.Truncate<MdsCompleteTable>();
                var countResult = connection.CountAll<MdsCompleteTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionTruncateAsyncWithoutExpression()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.TruncateAsync<MdsCompleteTable>().Result;
                var countResult = connection.CountAll<MdsCompleteTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionTruncateViaTableNameWithoutExpression()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.Truncate(ClassMappedNameCache.Get<MdsCompleteTable>());
                var countResult = connection.CountAll<MdsCompleteTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionTruncateAsyncViaTableNameWithoutExpression()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.TruncateAsync(ClassMappedNameCache.Get<MdsCompleteTable>()).Result;
                var countResult = connection.CountAll<MdsCompleteTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #endregion
    }
}
