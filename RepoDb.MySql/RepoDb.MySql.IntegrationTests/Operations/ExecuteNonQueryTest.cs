using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using RepoDb.MySql.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.MySql.IntegrationTests.Operations
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
        public void TestExecuteNonQuery()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [CompleteTable];");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestExecuteNonQueryWithParameters()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [CompleteTable] WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestExecuteNonQueryWithMultipleStatement()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [CompleteTable]; VACUUM;");

                // Assert
                Assert.AreEqual((tables.Count() * 2), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestExecuteNonQueryAsync()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10);

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [CompleteTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestExecuteNonQueryAsyncWithParameters()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10);

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [CompleteTable] WHERE Id = @Id;",
                    new { tables.Last().Id }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestExecuteNonQueryAsyncWithMultipleStatement()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10);

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [CompleteTable]; VACUUM;").Result;

                // Assert
                Assert.AreEqual((tables.Count() * 2), result);
            }
        }

        #endregion
    }
}
