#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Exceptions;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    [TestClass]
    public class ExecuteQuerySingleTest
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

        #region ExecuteQuerySingle<dynamic>

        [TestMethod]
        public void TestOracleConnectionExecuteQuerySingleViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.Last().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.Last(), kvp);
        }

        [TestMethod]
        public void TestOracleConnectionExecuteQuerySingleViaDynamicsWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC FETCH FIRST 1 ROWS ONLY");

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.First().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.First(), kvp);
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionExecuteQuerySingleViaDynamicsIfNoRowsFound()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionExecuteQuerySingleViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { Id = -1 }));
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionExecuteQuerySingleViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionExecuteQuerySingleViaDynamicsIfTheParametersAreNotDefined()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            Assert.Throws<OracleException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :MissingId"));
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionExecuteQuerySingleViaDynamicsIfThereAreSqlStatementProblems()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<OracleException>(() => connection.ExecuteQuerySingle("SELEC * FROM \"CompleteTable\""));
        }

        #endregion

        #region ExecuteQuerySingleAsync<dynamic>

        [TestMethod]
        public async Task TestOracleConnectionExecuteQuerySingleAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id }).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.Last().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.Last(), kvp);
        }

        [TestMethod]
        public async Task TestOracleConnectionExecuteQuerySingleAsyncViaDynamicsWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC FETCH FIRST 1 ROWS ONLY").ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.First().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.First(), kvp);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsFound()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { Id = -1 }).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionExecuteQuerySingleAsyncViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionExecuteQuerySingleAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            await Assert.ThrowsAsync<OracleException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :MissingId").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionExecuteQuerySingleAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<OracleException>(async () => await connection.ExecuteQuerySingleAsync("SELEC * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #region ExecuteQuerySingle<TEntity>

        [TestMethod]
        public void TestOracleConnectionExecuteQuerySingle()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(tables.Last().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.Last(), result);
        }

        [TestMethod]
        public void TestOracleConnectionExecuteQuerySingleWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC FETCH FIRST 1 ROWS ONLY");

            // Assert
            Assert.AreEqual(tables.First().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.First(), result);
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionExecuteQuerySingleIfNoRowsFound()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionExecuteQuerySingleIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { Id = -1 }));
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionExecuteQuerySingleIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionExecuteQuerySingleIfTheParametersAreNotDefined()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            Assert.Throws<OracleException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :MissingId"));
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionExecuteQuerySingleIfThereAreSqlStatementProblems()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<OracleException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELEC * FROM \"CompleteTable\""));
        }

        #endregion

        #region ExecuteQuerySingleAsync<TEntity>

        [TestMethod]
        public async Task TestOracleConnectionExecuteQuerySingleAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id }).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(tables.Last().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.Last(), result);
        }

        [TestMethod]
        public async Task TestOracleConnectionExecuteQuerySingleAsyncWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC FETCH FIRST 1 ROWS ONLY").ConfigureAwait(false);

            // Assert
            Assert.AreEqual(tables.First().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.First(), result);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionExecuteQuerySingleAsyncIfNoRowsFound()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionExecuteQuerySingleAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { Id = -1 }).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionExecuteQuerySingleAsyncIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionExecuteQuerySingleAsyncIfTheParametersAreNotDefined()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            await Assert.ThrowsAsync<OracleException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :MissingId").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionExecuteQuerySingleAsyncIfThereAreSqlStatementProblems()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<OracleException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELEC * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion
    }
}
