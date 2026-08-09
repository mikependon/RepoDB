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
    public class MergeTest
    {
        private static readonly Random m_random = new();

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

        // See UpdateTest.UpdateCompleteTableProperties for why this sticks to plain numeric/string columns.
        private static void UpdateCompleteTableProperties(CompleteTable table)
        {
            table.ColumnVarchar = $"Merged-{m_random.Next(int.MaxValue)}";
            table.ColumnInt = m_random.Next(int.MinValue, int.MaxValue);
            table.ColumnNumber = Math.Round(Convert.ToDecimal(m_random.NextDouble() * 1000), 12);
        }

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionMergeForNewRow()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Merge<CompleteTable>(table);

            // Assert
            Assert.IsTrue(Convert.ToInt64(result) > 0);
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }

        [TestMethod]
        public void TestDb2ConnectionMergeForNewRowWithAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = connection.Merge<CompleteTable>(table);

                // Assert
                Assert.IsTrue(Convert.ToInt64(result) > 0);
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestDb2ConnectionMergeForExistingRow()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Merge<CompleteTable>(table);

            // Assert
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            Assert.AreEqual(table.Id, Convert.ToInt32(result));

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }

        [TestMethod]
        public void TestDb2ConnectionMergeForExistingRowWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Merge<CompleteTable>(table,
                qualifiers: qualifiers);

            // Assert
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            Assert.AreEqual(table.Id, Convert.ToInt32(result));

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionMergeAsyncForNewRow()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.MergeAsync<CompleteTable>(table);

            // Assert
            Assert.IsTrue(Convert.ToInt64(result) > 0);
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }

        [TestMethod]
        public async Task TestDb2ConnectionMergeAsyncForNewRowWithAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = await connection.MergeAsync<CompleteTable>(table);

                // Assert
                Assert.IsTrue(Convert.ToInt64(result) > 0);
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionMergeAsyncForExistingRow()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.MergeAsync<CompleteTable>(table);

            // Assert
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            Assert.AreEqual(table.Id, Convert.ToInt32(result));

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }

        [TestMethod]
        public async Task TestDb2ConnectionMergeAsyncForExistingRowWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.MergeAsync<CompleteTable>(table,
                qualifiers: qualifiers);

            // Assert
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            Assert.AreEqual(table.Id, Convert.ToInt32(result));

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionMergeViaTableNameForNewRow()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                table);

            // Assert
            Assert.IsTrue(Convert.ToInt64(result) > 0);
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }

        [TestMethod]
        public void TestDb2ConnectionMergeViaTableNameForExistingRow()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                table);

            // Assert
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            Assert.AreEqual(table.Id, Convert.ToInt32(result));

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }

        [TestMethod]
        public void TestDb2ConnectionMergeViaTableNameForExistingRowWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                table,
                qualifiers: qualifiers);

            // Assert
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            Assert.AreEqual(table.Id, Convert.ToInt32(result));

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionMergeAsyncViaTableNameForNewRow()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                table);

            // Assert
            Assert.IsTrue(Convert.ToInt64(result) > 0);
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }

        [TestMethod]
        public async Task TestDb2ConnectionMergeAsyncViaTableNameForExistingRow()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                table);

            // Assert
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            Assert.AreEqual(table.Id, Convert.ToInt32(result));

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }

        [TestMethod]
        public async Task TestDb2ConnectionMergeAsyncViaTableNameForExistingRowWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            UpdateCompleteTableProperties(table);

            // Act
            var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                table,
                qualifiers: qualifiers);

            // Assert
            Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            Assert.AreEqual(table.Id, Convert.ToInt32(result));

            // Act
            var queryResult = connection.Query<CompleteTable>(result);

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult.First());
        }

        #endregion

        #endregion

        #region Hints

        [TestMethod]
        public void TestDb2ConnectionMergeWithHintsThrows()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act/Assert: AreTableHintsSupported = false for Db2 - BaseStatementBuilder.GuardHints
            // throws for any non-null/non-whitespace hints, regardless of operation.
            Assert.Throws<System.NotSupportedException>(() =>
                connection.Merge<CompleteTable>(table, hints: "NOLOCK"));
        }

        [TestMethod]
        public async Task TestDb2ConnectionMergeAsyncWithHintsThrows()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act/Assert: AreTableHintsSupported = false for Db2 - BaseStatementBuilder.GuardHints
            // throws for any non-null/non-whitespace hints, regardless of operation.
            await Assert.ThrowsAsync<System.NotSupportedException>(() =>
                connection.MergeAsync<CompleteTable>(table, hints: "NOLOCK"));
        }

        #endregion
    }
}
