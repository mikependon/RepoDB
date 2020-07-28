using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations
{
    [TestClass]
    public class ExecuteScalarTest
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
        public void TestSqLiteConnectionExecuteScalar()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteScalar("SELECT COUNT(*) FROM [CompleteTable];");

                // Assert
                Assert.AreEqual(tables.Count(), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionExecuteScalarWithReturnType()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM [CompleteTable];");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionExecuteScalarAsync()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteScalarAsync("SELECT COUNT(*) FROM [CompleteTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count(), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionExecuteScalarAsyncWithReturnType()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [CompleteTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion
    }
}
