using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    [TestClass]
    public class MaxAllTest
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
        public void TestOracleConnectionMaxAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MaxAll<CompleteTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestOracleConnectionMaxAllWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = connection.MaxAll<CompleteTable>(e => e.ColumnInt);

                    // Assert
                    Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
                }
                finally
                {
                    GlobalConfiguration.Options.ConversionType = ConversionType.Default;
                }
            }
        }

        [TestMethod]
        public void TestOracleConnectionMaxAllWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Oracle - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                Assert.Throws<NotSupportedException>(() =>
                    connection.MaxAll<CompleteTable>(e => e.ColumnInt, hints: "NOLOCK"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionMaxAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAllAsync<CompleteTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionMaxAllAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = await connection.MaxAllAsync<CompleteTable>(e => e.ColumnInt);

                    // Assert
                    Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
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
        public void TestOracleConnectionMaxAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MaxAll(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)));

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionMaxAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)));

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #endregion
    }
}
