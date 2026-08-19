using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver.ADO;
using RepoDb.ClickHouse.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.ClickHouse.IntegrationTests.Operations
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
        public void TestClickHouseConnectionExecuteNonQuery()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM `CompleteTable`;");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionExecuteNonQueryWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM `CompleteTable` WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionExecuteNonQueryWithMultipleStatement()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM `CompleteTable`; DELETE FROM `CompleteTable`;");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseConnectionExecuteNonQueryAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM `CompleteTable`;");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionExecuteNonQueryAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM `CompleteTable` WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionExecuteNonQueryAsyncWithMultipleStatement()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM `CompleteTable`; DELETE FROM `CompleteTable`;");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion
    }
}
