using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    [TestClass]
    public class SumAllTest
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

        // NOTE: see AverageTest.cs/SumTest.cs for why "ColumnSmallInt" (not "ColumnInt") is used as the
        // aggregate target - summing the full-Int32-range "ColumnInt" risks a checked-arithmetic overflow.

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestOracleConnectionSumAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SumAll<CompleteTable>(e => e.ColumnSmallInt);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnSmallInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestOracleConnectionSumAllWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Oracle - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                Assert.Throws<NotSupportedException>(() =>
                    connection.SumAll<CompleteTable>(e => e.ColumnSmallInt, hints: "NOLOCK"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionSumAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAllAsync<CompleteTable>(e => e.ColumnSmallInt);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnSmallInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestOracleConnectionSumAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SumAll(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnSmallInt", typeof(short)));

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnSmallInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionSumAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnSmallInt", typeof(short)));

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnSmallInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #endregion
    }
}
