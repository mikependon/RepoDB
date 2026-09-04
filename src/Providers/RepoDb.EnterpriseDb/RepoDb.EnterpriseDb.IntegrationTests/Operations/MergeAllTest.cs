#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.EnterpriseDb;
using RepoDb.Extensions;
using RepoDb.EnterpriseDb.IntegrationTests.Setup;
using RepoDb.EnterpriseDb.IntegrationTests.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepoDb.EnterpriseDb.IntegrationTests.Operations
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
        public void TestEnterpriseDbConnectionMergeAllForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllAsExpandoObjectViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var entities = Database.CreateCompleteTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateCompleteTablesAsExpandoObjects(10).AsList();
                tables.ForEach(e => ((IDictionary<string, object>)e)["Id"] = entities[tables.IndexOf(e)].Id);

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMergeAllViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllAsDynamicsViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsDynamics(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllViaTableNameAsyncForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncAsExpandoObjectViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var entities = Database.CreateCompleteTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateCompleteTablesAsExpandoObjects(10).AsList();
                tables.ForEach(e => ((IDictionary<string, object>)e)["Id"] = entities[tables.IndexOf(e)].Id);

                // Act
                var result = await connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsDynamics(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
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
