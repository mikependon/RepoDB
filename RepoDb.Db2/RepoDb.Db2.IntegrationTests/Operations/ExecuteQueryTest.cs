using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
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
        public void TestDb2ConnectionExecuteQuery()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, result.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteQueryWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new Db2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM \"CompleteTable\"");

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteQueryWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act: bind variables are prefixed with ":" (not "@") for Db2.
            var result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(1, result.Count());
            Helper.AssertPropertiesEquality(tables.Last(), result.First());
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteQueryWithFetchFirst()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act: Db2 has no "SELECT TOP n" - "FETCH FIRST n ROWS ONLY" is the equivalent.
            var result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" FETCH FIRST 5 ROWS ONLY");

            // Assert
            Assert.AreEqual(5, result.Count());
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteQueryWithNoResult()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQueryAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, result.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQueryAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new Db2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = await connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM \"CompleteTable\"");

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQueryAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(1, result.Count());
            Helper.AssertPropertiesEquality(tables.Last(), result.First());
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQueryAsyncWithFetchFirst()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" FETCH FIRST 5 ROWS ONLY");

            // Assert
            Assert.AreEqual(5, result.Count());
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQueryAsyncWithNoResult()
        {
            using var connection = new Db2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        #endregion
    }
}
