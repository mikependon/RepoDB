using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    [TestClass]
    public class TruncateTest
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
        public void TestOracleConnectionTruncate()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Truncate<CompleteTable>();
                var countResult = connection.CountAll<CompleteTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionTruncateWithAutomaticConversion()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = connection.Truncate<CompleteTable>();
                    var countResult = connection.CountAll<CompleteTable>();

                    // Assert
                    Assert.AreEqual(0, countResult);
                }
                finally
                {
                    GlobalConfiguration.Options.ConversionType = ConversionType.Default;
                }
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionTruncateAsync()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.TruncateAsync<CompleteTable>();
                var countResult = connection.CountAll<CompleteTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionTruncateAsyncWithAutomaticConversion()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = await connection.TruncateAsync<CompleteTable>();
                    var countResult = connection.CountAll<CompleteTable>();

                    // Assert
                    Assert.AreEqual(0, countResult);
                }
                finally
                {
                    GlobalConfiguration.Options.ConversionType = ConversionType.Default;
                }
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestOracleConnectionTruncateViaTableName()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Truncate(ClassMappedNameCache.Get<CompleteTable>());
                var countResult = connection.CountAll<CompleteTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionTruncateAsyncViaTableName()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.TruncateAsync(ClassMappedNameCache.Get<CompleteTable>());
                var countResult = connection.CountAll<CompleteTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #endregion
    }
}
