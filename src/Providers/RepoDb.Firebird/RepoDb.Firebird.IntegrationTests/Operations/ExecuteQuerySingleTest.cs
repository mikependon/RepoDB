#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Exceptions;
using RepoDb.Firebird.IntegrationTests.Models;
using RepoDb.Firebird.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Firebird.IntegrationTests.Operations
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
        public void TestFirebirdConnectionExecuteQuerySingleViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                new { tables.Last().Id });

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.Last(), kvp);
        }

        [TestMethod]
        public void TestFirebirdConnectionExecuteQuerySingleViaDynamicsWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC FETCH FIRST 1 ROWS ONLY");

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.First(), kvp);
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleViaDynamicsIfNoRowsFound()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                new { Id = -1 }));
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleViaDynamicsIfTheParametersAreNotDefined()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            Assert.Throws<FbException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @MissingId"));
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleViaDynamicsIfThereAreSqlStatementProblems()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<FbException>(() => connection.ExecuteQuerySingle("SELEC * FROM \"CompleteTable\""));
        }

        #endregion

        #region ExecuteQuerySingleAsync<dynamic>

        [TestMethod]
        public async Task TestFirebirdConnectionExecuteQuerySingleAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                new { tables.Last().Id }).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.Last(), kvp);
        }

        [TestMethod]
        public async Task TestFirebirdConnectionExecuteQuerySingleAsyncViaDynamicsWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC FETCH FIRST 1 ROWS ONLY").ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.First(), kvp);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsFound()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                new { Id = -1 }).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleAsyncViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            await Assert.ThrowsAsync<FbException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @MissingId").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<FbException>(async () => await connection.ExecuteQuerySingleAsync("SELEC * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #region ExecuteQuerySingle<TEntity>

        [TestMethod]
        public void TestFirebirdConnectionExecuteQuerySingle()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(tables.Last().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.Last(), result);
        }

        [TestMethod]
        public void TestFirebirdConnectionExecuteQuerySingleWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC FETCH FIRST 1 ROWS ONLY");

            // Assert
            Assert.AreEqual(tables.First().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.First(), result);
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleIfNoRowsFound()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                new { Id = -1 }));
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleIfTheParametersAreNotDefined()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            Assert.Throws<FbException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @MissingId"));
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleIfThereAreSqlStatementProblems()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<FbException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELEC * FROM \"CompleteTable\""));
        }

        #endregion

        #region ExecuteQuerySingleAsync<TEntity>

        [TestMethod]
        public async Task TestFirebirdConnectionExecuteQuerySingleAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                new { tables.Last().Id }).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(tables.Last().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.Last(), result);
        }

        [TestMethod]
        public async Task TestFirebirdConnectionExecuteQuerySingleAsyncWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC FETCH FIRST 1 ROWS ONLY").ConfigureAwait(false);

            // Assert
            Assert.AreEqual(tables.First().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.First(), result);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleAsyncIfNoRowsFound()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                new { Id = -1 }).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleAsyncIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleAsyncIfTheParametersAreNotDefined()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            await Assert.ThrowsAsync<FbException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @MissingId").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQuerySingleAsyncIfThereAreSqlStatementProblems()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<FbException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELEC * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion
    }
}
