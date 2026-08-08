using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
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
        // summing several rows (which averaging does internally), both here and inside Db2 itself.

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionAverageAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new Db2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.AverageAll<CompleteTable>(e => e.ColumnSmallInt);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestDb2ConnectionAverageAllWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new Db2Connection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = connection.AverageAll<CompleteTable>(e => e.ColumnSmallInt);

                    // Assert
                    Assert.AreEqual(tables.Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
                }
                finally
                {
                    GlobalConfiguration.Options.ConversionType = ConversionType.Default;
                }
            }
        }

        [TestMethod]
        public void TestDb2ConnectionAverageAllWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new Db2Connection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Db2 - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                Assert.Throws<NotSupportedException>(() =>
                    connection.AverageAll<CompleteTable>(e => e.ColumnSmallInt, hints: "NOLOCK"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionAverageAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new Db2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAllAsync<CompleteTable>(e => e.ColumnSmallInt);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionAverageAllAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new Db2Connection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = await connection.AverageAllAsync<CompleteTable>(e => e.ColumnSmallInt);

                    // Assert
                    Assert.AreEqual(tables.Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
                }
                finally
                {
                    GlobalConfiguration.Options.ConversionType = ConversionType.Default;
                }
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionAverageAllAsyncWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new Db2Connection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Db2 - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                await Assert.ThrowsAsync<NotSupportedException>(() =>
                    connection.AverageAllAsync<CompleteTable>(e => e.ColumnSmallInt, hints: "NOLOCK"));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionAverageAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new Db2Connection(Database.ConnectionString))
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
        public async Task TestDb2ConnectionAverageAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new Db2Connection(Database.ConnectionString))
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
