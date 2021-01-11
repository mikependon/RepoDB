using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
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

        #region ExecuteQuery<dynamic>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryViaDynamics()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery("SELECT * FROM [sc].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryViaDynamicsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryViaDynamicsWithArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } });

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryViaDynamicsWithTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery("SELECT TOP (@Top) * FROM [sc].[IdentityTable];",
                    new { Top = 2 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryViaDynamicsWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryViaDynamicsWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(1, result.Count());
                result.AsList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryViaDynamicsWithStoredProcedureWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure).FirstOrDefault();

                // Assert
                var kvp = result as IDictionary<string, object>;
                Assert.IsNotNull(result);
                Assert.AreEqual(20000, kvp.First().Value);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryViaDynamicsWithStoredProcedureWithMultipleParametersAndWithOuputParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var output = new DirectionalQueryField("Output", typeof(int), ParameterDirection.Output);
            var param = new[]
            {
                new QueryField("Value1", 100),
                new QueryField("Value2", 200),
                output
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery("[dbo].[sp_multiply_with_output]",
                    param: param,
                    commandType: CommandType.StoredProcedure).FirstOrDefault();

                // Assert
                var kvp = result as IDictionary<string, object>;
                Assert.IsNotNull(result);
                Assert.AreEqual(20000, kvp.First().Value);

                // Assert
                Assert.AreEqual(20000, output.Parameter.Value);
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteQueryAsync<dynamic>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncViaDynamics()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncViaDynamicsWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncViaDynamicsWithArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } }).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncViaDynamicsWithTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync("SELECT TOP (@Top) * FROM [sc].[IdentityTable];",
                    new { Top = 2 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncViaDynamicsWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncViaDynamicsWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                result.AsList().ForEach(kvpItem =>
                {
                    var kvp = kvpItem as IDictionary<string, object>;
                    var item = tables.First(t => t.Id == Convert.ToInt32(kvp[nameof(IdentityTable.Id)]));

                    // Assert
                    Assert.AreEqual(item.ColumnBit, kvp[nameof(IdentityTable.ColumnBit)]);
                    Assert.AreEqual(item.ColumnDateTime, kvp[nameof(IdentityTable.ColumnDateTime)]);
                    Assert.AreEqual(item.ColumnDateTime2, kvp[nameof(IdentityTable.ColumnDateTime2)]);
                    Assert.AreEqual(item.ColumnDecimal, kvp[nameof(IdentityTable.ColumnDecimal)]);
                    Assert.AreEqual(item.ColumnFloat, Convert.ToSingle(kvp[nameof(IdentityTable.ColumnFloat)]));
                    Assert.AreEqual(item.ColumnInt, kvp[nameof(IdentityTable.ColumnInt)]);
                    Assert.AreEqual(item.ColumnNVarChar, kvp[nameof(IdentityTable.ColumnNVarChar)]);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncViaDynamicsWithStoredProcedureWithMultipleParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync("[dbo].[sp_multiply]",
                    param: new { Value1 = 100, Value2 = 200 },
                    commandType: CommandType.StoredProcedure).Result.FirstOrDefault();

                // Assert
                var kvp = result as IDictionary<string, object>;
                Assert.IsNotNull(result);
                Assert.AreEqual(20000, kvp.First().Value);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncViaDynamicsWithStoredProcedureWithMultipleParametersAndWithOuputParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var output = new DirectionalQueryField("Output", typeof(int), ParameterDirection.Output);
            var param = new[]
            {
                new QueryField("Value1", 100),
                new QueryField("Value2", 200),
                output
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync("[dbo].[sp_multiply_with_output]",
                    param: param,
                    commandType: CommandType.StoredProcedure).Result.FirstOrDefault();

                // Assert
                var kvp = result as IDictionary<string, object>;
                Assert.IsNotNull(result);
                Assert.AreEqual(20000, kvp.First().Value);

                // Assert
                Assert.AreEqual(20000, output.Parameter.Value);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        #endregion

        #region ExecuteQuery<TEntity>

        [TestMethod]
        public void TestSqlConnectionExecuteQuery()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable];");

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } });

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT TOP (@Top) * FROM [sc].[IdentityTable];",
                    new { Top = 2 });

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWhereTheDataReaderColumnsAreMoreThanClassProperties()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<LiteIdentityTable>("SELECT * FROM [sc].[IdentityTable];");

                // Assert
                Assert.AreEqual(10, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.Where(t => t.Id == item.Id).First();
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnInt, item.ColumnInt);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWhereTheDataReaderColumnsAreLessThanClassProperties()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT Id, ColumnBit, ColumnDateTime, ColumnInt FROM [sc].[IdentityTable];");

                // Assert
                Assert.AreEqual(10, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.Where(t => t.Id == item.Id).First();
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnInt, item.ColumnInt);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithDictionaryParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new Dictionary<string, object>
            {
                { "ColumnFloat", last.ColumnFloat },
                { "ColumnInt", last.ColumnInt }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithExpandoObjectAsIDictionaryParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Add the parameters
            param.Add("ColumnFloat", last.ColumnFloat);
            param.Add("ColumnInt", last.ColumnInt);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithExpandoObjectAsDynamicParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = (dynamic)new ExpandoObject();

            // Add the parameters
            param.ColumnFloat = last.ColumnFloat;
            param.ColumnInt = last.ColumnInt;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", (object)param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithQueryGroupAsParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithQueryFieldsAsParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithQueryFieldAsParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new QueryField("ColumnFloat", last.ColumnFloat);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat;", param);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidParameterException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryIfTheParameterAreInvalidTypeDictionaryObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var param = new Dictionary<string, int>();

                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);", param);
            }
        }

        //[TestMethod, ExpectedException(typeof(InvalidOperationException))]
        //public void ThrowExceptionOnTestSqlConnectionExecuteQueryIfTheParameterIsQueryFieldAndTheOperationIsNotEqualsToEqual()
        //{
        //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        //    {
        //        // Setup
        //        var param = new QueryField("Id", Operation.NotEqual, 1);

        //        // Act
        //        connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);", param);
        //    }
        //}

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<IdentityTable>("SELECT FROM [sc].[IdentityTable] WHERE (Id = @Id);");
            }
        }

        #endregion

        #region ExecuteQueryAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable];").Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt BETWEEN @From AND @To;",
                    new { From = 3, To = 4 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithArrayParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnInt IN (@ColumnInt);",
                    new { ColumnInt = new[] { 5, 6, 7 } }).Result;

                // Assert
                Assert.AreEqual(3, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithTopParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT TOP (@Top) * FROM [sc].[IdentityTable];",
                    new { Top = 2 }).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.Where(r => r.Id == item.Id).First(), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithStoredProcedure()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("[dbo].[sp_get_identity_tables]",
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithStoredProcedureWithParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("[dbo].[sp_get_identity_table_by_id]",
                    param: new { tables.Last().Id },
                    commandType: CommandType.StoredProcedure).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWhereTheDataReaderColumnsAreMoreThanClassProperties()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<LiteIdentityTable>("SELECT * FROM [sc].[IdentityTable];").Result;

                // Assert
                Assert.AreEqual(10, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.Where(t => t.Id == item.Id).First();
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnInt, item.ColumnInt);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithDictionaryParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new Dictionary<string, object>
            {
                { "ColumnFloat", last.ColumnFloat },
                { "ColumnInt", last.ColumnInt }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithExpandoObjectAsIDictionaryParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Add the parameters
            param.Add("ColumnFloat", last.ColumnFloat);
            param.Add("ColumnInt", last.ColumnInt);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithExpandoObjectAsDynamicParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = (dynamic)new ExpandoObject();

            // Add the parameters
            param.ColumnFloat = last.ColumnFloat;
            param.ColumnInt = last.ColumnInt;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", (object)param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithQueryGroupAsParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithQueryFieldsAsParameters()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new[]
            {
                new QueryField("ColumnFloat", last.ColumnFloat),
                new QueryField("ColumnInt", last.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat AND ColumnInt = @ColumnInt;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithQueryFieldAsParameter()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var param = new QueryField("ColumnFloat", last.ColumnFloat);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE ColumnFloat = @ColumnFloat;", param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryAsyncIfTheParameterAreInvalidTypeDictionaryObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var param = new Dictionary<string, int>();

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);", param).Result;
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);").Result;
            }
        }

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnTestSqlConnectionExecuteQueryAsyncIfTheParameterIsQueryFieldAndTheOperationIsNotEqualsToEqual()
        //{
        //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        //    {
        //        // Setup
        //        var param = new QueryField("Id", Operation.NotEqual, 1);

        //        // Act
        //        var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (Id = @Id);", param).Result;
        //    }
        //}

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnTestSqlConnectionExecuteQueryAsyncIfThereAreSqlStatementProblems()
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
