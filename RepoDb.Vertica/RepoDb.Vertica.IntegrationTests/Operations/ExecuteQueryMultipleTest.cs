using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vertica.Data.VerticaClient;
using RepoDb.Vertica.IntegrationTests.Models;
using RepoDb.Vertica.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Vertica.IntegrationTests.Operations
{
    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class ExecuteQueryMultipleTest
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
        public void TestVerticaConnectionExecuteQueryMultipleMultiStatementText()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new VerticaConnection(Database.ConnectionString);

            // Act
            using var extractor = connection.ExecuteQueryMultiple("SELECT * FROM \"CompleteTable\"; SELECT * FROM \"CompleteTable\"");
            var result1 = extractor.Extract<CompleteTable>();
            var result2 = extractor.Extract<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count(), result1.Count());
            Assert.AreEqual(tables.Count(), result2.Count());
        }

        [TestMethod]
        public void TestVerticaConnectionExecuteQueryMultipleThrowsOnParameterizedMultiStatementText()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var id = tables.First().Id;

            using var connection = new VerticaConnection(Database.ConnectionString);

            // Act & Assert
            Assert.Throws<VerticaException>(() =>
                connection.ExecuteQueryMultiple(
                    "SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id; SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                    new { Id = id }));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestVerticaConnectionExecuteQueryMultipleAsyncMultiStatementText()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new VerticaConnection(Database.ConnectionString);

            // Act
            using var extractor = await connection.ExecuteQueryMultipleAsync("SELECT * FROM \"CompleteTable\"; SELECT * FROM \"CompleteTable\"");
            var result1 = await extractor.ExtractAsync<CompleteTable>();
            var result2 = await extractor.ExtractAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count(), result1.Count());
            Assert.AreEqual(tables.Count(), result2.Count());
        }

        [TestMethod]
        public async Task TestVerticaConnectionExecuteQueryMultipleAsyncThrowsOnParameterizedMultiStatementText()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var id = tables.First().Id;

            using var connection = new VerticaConnection(Database.ConnectionString);

            // Act & Assert
            await Assert.ThrowsAsync<VerticaException>(() =>
                connection.ExecuteQueryMultipleAsync(
                    "SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id; SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                    new { Id = id }));
        }

        #endregion
    }
}
