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
    public class AverageAllTest
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

        // NOTE: see AverageTest.cs for why "ColumnSmallInt" (not "ColumnInt") is used as the aggregate target -
        // "ColumnInt" is generated across the full Int32 range, which risks a checked-arithmetic overflow when
        // summing several rows (which averaging does internally), both here and inside Oracle itself.

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestOracleConnectionAverageAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.AverageAll<CompleteTable>(e => e.ColumnSmallInt);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestOracleConnectionAverageAllWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Oracle - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                Assert.Throws<NotSupportedException>(() =>
                    connection.AverageAll<CompleteTable>(e => e.ColumnSmallInt, hints: "NOLOCK"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionAverageAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAllAsync<CompleteTable>(e => e.ColumnSmallInt);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestOracleConnectionAverageAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.AverageAll(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnSmallInt).First());

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionAverageAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnSmallInt).First());

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        #endregion

        #endregion
    }
}
