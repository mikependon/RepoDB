using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS
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
        public void TestSqLiteConnectionDeleteAll()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.DeleteAll<SdsCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteAllViaPrimaryKeys()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                var primaryKeys = ClassExpression.GetEntitiesPropertyValues<SdsCompleteTable, object>(tables, e => e.Id);

                // Act
                var result = connection.DeleteAll<SdsCompleteTable>(primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        //[TestMethod]
        //public void TestSqLiteConnectionDeleteAllViaPrimaryKeysBeyondLimits()
        //{
        //    using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
        //    {
        //        // Setup
        //        var tables = Database.CreateCompleteTables(1000, connection);
        //        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        //        // Act
        //        var result = connection.DeleteAll<CompleteTable>(primaryKeys);

        //        // Assert
        //        Assert.AreEqual(tables.Count(), result);
        //    }
        //}

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqLiteConnectionDeleteAllAsync()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = await connection.DeleteAllAsync<SdsCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionDeleteAllAsyncViaPrimaryKeys()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                var primaryKeys = ClassExpression.GetEntitiesPropertyValues<SdsCompleteTable, object>(tables, e => e.Id);

                // Act
                var result = await connection.DeleteAllAsync<SdsCompleteTable>(primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        //[TestMethod]
        //public async Task TestSqLiteConnectionDeleteAllAsyncViaPrimaryKeysBeyondLimits()
        //{
        //    using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
        //    {
        //        // Setup
        //        var tables = Database.CreateCompleteTables(1000, connection);
        //        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        //        // Act
        //        var result = connection.DeleteAllAsync<CompleteTable>(primaryKeys);

        //        // Assert
        //        Assert.AreEqual(tables.Count(), result);
        //    }
        //}

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionDeleteAllViaTableName()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.DeleteAll(ClassMappedNameCache.Get<SdsCompleteTable>());

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionDeleteAllViaTableNameViaPrimaryKeys()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                var primaryKeys = ClassExpression.GetEntitiesPropertyValues<SdsCompleteTable, object>(tables, e => e.Id);

                // Act
                var result = connection.DeleteAll(ClassMappedNameCache.Get<SdsCompleteTable>(), primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        //[TestMethod]
        //public void TestSqLiteConnectionDeleteAllViaTableNameViaPrimaryKeysBeyondLimits()
        //{
        //    using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
        //    {
        //        // Setup
        //        var tables = Database.CreateCompleteTables(1000, connection);
        //        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        //        // Act
        //        var result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys);

        //        // Assert
        //        Assert.AreEqual(tables.Count(), result);
        //    }
        //}

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqLiteConnectionDeleteAllAsyncViaTableName()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>());

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionDeleteAllAsyncViaTableNameViaPrimaryKeys()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                var primaryKeys = ClassExpression.GetEntitiesPropertyValues<SdsCompleteTable, object>(tables, e => e.Id);

                // Act
                var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<SdsCompleteTable>(), primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        //[TestMethod]
        //public async Task TestSqLiteConnectionDeleteAllAsyncViaTableNameViaPrimaryKeysBeyondLimits()
        //{
        //    using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
        //    {
        //        // Setup
        //        var tables = Database.CreateCompleteTables(1000, connection);
        //        var primaryKeys = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables, e => e.Id);

        //        // Act
        //        var result = connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), primaryKeys);

        //        // Assert
        //        Assert.AreEqual(tables.Count(), result);
        //    }
        //}

        #endregion

        #endregion
    }
}
