#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;
using RepoDb.Extensions;
using RepoDb.CockroachDb.IntegrationTests.Models;
using RepoDb.CockroachDb.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.CockroachDb.IntegrationTests.Operations
{
    [TestClass]
    public class ExecuteQueryTest
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
        public void TestCockroachDbConnectionExecuteQuery()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM \"CompleteTable\";");

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionExecuteQueryWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id });

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCockroachDbConnectionExecuteQueryAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM \"CompleteTable\";").ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, result.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionExecuteQueryAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExecuteQueryAsync<CompleteTable>("SELECT * FROM \"CompleteTable\" WHERE \"Id\" = @Id;",
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(tables.Last(), result.First());
            }
        }

        #endregion
    }
}
