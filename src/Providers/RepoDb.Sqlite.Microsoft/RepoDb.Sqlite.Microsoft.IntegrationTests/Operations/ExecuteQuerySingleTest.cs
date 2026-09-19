#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.Sqlite;
using RepoDb.Exceptions;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS
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
        public void TestSqLiteConnectionExecuteQuerySingleViaDynamics()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var result = connection.ExecuteQuerySingle("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionExecuteQuerySingleViaDynamicsWithLimit()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var result = connection.ExecuteQuerySingle("SELECT * FROM [MdsCompleteTable] ORDER BY Id ASC LIMIT 1;");

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleViaDynamicsIfNoRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM [MdsCompleteTable];"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleViaDynamicsIfNoRowsMatchTheFilter()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleViaDynamicsIfMultipleRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle("SELECT * FROM [MdsCompleteTable];"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                Assert.Throws<System.InvalidOperationException>(() => connection.ExecuteQuerySingle("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                Assert.Throws<SqliteException>(() => connection.ExecuteQuerySingle("SELEC * FROM [MdsCompleteTable];"));
            }
        }

        #endregion

        #region ExecuteQuerySingleAsync<dynamic>

        [TestMethod]
        public async Task TestSqLiteConnectionExecuteQuerySingleAsyncViaDynamics()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionExecuteQuerySingleAsyncViaDynamicsWithLimit()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var result = await connection.ExecuteQuerySingleAsync("SELECT * FROM [MdsCompleteTable] ORDER BY Id ASC LIMIT 1;").ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM [MdsCompleteTable];").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleAsyncViaDynamicsIfNoRowsMatchTheFilter()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleAsyncViaDynamicsIfMultipleRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM [MdsCompleteTable];").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                await Assert.ThrowsAsync<System.InvalidOperationException>(async () => await connection.ExecuteQuerySingleAsync("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                await Assert.ThrowsAsync<SqliteException>(async () => await connection.ExecuteQuerySingleAsync("SELEC * FROM [MdsCompleteTable];").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteQuerySingle<TEntity>

        [TestMethod]
        public void TestSqLiteConnectionExecuteQuerySingle()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var result = connection.ExecuteQuerySingle<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionExecuteQuerySingleWithLimit()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var result = connection.ExecuteQuerySingle<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable] ORDER BY Id ASC LIMIT 1;");

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleIfNoRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable];"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleIfNoRowsMatchTheFilter()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQuerySingle<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleIfMultipleRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.ExecuteQuerySingle<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable];"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleIfTheParametersAreNotDefined()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                Assert.Throws<System.InvalidOperationException>(() => connection.ExecuteQuerySingle<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                Assert.Throws<SqliteException>(() => connection.ExecuteQuerySingle<MdsCompleteTable>("SELEC * FROM [MdsCompleteTable];"));
            }
        }

        #endregion

        #region ExecuteQuerySingleAsync<TEntity>

        [TestMethod]
        public async Task TestSqLiteConnectionExecuteQuerySingleAsync()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var result = await connection.ExecuteQuerySingleAsync<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionExecuteQuerySingleAsyncWithLimit()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var result = await connection.ExecuteQuerySingleAsync<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable] ORDER BY Id ASC LIMIT 1;").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleAsyncIfNoRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable];").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleAsyncIfNoRowsMatchTheFilter()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQuerySingleAsync<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleAsyncIfMultipleRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.ExecuteQuerySingleAsync<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable];").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                await Assert.ThrowsAsync<System.InvalidOperationException>(async () => await connection.ExecuteQuerySingleAsync<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQuerySingleAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                await Assert.ThrowsAsync<SqliteException>(async () => await connection.ExecuteQuerySingleAsync<MdsCompleteTable>("SELEC * FROM [MdsCompleteTable];").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
