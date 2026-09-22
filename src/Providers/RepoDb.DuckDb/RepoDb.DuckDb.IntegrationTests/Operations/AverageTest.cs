#region Copyright Attributions

// Copyright (c) 2019 Bradley Graigner and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuckDB.NET.Data;
using RepoDb.Enumerations;
using RepoDb.DuckDb.IntegrationTests.Models;
using RepoDb.DuckDb.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.DuckDb.IntegrationTests.Operations
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
        public void TestDuckDBConnectionAverageWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average<CompleteTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestDuckDBConnectionAverageWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.Average<CompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestDuckDBConnectionAverageWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Average<CompleteTable>(e => e.ColumnInt,
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDuckDBConnectionAverageAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync<CompleteTable>(e => e.ColumnInt,
                    (object)null).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionAverageAsyncWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = await connection.AverageAsync<CompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionAverageAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.AverageAsync<CompleteTable>(e => e.ColumnInt,
                        (object)null,
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestDuckDBConnectionAverageViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestDuckDBConnectionAverageViaTableNameWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                    new QueryField("Id", Operation.In, ids));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestDuckDBConnectionAverageViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                        Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDuckDBConnectionAverageAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                    (object)null).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionAverageAsyncViaTableNameWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                    new QueryField("Id", Operation.In, ids)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionAverageAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                        (object)null,
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}
