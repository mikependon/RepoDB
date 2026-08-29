using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver.ADO;
using RepoDb.ClickHouse.IntegrationTests.Models;
using RepoDb.ClickHouse.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.ClickHouse.IntegrationTests.Operations
{
    /// <summary>
    /// NOTE: there is no "...WithMultipleStatement" test here - ClickHouse's HTTP interface does
    /// not support multiple SQL statements in a single request under any circumstances (a plain
    /// command text is rejected outright with a SYNTAX_ERROR - "Multi-statements are not
    /// allowed" - as soon as it hits the separating semicolon and trailing text). See
    /// ClickHouseDbSetting.IsMultiStatementExecutable (always false for this provider) and
    /// ExecuteQueryMultipleTest.cs for a test that documents this limitation explicitly.
    /// </summary>
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
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                // ClickHouse's lightweight DELETE requires a WHERE clause, and its HTTP protocol does not
                // report an affected-row count for DELETE, so the effect is verified via CountAll instead.
                connection.ExecuteNonQuery("DELETE FROM `CompleteTable` WHERE 1 = 1;");

                // Assert
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionExecuteNonQueryWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                connection.ExecuteNonQuery("DELETE FROM `CompleteTable` WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(tables.Count() - 1, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseConnectionExecuteNonQueryAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM `CompleteTable` WHERE 1 = 1;");

                // Assert
                Assert.AreEqual(0, await connection.CountAllAsync<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionExecuteNonQueryAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM `CompleteTable` WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(tables.Count() - 1, await connection.CountAllAsync<CompleteTable>());
            }
        }

        #endregion
    }
}
