#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Enumerations;
using RepoDb.AuroraDb.PostgreSql.IntegrationTests.Models;
using RepoDb.AuroraDb.PostgreSql.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.AuroraDb.PostgreSql.IntegrationTests.Operations
{
    [TestClass]
    public class AverageTest
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

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestAuroraDbConnectionAverageWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average<CompleteTable>(e => e.ColumnInteger,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInteger), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionAverageWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.Average<CompleteTable>(e => e.ColumnInteger,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInteger), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionAverageWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Average<CompleteTable>(e => e.ColumnInteger,
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbConnectionAverageAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync<CompleteTable>(e => e.ColumnInteger,
                    (object)null).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInteger), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionAverageAsyncWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = await connection.AverageAsync<CompleteTable>(e => e.ColumnInteger,
                    e => ids.Contains(e.Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInteger), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionAverageAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.AverageAsync<CompleteTable>(e => e.ColumnInteger,
                        (object)null,
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestAuroraDbConnectionAverageViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInteger), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionAverageViaTableNameWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
                    new QueryField("Id", Operation.In, ids));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInteger), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionAverageViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                        Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbConnectionAverageAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
                    (object)null).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInteger), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionAverageAsyncViaTableNameWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
                    new QueryField("Id", Operation.In, ids)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInteger), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionAverageAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
                        (object)null,
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}
