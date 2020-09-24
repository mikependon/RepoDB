using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using RepoDb.Extensions;
using RepoDb.Reflection;
using RepoDb.MySql.IntegrationTests.Models;
using RepoDb.MySql.IntegrationTests.Setup;
using System.Data.Common;
using System.Linq;

namespace RepoDb.MySql.IntegrationTests.Operations
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
        public void TestMySqlConnectionExecuteReader()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = connection.ExecuteReader("SELECT Id, ColumnInt, ColumnDateTime FROM `CompleteTable`;"))
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
        public void TestMySqlConnectionExecuteReaderWithMultipleStatements()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = connection.ExecuteReader("SELECT Id, ColumnInt, ColumnDateTime FROM `CompleteTable`; SELECT Id, ColumnInt, ColumnDateTime FROM `CompleteTable`;"))
                {
                    do
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
                    } while (reader.NextResult());
                }
            }
        }

        [TestMethod]
        public void TestMySqlConnectionExecuteReaderAsExtractedEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM `CompleteTable`;"))
                {
                    // Act
                    var result = DataReader.ToEnumerable<CompleteTable>((DbDataReader)reader).AsList();

                    // Assert
                    tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
                }
            }
        }

        [TestMethod]
        public void TestMySqlConnectionExecuteReaderAsExtractedDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM `CompleteTable`;"))
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
        public void TestMySqlConnectionExecuteReaderAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT Id, ColumnInt, ColumnDateTime FROM `CompleteTable`;").Result)
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
        public void TestMySqlConnectionExecuteReaderAsyncWithMultipleStatements()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT Id, ColumnInt, ColumnDateTime FROM `CompleteTable`; SELECT Id, ColumnInt, ColumnDateTime FROM `CompleteTable`;").Result)
                {
                    do
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
                    } while (reader.NextResult());
                }
            }
        }

        [TestMethod]
        public void TestMySqlConnectionExecuteReaderAsyncAsExtractedEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM `CompleteTable`;").Result)
                {
                    // Act
                    var result = DataReader.ToEnumerable<CompleteTable>((DbDataReader)reader).AsList();

                    // Assert
                    tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
                }
            }
        }

        [TestMethod]
        public void TestMySqlConnectionExecuteReaderAsyncAsExtractedDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM `CompleteTable`;").Result)
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
