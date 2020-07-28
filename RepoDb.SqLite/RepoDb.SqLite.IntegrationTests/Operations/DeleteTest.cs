using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations
{
    [TestClass]
    public class DeleteTest
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
        public void TestSqLiteConnectionDeleteWithoutExpression()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Delete<CompleteTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteViaPrimaryKey()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Delete<CompleteTable>(tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteViaDataEntity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Delete<CompleteTable>(tables.First());

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteViaExpression()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Delete<CompleteTable>(e => e.Id == tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteViaDynamic()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Delete<CompleteTable>(new { Id = tables.First().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteViaQueryField()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Delete<CompleteTable>(new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteViaQueryFields()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };

                // Act
                var result = connection.Delete<CompleteTable>(queryFields);

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteViaQueryGroup()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };
                var queryGroup = new QueryGroup(queryFields);

                // Act
                var result = connection.Delete<CompleteTable>(queryGroup);

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionDeleteAsyncWithoutExpression()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.DeleteAsync<CompleteTable>((object)null).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteAsyncViaPrimaryKey()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.DeleteAsync<CompleteTable>(tables.First().Id).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteAsyncViaDataEntity()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.DeleteAsync<CompleteTable>(tables.First()).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteAsyncViaExpression()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.DeleteAsync<CompleteTable>(e => e.Id == tables.First().Id).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteAsyncViaDynamic()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.DeleteAsync<CompleteTable>(new { Id = tables.First().Id }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteAsyncViaQueryField()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.DeleteAsync<CompleteTable>(new QueryField("Id", tables.First().Id)).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteAsyncViaQueryFields()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };

                // Act
                var result = connection.DeleteAsync<CompleteTable>(queryFields).Result;

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteAsyncViaQueryGroup()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };
                var queryGroup = new QueryGroup(queryFields);

                // Act
                var result = connection.DeleteAsync<CompleteTable>(queryGroup).Result;

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionDeleteViaTableNameWithoutExpression()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), (object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteViaTableNameViaPrimaryKey()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Delete<CompleteTable>(tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteViaTableNameViaDynamic()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteViaTableNameViaQueryField()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteViaTableNameViaQueryFields()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteViaTableNameViaQueryGroup()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };
                var queryGroup = new QueryGroup(queryFields);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryGroup);

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionDeleteAsyncViaTableNameWithoutExpression()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteAsyncViaTableNameViaPrimaryKey()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.DeleteAsync<CompleteTable>(tables.First().Id).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteAsyncViaTableNameViaDynamic()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id }).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteAsyncViaTableNameViaQueryField()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id)).Result;

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteAsyncViaTableNameViaQueryFields()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };

                // Act
                var result = connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields).Result;

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteAsyncViaTableNameViaQueryGroup()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };
                var queryGroup = new QueryGroup(queryFields);

                // Act
                var result = connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup).Result;

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        #endregion

        #endregion
    }
}
