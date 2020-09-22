using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Dynamic;
using System.Linq;
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
        public void TestSqlConnectionExecuteQueryAsyncWithArrayParameterAsDynamic()
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

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
        public void TestSqlConnectionExecuteQueryAsyncWithArrayParameterAsExpandoObjectAsDynamic()
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param).Result;

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
        public void TestSqlConnectionExecuteQueryAsyncWithArrayParameterAsExpandoObjectAsDictionary()
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

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
        public void TestSqlConnectionExecuteQueryAsyncWithArrayParameterAsDictionary()
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

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
        public void TestSqlConnectionExecuteQueryAsyncWithArrayParameterAsQueryField()
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

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
        public void TestSqlConnectionExecuteQueryAsyncWithArrayParameterAsQueryFields()
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

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
        public void TestSqlConnectionExecuteQueryAsyncWithArrayParameterAsQueryGroup()
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

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
        public void TestSqlConnectionExecuteNonQueryAsyncWithArrayParameterAsDynamic()
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithArrayParameterAsExpandoObjectAsDynamic()
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithArrayParameterAsExpandoObjectAsDictionary()
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithArrayParameterAsDictionary()
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithArrayParameterAsQueryField()
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithArrayParameterAsQueryFields()
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryAsyncWithArrayParameterAsQueryGroup()
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

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
        public void TestSqlConnectionExecuteReaderAsyncWithArrayParameterAsDynamic()
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result)
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
        public void TestSqlConnectionExecuteReaderAsyncWithArrayParameterAsExpandoObjectAsDynamic()
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param).Result)
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
        public void TestSqlConnectionExecuteReaderAsyncWithArrayParameterAsExpandoObjectAsDictionary()
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result)
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
        public void TestSqlConnectionExecuteReaderAsyncWithArrayParameterAsDictionary()
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result)
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
        public void TestSqlConnectionExecuteReaderAsyncWithArrayParameterAsQueryField()
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result)
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
        public void TestSqlConnectionExecuteReaderAsyncWithArrayParameterAsQueryFields()
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result)
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
        public void TestSqlConnectionExecuteReaderAsyncWithArrayParameterAsQueryGroup()
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result)
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
        public void TestSqlConnectionExecuteScalarAsyncWithArrayParameterAsDynamic()
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithArrayParameterAsExpandoObjectAsDynamic()
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    (object)param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithArrayParameterAsExpandoObjectAsDictionary()
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithArrayParameterAsDictionary()
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithArrayParameterAsQueryField()
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithArrayParameterAsQueryFields()
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncWithArrayParameterAsQueryGroup()
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param).Result;

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
        public void TestDbRepositoryExecuteQueryAsyncWithArrayParameterAsDynamic()
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
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

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
        public void TestDbRepositoryExecuteQueryAsyncWithArrayParameterAsExpandoObjectAsDynamic()
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
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param).Result;

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
        public void TestDbRepositoryExecuteQueryAsyncWithArrayParameterAsExpandoObjectAsDictionary()
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
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

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
        public void TestDbRepositoryExecuteQueryAsyncWithArrayParameterAsDictionary()
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
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

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
        public void TestDbRepositoryExecuteQueryAsyncWithArrayParameterAsQueryField()
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
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

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
        public void TestDbRepositoryExecuteQueryAsyncWithArrayParameterAsQueryFields()
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
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

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
        public void TestDbRepositoryExecuteQueryAsyncWithArrayParameterAsQueryGroup()
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
                var result = repository.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

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
        public void TestDbRepositoryExecuteNonQueryAsyncWithArrayParameterAsDynamic()
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
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncWithArrayParameterAsExpandoObjectAsDynamic()
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
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncWithArrayParameterAsExpandoObjectAsDictionary()
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
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncWithArrayParameterAsDictionary()
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
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncWithArrayParameterAsQueryField()
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
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncWithArrayParameterAsQueryFields()
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
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteNonQueryAsyncWithArrayParameterAsQueryGroup()
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
                var result = repository.ExecuteNonQueryAsync("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

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
        public void TestDbRepositoryExecuteScalarAsyncWithArrayParameterAsDynamic()
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
                var result = repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithArrayParameterAsExpandoObjectAsDynamic()
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
                var result = repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    (object)param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithArrayParameterAsExpandoObjectAsDictionary()
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
                var result = repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithArrayParameterAsDictionary()
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
                var result = repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithArrayParameterAsQueryField()
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
                var result = repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithArrayParameterAsQueryFields()
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
                var result = repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryExecuteScalarAsyncWithArrayParameterAsQueryGroup()
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
                var result = repository.ExecuteScalarAsync<int>("DELETE FROM [sc].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        #endregion

        #endregion
    }
}
