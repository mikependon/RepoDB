using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
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
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.Query<CompleteTable>(e => e.Id == table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.Query<CompleteTable>(new { table.Id }).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.Query<CompleteTable>(new QueryField("Id", table.Id)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.Query<CompleteTable>(queryFields).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.Query<CompleteTable>(queryGroup).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestQueryAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAsync<CompleteTable>(table.Id).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAsync<CompleteTable>(e => e.Id == table.Id).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAsync<CompleteTable>(new { table.Id }).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAsync<CompleteTable>(new QueryField("Id", table.Id)).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAsync<CompleteTable>(queryFields).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAsync<CompleteTable>(queryGroup).Result.First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestQueryViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, queryResult);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestQueryAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup).Result.First();

                // Assert
                Helper.AssertMembersEquality(table, queryResult);
            }
        }

        #endregion

        #endregion
    }
}
