using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
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
    /// ODP.NET's OracleCommand does not support that at all: a single command text may contain
    /// exactly one SQL statement (or one PL/SQL block); anything else - even after removing the
    /// trailing semicolon a lone Oracle statement requires be absent - fails immediately with
    /// ORA-00911 ("invalid character") as soon as the parser reaches the statement-separating
    /// semicolon. This is a hard incompatibility, not a style difference, and it is corroborated
    /// by OracleDbSetting.IsMultiStatementExecutable being hard-coded to false for this provider
    /// (see RepoDb.Oracle/DbSettings/OracleDbSetting.cs) - the same flag that forces every
    /// batched fluent operation (InsertAll/MergeAll/UpdateAll) down to one round-trip per row.
    ///
    /// Unlike those fluent APIs, RepoDb.Core has no opportunity to rewrite raw SQL text the
    /// caller supplied into N separate round-trips - it does not parse or split the string. So
    /// there is no meaningful "ported" version of the SqlServer scenarios in this file: instead,
    /// the tests below assert the actual (and only) behavior a caller gets if they try the
    /// classic multi-statement raw-SQL pattern against Oracle - an exception - so this limitation
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
        public void TestOracleConnectionExecuteQueryMultipleThrowsOnMultiStatementText()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act & Assert: see the class-level remarks above - ODP.NET rejects multiple
            // statements in a single command text outright, so the raw-SQL ExecuteQueryMultiple
            // API cannot be used this way on Oracle.
            Assert.Throws<OracleException>(() =>
                connection.ExecuteQueryMultiple("SELECT * FROM \"CompleteTable\"; SELECT * FROM \"CompleteTable\""));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionExecuteQueryMultipleAsyncThrowsOnMultiStatementText()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act & Assert: async counterpart of the same known Oracle limitation.
            await Assert.ThrowsAsync<OracleException>(() =>
                connection.ExecuteQueryMultipleAsync("SELECT * FROM \"CompleteTable\"; SELECT * FROM \"CompleteTable\""));
        }

        #endregion
    }
}
