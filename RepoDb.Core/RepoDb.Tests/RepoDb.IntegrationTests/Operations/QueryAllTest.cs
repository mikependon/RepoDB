using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class QueryAllTest
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

        #region QueryAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionQueryAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>(orderBy: orderBy);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>(hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>(orderBy: orderBy,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        #endregion

        #region QueryAll<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionQueryAllWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<WithExtraFieldsIdentityTable>();

                // Assert
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        #endregion

        #region QueryAllAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionQueryAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<IdentityTable>().Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<IdentityTable>(orderBy: orderBy).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<IdentityTable>(hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<IdentityTable>(orderBy: orderBy,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.ElementAt(tables.IndexOf(table)));
                });
            }
        }

        #endregion

        #region QueryAllAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<WithExtraFieldsIdentityTable>().Result;

                // Assert
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        #endregion

        #region QueryAll(TableName)

        [TestMethod]
        public void TestSqlConnectionQueryAllViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: orderBy);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>(),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaTableNameWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: orderBy,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        #endregion

        #region QueryAllAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>()).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: orderBy).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncViaTableNameWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: orderBy,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(target.Id, item.Id);
                    Assert.AreEqual(target.RowGuid, item.RowGuid);
                    Assert.AreEqual(target.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(target.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(target.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(target.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(target.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(target.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        #endregion
    }
}
