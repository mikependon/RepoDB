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
        public void TestClickHouseConnectionExecuteQueryFirstViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public void TestClickHouseConnectionExecuteQueryFirstViaDynamicsWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQueryFirstViaDynamicsIfNoRowsFound()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst("SELECT * FROM `CompleteTable`;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQueryFirstViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<ClickHouseServerException>(() => connection.ExecuteQueryFirst("SELECT * FROM `CompleteTable` WHERE Id = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQueryFirstViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<ClickHouseServerException>(() => connection.ExecuteQueryFirst("SELEC * FROM `CompleteTable`;"));
            }
        }

        #endregion

        #region ExecuteQueryFirstAsync<dynamic>

        [TestMethod]
        public async Task TestClickHouseConnectionExecuteQueryFirstAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public async Task TestClickHouseConnectionExecuteQueryFirstAsyncViaDynamicsWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQueryFirstAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQueryFirstAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<ClickHouseServerException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM `CompleteTable` WHERE Id = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQueryFirstAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<ClickHouseServerException>(async () => await connection.ExecuteQueryFirstAsync("SELEC * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteQueryFirst<TEntity>

        [TestMethod]
        public void TestClickHouseConnectionExecuteQueryFirst()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM `CompleteTable` ORDER BY Id ASC;");

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionExecuteQueryFirstWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQueryFirstIfNoRowsFound()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM `CompleteTable`;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQueryFirstIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQueryFirstIfTheParametersAreNotDefined()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<ClickHouseServerException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionExecuteQueryFirstIfThereAreSqlStatementProblems()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<ClickHouseServerException>(() => connection.ExecuteQueryFirst<CompleteTable>("SELEC * FROM `CompleteTable`;"));
            }
        }

        #endregion

        #region ExecuteQueryFirstAsync<TEntity>

        [TestMethod]
        public async Task TestClickHouseConnectionExecuteQueryFirstAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM `CompleteTable` ORDER BY Id ASC;").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionExecuteQueryFirstAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQueryFirstAsyncIfNoRowsFound()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQueryFirstAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQueryFirstAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<ClickHouseServerException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELECT * FROM `CompleteTable` WHERE Id = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionExecuteQueryFirstAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<ClickHouseServerException>(async () => await connection.ExecuteQueryFirstAsync<CompleteTable>("SELEC * FROM `CompleteTable`;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
