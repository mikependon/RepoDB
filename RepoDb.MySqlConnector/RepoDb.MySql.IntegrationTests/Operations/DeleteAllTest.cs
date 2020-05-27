using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using RepoDb.MySql.IntegrationTests.Models;
using RepoDb.MySql.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.MySql.IntegrationTests.Operations
{
    [TestClass]
    public class DeleteAllTest
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
        public void TestMySqlConnectionDeleteAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.DeleteAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionDeleteAllViaPrimaryKeys()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

            using (var connection = new MySqlConnection(Database.ConnectionString).EnsureOpen())
            {
                // Act
                var result = connection.DeleteAll<CompleteTable>(primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionDeleteAllViaPrimaryKeysBeyondLimits()
        {
            // Setup
            var tables = Database.CreateCompleteTables(5000);
            var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

            using (var connection = new MySqlConnection(Database.ConnectionString).EnsureOpen())
            {
                // Act
                var result = connection.DeleteAll<CompleteTable>(primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMySqlConnectionDeleteAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.DeleteAllAsync<CompleteTable>().Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionDeleteAllAsyncViaPrimaryKeys()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

            using (var connection = new MySqlConnection(Database.ConnectionString).EnsureOpen())
            {
                // Act
                var result = connection.DeleteAllAsync<CompleteTable>(primaryKeys).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionDeleteAllAsyncViaPrimaryKeysBeyondLimits()
        {
            // Setup
            var tables = Database.CreateCompleteTables(5000);
            var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

            using (var connection = new MySqlConnection(Database.ConnectionString).EnsureOpen())
            {
                // Act
                var result = connection.DeleteAllAsync<CompleteTable>(primaryKeys).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestMySqlConnectionDeleteAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>());

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionDeleteAllViaTableNameViaPrimaryKeys()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

            using (var connection = new MySqlConnection(Database.ConnectionString).EnsureOpen())
            {
                // Act
                var result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionDeleteAllViaTableNameViaPrimaryKeysBeyondLimits()
        {
            // Setup
            var tables = Database.CreateCompleteTables(5000);
            var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

            using (var connection = new MySqlConnection(Database.ConnectionString).EnsureOpen())
            {
                // Act
                var result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMySqlConnectionDeleteAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>()).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionDeleteAllAsyncViaTableNameViaPrimaryKeys()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

            using (var connection = new MySqlConnection(Database.ConnectionString).EnsureOpen())
            {
                // Act
                var result = connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionDeleteAllAsyncViaTableNameViaPrimaryKeysBeyondLimits()
        {
            // Setup
            var tables = Database.CreateCompleteTables(5000);
            var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

            using (var connection = new MySqlConnection(Database.ConnectionString).EnsureOpen())
            {
                // Act
                var result = connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #endregion
    }
}
