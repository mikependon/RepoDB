#region Copyright Attributions

// Copyright (c) 2026 mamoreau-devolutions and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Ahtola.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Ahtola.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Ahtola.IntegrationTests.Operations.MDS
{
    [TestClass]
    public class ExecuteNonQueryTest
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

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionExecuteNonQuery()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [MdsCompleteTable];");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionExecuteNonQueryWithParameters()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [MdsCompleteTable] WHERE Id = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionExecuteNonQueryWithMultipleStatement()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM [MdsCompleteTable]; VACUUM;");

                // Assert
                // Ahtola does not carry over the previous statement's change count for VACUUM (native SQLite does)
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqLiteConnectionExecuteNonQueryAsync()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [MdsCompleteTable];").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionExecuteNonQueryAsyncWithParameters()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [MdsCompleteTable] WHERE Id = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionExecuteNonQueryAsyncWithMultipleStatement()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM [MdsCompleteTable]; VACUUM;").ConfigureAwait(false);

                // Assert
                // Ahtola does not carry over the previous statement's change count for VACUUM (native SQLite does)
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion
    }
}
