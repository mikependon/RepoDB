#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver;
using ClickHouse.Driver.ADO;
using RepoDb.Exceptions;
using RepoDb.ClickHouse.IntegrationTests.Models;
using RepoDb.ClickHouse.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.ClickHouse.IntegrationTests.Operations
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
        public void TestClickHouseConnectionExecuteQuerySingleViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionExecuteQuerySingleViaDynamicsWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle("SELECT * FROM `CompleteTable` ORDER BY Id ASC LIMIT 1;");

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleViaDynamicsIfNoRowsFound()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM `CompleteTable`;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle("SELECT * FROM `CompleteTable`;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<ClickHouseServerException>(() => connection.ExecuteQuerySingle("SELECT * FROM `CompleteTable` WHERE Id = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<ClickHouseServerException>(() => connection.ExecuteQuerySingle("SELEC * FROM `CompleteTable`;"));
            }
        }

        #endregion

        #region ExecuteQuerySingleAsync<dynamic>

        [TestMethod]
        public async Task TestClickHouseConnectionExecuteQuerySingleAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionExecuteQuerySingleAsyncViaDynamicsWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM `CompleteTable` ORDER BY Id ASC LIMIT 1;").ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleAsyncViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<ClickHouseServerException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM `CompleteTable` WHERE Id = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<ClickHouseServerException>(async () => await connection.ExecuteQuerySingleAsync("SELEC * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteQuerySingle<TEntity>

        [TestMethod]
        public void TestClickHouseConnectionExecuteQuerySingle()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionExecuteQuerySingleWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM `CompleteTable` ORDER BY Id ASC LIMIT 1;");

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleIfNoRowsFound()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM `CompleteTable`;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM `CompleteTable`;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleIfTheParametersAreNotDefined()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<ClickHouseServerException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleIfThereAreSqlStatementProblems()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<ClickHouseServerException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELEC * FROM `CompleteTable`;"));
            }
        }

        #endregion

        #region ExecuteQuerySingleAsync<TEntity>

        [TestMethod]
        public async Task TestClickHouseConnectionExecuteQuerySingleAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionExecuteQuerySingleAsyncWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM `CompleteTable` ORDER BY Id ASC LIMIT 1;").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleAsyncIfNoRowsFound()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleAsyncIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<ClickHouseServerException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQuerySingleAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<ClickHouseServerException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELEC * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
