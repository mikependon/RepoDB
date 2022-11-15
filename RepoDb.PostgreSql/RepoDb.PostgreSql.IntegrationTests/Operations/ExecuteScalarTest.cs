using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.PostgreSql.IntegrationTests.Operations
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
        public void TestPostgreSqlConnectionExecuteScalar()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteScalar("SELECT COUNT(*) FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionExecuteScalarWithReturnType()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestPostgreSqlConnectionExecuteScalarAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteScalarAsync("SELECT COUNT(*) FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionExecuteScalarAsyncWithReturnType()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion
    }
}
