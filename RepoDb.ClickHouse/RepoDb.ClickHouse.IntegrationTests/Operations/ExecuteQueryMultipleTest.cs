using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver;
using ClickHouse.Driver.ADO;
using RepoDb.ClickHouse.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.ClickHouse.IntegrationTests.Operations
{
    /// <summary>
    /// The raw-SQL "ExecuteQueryMultiple"/"ExecuteQueryMultipleAsync" extension methods
    /// (RepoDb.Core's DbConnectionExtension.ExecuteQueryMultipleInternal) take the literal
    /// command text the caller wrote, hand it once to ExecuteReaderInternal(), and then step
    /// through additional result sets purely via IDataReader.NextResult(). That is exactly the
    /// classic "SELECT ...; SELECT ...;" pattern used by the SqlServer/PostgreSql counterparts of
    /// this file - it relies on the *driver*/server accepting several statements batched into one
    /// command text and returning several result sets for a single execution.
    ///
    /// ClickHouse's HTTP interface does not support that at all: a single request may contain
    /// exactly one SQL statement; anything else fails immediately with a SYNTAX_ERROR
    /// ("Multi-statements are not allowed") as soon as the server reaches the statement-separating
    /// semicolon. This is a hard incompatibility, not a style difference, and it is corroborated
    /// by ClickHouseDbSetting.IsMultiStatementExecutable being hard-coded to false for this
    /// provider (see RepoDb.ClickHouse/DbSettings/ClickHouseDbSetting.cs) - the same flag that
    /// forces every batched fluent operation (InsertAll/MergeAll/UpdateAll) down to one round-trip
    /// per row/statement.
    ///
    /// Unlike those fluent APIs, RepoDb.Core has no opportunity to rewrite raw SQL text the
    /// caller supplied into N separate round-trips - it does not parse or split the string. So
    /// there is no meaningful "ported" version of the SqlServer scenarios in this file: instead,
    /// the tests below assert the actual (and only) behavior a caller gets if they try the
    /// classic multi-statement raw-SQL pattern against ClickHouse - an exception - so this
    /// limitation is documented and regression-tested rather than silently unsupported. See
    /// ExecuteNonQueryTest.cs for the same limitation's effect on ExecuteNonQuery(Async).
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
        public void TestClickHouseConnectionExecuteQueryMultipleThrowsOnMultiStatementText()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new ClickHouseConnection(Database.ConnectionString);

            // Act & Assert: see the class-level remarks above - ClickHouse's HTTP interface
            // rejects multiple statements in a single command text outright, so the raw-SQL
            // ExecuteQueryMultiple API cannot be used this way against ClickHouse.
            Assert.Throws<ClickHouseServerException>(() =>
                connection.ExecuteQueryMultiple(@"SELECT * FROM `CompleteTable`;
                    SELECT * FROM `CompleteTable`;"));
        }

        [TestMethod]
        public void TestClickHouseConnectionExecuteQueryMultipleThrowsOnMultiStatementTextWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new ClickHouseConnection(Database.ConnectionString);

            // Act & Assert: the multi-statement limitation is independent of parameterization.
            Assert.Throws<ClickHouseServerException>(() =>
                connection.ExecuteQueryMultiple(@"SELECT * FROM `CompleteTable` WHERE Id = @Id1;
                    SELECT * FROM `CompleteTable` WHERE Id = @Id2;",
                    new
                    {
                        Id1 = tables.First().Id,
                        Id2 = tables.Last().Id
                    }));
        }

        [TestMethod]
        public void TestClickHouseConnectionExecuteQueryMultipleThrowsOnMultiStatementTextWithSharedParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new ClickHouseConnection(Database.ConnectionString);

            // Act & Assert: the multi-statement limitation is independent of parameterization.
            Assert.Throws<ClickHouseServerException>(() =>
                connection.ExecuteQueryMultiple(@"SELECT * FROM `CompleteTable` WHERE Id = @Id;
                    SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = tables.Last().Id }));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseConnectionExecuteQueryMultipleAsyncThrowsOnMultiStatementText()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new ClickHouseConnection(Database.ConnectionString);

            // Act & Assert: async counterpart of the same known ClickHouse limitation.
            await Assert.ThrowsAsync<ClickHouseServerException>(() =>
                connection.ExecuteQueryMultipleAsync(@"SELECT * FROM `CompleteTable`;
                    SELECT * FROM `CompleteTable`;"));
        }

        [TestMethod]
        public async Task TestClickHouseConnectionExecuteQueryMultipleAsyncThrowsOnMultiStatementTextWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new ClickHouseConnection(Database.ConnectionString);

            // Act & Assert: the multi-statement limitation is independent of parameterization.
            await Assert.ThrowsAsync<ClickHouseServerException>(() =>
                connection.ExecuteQueryMultipleAsync(@"SELECT * FROM `CompleteTable` WHERE Id = @Id1;
                    SELECT * FROM `CompleteTable` WHERE Id = @Id2;",
                    new
                    {
                        Id1 = tables.First().Id,
                        Id2 = tables.Last().Id
                    }));
        }

        [TestMethod]
        public async Task TestClickHouseConnectionExecuteQueryMultipleAsyncThrowsOnMultiStatementTextWithSharedParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new ClickHouseConnection(Database.ConnectionString);

            // Act & Assert: the multi-statement limitation is independent of parameterization.
            await Assert.ThrowsAsync<ClickHouseServerException>(() =>
                connection.ExecuteQueryMultipleAsync(@"SELECT * FROM `CompleteTable` WHERE Id = @Id;
                    SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = tables.Last().Id }));
        }

        #endregion
    }
}
