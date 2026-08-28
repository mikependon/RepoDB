using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vertica.Data.VerticaClient;
using RepoDb.Vertica.IntegrationTests.Setup;
using System.Threading.Tasks;

namespace RepoDb.Vertica.IntegrationTests.Operations
{
    /// <summary>
    /// The raw-SQL "ExecuteQueryMultiple"/"ExecuteQueryMultipleAsync" extension methods
    /// (RepoDb.Core's DbConnectionExtension.ExecuteQueryMultipleInternal) take the literal command
    /// text the caller wrote, hand it once to ExecuteReaderInternal(), and then step through
    /// additional result sets purely via IDataReader.NextResult(). That is exactly the classic
    /// "SELECT ...; SELECT ...;" pattern used by the SqlServer/MySqlConnector counterparts of this
    /// file - it relies on the *driver* accepting several statements batched into one command text
    /// and returning several result sets for a single execution.
    ///
    /// VerticaCommand does not support that at all: a single command text may contain exactly one SQL
    /// statement. This is a hard incompatibility, not a style difference, and it is corroborated by
    /// VerticaDbSetting.IsMultiStatementExecutable being hard-coded to false for this provider (see
    /// RepoDb.Vertica/DbSettings/VerticaDbSetting.cs) - the same flag that forces every batched
    /// fluent operation (InsertAll/MergeAll/UpdateAll) down to one round-trip per row.
    ///
    /// Unlike those fluent APIs, RepoDb.Core has no opportunity to rewrite raw SQL text the caller
    /// supplied into N separate round-trips - it does not parse or split the string. So there is no
    /// meaningful "ported" version of the SqlServer scenarios in this file: instead, the tests below
    /// assert the actual (and only) behavior a caller gets if they try the classic multi-statement
    /// raw-SQL pattern against Vertica - an exception - so this limitation is documented and
    /// regression-tested rather than silently unsupported.
    ///
    /// The high-level fluent connection.QueryMultiple&lt;T1, T2&gt;(...) API is unaffected by this -
    /// see Operations\QueryMultipleTest.cs and TransactionTests.cs - because RepoDb.Core already
    /// falls back to one round-trip per requested type when IsMultiStatementExecutable is false.
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
        public void TestVerticaConnectionExecuteQueryMultipleThrowsOnMultiStatementText()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new VerticaConnection(Database.ConnectionString);

            // Act & Assert: see the class-level remarks above - VerticaCommand rejects multiple
            // statements in a single command text outright, so the raw-SQL ExecuteQueryMultiple
            // API cannot be used this way on Vertica.
            Assert.Throws<VerticaException>(() =>
                connection.ExecuteQueryMultiple("SELECT * FROM \"CompleteTable\"; SELECT * FROM \"CompleteTable\""));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestVerticaConnectionExecuteQueryMultipleAsyncThrowsOnMultiStatementText()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new VerticaConnection(Database.ConnectionString);

            // Act & Assert: async counterpart of the same known Vertica limitation.
            await Assert.ThrowsAsync<VerticaException>(() =>
                connection.ExecuteQueryMultipleAsync("SELECT * FROM \"CompleteTable\"; SELECT * FROM \"CompleteTable\""));
        }

        #endregion
    }
}
