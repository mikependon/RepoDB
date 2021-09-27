using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Setup;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations.SDS
{
    [TestClass]
    public class ExecuteNonQueryTest
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
        public void TestSqLiteConnectionExecuteNonQuery()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [SdsCompleteTable];");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionExecuteNonQueryWithParameters()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [SdsCompleteTable] WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionExecuteNonQueryWithMultipleStatement()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [SdsCompleteTable]; VACUUM;");

                // Assert
                Assert.AreEqual((tables.Count() * 2), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionExecuteNonQueryAsync()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [SdsCompleteTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionExecuteNonQueryAsyncWithParameters()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [SdsCompleteTable] WHERE Id = @Id;",
                    new { tables.Last().Id }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionExecuteNonQueryAsyncWithMultipleStatement()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [SdsCompleteTable]; VACUUM;").Result;

                // Assert
                Assert.AreEqual((tables.Count() * 2), result);
            }
        }

        #endregion
    }
}
