using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations
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
        public void TestQueryViaPrimaryKey()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryViaExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.Query<CompleteTable>(e => e.Id == table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.Query<CompleteTable>(new { table.Id }).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.Query<CompleteTable>(new QueryField("Id", table.Id)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };

                // Act
                var result = connection.Query<CompleteTable>(queryFields).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                var queryGroup = new QueryGroup(queryFields);

                // Act
                var result = connection.Query<CompleteTable>(queryGroup).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryWithTop()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Query<CompleteTable>((object)null,
                    top: 2);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionQueryWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                connection.Query<CompleteTable>((object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestQueryAsyncViaPrimaryKey()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.QueryAsync<CompleteTable>(table.Id).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.QueryAsync<CompleteTable>(e => e.Id == table.Id).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.QueryAsync<CompleteTable>(new { table.Id }).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.QueryAsync<CompleteTable>(new QueryField("Id", table.Id)).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };

                // Act
                var result = connection.QueryAsync<CompleteTable>(queryFields).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                var queryGroup = new QueryGroup(queryFields);

                // Act
                var result = connection.QueryAsync<CompleteTable>(queryGroup).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryAsyncWithTop()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.QueryAsync<CompleteTable>((object)null,
                    top: 2).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionQueryAsyncWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                connection.QueryAsync<CompleteTable>((object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestQueryViaTableNameViaPrimaryKey()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryViaTableNameViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryViaTableNameViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryViaTableNameViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), queryFields).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryViaTableNameViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                var queryGroup = new QueryGroup(queryFields);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), queryGroup).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryViaTableNameWithTop()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null,
                    top: 2);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionQueryViaTableNameWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                connection.Query(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestQueryAsyncViaTableNameViaPrimaryKey()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaTableNameViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaTableNameViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaTableNameViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaTableNameViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                var queryGroup = new QueryGroup(queryFields);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaTableNameWithTop()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null,
                    top: 2).Result;

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionQueryAsyncViaTableNameWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
