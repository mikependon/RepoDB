#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using RepoDb.Exceptions;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests.Operations
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
        public void TestSqlServerConnectionExecuteQuerySingleViaDynamics()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id });

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExecuteQuerySingleViaDynamicsWithTop()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle("SELECT TOP 1 * FROM \"IdentityCompleteTable\" ORDER BY \"Id\" ASC;");

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleViaDynamicsIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"IdentityCompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;",
                    new { Id = -1 }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"IdentityCompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<SqlException>(() => connection.ExecuteQuerySingle("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<SqlException>(() => connection.ExecuteQuerySingle("SELEC * FROM \"IdentityCompleteTable\";"));
            }
        }

        #endregion

        #region ExecuteQuerySingleAsync<dynamic>

        [TestMethod]
        public async Task TestSqlServerConnectionExecuteQuerySingleAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionExecuteQuerySingleAsyncViaDynamicsWithTop()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync("SELECT TOP 1 * FROM \"IdentityCompleteTable\" ORDER BY \"Id\" ASC;").ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"IdentityCompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;",
                    new { Id = -1 }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleAsyncViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"IdentityCompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<SqlException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<SqlException>(async () => await connection.ExecuteQuerySingleAsync("SELEC * FROM \"IdentityCompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteQuerySingle<TEntity>

        [TestMethod]
        public void TestSqlServerConnectionExecuteQuerySingle()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExecuteQuerySingleWithTop()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuerySingle<IdentityCompleteTable>("SELECT TOP 1 * FROM \"IdentityCompleteTable\" ORDER BY \"Id\" ASC;");

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;",
                    new { Id = -1 }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleIfMultipleRowsFound()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<SqlException>(() => connection.ExecuteQuerySingle<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<SqlException>(() => connection.ExecuteQuerySingle<IdentityCompleteTable>("SELEC * FROM \"IdentityCompleteTable\";"));
            }
        }

        #endregion

        #region ExecuteQuerySingleAsync<TEntity>

        [TestMethod]
        public async Task TestSqlServerConnectionExecuteQuerySingleAsync()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionExecuteQuerySingleAsyncWithTop()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQuerySingleAsync<IdentityCompleteTable>("SELECT TOP 1 * FROM \"IdentityCompleteTable\" ORDER BY \"Id\" ASC;").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleAsyncIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;",
                    new { Id = -1 }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleAsyncIfMultipleRowsFound()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<SqlException>(async () => await connection.ExecuteQuerySingleAsync<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQuerySingleAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<SqlException>(async () => await connection.ExecuteQuerySingleAsync<IdentityCompleteTable>("SELEC * FROM \"IdentityCompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
