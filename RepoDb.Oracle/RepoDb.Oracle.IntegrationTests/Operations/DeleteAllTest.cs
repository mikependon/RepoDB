using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
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

        #region Sync

        [TestMethod]
        public void TestOracleConnectionDeleteAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.DeleteAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count(), result);
            Assert.AreEqual(0, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public void TestOracleConnectionDeleteAllWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = connection.DeleteAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestOracleConnectionDeleteAllViaPrimaryKeys()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var keysToDelete = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables.Take(5), e => e.Id);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.DeleteAll<CompleteTable>(keysToDelete);

            // Assert
            Assert.AreEqual(5, result);
            Assert.AreEqual(5, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public void TestOracleConnectionDeleteAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>());

            // Assert
            Assert.AreEqual(tables.Count(), result);
            Assert.AreEqual(0, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public void TestOracleConnectionDeleteAllViaTableNameViaPrimaryKeys()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var keysToDelete = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables.Take(5), e => e.Id);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>(), keysToDelete);

            // Assert
            Assert.AreEqual(5, result);
            Assert.AreEqual(5, connection.CountAll<CompleteTable>());
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionDeleteAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count(), result);
            Assert.AreEqual(0, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAllAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = await connection.DeleteAllAsync<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAllAsyncViaPrimaryKeys()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var keysToDelete = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables.Take(5), e => e.Id);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAllAsync<CompleteTable>(keysToDelete);

            // Assert
            Assert.AreEqual(5, result);
            Assert.AreEqual(5, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>());

            // Assert
            Assert.AreEqual(tables.Count(), result);
            Assert.AreEqual(0, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAllAsyncViaTableNameViaPrimaryKeys()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var keysToDelete = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables.Take(5), e => e.Id);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), keysToDelete);

            // Assert
            Assert.AreEqual(5, result);
            Assert.AreEqual(5, connection.CountAll<CompleteTable>());
        }

        #endregion
    }
}
