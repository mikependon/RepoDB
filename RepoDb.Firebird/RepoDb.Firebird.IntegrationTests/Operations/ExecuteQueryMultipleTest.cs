#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Firebird.IntegrationTests.Setup;
using System.Threading.Tasks;

namespace RepoDb.Firebird.IntegrationTests.Operations
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
    /// FbCommand does not support that at all: a single command text may contain exactly one SQL
    /// statement. This is a hard incompatibility, not a style difference, and it is corroborated by
    /// FirebirdDbSetting.IsMultiStatementExecutable being hard-coded to false for this provider (see
    /// RepoDb.Firebird/DbSettings/FirebirdDbSetting.cs) - the same flag that forces every batched
    /// fluent operation (InsertAll/MergeAll/UpdateAll) down to one round-trip per row.
    ///
    /// Unlike those fluent APIs, RepoDb.Core has no opportunity to rewrite raw SQL text the caller
    /// supplied into N separate round-trips - it does not parse or split the string. So there is no
    /// meaningful "ported" version of the SqlServer scenarios in this file: instead, the tests below
    /// assert the actual (and only) behavior a caller gets if they try the classic multi-statement
    /// raw-SQL pattern against Firebird - an exception - so this limitation is documented and
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
        public void TestFirebirdConnectionExecuteQueryMultipleThrowsOnMultiStatementText()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act & Assert: see the class-level remarks above - FbCommand rejects multiple
            // statements in a single command text outright, so the raw-SQL ExecuteQueryMultiple
            // API cannot be used this way on Firebird.
            Assert.Throws<FbException>(() =>
                connection.ExecuteQueryMultiple("SELECT * FROM \"CompleteTable\"; SELECT * FROM \"CompleteTable\""));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestFirebirdConnectionExecuteQueryMultipleAsyncThrowsOnMultiStatementText()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act & Assert: async counterpart of the same known Firebird limitation.
            await Assert.ThrowsAsync<FbException>(() =>
                connection.ExecuteQueryMultipleAsync("SELECT * FROM \"CompleteTable\"; SELECT * FROM \"CompleteTable\""));
        }

        #endregion
    }
}
