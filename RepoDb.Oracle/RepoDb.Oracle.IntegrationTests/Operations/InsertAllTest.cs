using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Extensions;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    [TestClass]
    public class InsertAllTest
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
        public void TestOracleConnectionInsertAll()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.InsertAll<CompleteTable>(tables);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            Assert.IsTrue(tables.All(table => table.Id > 0));

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestOracleConnectionInsertAllViaTableName()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act: the mapped-name overload still returns typed CompleteTable rows once queried back below,
            // so this is a genuine additional scenario rather than a re-run of the test above.
            var result = connection.InsertAll(ClassMappedNameCache.Get<CompleteTable>(), tables);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionInsertAllAsync()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.InsertAllAsync<CompleteTable>(tables);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            Assert.IsTrue(tables.All(table => table.Id > 0));

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestOracleConnectionInsertAllAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(), tables);

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        #endregion
    }
}
