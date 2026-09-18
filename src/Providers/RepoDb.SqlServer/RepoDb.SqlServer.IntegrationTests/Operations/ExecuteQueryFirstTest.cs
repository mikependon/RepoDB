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
        public void TestSqlServerConnectionExecuteQueryFirstViaDynamics()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst("SELECT * FROM \"IdentityCompleteTable\" ORDER BY \"Id\" ASC;");

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExecuteQueryFirstViaDynamicsWithParameters()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id });

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQueryFirstViaDynamicsIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst("SELECT * FROM \"IdentityCompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQueryFirstViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<SqlException>(() => connection.ExecuteQueryFirst("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQueryFirstViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<SqlException>(() => connection.ExecuteQueryFirst("SELEC * FROM \"IdentityCompleteTable\";"));
            }
        }

        #endregion

        #region ExecuteQueryFirstAsync<dynamic>

        [TestMethod]
        public async Task TestSqlServerConnectionExecuteQueryFirstAsyncViaDynamics()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync("SELECT * FROM \"IdentityCompleteTable\" ORDER BY \"Id\" ASC;").ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionExecuteQueryFirstAsyncViaDynamicsWithParameters()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQueryFirstAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM \"IdentityCompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQueryFirstAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<SqlException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQueryFirstAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<SqlException>(async () => await connection.ExecuteQueryFirstAsync("SELEC * FROM \"IdentityCompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteQueryFirst<TEntity>

        [TestMethod]
        public void TestSqlServerConnectionExecuteQueryFirst()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\" ORDER BY \"Id\" ASC;");

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExecuteQueryFirstWithParameters()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQueryFirst<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQueryFirstIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\";"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQueryFirstIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;",
                    new { Id = -1 }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQueryFirstIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<SqlException>(() => connection.ExecuteQueryFirst<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionExecuteQueryFirstIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<SqlException>(() => connection.ExecuteQueryFirst<IdentityCompleteTable>("SELEC * FROM \"IdentityCompleteTable\";"));
            }
        }

        #endregion

        #region ExecuteQueryFirstAsync<TEntity>

        [TestMethod]
        public async Task TestSqlServerConnectionExecuteQueryFirstAsync()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\" ORDER BY \"Id\" ASC;").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionExecuteQueryFirstAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryFirstAsync<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQueryFirstAsyncIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQueryFirstAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;",
                    new { Id = -1 }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQueryFirstAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<SqlException>(async () => await connection.ExecuteQueryFirstAsync<IdentityCompleteTable>("SELECT * FROM \"IdentityCompleteTable\" WHERE \"Id\" = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionExecuteQueryFirstAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<SqlException>(async () => await connection.ExecuteQueryFirstAsync<IdentityCompleteTable>("SELEC * FROM \"IdentityCompleteTable\";").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
