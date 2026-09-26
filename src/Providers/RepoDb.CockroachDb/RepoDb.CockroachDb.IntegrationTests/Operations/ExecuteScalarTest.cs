#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;
using RepoDb.CockroachDb.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.CockroachDb.IntegrationTests.Operations
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
        public void TestCockroachDbConnectionExecuteScalar()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteScalar("SELECT COUNT(*) FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionExecuteScalarWithReturnType()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
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
        public async Task TestCockroachDbConnectionExecuteScalarAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteScalarAsync("SELECT COUNT(*) FROM \"CompleteTable\";").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count(), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionExecuteScalarAsyncWithReturnType()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"CompleteTable\";").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion
    }
}
