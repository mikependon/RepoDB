using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vertica.Data.VerticaClient;
using RepoDb.Vertica.IntegrationTests.Models;
using RepoDb.Vertica.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Vertica.IntegrationTests.Operations
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
        public void TestVerticaConnectionExecuteNonQuery()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\"");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestVerticaConnectionExecuteNonQueryWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestVerticaConnectionExecuteNonQueryMultiStatementText()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\"; DELETE FROM \"CompleteTable\"");

                // Assert
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestVerticaConnectionExecuteNonQueryThrowsOnParameterizedMultiStatementText()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act & Assert
                Assert.Throws<VerticaException>(() =>
                    connection.ExecuteNonQuery(
                        "DELETE FROM \"CompleteTable\" WHERE \"Id\" = @Id; DELETE FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                        new { Id = 1 }));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestVerticaConnectionExecuteNonQueryAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\"");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestVerticaConnectionExecuteNonQueryAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestVerticaConnectionExecuteNonQueryAsyncMultiStatementText()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\"; DELETE FROM \"CompleteTable\"");

                // Assert
                Assert.AreEqual(0, await connection.CountAllAsync<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestVerticaConnectionExecuteNonQueryAsyncThrowsOnParameterizedMultiStatementText()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act & Assert
                await Assert.ThrowsAsync<VerticaException>(() =>
                    connection.ExecuteNonQueryAsync(
                        "DELETE FROM \"CompleteTable\" WHERE \"Id\" = @Id; DELETE FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                        new { Id = 1 }));
            }
        }

        #endregion
    }
}
