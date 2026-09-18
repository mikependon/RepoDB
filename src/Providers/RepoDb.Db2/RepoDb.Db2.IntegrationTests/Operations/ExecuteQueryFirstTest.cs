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
        public void TestDb2ConnectionExecuteQueryFirstViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQueryFirst("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC");

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.First().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.First(), kvp);
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteQueryFirstViaDynamicsWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQueryFirst("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.Last().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.Last(), kvp);
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQueryFirstViaDynamicsIfNoRowsFound()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst("SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQueryFirstViaDynamicsIfTheParametersAreNotDefined()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            Assert.Throws<DB2Exception>(() => connection.ExecuteQueryFirst("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :MissingId"));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQueryFirstViaDynamicsIfThereAreSqlStatementProblems()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<DB2Exception>(() => connection.ExecuteQueryFirst("SELEC * FROM \"CompleteTable\""));
        }

        #endregion

        #region ExecuteQueryFirstAsync<dynamic>

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQueryFirstAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQueryFirstAsync("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC").ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.First().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.First(), kvp);
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQueryFirstAsyncViaDynamicsWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQueryFirstAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id }).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(tables.Last().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.Last(), kvp);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQueryFirstAsyncViaDynamicsIfNoRowsFound()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQueryFirstAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            await Assert.ThrowsAsync<DB2Exception>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :MissingId").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQueryFirstAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<DB2Exception>(async () => await connection.ExecuteQueryFirstAsync("SELEC * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #region ExecuteQueryFirst<TEntity>

        [TestMethod]
        public void TestDb2ConnectionExecuteQueryFirst()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC");

            // Assert
            Assert.AreEqual(tables.First().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.First(), result);
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteQueryFirstWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(tables.Last().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.Last(), result);
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQueryFirstIfNoRowsFound()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\""));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQueryFirstIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { Id = -1 }));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQueryFirstIfTheParametersAreNotDefined()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            Assert.Throws<DB2Exception>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :MissingId"));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionExecuteQueryFirstIfThereAreSqlStatementProblems()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<DB2Exception>(() => connection.ExecuteQueryFirst<CompleteTable>("SELEC * FROM \"CompleteTable\""));
        }

        #endregion

        #region ExecuteQueryFirstAsync<TEntity>

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQueryFirstAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC").ConfigureAwait(false);

            // Assert
            Assert.AreEqual(tables.First().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.First(), result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteQueryFirstAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id }).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(tables.Last().Id, result.Id);
            Helper.AssertPropertiesEquality(tables.Last(), result);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQueryFirstAsyncIfNoRowsFound()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQueryFirstAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { Id = -1 }).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQueryFirstAsyncIfTheParametersAreNotDefined()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            // A bind name no other test uses: ODP.NET hangs when a statement text that was already
            // executed with binds is re-executed without them, so this text must stay unique.
            await Assert.ThrowsAsync<DB2Exception>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = :MissingId").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionExecuteQueryFirstAsyncIfThereAreSqlStatementProblems()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<DB2Exception>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELEC * FROM \"CompleteTable\"").ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion
    }
}
