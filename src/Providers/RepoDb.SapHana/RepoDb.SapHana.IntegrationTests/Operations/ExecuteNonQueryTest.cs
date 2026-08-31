#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.SapHana.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SapHana.IntegrationTests.Operations
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
        public void TestHanaConnectionExecuteNonQuery()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionExecuteNonQueryWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\" WHERE \"Id\" = :Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        [Ignore("HANA's ADO.NET client rejects a command text containing more than one SQL statement.")]
        public void TestHanaConnectionExecuteNonQueryWithMultipleStatement()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteNonQuery("DELETE FROM \"CompleteTable\"; DELETE FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestHanaConnectionExecuteNonQueryAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionExecuteNonQueryAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\" WHERE \"Id\" = :Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        [Ignore("See the remark on TestHanaConnectionExecuteNonQueryWithMultipleStatement.")]
        public async Task TestHanaConnectionExecuteNonQueryAsyncWithMultipleStatement()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteNonQueryAsync("DELETE FROM \"CompleteTable\"; DELETE FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion
    }
}
