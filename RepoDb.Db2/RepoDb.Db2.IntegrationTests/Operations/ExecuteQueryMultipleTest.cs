using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Db2.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
{
    /// <summary>
    /// The raw-SQL "ExecuteQueryMultiple"/"ExecuteQueryMultipleAsync" extension methods
    /// (RepoDb.Core's DbConnectionExtension.ExecuteQueryMultipleInternal) take the literal
    /// command text the caller wrote, hand it once to ExecuteReaderInternal(), and then step
    /// through additional result sets purely via IDataReader.NextResult(). That is exactly the
    /// classic "SELECT ...; SELECT ...;" pattern used by the SqlServer/PostgreSql counterparts of
    /// this file - it relies on the *driver* accepting several statements batched into one
    /// command text and returning several result sets for a single execution.
    ///
    /// UPDATE: this file previously asserted that Db2 rejects a multi-statement command text
    /// outright (an assumption inherited, along with an Oracle-specific error code, from the
    /// Oracle provider this project was originally templated from - see git history for
    /// "Initial template checkin - copied over from Oracle implementation"). That assumption was
    /// never actually verified against a live Db2 instance, and turned out to be wrong: the IBM
    /// Data Server .NET Provider *does* accept more than one SELECT statement in a single command
    /// text and steps through their result sets via NextResult(), same as SqlServer/PostgreSql.
    /// The tests below now verify that behavior is genuinely correct (two distinct, correctly
    /// populated result sets - not just "no exception").
    ///
    /// This does NOT by itself mean <c>Db2DbSetting.IsMultiStatementExecutable</c> should become
    /// true: that flag also governs whether batched DML (InsertAll/MergeAll/UpdateAll) can be sent
    /// as one multi-statement round-trip, which is a separate question from read-only SELECT
    /// batching and has not yet been confirmed - see ExecuteNonQueryTest.cs for that half of the
    /// investigation. Leave IsMultiStatementExecutable as-is until both halves are confirmed.
    /// </summary>
    [TestClass]
    public class ExecuteQueryMultipleTest
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
        public void TestDb2ConnectionExecuteQueryMultipleWithMultiStatementText()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act: contrary to this provider's original (Oracle-inherited, never-verified)
            // assumption, Db2 accepts two SELECT statements in a single command text. Assert on
            // the actual, distinct values from each result set - not merely that no exception was
            // thrown - to confirm this is genuine multi-resultset support and not, say, the driver
            // silently only running the first statement.
            using var extractor = connection.ExecuteQueryMultiple(
                "SELECT 1 AS \"Value\" FROM SYSIBM.SYSDUMMY1; SELECT 2 AS \"Value\" FROM SYSIBM.SYSDUMMY1");

            var first = extractor.Extract().Single();
            var second = extractor.Extract().Single();

            // Assert
            Assert.AreEqual(1, (int)first.Value);
            Assert.AreEqual(2, (int)second.Value);
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteQueryMultipleWithMultiStatementTextWithAutomaticConversion()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                using var extractor = connection.ExecuteQueryMultiple(
                    "SELECT 1 AS \"Value\" FROM SYSIBM.SYSDUMMY1; SELECT 2 AS \"Value\" FROM SYSIBM.SYSDUMMY1");

                var first = extractor.Extract().Single();
                var second = extractor.Extract().Single();

                // Assert
                Assert.AreEqual(1, (int)first.Value);
                Assert.AreEqual(2, (int)second.Value);
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQueryMultipleAsyncWithMultiStatementText()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            using var extractor = await connection.ExecuteQueryMultipleAsync(
                "SELECT 1 AS \"Value\" FROM SYSIBM.SYSDUMMY1; SELECT 2 AS \"Value\" FROM SYSIBM.SYSDUMMY1");

            var first = extractor.Extract().Single();
            var second = extractor.Extract().Single();

            // Assert
            Assert.AreEqual(1, (int)first.Value);
            Assert.AreEqual(2, (int)second.Value);
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQueryMultipleAsyncWithMultiStatementTextWithAutomaticConversion()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                using var extractor = await connection.ExecuteQueryMultipleAsync(
                    "SELECT 1 AS \"Value\" FROM SYSIBM.SYSDUMMY1; SELECT 2 AS \"Value\" FROM SYSIBM.SYSDUMMY1");

                var first = extractor.Extract().Single();
                var second = extractor.Extract().Single();

                // Assert
                Assert.AreEqual(1, (int)first.Value);
                Assert.AreEqual(2, (int)second.Value);
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        #endregion
    }
}
