using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Db2.IntegrationTests.Setup;
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
    /// ODP.NET's Db2Command does not support that at all: a single command text may contain
    /// exactly one SQL statement (or one PL/SQL block); anything else - even after removing the
    /// trailing semicolon a lone Db2 statement requires be absent - fails immediately with
    /// ORA-00911 ("invalid character") as soon as the parser reaches the statement-separating
    /// semicolon. This is a hard incompatibility, not a style difference, and it is corroborated
    /// by Db2DbSetting.IsMultiStatementExecutable being hard-coded to false for this provider
    /// (see RepoDb.Db2/DbSettings/Db2DbSetting.cs) - the same flag that forces every
    /// batched fluent operation (InsertAll/MergeAll/UpdateAll) down to one round-trip per row.
    ///
    /// Unlike those fluent APIs, RepoDb.Core has no opportunity to rewrite raw SQL text the
    /// caller supplied into N separate round-trips - it does not parse or split the string. So
    /// there is no meaningful "ported" version of the SqlServer scenarios in this file: instead,
    /// the tests below assert the actual (and only) behavior a caller gets if they try the
    /// classic multi-statement raw-SQL pattern against Db2 - an exception - so this limitation
    /// is documented and regression-tested rather than silently unsupported.
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
        public void TestDb2ConnectionExecuteQueryMultipleThrowsOnMultiStatementText()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act & Assert: see the class-level remarks above - ODP.NET rejects multiple
            // statements in a single command text outright, so the raw-SQL ExecuteQueryMultiple
            // API cannot be used this way on Db2.
            Assert.Throws<Db2Exception>(() =>
                connection.ExecuteQueryMultiple("SELECT * FROM \"CompleteTable\"; SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteQueryMultipleThrowsOnMultiStatementTextWithAutomaticConversion()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new Db2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act & Assert: the multi-statement limitation is independent of ConversionType.
                Assert.Throws<Db2Exception>(() =>
                    connection.ExecuteQueryMultiple("SELECT * FROM \"CompleteTable\"; SELECT * FROM \"CompleteTable\""));
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQueryMultipleAsyncThrowsOnMultiStatementText()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act & Assert: async counterpart of the same known Db2 limitation.
            await Assert.ThrowsAsync<Db2Exception>(() =>
                connection.ExecuteQueryMultipleAsync("SELECT * FROM \"CompleteTable\"; SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQueryMultipleAsyncThrowsOnMultiStatementTextWithAutomaticConversion()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new Db2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act & Assert: the multi-statement limitation is independent of ConversionType.
                await Assert.ThrowsAsync<Db2Exception>(() =>
                    connection.ExecuteQueryMultipleAsync("SELECT * FROM \"CompleteTable\"; SELECT * FROM \"CompleteTable\""));
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        #endregion
    }
}
