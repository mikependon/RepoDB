using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Dynamic;
using System.Linq;
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
        public void TestSqlConnectionExecuteQueryAsyncWithParameterAsDynamic()
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithParameterAsExpandoObjectAsDynamic()
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    (object)param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithParameterAsExpandoObjectAsDictionary()
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithParameterAsDictionary()
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithParameterAsQueryFields()
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncWithParameterAsQueryGroup()
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

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
        public void TestSqlConnectionExecuteNonQueryAsyncWithParameterAsDynamic()
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithParameterAsExpandoObjectAsDynamic()
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    (object)param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithParameterAsExpandoObjectAsDictionary()
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithParameterAsDictionary()
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithParameterAsQueryFields()
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithParameterAsQueryGroup()
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

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
        public void TestSqlConnectionExecuteReaderAsyncWithParameterAsDynamic()
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result)
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithParameterAsExpandoObjectAsDynamic()
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    (object)param).Result)
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithParameterAsExpandoObjectAsDictionary()
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result)
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithParameterAsDictionary()
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result)
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt);",
                    param).Result)
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithParameterAsQueryFields()
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result)
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(1, result.Count());
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderAsyncWithParameterAsQueryGroup()
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result)
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
        public void TestSqlConnectionExecuteScalarAsyncWithParameterAsDynamic()
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithParameterAsExpandoObjectAsDynamic()
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    (object)param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithParameterAsExpandoObjectAsDictionary()
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithParameterAsDictionary()
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithParameterAsQueryFields()
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithParameterAsQueryGroup()
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param).Result;

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
        public void TestDbRepositoryExecuteQueryAsyncWithParameterAsDynamic()
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
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithParameterAsExpandoObjectAsDynamic()
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
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    (object)param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithParameterAsExpandoObjectAsDictionary()
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
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithParameterAsDictionary()
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
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithParameterAsQueryFields()
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
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result.Count());
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryAsyncWithParameterAsQueryGroup()
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
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

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
        public void TestDbRepositoryExecuteNonQueryAsyncWithParameterAsDynamic()
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
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncWithParameterAsExpandoObjectAsDynamic()
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
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    (object)param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncWithParameterAsExpandoObjectAsDictionary()
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
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncWithParameterAsDictionary()
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
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncWithParameterAsQueryFields()
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
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncWithParameterAsQueryGroup()
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
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit);",
                    param).Result;

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
        public void TestDbRepositoryExecuteScalarAsyncWithParameterAsDynamic()
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
                var result = repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithParameterAsExpandoObjectAsDynamic()
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
                var result = repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    (object)param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithParameterAsExpandoObjectAsDictionary()
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
                var result = repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithParameterAsDictionary()
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
                var result = repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var param = new QueryField("ColumnInt", 5);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithParameterAsQueryFields()
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
                var result = repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithParameterAsQueryGroup()
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
                var result = repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt = @ColumnInt) AND (ColumnBit = @ColumnBit); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        #endregion

        #endregion
    }
}
