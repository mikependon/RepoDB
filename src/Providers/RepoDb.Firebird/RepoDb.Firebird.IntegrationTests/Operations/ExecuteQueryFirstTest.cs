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
    public class ExecuteQueryFirstTest
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

        #region ExecuteQueryFirst<dynamic>

        [TestMethod]
        public void TestFirebirdConnectionExecuteQueryFirstViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQueryFirst("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC");

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.First(), kvp);
        }

        [TestMethod]
        public void TestFirebirdConnectionExecuteQueryFirstViaDynamicsWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQueryFirst("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                new { tables.Last().Id });

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.Last(), kvp);
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQueryFirstViaDynamicsIfNoRowsFound()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst("SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQueryFirstViaDynamicsIfTheParametersAreNotDefined()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            Assert.Throws<FbException>(() => connection.ExecuteQueryFirst("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @MissingId"));
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQueryFirstViaDynamicsIfThereAreSqlStatementProblems()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<FbException>(() => connection.ExecuteQueryFirst("SELEC * FROM \"CompleteTable\""));
        }

        #endregion

        #region ExecuteQueryFirstAsync<dynamic>

        [TestMethod]
        public async Task TestFirebirdConnectionExecuteQueryFirstAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQueryFirstAsync("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC").ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.First(), kvp);
        }

        [TestMethod]
        public async Task TestFirebirdConnectionExecuteQueryFirstAsyncViaDynamicsWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQueryFirstAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                new { tables.Last().Id }).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.Last(), kvp);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQueryFirstAsyncViaDynamicsIfNoRowsFound()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQueryFirstAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            await Assert.ThrowsAsync<FbException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @MissingId").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQueryFirstAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<FbException>(async () => await connection.ExecuteQueryFirstAsync("SELEC * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #region ExecuteQueryFirst<TEntity>

        [TestMethod]
        public void TestFirebirdConnectionExecuteQueryFirst()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC");

            // Assert
            Assert.AreEqual(tables.First().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.First(), result);
        }

        [TestMethod]
        public void TestFirebirdConnectionExecuteQueryFirstWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(tables.Last().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.Last(), result);
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQueryFirstIfNoRowsFound()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQueryFirstIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                new { Id = -1 }));
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQueryFirstIfTheParametersAreNotDefined()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            Assert.Throws<FbException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @MissingId"));
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionExecuteQueryFirstIfThereAreSqlStatementProblems()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<FbException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELEC * FROM \"CompleteTable\""));
        }

        #endregion

        #region ExecuteQueryFirstAsync<TEntity>

        [TestMethod]
        public async Task TestFirebirdConnectionExecuteQueryFirstAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC").ConfigureAwait(false);

            // Assert
            Assert.AreEqual(tables.First().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.First(), result);
        }

        [TestMethod]
        public async Task TestFirebirdConnectionExecuteQueryFirstAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                new { tables.Last().Id }).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(tables.Last().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.Last(), result);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQueryFirstAsyncIfNoRowsFound()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQueryFirstAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id",
                new { Id = -1 }).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQueryFirstAsyncIfTheParametersAreNotDefined()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            await Assert.ThrowsAsync<FbException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @MissingId").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionExecuteQueryFirstAsyncIfThereAreSqlStatementProblems()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<FbException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELEC * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion
    }
}
