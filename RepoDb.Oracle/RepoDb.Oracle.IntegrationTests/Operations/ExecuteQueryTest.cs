using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    [TestClass]
    public class ExecuteQueryTest
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
        public void TestOracleConnectionExecuteQuery()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, result.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestOracleConnectionExecuteQueryWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act: bind variables are prefixed with ":" (not "@") for Oracle.
            var result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(1, result.Count());
            Helper.AssertPropertiesEquality(tables.Last(), result.First());
        }

        [TestMethod]
        public void TestOracleConnectionExecuteQueryWithFetchFirst()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act: Oracle has no "SELECT TOP n" - "FETCH FIRST n ROWS ONLY" is the equivalent.
            var result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" FETCH FIRST 5 ROWS ONLY");

            // Assert
            Assert.AreEqual(5, result.Count());
        }

        [TestMethod]
        public void TestOracleConnectionExecuteQueryWithNoResult()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionExecuteQueryAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, result.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestOracleConnectionExecuteQueryAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(1, result.Count());
            Helper.AssertPropertiesEquality(tables.Last(), result.First());
        }

        [TestMethod]
        public async Task TestOracleConnectionExecuteQueryAsyncWithFetchFirst()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" FETCH FIRST 5 ROWS ONLY");

            // Assert
            Assert.AreEqual(5, result.Count());
        }

        #endregion
    }
}
