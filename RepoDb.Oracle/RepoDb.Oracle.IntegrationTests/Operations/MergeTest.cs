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
    /// <summary>
    /// Also exercises OracleStatementBuilder's RETURNING/DBMS_SQL.RETURN_RESULT wrapping (see
    /// InsertTest), plus Oracle's own extra restriction that a MERGE RETURNING clause requires
    /// 12.2+ and exactly one affected row.
    /// </summary>
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
        public void TestOracleConnectionMergeForNewRow()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public void TestOracleConnectionMergeForNewRowWithAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public void TestOracleConnectionMergeForExistingRow()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public void TestOracleConnectionMergeForExistingRowWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task TestOracleConnectionMergeAsyncForNewRow()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task TestOracleConnectionMergeAsyncForNewRowWithAutomaticConversion()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task TestOracleConnectionMergeAsyncForExistingRow()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task TestOracleConnectionMergeAsyncForExistingRowWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public void TestOracleConnectionMergeViaTableNameForNewRow()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public void TestOracleConnectionMergeViaTableNameForExistingRow()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public void TestOracleConnectionMergeViaTableNameForExistingRowWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task TestOracleConnectionMergeAsyncViaTableNameForNewRow()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task TestOracleConnectionMergeAsyncViaTableNameForExistingRow()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task TestOracleConnectionMergeAsyncViaTableNameForExistingRowWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(int))
            };

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public void TestOracleConnectionMergeWithHintsThrows()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act/Assert: AreTableHintsSupported = false for Oracle - BaseStatementBuilder.GuardHints
            // throws for any non-null/non-whitespace hints, regardless of operation.
            Assert.Throws<System.NotSupportedException>(() =>
                connection.Merge<CompleteTable>(table, hints: "NOLOCK"));
        }

        #endregion
    }
}
