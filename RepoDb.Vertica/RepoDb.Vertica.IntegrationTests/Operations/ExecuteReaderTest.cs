using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vertica.Data.VerticaClient;
using RepoDb.Extensions;
using RepoDb.Reflection;
using RepoDb.Vertica.IntegrationTests.Models;
using RepoDb.Vertica.IntegrationTests.Setup;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Vertica.IntegrationTests.Operations
{
    [TestClass]
    public class ExecuteReaderTest
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
        public void TestVerticaConnectionExecuteReader()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = connection.ExecuteReader("SELECT \"Id\", \"ColumnInt\", \"ColumnDateTime\" FROM \"CompleteTable\""))
                {
                    while (reader.Read())
                    {
                        // Act
                        var id = reader.GetInt64(0);
                        var columnInt = reader.GetInt32(1);
                        var columnDateTime = reader.GetDateTime(2);
                        var table = tables.FirstOrDefault(e => e.Id == id);

                        // Assert
                        Assert.IsNotNull(table);
                        Assert.AreEqual(columnInt, table.ColumnInt);
                        Assert.AreEqual(columnDateTime, table.ColumnDateTime);
                    }
                }
            }
        }

        [TestMethod]
        public void TestVerticaConnectionExecuteReaderMultiStatementText()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = connection.ExecuteReader("SELECT \"Id\" FROM \"CompleteTable\"; SELECT \"Id\" FROM \"CompleteTable\""))
                {
                    var count1 = 0;
                    while (reader.Read())
                    {
                        count1++;
                    }

                    var hasSecondResult = reader.NextResult();
                    var count2 = 0;
                    while (hasSecondResult && reader.Read())
                    {
                        count2++;
                    }

                    // Assert
                    Assert.AreEqual(tables.Count(), count1);
                    Assert.IsTrue(hasSecondResult);
                    Assert.AreEqual(tables.Count(), count2);
                }
            }
        }

        [TestMethod]
        public void TestVerticaConnectionExecuteReaderThrowsOnParameterizedMultiStatementText()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var id = tables.First().Id;

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act & Assert
                Assert.Throws<VerticaException>(() =>
                    connection.ExecuteReader(
                        "SELECT \"Id\" FROM \"CompleteTable\" WHERE \"Id\" = @Id; SELECT \"Id\" FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                        new { Id = id }));
            }
        }

        [TestMethod]
        public void TestVerticaConnectionExecuteReaderAsExtractedEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM \"CompleteTable\""))
                {
                    // Act
                    var result = DataReader.ToEnumerable<CompleteTable>((DbDataReader)reader).AsList();

                    // Assert
                    tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
                }
            }
        }

        [TestMethod]
        public void TestVerticaConnectionExecuteReaderAsExtractedDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM \"CompleteTable\""))
                {
                    // Act
                    var result = DataReader.ToEnumerable((DbDataReader)reader).AsList();

                    // Assert
                    tables.AsList().ForEach(table => Helper.AssertMembersEquality(table, result.First(e => e.Id == table.Id)));
                }
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestVerticaConnectionExecuteReaderAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT \"Id\", \"ColumnInt\", \"ColumnDateTime\" FROM \"CompleteTable\""))
                {
                    while (reader.Read())
                    {
                        // Act
                        var id = reader.GetInt64(0);
                        var columnInt = reader.GetInt32(1);
                        var columnDateTime = reader.GetDateTime(2);
                        var table = tables.FirstOrDefault(e => e.Id == id);

                        // Assert
                        Assert.IsNotNull(table);
                        Assert.AreEqual(columnInt, table.ColumnInt);
                        Assert.AreEqual(columnDateTime, table.ColumnDateTime);
                    }
                }
            }
        }

        [TestMethod]
        public async Task TestVerticaConnectionExecuteReaderAsyncMultiStatementText()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act - async counterpart of TestVerticaConnectionExecuteReaderMultiStatementText.
                using (var reader = await connection.ExecuteReaderAsync("SELECT \"Id\" FROM \"CompleteTable\"; SELECT \"Id\" FROM \"CompleteTable\""))
                {
                    var count1 = 0;
                    while (reader.Read())
                    {
                        count1++;
                    }

                    var hasSecondResult = reader.NextResult();
                    var count2 = 0;
                    while (hasSecondResult && reader.Read())
                    {
                        count2++;
                    }

                    // Assert
                    Assert.AreEqual(tables.Count(), count1);
                    Assert.IsTrue(hasSecondResult);
                    Assert.AreEqual(tables.Count(), count2);
                }
            }
        }

        [TestMethod]
        public async Task TestVerticaConnectionExecuteReaderAsyncThrowsOnParameterizedMultiStatementText()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var id = tables.First().Id;

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act & Assert
                await Assert.ThrowsAsync<VerticaException>(() =>
                    connection.ExecuteReaderAsync(
                        "SELECT \"Id\" FROM \"CompleteTable\" WHERE \"Id\" = @Id; SELECT \"Id\" FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                        new { Id = id }));
            }
        }

        [TestMethod]
        public async Task TestVerticaConnectionExecuteReaderAsyncAsExtractedEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM \"CompleteTable\""))
                {
                    // Act
                    var result = DataReader.ToEnumerable<CompleteTable>((DbDataReader)reader).AsList();

                    // Assert
                    tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
                }
            }
        }

        [TestMethod]
        public async Task TestVerticaConnectionExecuteReaderAsyncAsExtractedDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM \"CompleteTable\""))
                {
                    // Act
                    var result = DataReader.ToEnumerable((DbDataReader)reader).AsList();

                    // Assert
                    tables.AsList().ForEach(table => Helper.AssertMembersEquality(table, result.First(e => e.Id == table.Id)));
                }
            }
        }

        #endregion
    }
}
