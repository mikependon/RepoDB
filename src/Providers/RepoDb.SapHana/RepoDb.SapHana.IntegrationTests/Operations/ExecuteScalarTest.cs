#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.SapHana.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SapHana.IntegrationTests.Operations
{
    [TestClass]
    public class ExecuteScalarTest
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
        public void TestHanaConnectionExecuteScalar()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteScalar("SELECT COUNT(*) FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestHanaConnectionExecuteScalarWithReturnType()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestHanaConnectionExecuteScalarAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteScalarAsync("SELECT COUNT(*) FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionExecuteScalarAsyncWithReturnType()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion
    }
}
