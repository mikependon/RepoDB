#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Exceptions;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.PostgreSql.IntegrationTests.Operations
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
        public void TestPostgreSqlConnectionExecuteQuerySingleViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionExecuteQuerySingleViaDynamicsWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleViaDynamicsIfNoRowsFound()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<PostgresException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<PostgresException>(() => connection.ExecuteQuerySingle("SELEC * FROM \"CompleteTable\";"));
            }
        }

        #endregion

        #region ExecuteQuerySingleAsync<dynamic>

        [TestMethod]
        public async Task TestPostgreSqlConnectionExecuteQuerySingleAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public async Task TestPostgreSqlConnectionExecuteQuerySingleAsyncViaDynamicsWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public async Task ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleAsyncViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<PostgresException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<PostgresException>(async () => await connection.ExecuteQuerySingleAsync("SELEC * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteQuerySingle<TEntity>

        [TestMethod]
        public void TestPostgreSqlConnectionExecuteQuerySingle()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionExecuteQuerySingleWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC LIMIT 1;");

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleIfNoRowsFound()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleIfTheParametersAreNotDefined()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<PostgresException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleIfThereAreSqlStatementProblems()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<PostgresException>(() => connection.ExecuteQuerySingle<CompleteTable>("SELEC * FROM \"CompleteTable\";"));
            }
        }

        #endregion

        #region ExecuteQuerySingleAsync<TEntity>

        [TestMethod]
        public async Task TestPostgreSqlConnectionExecuteQuerySingleAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public async Task TestPostgreSqlConnectionExecuteQuerySingleAsyncWithLimit()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" ORDER BY \"Id\" ASC LIMIT 1;").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleAsyncIfNoRowsFound()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleAsyncIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<PostgresException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestPostgreSqlConnectionExecuteQuerySingleAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<PostgresException>(async () => await connection.ExecuteQuerySingleAsync<CompleteTable>("SELEC * FROM \"CompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
