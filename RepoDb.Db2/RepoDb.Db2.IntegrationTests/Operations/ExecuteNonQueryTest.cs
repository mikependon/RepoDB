using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
{
    /// <summary>
    /// NOTE: this file previously claimed (copied, along with an Oracle-specific error code, from
    /// the Oracle provider this project was templated from) that Db2 rejects multiple SQL
    /// statements in a single command text under any circumstances. That assumption was never
    /// verified against a live Db2 instance and turned out to be wrong for read-only SELECT
    /// batches - see ExecuteQueryMultipleTest.cs, where a "SELECT ...; SELECT ...;" command text
    /// now demonstrably returns two correct result sets.
    ///
    /// <see cref="TestDb2ConnectionExecuteNonQueryWithMultiStatementText"/> below confirms the DML
    /// half of that same investigation: a multi-statement *write* batch also applies both
    /// statements in one round trip. That's what justified flipping
    /// <c>Db2DbSetting.IsMultiStatementExecutable</c> to <c>true</c>, which now lets
    /// InsertAll/MergeAll/UpdateAll batch multiple entities into a single round trip instead of
    /// one round trip per row - see Db2StatementBuilder.cs's CreateInsertAll/CreateMergeAll/
    /// CreateUpdateAll for the mechanism.
    /// </summary>
    [TestClass]
    public class ExecuteNonQueryTest
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
        public void TestDb2ConnectionExecuteNonQuery()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(0, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteNonQueryWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\"");

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteNonQueryWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act: bind variables are prefixed with ":" (not "@") for Db2.
            var result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(tables.Count - 1, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteNonQueryUpdate()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteNonQuery("UPDATE \"CompleteTable\" SET \"ColumnVarchar\" = :ColumnVarchar WHERE \"Id\" = :Id",
                new { ColumnVarchar = "Updated", tables.Last().Id });

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteNonQueryWithMultiStatementText()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var idsToDelete = tables.Take(2).Select(t => t.Id).ToArray();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act: diagnostic for the open investigation described in the class-level remarks -
            // does a DML multi-statement batch also apply both statements in a single round trip,
            // the way the read-only SELECT;SELECT case in ExecuteQueryMultipleTest.cs already
            // does? Literal IDs are inlined (rather than parameterized) to keep this test isolated
            // to just the multi-statement-execution question.
            var result = connection.ExecuteNonQuery(
                $"DELETE FROM \"CompleteTable\" WHERE \"Id\" = {idsToDelete[0]}; " +
                $"DELETE FROM \"CompleteTable\" WHERE \"Id\" = {idsToDelete[1]}");

            // Assert: if only the first statement actually ran, this comes back as
            // (tables.Count - 1) instead of (tables.Count - 2).
            Assert.AreEqual(tables.Count - 2, connection.CountAll<CompleteTable>());
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionExecuteNonQueryAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, result);
            Assert.AreEqual(0, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteNonQueryAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\"");

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteNonQueryAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(tables.Count - 1, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteNonQueryAsyncUpdate()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteNonQueryAsync("UPDATE \"CompleteTable\" SET \"ColumnVarchar\" = :ColumnVarchar WHERE \"Id\" = :Id",
                new { ColumnVarchar = "Updated", tables.Last().Id });

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteNonQueryAsyncWithMultiStatementText()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var idsToDelete = tables.Take(2).Select(t => t.Id).ToArray();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act: async counterpart of the same DML multi-statement diagnostic - see the sync
            // version above and the class-level remarks for what this is investigating.
            var result = await connection.ExecuteNonQueryAsync(
                $"DELETE FROM \"CompleteTable\" WHERE \"Id\" = {idsToDelete[0]}; " +
                $"DELETE FROM \"CompleteTable\" WHERE \"Id\" = {idsToDelete[1]}");

            // Assert
            Assert.AreEqual(tables.Count - 2, connection.CountAll<CompleteTable>());
        }

        #endregion
    }
}
