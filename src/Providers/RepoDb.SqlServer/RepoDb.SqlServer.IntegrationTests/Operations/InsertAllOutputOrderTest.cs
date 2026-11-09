#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests.Operations
{
    /// <summary>
    /// Regression coverage for #1393: InsertAll must give every entity the key generated for its own row.
    /// SQL Server does not guarantee that the OUTPUT clause emits rows in the order of the source rows. On a table whose
    /// clustered index is not the identity column, a large enough batch is sorted by the clustered key before the insert,
    /// so the OUTPUT order differs from the source order. Reading the keys back by position then assigns them to the
    /// wrong entities, without any error.
    /// </summary>
    [TestClass]
    public class InsertAllOutputOrderTest
    {
        // Large enough for SQL Server to sort the batch by the clustered key before inserting it
        // (3 parameters per row, well below the 2100-parameter limit).
        private const int BatchSize = 500;

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

        private static List<InsertAllOutputOrderTable> CreateEntities(int count) =>
            // [SortKey] descends while the source order ascends, so a sort by the clustered key reverses the rows.
            Enumerable.Range(0, count)
                .Select(i => new InsertAllOutputOrderTable { SortKey = count - i, Name = $"Name_{i}" })
                .ToList();

        private static void AssertEveryEntityHasItsOwnKey(SqlConnection connection,
            IEnumerable<InsertAllOutputOrderTable> entities)
        {
            var rows = connection.QueryAll<InsertAllOutputOrderTable>().ToDictionary(row => row.Id);
            foreach (var entity in entities)
            {
                Assert.IsTrue(rows.TryGetValue(entity.Id, out var row), $"No row has the key {entity.Id} assigned to '{entity.Name}'.");
                Assert.AreEqual(entity.Name, row.Name, $"The key {entity.Id} assigned to '{entity.Name}' belongs to '{row.Name}'.");
                Assert.AreEqual(entity.SortKey, row.SortKey);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAllAssignsEachEntityTheKeyOfItsOwnRow()
        {
            // Setup
            var entities = CreateEntities(BatchSize);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll(entities, batchSize: BatchSize);

                // Assert
                Assert.AreEqual(entities.Count, result);
                Assert.AreEqual(entities.Count, entities.Select(e => e.Id).Distinct().Count());
                AssertEveryEntityHasItsOwnKey(connection, entities);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncAssignsEachEntityTheKeyOfItsOwnRow()
        {
            // Setup
            var entities = CreateEntities(BatchSize);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync(entities, batchSize: BatchSize);

                // Assert
                Assert.AreEqual(entities.Count, result);
                Assert.AreEqual(entities.Count, entities.Select(e => e.Id).Distinct().Count());
                AssertEveryEntityHasItsOwnKey(connection, entities);
            }
        }
    }
}
