using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
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
        public void TestDb2ConnectionTruncate()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new Db2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.Truncate<CompleteTable>();
                var countResult = connection.CountAll<CompleteTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestDb2ConnectionTruncateWithAutomaticConversion()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new Db2Connection(Database.ConnectionString))
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
        public async Task TestDb2ConnectionTruncateAsync()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new Db2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.TruncateAsync<CompleteTable>();
                var countResult = connection.CountAll<CompleteTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionTruncateAsyncWithAutomaticConversion()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new Db2Connection(Database.ConnectionString))
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
        public void TestDb2ConnectionTruncateViaTableName()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new Db2Connection(Database.ConnectionString))
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
        public async Task TestDb2ConnectionTruncateAsyncViaTableName()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new Db2Connection(Database.ConnectionString))
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
