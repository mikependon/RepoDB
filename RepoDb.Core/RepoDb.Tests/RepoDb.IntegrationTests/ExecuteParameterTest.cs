using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Reflection;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class ExecuteParameterTest
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

        #region DbConnection

        #region ExecuteQuery

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new
            {
                ColumnInt = 5,
                ColumnBit = true
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.ColumnInt = 5;
            param.ColumnBit = true;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    (object)param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("ColumnInt", 5);
            param.Add("ColumnBit", true);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new Dictionary<string, object>
            {
                { "ColumnInt", 5 },
                { "ColumnBit", true }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt);",
                    new QueryField("ColumnInt", 5));

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {

                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithParameterAsNullDecimal()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<decimal?>("select @value", new { value = (decimal?)null }).First();

                // Assert
                Assert.IsNull(data);
            }
        }

        #endregion

        #region ExecuteQueryAsync

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new
            {
                ColumnInt = 5,
                ColumnBit = true
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.ColumnInt = 5;
            param.ColumnBit = true;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    (object)param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("ColumnInt", 5);
            param.Add("ColumnBit", true);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new Dictionary<string, object>
            {
                { "ColumnInt", 5 },
                { "ColumnBit", true }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        #endregion

        #region ExecuteNonQuery

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new
            {
                ColumnInt = 5,
                ColumnBit = true
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.ColumnInt = 5;
            param.ColumnBit = true;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    (object)param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("ColumnInt", 5);
            param.Add("ColumnBit", true);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new Dictionary<string, object>
            {
                { "ColumnInt", 5 },
                { "ColumnBit", true }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var param = new QueryField("ColumnInt", 5);

                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        #endregion

        #region ExecuteNonQueryAsync

        [TestMethod]
        public async Task TestSqlConnectionExecuteNonQueryAsyncWithParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new
            {
                ColumnInt = 5,
                ColumnBit = true
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteNonQueryAsyncWithParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.ColumnInt = 5;
            param.ColumnBit = true;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    (object)param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteNonQueryAsyncWithParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("ColumnInt", 5);
            param.Add("ColumnBit", true);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteNonQueryAsyncWithParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new Dictionary<string, object>
            {
                { "ColumnInt", 5 },
                { "ColumnBit", true }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteNonQueryAsyncWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteNonQueryAsyncWithParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteNonQueryAsyncWithParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        #endregion

        #region ExecuteReader

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new
            {
                ColumnInt = 5,
                ColumnBit = true
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.ColumnInt = 5;
            param.ColumnBit = true;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    (object)param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("ColumnInt", 5);
            param.Add("ColumnBit", true);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new Dictionary<string, object>
            {
                { "ColumnInt", 5 },
                { "ColumnBit", true }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt);",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        #endregion

        #region ExecuteReaderAsync

        [TestMethod]
        public async Task TestSqlConnectionExecuteReaderAsyncWithParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new
            {
                ColumnInt = 5,
                ColumnBit = true
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteReaderAsyncWithParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.ColumnInt = 5;
            param.ColumnBit = true;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    (object)param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteReaderAsyncWithParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("ColumnInt", 5);
            param.Add("ColumnBit", true);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteReaderAsyncWithParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new Dictionary<string, object>
            {
                { "ColumnInt", 5 },
                { "ColumnBit", true }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteReaderAsyncWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt);",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteReaderAsyncWithParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteReaderAsyncWithParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        #endregion

        #region ExecuteScalar

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new
            {
                ColumnInt = 5,
                ColumnBit = true
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.ColumnInt = 5;
            param.ColumnBit = true;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    (object)param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("ColumnInt", 5);
            param.Add("ColumnBit", true);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new Dictionary<string, object>
            {
                { "ColumnInt", 5 },
                { "ColumnBit", true }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        #endregion

        #region ExecuteScalarAsync

        [TestMethod]
        public async Task TestSqlConnectionExecuteScalarAsyncWithParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new
            {
                ColumnInt = 5,
                ColumnBit = true
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteScalarAsyncWithParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.ColumnInt = 5;
            param.ColumnBit = true;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    (object)param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteScalarAsyncWithParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("ColumnInt", 5);
            param.Add("ColumnBit", true);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteScalarAsyncWithParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new Dictionary<string, object>
            {
                { "ColumnInt", 5 },
                { "ColumnBit", true }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteScalarAsyncWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteScalarAsyncWithParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteScalarAsyncWithParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        #endregion

        #endregion

        #region DbRepository

        #region ExecuteQuery

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new
            {
                ColumnInt = 5,
                ColumnBit = true
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.ColumnInt = 5;
            param.ColumnBit = true;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    (object)param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("ColumnInt", 5);
            param.Add("ColumnBit", true);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new Dictionary<string, object>
            {
                { "ColumnInt", 5 },
                { "ColumnBit", true }
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        #endregion

        #region ExecuteQueryAsync

        [TestMethod]
        public async Task TestDbRepositoryExecuteQueryAsyncWithParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new
            {
                ColumnInt = 5,
                ColumnBit = true
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteQueryAsyncWithParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.ColumnInt = 5;
            param.ColumnBit = true;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    (object)param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteQueryAsyncWithParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("ColumnInt", 5);
            param.Add("ColumnBit", true);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteQueryAsyncWithParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new Dictionary<string, object>
            {
                { "ColumnInt", 5 },
                { "ColumnBit", true }
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteQueryAsyncWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteQueryAsyncWithParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteQueryAsyncWithParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        #endregion

        #region ExecuteNonQuery

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new
            {
                ColumnInt = 5,
                ColumnBit = true
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.ColumnInt = 5;
            param.ColumnBit = true;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    (object)param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("ColumnInt", 5);
            param.Add("ColumnBit", true);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new Dictionary<string, object>
            {
                { "ColumnInt", 5 },
                { "ColumnBit", true }
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        #endregion

        #region ExecuteNonQueryAsync

        [TestMethod]
        public async Task TestDbRepositoryExecuteNonQueryAsyncWithParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new
            {
                ColumnInt = 5,
                ColumnBit = true
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteNonQueryAsyncWithParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.ColumnInt = 5;
            param.ColumnBit = true;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    (object)param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteNonQueryAsyncWithParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("ColumnInt", 5);
            param.Add("ColumnBit", true);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteNonQueryAsyncWithParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new Dictionary<string, object>
            {
                { "ColumnInt", 5 },
                { "ColumnBit", true }
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteNonQueryAsyncWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteNonQueryAsyncWithParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteNonQueryAsyncWithParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        #endregion

        #region ExecuteScalar

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new
            {
                ColumnInt = 5,
                ColumnBit = true
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.ColumnInt = 5;
            param.ColumnBit = true;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    (object)param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("ColumnInt", 5);
            param.Add("ColumnBit", true);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new Dictionary<string, object>
            {
                { "ColumnInt", 5 },
                { "ColumnBit", true }
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        #endregion

        #region ExecuteScalarAsync

        [TestMethod]
        public async Task TestDbRepositoryExecuteScalarAsyncWithParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new
            {
                ColumnInt = 5,
                ColumnBit = true
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteScalarAsyncWithParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.ColumnInt = 5;
            param.ColumnBit = true;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    (object)param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteScalarAsyncWithParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("ColumnInt", 5);
            param.Add("ColumnBit", true);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteScalarAsyncWithParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new Dictionary<string, object>
            {
                { "ColumnInt", 5 },
                { "ColumnBit", true }
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteScalarAsyncWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteScalarAsyncWithParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteScalarAsyncWithParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", 5),
                new QueryField("ColumnBit", true)
            });

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        #endregion

        #endregion
    }
}
