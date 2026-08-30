#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver.ADO;
using RepoDb.ClickHouse.IntegrationTests.Models;
using RepoDb.ClickHouse.IntegrationTests.Setup;
using RepoDb.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.ClickHouse.IntegrationTests.Operations
{
    /*
     * See the comment atop MergeTest.cs: MergeAll emits plain multi-row-shaped INSERTs (no native
     * upsert), so merging onto rows that already exist adds duplicate physical rows rather than
     * de-duplicating them - the row count doubles instead of staying the same. MergeAll's return value
     * is still the number of entities processed (same mechanism as InsertAll), unaffected by this.
     * The Identity/NonIdentity split from the MariaDb-derived suite is dropped here: ClickHouse has no
     * identity concept at all, so CompleteTable and NonIdentityCompleteTable now behave identically.
     */

    [TestClass]
    public class MergeAllTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();

            // See Helper.StopMerges: pins CompleteTable's physical row count for the duration of every
            // test in this class so the "...AddsRowsInsteadOfDeduping" assertions aren't racing
            // ClickHouse's background merge scheduler. Restarted in Cleanup() below.
            using var connection = new ClickHouseConnection(Database.ConnectionString);
            Helper.StopMerges(connection, "CompleteTable");
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                Helper.StartMerges(connection, "CompleteTable");
            }
            Database.Cleanup();
        }

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestClickHouseConnectionMergeAllForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMergeAllForNonEmptyTableAddsRowsInsteadOfDeduping()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count * 2, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMergeAllForNonEmptyTableWithQualifiersAddsRowsInsteadOfDeduping()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<CompleteTable>(tables,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count * 2, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAllAsyncForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAllAsync<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAllAsyncForNonEmptyTableAddsRowsInsteadOfDeduping()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = await connection.MergeAllAsync<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count * 2, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAllAsyncForNonEmptyTableWithQualifiersAddsRowsInsteadOfDeduping()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = await connection.MergeAllAsync<CompleteTable>(tables,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count * 2, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestClickHouseConnectionMergeAllViaTableNameForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMergeAllAsDynamicsViaTableNameForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsDynamics(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    (IEnumerable<object>)tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMergeAllAsExpandoObjectViaTableNameForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMergeAllViaTableNameForNonEmptyTableAddsRowsInsteadOfDeduping()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count * 2, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMergeAllViaTableNameForNonEmptyTableWithQualifiersAddsRowsInsteadOfDeduping()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count * 2, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAllViaTableNameAsyncForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAllAsyncAsDynamicsViaTableNameForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsDynamics(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (IEnumerable<object>)tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAllAsyncAsExpandoObjectViaTableNameForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAllViaTableNameAsyncForNonEmptyTableAddsRowsInsteadOfDeduping()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count * 2, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAllViaTableNameAsyncForNonEmptyTableWithQualifiersAddsRowsInsteadOfDeduping()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count * 2, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #endregion
    }
}
