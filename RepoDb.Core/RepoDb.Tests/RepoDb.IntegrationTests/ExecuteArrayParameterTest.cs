using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Reflection;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class ExecuteArrayParameterTest
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
        public void TestSqlConnectionExecuteQueryWithArrayParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new { Values = values };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithArrayParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.Values = values;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithArrayParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("Values", values);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithArrayParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new Dictionary<string, object>
            {
                {"Values", values }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithArrayParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithArrayParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values).AsEnumerable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithArrayParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryGroup(new QueryField("Values", values));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        #endregion

        #region ExecuteQueryAsync

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithArrayParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new { Values = values };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithArrayParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.Values = values;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithArrayParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("Values", values);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithArrayParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new Dictionary<string, object>
            {
                {"Values", values }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithArrayParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithArrayParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values).AsEnumerable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithArrayParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryGroup(new QueryField("Values", values));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        #endregion

        #region ExecuteNonQuery

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithArrayParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new { Values = values };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithArrayParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.Values = values;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithArrayParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("Values", values);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithArrayParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new Dictionary<string, object>
            {
                {"Values", values }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithArrayParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithArrayParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values).AsEnumerable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryWithArrayParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryGroup(new QueryField("Values", values));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        #endregion

        #region ExecuteNonQueryAsync

        [TestMethod]
        public async Task TestSqlConnectionExecuteNonQueryAsyncWithArrayParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new { Values = values };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteNonQueryAsyncWithArrayParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.Values = values;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteNonQueryAsyncWithArrayParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("Values", values);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteNonQueryAsyncWithArrayParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new Dictionary<string, object>
            {
                {"Values", values }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteNonQueryAsyncWithArrayParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteNonQueryAsyncWithArrayParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values).AsEnumerable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteNonQueryAsyncWithArrayParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryGroup(new QueryField("Values", values));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        #endregion

        #region ExecuteReader

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithArrayParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new { Values = values };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.AsList().ForEach(item =>
                    {
                        Assert.IsTrue(values.Contains(item.ColumnInt));
                        Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                    });
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithArrayParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.Values = values;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.AsList().ForEach(item =>
                    {
                        Assert.IsTrue(values.Contains(item.ColumnInt));
                        Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                    });
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithArrayParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("Values", values);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.AsList().ForEach(item =>
                    {
                        Assert.IsTrue(values.Contains(item.ColumnInt));
                        Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                    });
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithArrayParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new Dictionary<string, object>
            {
                {"Values", values }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.AsList().ForEach(item =>
                    {
                        Assert.IsTrue(values.Contains(item.ColumnInt));
                        Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                    });
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithArrayParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.AsList().ForEach(item =>
                    {
                        Assert.IsTrue(values.Contains(item.ColumnInt));
                        Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                    });
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithArrayParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values).AsEnumerable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.AsList().ForEach(item =>
                    {
                        Assert.IsTrue(values.Contains(item.ColumnInt));
                        Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                    });
                }
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteReaderWithArrayParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryGroup(new QueryField("Values", values));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = connection.ExecuteReader("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.AsList().ForEach(item =>
                    {
                        Assert.IsTrue(values.Contains(item.ColumnInt));
                        Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                    });
                }
            }
        }

        #endregion

        #region ExecuteReaderAsync

        [TestMethod]
        public async Task TestSqlConnectionExecuteReaderAsyncWithArrayParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new { Values = values };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.AsList().ForEach(item =>
                    {
                        Assert.IsTrue(values.Contains(item.ColumnInt));
                        Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                    });
                }
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteReaderAsyncWithArrayParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.Values = values;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.AsList().ForEach(item =>
                    {
                        Assert.IsTrue(values.Contains(item.ColumnInt));
                        Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                    });
                }
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteReaderAsyncWithArrayParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("Values", values);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.AsList().ForEach(item =>
                    {
                        Assert.IsTrue(values.Contains(item.ColumnInt));
                        Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                    });
                }
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteReaderAsyncWithArrayParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new Dictionary<string, object>
            {
                {"Values", values }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.AsList().ForEach(item =>
                    {
                        Assert.IsTrue(values.Contains(item.ColumnInt));
                        Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                    });
                }
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteReaderAsyncWithArrayParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.AsList().ForEach(item =>
                    {
                        Assert.IsTrue(values.Contains(item.ColumnInt));
                        Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                    });
                }
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteReaderAsyncWithArrayParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values).AsEnumerable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.AsList().ForEach(item =>
                    {
                        Assert.IsTrue(values.Contains(item.ColumnInt));
                        Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                    });
                }
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteReaderAsyncWithArrayParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryGroup(new QueryField("Values", values));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReader.ToEnumerable<IdentityTable>((DbDataReader)reader);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.AsList().ForEach(item =>
                    {
                        Assert.IsTrue(values.Contains(item.ColumnInt));
                        Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                    });
                }
            }
        }

        #endregion

        #region ExecuteScalar

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithArrayParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new { Values = values };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithArrayParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.Values = values;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    (object)param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithArrayParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("Values", values);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithArrayParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new Dictionary<string, object>
            {
                {"Values", values }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithArrayParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithArrayParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values).AsEnumerable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarWithArrayParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryGroup(new QueryField("Values", values));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = connection.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        #endregion

        #region ExecuteScalarAsync

        [TestMethod]
        public async Task TestSqlConnectionExecuteScalarAsyncWithArrayParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new { Values = values };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteScalarAsyncWithArrayParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.Values = values;

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    (object)param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteScalarAsyncWithArrayParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("Values", values);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteScalarAsyncWithArrayParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new Dictionary<string, object>
            {
                {"Values", values }
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteScalarAsyncWithArrayParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteScalarAsyncWithArrayParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values).AsEnumerable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteScalarAsyncWithArrayParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryGroup(new QueryField("Values", values));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(connection.Insert(item)));

                // Act
                var result = await connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        #endregion

        #endregion

        #region DbRepository

        #region ExecuteQuery

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithArrayParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new { Values = values };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithArrayParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.Values = values;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithArrayParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("Values", values);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithArrayParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new Dictionary<string, object>
            {
                {"Values", values }
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithArrayParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithArrayParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values).AsEnumerable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteQueryWithArrayParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryGroup(new QueryField("Values", values));

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        #endregion

        #region ExecuteQueryAsync

        [TestMethod]
        public async Task TestDbRepositoryExecuteQueryAsyncWithArrayParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new { Values = values };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteQueryAsyncWithArrayParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.Values = values;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteQueryAsyncWithArrayParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("Values", values);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteQueryAsyncWithArrayParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new Dictionary<string, object>
            {
                {"Values", values }
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteQueryAsyncWithArrayParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteQueryAsyncWithArrayParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values).AsEnumerable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteQueryAsyncWithArrayParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryGroup(new QueryField("Values", values));

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.ColumnInt));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        #endregion

        #region ExecuteNonQuery

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithArrayParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new { Values = values };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithArrayParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.Values = values;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithArrayParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("Values", values);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithArrayParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new Dictionary<string, object>
            {
                {"Values", values }
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithArrayParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithArrayParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values).AsEnumerable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryWithArrayParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryGroup(new QueryField("Values", values));

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteNonQuery("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        #endregion

        #region ExecuteNonQueryAsync

        [TestMethod]
        public async Task TestDbRepositoryExecuteNonQueryAsyncWithArrayParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new { Values = values };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteNonQueryAsyncWithArrayParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.Values = values;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteNonQueryAsyncWithArrayParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("Values", values);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteNonQueryAsyncWithArrayParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new Dictionary<string, object>
            {
                {"Values", values }
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteNonQueryAsyncWithArrayParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteNonQueryAsyncWithArrayParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values).AsEnumerable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteNonQueryAsyncWithArrayParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryGroup(new QueryField("Values", values));

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        #endregion

        #region ExecuteScalar

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithArrayParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new { Values = values };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithArrayParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.Values = values;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    (object)param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithArrayParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("Values", values);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithArrayParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new Dictionary<string, object>
            {
                {"Values", values }
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithArrayParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithArrayParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values).AsEnumerable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarWithArrayParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryGroup(new QueryField("Values", values));

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = repository.ExecuteScalar<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        #endregion

        #region ExecuteScalarAsync

        [TestMethod]
        public async Task TestDbRepositoryExecuteScalarAsyncWithArrayParameterAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new { Values = values };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteScalarAsyncWithArrayParameterAsExpandoObjectAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = (dynamic)new ExpandoObject();

            // Set the properties
            param.Values = values;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    (object)param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteScalarAsyncWithArrayParameterAsExpandoObjectAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new ExpandoObject() as IDictionary<string, object>;

            // Set the properties
            param.Add("Values", values);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteScalarAsyncWithArrayParameterAsDictionary()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new Dictionary<string, object>
            {
                {"Values", values }
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteScalarAsyncWithArrayParameterAsQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values);

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteScalarAsyncWithArrayParameterAsQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryField("Values", values).AsEnumerable();

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryExecuteScalarAsyncWithArrayParameterAsQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new int?[] { 1, 3, 4, 8 };
            var param = new QueryGroup(new QueryField("Values", values));

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act
                tables.ForEach(item => item.Id = Convert.ToInt32(repository.Insert(item)));

                // Act
                var result = await repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        #endregion

        #endregion
    }
}
