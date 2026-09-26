#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;
using RepoDb.Exceptions;
using RepoDb.CockroachDB.IntegrationTests.Models;
using RepoDb.CockroachDB.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.CockroachDB.IntegrationTests.Operations
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
        public void TestCockroachDbConnectionExecuteQuerySingleViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id });

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionExecuteQuerySingleViaDynamicsWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC LIMIT 1;");

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleViaDynamicsIfNoRowsFound()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<CockroachDbException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<CockroachDbException>(() => connection.ExecuteQuerySingle("SELEC * FROM \"CompleteTable\";"));
            }
        }

        #endregion

        #region ExecuteQuerySingleAsync<dynamic>

        [TestMethod]
        public async Task TestCockroachDbConnectionExecuteQuerySingleAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionExecuteQuerySingleAsyncViaDynamicsWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC LIMIT 1;").ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleAsyncViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<CockroachDbException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<CockroachDbException>(async () => await connection.ExecuteQuerySingleAsync("SELEC * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteQuerySingle<TEntity>

        [TestMethod]
        public void TestCockroachDbConnectionExecuteQuerySingle()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionExecuteQuerySingleWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC LIMIT 1;");

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleIfNoRowsFound()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleIfTheParametersAreNotDefined()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<CockroachDbException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleIfThereAreSqlStatementProblems()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<CockroachDbException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELEC * FROM \"CompleteTable\";"));
            }
        }

        #endregion

        #region ExecuteQuerySingleAsync<TEntity>

        [TestMethod]
        public async Task TestCockroachDbConnectionExecuteQuerySingleAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionExecuteQuerySingleAsyncWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC LIMIT 1;").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleAsyncIfNoRowsFound()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleAsyncIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<CockroachDbException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestCockroachDbConnectionExecuteQuerySingleAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<CockroachDbException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELEC * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
