#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.MariaDbConnector;
using RepoDb.Exceptions;
using RepoDb.MariaDb.IntegrationTests.Models;
using RepoDb.MariaDb.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.MariaDb.IntegrationTests.Operations
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
        public void TestMariaDbConnectionExecuteQueryFirstViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
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
        public void TestMariaDbConnectionExecuteQueryFirstViaDynamicsWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnTestMariaDbConnectionExecuteQueryFirstViaDynamicsIfNoRowsFound()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst("SELECT * FROM `CompleteTable`;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMariaDbConnectionExecuteQueryFirstViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MariaDbException>(() => connection.ExecuteQueryFirst("SELECT * FROM `CompleteTable` WHERE Id = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMariaDbConnectionExecuteQueryFirstViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MariaDbException>(() => connection.ExecuteQueryFirst("SELEC * FROM `CompleteTable`;"));
            }
        }

        #endregion

        #region ExecuteQueryFirstAsync<dynamic>

        [TestMethod]
        public async Task TestMariaDbConnectionExecuteQueryFirstAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
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
        public async Task TestMariaDbConnectionExecuteQueryFirstAsyncViaDynamicsWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
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
        public async Task ThrowExceptionOnTestMariaDbConnectionExecuteQueryFirstAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMariaDbConnectionExecuteQueryFirstAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MariaDbException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM `CompleteTable` WHERE Id = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMariaDbConnectionExecuteQueryFirstAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MariaDbException>(async () => await connection.ExecuteQueryFirstAsync("SELEC * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteQueryFirst<TEntity>

        [TestMethod]
        public void TestMariaDbConnectionExecuteQueryFirst()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM `CompleteTable` ORDER BY Id ASC;");

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionExecuteQueryFirstWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnTestMariaDbConnectionExecuteQueryFirstIfNoRowsFound()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM `CompleteTable`;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMariaDbConnectionExecuteQueryFirstIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMariaDbConnectionExecuteQueryFirstIfTheParametersAreNotDefined()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MariaDbException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestMariaDbConnectionExecuteQueryFirstIfThereAreSqlStatementProblems()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MariaDbException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELEC * FROM `CompleteTable`;"));
            }
        }

        #endregion

        #region ExecuteQueryFirstAsync<TEntity>

        [TestMethod]
        public async Task TestMariaDbConnectionExecuteQueryFirstAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM `CompleteTable` ORDER BY Id ASC;").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionExecuteQueryFirstAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
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
        public async Task ThrowExceptionOnTestMariaDbConnectionExecuteQueryFirstAsyncIfNoRowsFound()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMariaDbConnectionExecuteQueryFirstAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMariaDbConnectionExecuteQueryFirstAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MariaDbException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestMariaDbConnectionExecuteQueryFirstAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MariaDbException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELEC * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
