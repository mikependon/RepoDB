#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.SapHana.IntegrationTests.Models;
using RepoDb.SapHana.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SapHana.IntegrationTests.Operations
{
    [TestClass]
    [Ignore("Every test in this class asserts on the value Merge() returns (the row's Id, used both directly " +
        "and to re-query the row). HANA's UPSERT ... WITH PRIMARY KEY can't be combined with a trailing " +
        "SELECT in one statement - the ADO.NET client rejects any multi-statement command text - so unlike " +
        "Insert (which has a GetScopeIdentity() round-trip fallback wired into RepoDb.Core), Merge has no " +
        "way to hand a value back at all. See the remark on SapHanaStatementBuilder.CreateMerge.")]
    public class MergeTest
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
        public void TestHanaConnectionMergeForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge<CompleteTable, int>(table);
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.IsTrue((int)result > 0);
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge<CompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);
                table.ColumnInt = 0;
                table.ColumnChar = "C";

                // Act
                var result = connection.Merge<CompleteTable>(table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestHanaConnectionMergeAsyncForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAsync<CompleteTable, int>(table);
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.IsTrue(result > 0);
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAsyncForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync<CompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);
                table.ColumnInt = 0;
                table.ColumnChar = "C";

                // Act
                var result = await connection.MergeAsync<CompleteTable>(table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestHanaConnectionMergeViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge<int>(ClassMappedNameCache.Get<CompleteTable>(),
                    table);
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.IsTrue(result > 0);
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAsExpandoObjectViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table);
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(((dynamic)table).Id == Convert.ToInt64(result));
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
                ((IDictionary<string, object>)entity)["Id"] = table.Id;

                // Act
                var result = connection.Merge<long>(ClassMappedNameCache.Get<CompleteTable>(),
                    entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, result);
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertMembersEquality(queryResult.First(), entity);
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);
                table.ColumnInt = 0;
                table.ColumnChar = "C";

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAsDynamicViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestHanaConnectionMergeAsyncViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAsync<int>(ClassMappedNameCache.Get<CompleteTable>(),
                    table);
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.IsTrue(result > 0);
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAsyncAsExpandoObjectViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table);
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(((dynamic)table).Id == Convert.ToInt64(result));
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAsyncAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
                ((IDictionary<string, object>)entity)["Id"] = table.Id;

                // Act
                var result = await connection.MergeAsync<long>(ClassMappedNameCache.Get<CompleteTable>(),
                    entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, result);
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertMembersEquality(queryResult.First(), entity);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAsyncAsDynamicViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #endregion
    }
}
