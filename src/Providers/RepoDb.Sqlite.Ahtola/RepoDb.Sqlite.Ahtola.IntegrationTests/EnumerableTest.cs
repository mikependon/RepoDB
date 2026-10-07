#region Copyright Attributions

// Copyright (c) 2026 mamoreau-devolutions and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Ahtola.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.Ahtola.IntegrationTests.Models;
using RepoDb.Ahtola.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Ahtola.IntegrationTests.Operations.MDS
{
    [TestClass]
    public class EnumerableTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
            GlobalConfiguration
                .Setup(new()
                {
                    ConversionType = Enumerations.ConversionType.Default
                })
                .UseAhtola();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region List

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionQueryListContains()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsNonIdentityCompleteTables(10, connection).AsList();
                var ids = tables.Select(e => e.Id).AsList();

                // Act
                var result = connection.Query<MdsNonIdentityCompleteTable>(e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                    Helper.AssertPropertiesEquality(table, result.First(item => item.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQueryEmptyList()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsNonIdentityCompleteTables(10, connection).AsList();

                // Act
                var result = connection.Query<MdsNonIdentityCompleteTable>(e => Enumerable.Empty<Guid>().Contains(e.Id));

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqLiteConnectionQueryAsyncListContains()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsNonIdentityCompleteTables(10, connection).AsList();
                var ids = tables.Select(e => e.Id).AsList();

                // Act
                var result = await connection.QueryAsync<MdsNonIdentityCompleteTable>(e => ids.Contains(e.Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                    Helper.AssertPropertiesEquality(table, result.First(item => item.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQueryAsyncEmptyList()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsNonIdentityCompleteTables(10, connection).AsList();

                // Act
                var result = await connection.QueryAsync<MdsNonIdentityCompleteTable>(e => Enumerable.Empty<Guid>().Contains(e.Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(0, result.Count());
            }
        }

        #endregion

        #endregion
    }
}
