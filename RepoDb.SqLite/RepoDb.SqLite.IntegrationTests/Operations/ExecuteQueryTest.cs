using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations
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
        public void TestSqLiteConnectionExecuteQuery()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM [CompleteTable];");

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionExecuteQueryWithParameters()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM [CompleteTable] WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionExecuteQueryAsync()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM [CompleteTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionExecuteQueryAsyncWithParameters()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM [CompleteTable] WHERE Id = @Id;",
                    new { tables.Last().Id }).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        #endregion
    }
}
