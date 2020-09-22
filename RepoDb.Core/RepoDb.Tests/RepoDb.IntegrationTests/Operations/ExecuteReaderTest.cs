using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
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

        #region ExecuteReader

        [TestMethod]
        public void TestSqlConnectionExecuteReader()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable];"))
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader).AsList();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 }))
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader).AsList();

                    // Assert
                    Assert.AreEqual(2, result.Count());
                    result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } }))
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader).AsList();

                    // Assert
                    Assert.AreEqual(3, result.Count());
                    result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReader("SELECT TOP (@Top) * FROM [sc].[IdentityTable];", new { Top = 2 }))
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader).AsList();

                    // Assert
                    Assert.AreEqual(2, result.Count());
                    result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReader("[dbo].[sp_get_identity_tables]", commandType: CommandType.StoredProcedure))
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader).AsList();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReader("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure))
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader).AsList();

                    // Assert
                    Assert.AreEqual(1, result.Count());
                    Helper.AssertPropertiesEquality(tables.Last(), result.First());
                }
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteReaderIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteReaderIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteReaderAsync

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable];").Result)
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader).AsList();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 }).Result)
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader).AsList();

                    // Assert
                    Assert.AreEqual(2, result.Count());
                    result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } }).Result)
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader).AsList();

                    // Assert
                    Assert.AreEqual(3, result.Count());
                    result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT TOP (@Top) * FROM [sc].[IdentityTable];", new { Top = 2 }).Result)
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader).AsList();

                    // Assert
                    Assert.AreEqual(2, result.Count());
                    result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReaderAsync("[dbo].[sp_get_identity_tables]", commandType: CommandType.StoredProcedure).Result)
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader).AsList();

                    // Assert
                    Assert.AreEqual(tables.Count, result.Count());
                    tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                using (var reader = connection.ExecuteReaderAsync("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result)
                {
                    // Act
                    var result = Reflection.DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader).AsList();

                    // Assert
                    Assert.AreEqual(1, result.Count());
                    Helper.AssertPropertiesEquality(tables.Last(), result.First());
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteReaderAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteReaderAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion
    }
}
