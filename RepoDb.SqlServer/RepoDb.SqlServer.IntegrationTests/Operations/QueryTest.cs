using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using RepoDb.Extensions;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests.Operations
{
    [TestClass]
    public class QueryTest
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

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestSqlServerConnectionQueryViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<IdentityCompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaExpression()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<IdentityCompleteTable>(e => e.Id == table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaDynamic()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<IdentityCompleteTable>(new { table.Id }).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaQueryField()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<IdentityCompleteTable>(new QueryField("Id", table.Id)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaQueryFields()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<IdentityCompleteTable>(queryFields).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaQueryGroup()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<IdentityCompleteTable>(queryGroup).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryWithTop()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<IdentityCompleteTable>((object)null,
                    top: 2);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaPrimaryKeyWithHints()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<IdentityCompleteTable>(table.Id,
                    hints: SqlServerTableHints.NoLock).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerConnectionQueryAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<IdentityCompleteTable>(table.Id)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<IdentityCompleteTable>(e => e.Id == table.Id)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<IdentityCompleteTable>(new { table.Id })).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<IdentityCompleteTable>(new QueryField("Id", table.Id))).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<IdentityCompleteTable>(queryFields)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<IdentityCompleteTable>(queryGroup)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryAsyncWithTop()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryAsync<IdentityCompleteTable>((object)null,
                    top: 2);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryAsyncViaPrimaryKeyWithHints()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<IdentityCompleteTable>(table.Id,
                    hints: SqlServerTableHints.NoLock)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqlServerConnectionQueryViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityCompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityCompleteTable>(), new { table.Id }).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityCompleteTable>(), new QueryField("Id", table.Id)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityCompleteTable>(), queryFields).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityCompleteTable>(), queryGroup).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaTableNameWithTop()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    (object)null,
                    top: 2);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaTableNameViaPrimaryKeyWithHints()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    table.Id,
                    hints: SqlServerTableHints.NoLock).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerConnectionQueryAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), table.Id)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), new { table.Id })).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), new QueryField("Id", table.Id))).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), queryFields)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), queryGroup)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryAsyncViaTableNameWithTop()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    (object)null,
                    top: 2);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryAsyncViaTableNameViaPrimaryKeyWithHints()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    table.Id,
                    hints: SqlServerTableHints.NoLock)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        #endregion

        #endregion
    }
}
