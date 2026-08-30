#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver.ADO;
using RepoDb.ClickHouse.IntegrationTests.Models;
using RepoDb.ClickHouse.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.ClickHouse.IntegrationTests.Operations
{
    /*
     * ClickHouse has no native upsert (ON DUPLICATE KEY / MERGE), so ClickHouseStatementBuilder.CreateMerge
     * emits a plain INSERT and relies on the table's ReplacingMergeTree engine + background merges for
     * de-duplication (see Setup.Database's DDL). Merging onto a row that already exists therefore adds a
     * second physical row rather than updating the first one - these tests assert that insert-semantics
     * row-count behavior (and null-Result, same as Insert) instead of asserting immediate de-duplication,
     * which ClickHouse does not guarantee synchronously.
     */

    [TestClass]
    public class MergeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();

            // See Helper.StopMerges: pins CompleteTable's physical row count for the duration of every
            // test in this class so the "...AddsRowInsteadOfDeduping" assertions aren't racing
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
        public void TestClickHouseConnectionMergeForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge<CompleteTable>(table);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMergeForNonEmptyTableAddsRowInsteadOfDeduping()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge<CompleteTable>(table);

                // Assert - no native upsert: merging onto an existing Id adds a second physical row
                Assert.IsNull(result);
                Assert.AreEqual(2, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMergeForNonEmptyTableWithQualifiersAddsRowInsteadOfDeduping()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge<CompleteTable>(table,
                    qualifiers: qualifiers);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(2, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAsyncForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAsync<CompleteTable>(table);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAsyncForNonEmptyTableAddsRowInsteadOfDeduping()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync<CompleteTable>(table);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(2, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAsyncForNonEmptyTableWithQualifiersAddsRowInsteadOfDeduping()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync<CompleteTable>(table,
                    qualifiers: qualifiers);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(2, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestClickHouseConnectionMergeViaTableNameForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMergeAsExpandoObjectViaTableNameForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.Query<CompleteTable>((long)((dynamic)table).Id);

                // Assert
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMergeViaTableNameForNonEmptyTableAddsRowInsteadOfDeduping()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(2, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMergeAsExpandoObjectViaTableNameForNonEmptyTableAddsRowInsteadOfDeduping()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
                ((IDictionary<string, object>)entity)["Id"] = table.Id;

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    entity);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(2, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMergeViaTableNameForNonEmptyTableWithQualifiersAddsRowInsteadOfDeduping()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(2, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMergeAsDynamicViaTableNameForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)table);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.Query<CompleteTable>((long)table.Id);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMergeAsDynamicViaTableNameForNonEmptyTableAddsRowInsteadOfDeduping()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(2, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMergeAsDynamicViaTableNameForNonEmptyTableWithQualifiersAddsRowInsteadOfDeduping()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(2, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAsyncViaTableNameForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAsyncAsExpandoObjectViaTableNameForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.Query<CompleteTable>((long)((dynamic)table).Id);

                // Assert
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAsyncViaTableNameForNonEmptyTableAddsRowInsteadOfDeduping()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(2, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAsyncAsExpandoObjectViaTableNameForNonEmptyTableAddsRowInsteadOfDeduping()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
                ((IDictionary<string, object>)entity)["Id"] = table.Id;

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    entity);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(2, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAsyncViaTableNameForNonEmptyTableWithQualifiersAddsRowInsteadOfDeduping()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(2, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAsyncAsDynamicViaTableNameForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)table);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.Query<CompleteTable>((long)table.Id);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAsyncAsDynamicViaTableNameForNonEmptyTableAddsRowInsteadOfDeduping()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(2, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMergeAsyncAsDynamicViaTableNameForNonEmptyTableWithQualifiersAddsRowInsteadOfDeduping()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.IsNull(result);
                Assert.AreEqual(2, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #endregion
    }
}
