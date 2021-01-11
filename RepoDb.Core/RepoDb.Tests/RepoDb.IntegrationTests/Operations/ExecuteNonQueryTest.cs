using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
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

        #region ExecuteNonQuery

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithNoAffectedTableRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteNonQuery("SELECT * FROM (SELECT 1 * 100 AS Value) TMP;");

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryDeleteSingle()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = 10;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryDeleteWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryDeleteWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryDeleteAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, 10);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryUpdateSingle()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;");

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryUpdateWithSigleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryUpdateWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryUpdateAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100;");

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithMultipleSqlStatementsWithoutParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;" +
                    "UPDATE [sc].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = 9;" +
                    "DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = 1;");

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithMultipleSqlStatementsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @Value1;" +
                    "UPDATE [sc].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = @Value2;" +
                    "DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @Value3;",
                    new { Value1 = 10, Value2 = 9, Value3 = 1 });

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQuery("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteNonQuery("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryByExecutingAStoredProcedureWithMultipleParametersAndWithOuputParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var output = new DirectionalQueryField("Output", typeof(int), ParameterDirection.Output);
                var param = new[]
                {
                    new QueryField("Value1", 100),
                    new QueryField("Value2", 200),
                    output
                };

                // Act
                var result = connection.ExecuteNonQuery("[dbo].[sp_multiply_with_output]",
                    param: param,
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(-1, result);
                Assert.AreEqual(20000, output.Parameter.Value);
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteNonQueryIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteNonQueryIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteNonQueryAsync

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithNoAffectedTableRows()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteNonQueryAsync("SELECT * FROM (SELECT 1 * 100 AS Value) TMP;").Result;

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncDeleteSingle()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = 10;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncDeleteWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncDeleteWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncDeleteAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count, 10);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncUpdateSingle()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;").Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncUpdateWithSigleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt;",
                    new { ColumnInt = 10 }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncUpdateWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @ColumnInt AND ColumnBit = @ColumnBit;",
                    new { ColumnInt = 10, ColumnBit = true }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncUpdateAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100;").Result;

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithMultipleSqlStatementsWithoutParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = 10;" +
                    "UPDATE [sc].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = 9;" +
                    "DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = 1;").Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithMultipleSqlStatementsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("UPDATE [sc].[IdentityTable] SET ColumnInt = 100 WHERE ColumnInt = @Value1;" +
                    "UPDATE [sc].[IdentityTable] SET ColumnInt = 90 WHERE ColumnInt = @Value2;" +
                    "DELETE FROM [sc].[IdentityTable] WHERE ColumnInt = @Value3;",
                    new { Value1 = 10, Value2 = 9, Value3 = 1 }).Result;

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncByExecutingAStoredProcedureWithSingleParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteNonQueryAsync("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncByExecutingAStoredProcedureWithMultipleParameters()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteNonQueryAsync("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(-1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncByExecutingAStoredProcedureWithMultipleParametersAndWithOuputParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var output = new DirectionalQueryField("Output", typeof(int), ParameterDirection.Output);
                var param = new[]
                {
                    new QueryField("Value1", 100),
                    new QueryField("Value2", 200),
                    output
                };

                // Act
                var result = connection.ExecuteNonQueryAsync("[dbo].[sp_multiply_with_output]",
                    param: param,
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(-1, result);
                Assert.AreEqual(20000, output.Parameter.Value);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteNonQueryAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteNonQueryAsyncIfThereAreSqlStatementProblems()
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
