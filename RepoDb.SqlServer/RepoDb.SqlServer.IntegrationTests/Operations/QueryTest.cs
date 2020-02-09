using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using RepoDb.Extensions;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System;
using System.Linq;

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
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(e => e.Id == table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(new { table.Id }).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(new QueryField("Id", table.Id)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(queryFields).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(queryGroup).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>((object)null,
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
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(table.Id,
                    hints: SqlServerTableHints.NoLock).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqlServerConnectionQueryAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryAsync<CompleteTable>(table.Id).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryAsync<CompleteTable>(e => e.Id == table.Id).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryAsync<CompleteTable>(new { table.Id }).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryAsync<CompleteTable>(new QueryField("Id", table.Id)).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryAsync<CompleteTable>(queryFields).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryAsync<CompleteTable>(queryGroup).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAsyncWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryAsync<CompleteTable>((object)null,
                    top: 2).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAsyncViaPrimaryKeyWithHints()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryAsync<CompleteTable>(table.Id,
                    hints: SqlServerTableHints.NoLock).Result.First();

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
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), queryFields).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), queryGroup).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryViaTableNameWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(),
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
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(),
                    table.Id,
                    hints: SqlServerTableHints.NoLock).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqlServerConnectionQueryAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAsyncViaTableNameWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null,
                    top: 2).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAsyncViaTableNameViaPrimaryKeyWithHints()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table.Id,
                    hints: SqlServerTableHints.NoLock).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        #endregion

        #endregion
    }
}
