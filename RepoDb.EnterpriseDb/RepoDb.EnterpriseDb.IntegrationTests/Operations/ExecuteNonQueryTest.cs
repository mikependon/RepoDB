using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnterpriseDB.EDBClient;
using RepoDb.EnterpriseDb.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.EnterpriseDb.IntegrationTests.Operations
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
        public void TestEnterpriseDbConnectionExecuteNonQuery()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionExecuteNonQueryWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionExecuteNonQueryWithMultipleStatement()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\"; DELETE FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestEnterpriseDbConnectionExecuteNonQueryAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionExecuteNonQueryAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionExecuteNonQueryAsyncWithMultipleStatement()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\"; DELETE FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion
    }
}
