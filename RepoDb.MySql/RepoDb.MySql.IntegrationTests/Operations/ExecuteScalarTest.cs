using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using RepoDb.MySql.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.MySql.IntegrationTests.Operations
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
        public void TestExecuteScalar()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10);

                // Act
                var result = connection.ExecuteScalar("SELECT COUNT(*) FROM [CompleteTable];");

                // Assert
                Assert.AreEqual(tables.Count(), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestExecuteScalarWithReturnType()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10);

                // Act
                var result = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM [CompleteTable];");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestExecuteScalarAsync()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10);

                // Act
                var result = connection.ExecuteScalarAsync("SELECT COUNT(*) FROM [CompleteTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count(), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestExecuteScalarAsyncWithReturnType()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10);

                // Act
                var result = connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [CompleteTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion
    }
}
