using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests.Operations
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
        public void TestSqlServerConnectionDeleteAll()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.DeleteAll<IdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionDeleteAllViaPrimaryKeys()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);
            var primaryKeys = ClassExpression.GetEntitiesPropertyValues<IdentityCompleteTable, object>(tables, e => e.Id);

            using (var connection = new SqlConnection(Database.ConnectionString).EnsureOpen())
            {
                // Act
                var result = connection.DeleteAll<IdentityCompleteTable>(primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionDeleteAllViaPrimaryKeysBeyondLimits()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(5000);
            var primaryKeys = ClassExpression.GetEntitiesPropertyValues<IdentityCompleteTable, object>(tables, e => e.Id);

            using (var connection = new SqlConnection(Database.ConnectionString).EnsureOpen())
            {
                // Act
                var result = connection.DeleteAll<IdentityCompleteTable>(primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerConnectionDeleteAllAsync()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAllAsync<IdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionDeleteAllAsyncViaPrimaryKeys()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);
            var primaryKeys = ClassExpression.GetEntitiesPropertyValues<IdentityCompleteTable, object>(tables, e => e.Id);

            using (var connection = new SqlConnection(Database.ConnectionString).EnsureOpen())
            {
                // Act
                var result = await connection.DeleteAllAsync<IdentityCompleteTable>(primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionDeleteAllAsyncViaPrimaryKeysBeyondLimits()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(5000);
            var primaryKeys = ClassExpression.GetEntitiesPropertyValues<IdentityCompleteTable, object>(tables, e => e.Id);

            using (var connection = new SqlConnection(Database.ConnectionString).EnsureOpen())
            {
                // Act
                var result = await connection.DeleteAllAsync<IdentityCompleteTable>(primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqlServerConnectionDeleteAllViaTableName()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.DeleteAll(ClassMappedNameCache.Get<IdentityCompleteTable>());

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionDeleteAllViaTableNameViaPrimaryKeys()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);
            var primaryKeys = ClassExpression.GetEntitiesPropertyValues<IdentityCompleteTable, object>(tables, e => e.Id);

            using (var connection = new SqlConnection(Database.ConnectionString).EnsureOpen())
            {
                // Act
                var result = connection.DeleteAll(ClassMappedNameCache.Get<IdentityCompleteTable>(), primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionDeleteAllViaTableNameViaPrimaryKeysBeyondLimits()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(5000);
            var primaryKeys = ClassExpression.GetEntitiesPropertyValues<IdentityCompleteTable, object>(tables, e => e.Id);

            using (var connection = new SqlConnection(Database.ConnectionString).EnsureOpen())
            {
                // Act
                var result = connection.DeleteAll(ClassMappedNameCache.Get<IdentityCompleteTable>(), primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerConnectionDeleteAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<IdentityCompleteTable>());

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionDeleteAllAsyncViaTableNameViaPrimaryKeys()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);
            var primaryKeys = ClassExpression.GetEntitiesPropertyValues<IdentityCompleteTable, object>(tables, e => e.Id);

            using (var connection = new SqlConnection(Database.ConnectionString).EnsureOpen())
            {
                // Act
                var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionDeleteAllAsyncViaTableNameViaPrimaryKeysBeyondLimits()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(5000);
            var primaryKeys = ClassExpression.GetEntitiesPropertyValues<IdentityCompleteTable, object>(tables, e => e.Id);

            using (var connection = new SqlConnection(Database.ConnectionString).EnsureOpen())
            {
                // Act
                var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #endregion
    }
}
