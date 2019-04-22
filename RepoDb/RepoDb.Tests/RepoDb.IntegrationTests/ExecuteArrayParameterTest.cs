using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;

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

        #region Sync

        #region ExecuteQuery

        [TestMethod]
        public void TestSqlConnectionExecuteQueryArrayParameterAsDynamic()
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
        public void TestSqlConnectionExecuteQueryArrayParameterAsExpandoObjectAsDynamic()
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
        public void TestSqlConnectionExecuteQueryArrayParameterAsExpandoObjectAsDictionary()
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
        public void TestSqlConnectionExecuteQueryArrayParameterAsDictionary()
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
        public void TestSqlConnectionExecuteQueryArrayParameterAsQueryField()
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
        public void TestSqlConnectionExecuteQueryArrayParameterAsQueryFields()
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
        public void TestSqlConnectionExecuteQueryArrayParameterAsQueryGroup()
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
