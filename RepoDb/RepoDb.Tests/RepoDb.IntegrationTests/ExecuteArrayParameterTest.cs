using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
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
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.ToList().ForEach(item =>
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
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.ToList().ForEach(item =>
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
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.ToList().ForEach(item =>
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
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.ToList().ForEach(item =>
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
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.ToList().ForEach(item =>
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
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.ToList().ForEach(item =>
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
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param);

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.ToList().ForEach(item =>
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.ToList().ForEach(item =>
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.ToList().ForEach(item =>
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.ToList().ForEach(item =>
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.ToList().ForEach(item =>
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.ToList().ForEach(item =>
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.ToList().ForEach(item =>
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
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result.Count());
                result.ToList().ForEach(item =>
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
                var result = connection.ExecuteNonQuery("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
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
                var result = connection.ExecuteNonQuery("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
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
                var result = connection.ExecuteNonQuery("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
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
                var result = connection.ExecuteNonQuery("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
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
                var result = connection.ExecuteNonQuery("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
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
                var result = connection.ExecuteNonQuery("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
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
                var result = connection.ExecuteNonQuery("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
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
                var result = connection.ExecuteNonQueryAsync("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
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
                using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader, connection);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.ToList().ForEach(item =>
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
                using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param))
                {
                    // Extract the reader
                    var result = DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader, connection);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.ToList().ForEach(item =>
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
                using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader, connection);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.ToList().ForEach(item =>
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
                using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader, connection);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.ToList().ForEach(item =>
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
                using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader, connection);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.ToList().ForEach(item =>
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
                using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader, connection);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.ToList().ForEach(item =>
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
                using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param))
                {
                    // Extract the reader
                    var result = DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader, connection);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.ToList().ForEach(item =>
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result)
                {
                    // Extract the reader
                    var result = DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader, connection);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.ToList().ForEach(item =>
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    (object)param).Result)
                {
                    // Extract the reader
                    var result = DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader, connection);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.ToList().ForEach(item =>
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result)
                {
                    // Extract the reader
                    var result = DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader, connection);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.ToList().ForEach(item =>
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result)
                {
                    // Extract the reader
                    var result = DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader, connection);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.ToList().ForEach(item =>
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result)
                {
                    // Extract the reader
                    var result = DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader, connection);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.ToList().ForEach(item =>
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result)
                {
                    // Extract the reader
                    var result = DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader, connection);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.ToList().ForEach(item =>
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
                using (var reader = connection.ExecuteReaderAsync("SELECT * FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values));",
                    param).Result)
                {
                    // Extract the reader
                    var result = DataReaderConverter.ToEnumerable<IdentityTable>((DbDataReader)reader, connection);

                    // Assert
                    Assert.AreEqual(values.Count(), result.Count());
                    result.ToList().ForEach(item =>
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
                var result = connection.ExecuteScalar<int>("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
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
                var result = connection.ExecuteScalar<int>("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
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
                var result = connection.ExecuteScalar<int>("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
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
                var result = connection.ExecuteScalar<int>("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
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
                var result = connection.ExecuteScalar<int>("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
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
                var result = connection.ExecuteScalar<int>("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
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
                var result = connection.ExecuteScalar<int>("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
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
                var result = connection.ExecuteScalarAsync<int>("DELETE FROM [dbo].[IdentityTable] WHERE (ColumnInt IN (@Values)); SELECT @@ROWCOUNT;",
                    param).Result;

                // Assert
                Assert.AreEqual(values.Count(), result);
            }
        }

        #endregion

        #endregion

        #region DbRepository

        #region Sync

        #endregion

        #region Async

        #endregion

        #endregion
    }
}
