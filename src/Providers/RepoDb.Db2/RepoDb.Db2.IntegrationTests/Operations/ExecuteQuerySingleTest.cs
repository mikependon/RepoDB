#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Exceptions;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
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
        public void TestDb2ConnectionExecuteQuerySingleViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.Last().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.Last(), kvp);
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteQuerySingleViaDynamicsWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC FETCH FIRST 1 ROWS ONLY");

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.First().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.First(), kvp);
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleViaDynamicsIfNoRowsFound()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { Id = -1 }));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleViaDynamicsIfTheParametersAreNotDefined()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            Assert.Throws<DB2Exception>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :MissingId"));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleViaDynamicsIfThereAreSqlStatementProblems()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<DB2Exception>(() => connection.ExecuteQuerySingle("SELEC * FROM \"CompleteTable\""));
        }

        #endregion

        #region ExecuteQuerySingleAsync<dynamic>

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQuerySingleAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id }).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.Last().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.Last(), kvp);
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQuerySingleAsyncViaDynamicsWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC FETCH FIRST 1 ROWS ONLY").ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.First().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.First(), kvp);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsFound()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { Id = -1 }).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleAsyncViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            await Assert.ThrowsAsync<DB2Exception>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :MissingId").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<DB2Exception>(async () => await connection.ExecuteQuerySingleAsync("SELEC * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #region ExecuteQuerySingle<TEntity>

        [TestMethod]
        public void TestDb2ConnectionExecuteQuerySingle()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(tables.Last().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.Last(), result);
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteQuerySingleWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC FETCH FIRST 1 ROWS ONLY");

            // Assert
            Assert.AreEqual(tables.First().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.First(), result);
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleIfNoRowsFound()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { Id = -1 }));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleIfTheParametersAreNotDefined()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            Assert.Throws<DB2Exception>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :MissingId"));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleIfThereAreSqlStatementProblems()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<DB2Exception>(() => connection.ExecuteQuerySingle<CompleteTable>("SELEC * FROM \"CompleteTable\""));
        }

        #endregion

        #region ExecuteQuerySingleAsync<TEntity>

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQuerySingleAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id }).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(tables.Last().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.Last(), result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQuerySingleAsyncWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC FETCH FIRST 1 ROWS ONLY").ConfigureAwait(false);

            // Assert
            Assert.AreEqual(tables.First().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.First(), result);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleAsyncIfNoRowsFound()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { Id = -1 }).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleAsyncIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleAsyncIfTheParametersAreNotDefined()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            await Assert.ThrowsAsync<DB2Exception>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :MissingId").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQuerySingleAsyncIfThereAreSqlStatementProblems()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<DB2Exception>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELEC * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion
    }
}
