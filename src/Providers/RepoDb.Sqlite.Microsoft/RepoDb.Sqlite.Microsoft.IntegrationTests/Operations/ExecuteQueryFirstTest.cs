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
        public void TestSqLiteConnectionExecuteQueryFirstViaDynamics()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var result = connection.ExecuteQueryFirst("SELECT * FROM [MdsCompleteTable] ORDER BY Id ASC;");

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionExecuteQueryFirstViaDynamicsWithParameters()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var result = connection.ExecuteQueryFirst("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQueryFirstViaDynamicsIfNoRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst("SELECT * FROM [MdsCompleteTable];"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQueryFirstViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                Assert.Throws<System.InvalidOperationException>(() => connection.ExecuteQueryFirst("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQueryFirstViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                Assert.Throws<SqliteException>(() => connection.ExecuteQueryFirst("SELEC * FROM [MdsCompleteTable];"));
            }
        }

        #endregion

        #region ExecuteQueryFirstAsync<dynamic>

        [TestMethod]
        public async Task TestSqLiteConnectionExecuteQueryFirstAsyncViaDynamics()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var result = await connection.ExecuteQueryFirstAsync("SELECT * FROM [MdsCompleteTable] ORDER BY Id ASC;").ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), kvp);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionExecuteQueryFirstAsyncViaDynamicsWithParameters()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var result = await connection.ExecuteQueryFirstAsync("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), kvp);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQueryFirstAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM [MdsCompleteTable];").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQueryFirstAsyncViaDynamicsIfTheParametersAreNotDefined()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                await Assert.ThrowsAsync<System.InvalidOperationException>(async () => await connection.ExecuteQueryFirstAsync("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQueryFirstAsyncViaDynamicsIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                await Assert.ThrowsAsync<SqliteException>(async () => await connection.ExecuteQueryFirstAsync("SELEC * FROM [MdsCompleteTable];").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region ExecuteQueryFirst<TEntity>

        [TestMethod]
        public void TestSqLiteConnectionExecuteQueryFirst()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var result = connection.ExecuteQueryFirst<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable] ORDER BY Id ASC;");

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionExecuteQueryFirstWithParameters()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var result = connection.ExecuteQueryFirst<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQueryFirstIfNoRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable];"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQueryFirstIfNoRowsMatchTheFilter()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                Assert.Throws<EmptyException>(() => connection.ExecuteQueryFirst<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;",
                    new { Id = -1L }));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQueryFirstIfTheParametersAreNotDefined()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                Assert.Throws<System.InvalidOperationException>(() => connection.ExecuteQueryFirst<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionExecuteQueryFirstIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                Assert.Throws<SqliteException>(() => connection.ExecuteQueryFirst<MdsCompleteTable>("SELEC * FROM [MdsCompleteTable];"));
            }
        }

        #endregion

        #region ExecuteQueryFirstAsync<TEntity>

        [TestMethod]
        public async Task TestSqLiteConnectionExecuteQueryFirstAsync()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var result = await connection.ExecuteQueryFirstAsync<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable] ORDER BY Id ASC;").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.First(), result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionExecuteQueryFirstAsyncWithParameters()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var result = await connection.ExecuteQueryFirstAsync<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Last().Id, result.Id);
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQueryFirstAsyncIfNoRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable];").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQueryFirstAsyncIfNoRowsMatchTheFilter()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.ExecuteQueryFirstAsync<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;",
                    new { Id = -1L }).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQueryFirstAsyncIfTheParametersAreNotDefined()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                await Assert.ThrowsAsync<System.InvalidOperationException>(async () => await connection.ExecuteQueryFirstAsync<MdsCompleteTable>("SELECT * FROM [MdsCompleteTable] WHERE Id = @Id;").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionExecuteQueryFirstAsyncIfThereAreSqlStatementProblems()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                await Assert.ThrowsAsync<SqliteException>(async () => await connection.ExecuteQueryFirstAsync<MdsCompleteTable>("SELEC * FROM [MdsCompleteTable];").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
