#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.Exceptions;
using RepoDb.MySqlConnector.IntegrationTests.Models;
using RepoDb.MySqlConnector.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.MySqlConnector.IntegrationTests.Operations
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
        public void TestMySqlConnectionExecuteQuerySingleViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionExecuteQuerySingleViaDynamicsWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleViaDynamicsIfNoRowsFound()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM `CompleteTable`;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle("SELECT * FROM `CompleteTable`;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MySqlException>(() => connection.ExecuteQuerySingle("SELECT * FROM `CompleteTable` WHERE Id = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MySqlException>(() => connection.ExecuteQuerySingle("SELEC * FROM `CompleteTable`;"));
            }
        }

        #endregion

        #region ExecuteQuerySingleAsync<dynamic>

        [TestMethod]
        public async Task TestMySqlConnectionExecuteQuerySingleAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public async Task TestMySqlConnectionExecuteQuerySingleAsyncViaDynamicsWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleAsyncViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MySqlException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM `CompleteTable` WHERE Id = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MySqlException>(async () => await connection.ExecuteQuerySingleAsync("SELEC * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteQuerySingle<TEntity>

        [TestMethod]
        public void TestMySqlConnectionExecuteQuerySingle()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionExecuteQuerySingleWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM `CompleteTable` ORDER BY Id ASC LIMIT 1;");

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleIfNoRowsFound()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM `CompleteTable`;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM `CompleteTable`;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleIfTheParametersAreNotDefined()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MySqlException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleIfThereAreSqlStatementProblems()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MySqlException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELEC * FROM `CompleteTable`;"));
            }
        }

        #endregion

        #region ExecuteQuerySingleAsync<TEntity>

        [TestMethod]
        public async Task TestMySqlConnectionExecuteQuerySingleAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public async Task TestMySqlConnectionExecuteQuerySingleAsyncWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM `CompleteTable` ORDER BY Id ASC LIMIT 1;").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleAsyncIfNoRowsFound()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleAsyncIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MySqlException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQuerySingleAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MySqlException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELEC * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
