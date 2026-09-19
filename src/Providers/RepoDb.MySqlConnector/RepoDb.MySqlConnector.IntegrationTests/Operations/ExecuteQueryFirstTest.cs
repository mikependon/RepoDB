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
        public void TestMySqlConnectionExecuteQueryFirstViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst("SELECT * FROM `CompleteTable` ORDER BY Id ASC;");

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionExecuteQueryFirstViaDynamicsWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMySqlConnectionExecuteQueryFirstViaDynamicsIfNoRowsFound()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst("SELECT * FROM `CompleteTable`;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMySqlConnectionExecuteQueryFirstViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MySqlException>(() => connection.ExecuteQueryFirst("SELECT * FROM `CompleteTable` WHERE Id = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMySqlConnectionExecuteQueryFirstViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MySqlException>(() => connection.ExecuteQueryFirst("SELEC * FROM `CompleteTable`;"));
            }
        }

        #endregion

        #region ExecuteQueryFirstAsync<dynamic>

        [TestMethod]
        public async Task TestMySqlConnectionExecuteQueryFirstAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync("SELECT * FROM `CompleteTable` ORDER BY Id ASC;").ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public async Task TestMySqlConnectionExecuteQueryFirstAsyncViaDynamicsWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQueryFirstAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQueryFirstAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MySqlException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM `CompleteTable` WHERE Id = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQueryFirstAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MySqlException>(async () => await connection.ExecuteQueryFirstAsync("SELEC * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteQueryFirst<TEntity>

        [TestMethod]
        public void TestMySqlConnectionExecuteQueryFirst()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM `CompleteTable` ORDER BY Id ASC;");

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionExecuteQueryFirstWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMySqlConnectionExecuteQueryFirstIfNoRowsFound()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM `CompleteTable`;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMySqlConnectionExecuteQueryFirstIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMySqlConnectionExecuteQueryFirstIfTheParametersAreNotDefined()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MySqlException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMySqlConnectionExecuteQueryFirstIfThereAreSqlStatementProblems()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MySqlException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELEC * FROM `CompleteTable`;"));
            }
        }

        #endregion

        #region ExecuteQueryFirstAsync<TEntity>

        [TestMethod]
        public async Task TestMySqlConnectionExecuteQueryFirstAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM `CompleteTable` ORDER BY Id ASC;").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public async Task TestMySqlConnectionExecuteQueryFirstAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQueryFirstAsyncIfNoRowsFound()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQueryFirstAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQueryFirstAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MySqlException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMySqlConnectionExecuteQueryFirstAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MySqlException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELEC * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
