#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.Extensions;
using RepoDb.SapHana.IntegrationTests.Models;
using RepoDb.SapHana.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SapHana.IntegrationTests.Operations
{
    [TestClass]
    public class MergeAllTest
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
        [Ignore("Unlike TestHanaConnectionMergeAllForIdentityForNonEmptyTable, this merges brand-new " +
            "rows with no Id set, expecting the framework to auto-generate the identity value as part of " +
            "the merge. HANA's UPSERT ... WITH PRIMARY KEY requires a concrete key to match/insert " +
            "against, so it rejects a NULL identity column outright ('cannot insert NULL... Not nullable " +
            "\"Id\" column'); there's no client-side way to auto-generate a key before the row exists.")]
        public void TestHanaConnectionMergeAllForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<CompleteTable>(tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll<NonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<NonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<NonIdentityCompleteTable>(tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        [Ignore("See the remark on TestHanaConnectionMergeAllForIdentityForEmptyTable.")]
        public async Task TestHanaConnectionMergeAllAsyncForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAllAsync<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllAsyncForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = await connection.MergeAllAsync<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = await connection.MergeAllAsync<CompleteTable>(tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllAsyncForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAllAsync<NonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllAsyncForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = await connection.MergeAllAsync<NonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllAsyncForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = await connection.MergeAllAsync<NonIdentityCompleteTable>(tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        [Ignore("See the remark on TestHanaConnectionMergeAllForIdentityForEmptyTable.")]
        public void TestHanaConnectionMergeAllViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllAsExpandoObjectViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var entities = Database.CreateCompleteTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateCompleteTablesAsExpandoObjects(10).AsList();
                tables.ForEach(e => ((IDictionary<string, object>)e)["Id"] = entities[tables.IndexOf(e)].Id);

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
                Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllAsDynamicsViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsDynamics(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Assert
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestHanaConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        [Ignore("See the remark on TestHanaConnectionMergeAllForIdentityForEmptyTable.")]
        public async Task TestHanaConnectionMergeAllViaTableNameAsyncForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllAsyncAsExpandoObjectViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllAsyncAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var entities = Database.CreateCompleteTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateCompleteTablesAsExpandoObjects(10).AsList();
                tables.ForEach(e => ((IDictionary<string, object>)e)["Id"] = entities[tables.IndexOf(e)].Id);

                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
                Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsDynamics(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllAsyncViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #endregion
    }
}
