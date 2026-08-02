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
    public class QueryAllTest
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
        public void TestOracleConnectionQueryAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var queryResult = connection.QueryAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestOracleConnectionQueryAllWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestOracleConnectionQueryAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act: typed via the mapped-name overload (rather than the untyped/dynamic one) so the
            // result can still go through Helper.AssertPropertiesEquality below.
            var queryResult = connection.QueryAll<CompleteTable>(ClassMappedNameCache.Get<CompleteTable>());

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestOracleConnectionQueryAllWithHintsThrowsNotSupportedException()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act/Assert: OracleDbSetting.AreTableHintsSupported is false - any non-null/non-whitespace
            // "hints" argument must throw rather than silently being ignored.
            Assert.Throws<System.NotSupportedException>(() => connection.QueryAll<CompleteTable>(hints: "NOLOCK"));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionQueryAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryAllAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var queryResult = await connection.QueryAllAsync<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var queryResult = await connection.QueryAllAsync<CompleteTable>(ClassMappedNameCache.Get<CompleteTable>());

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryAllAsyncWithHintsThrowsNotSupportedException()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act/Assert: OracleDbSetting.AreTableHintsSupported is false - any non-null/non-whitespace
            // "hints" argument must throw rather than silently being ignored.
            await Assert.ThrowsAsync<System.NotSupportedException>(() => connection.QueryAllAsync<CompleteTable>(hints: "NOLOCK"));
        }

        #endregion
    }
}
