#region Copyright Attributions

// Copyright (c) 2019 Bradley Graigner and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuckDB.NET.Data;
using RepoDb.DuckDb.IntegrationTests.Models;
using RepoDb.DuckDb.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.DuckDb.IntegrationTests.Operations
{
    [TestClass]
    public class InsertAllTest
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
        public void TestDuckDBConnectionInsertAllForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => table.Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestDuckDBConnectionInsertAllForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll<NonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDuckDBConnectionInsertAllAsyncForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync<CompleteTable>(tables).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => table.Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionInsertAllAsyncForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync<NonIdentityCompleteTable>(tables).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestDuckDBConnectionInsertAllViaTableNameForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestDuckDBConnectionInsertAllViaTableNameAsDynamicsForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsDynamics(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestDuckDBConnectionInsertAllViaTableNameAsExpandoObjectsForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        [TestMethod]
        public void TestDuckDBConnectionInsertAllViaTableNameForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestDuckDBConnectionInsertAllViaTableNameAsDynamicsForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestDuckDBConnectionInsertAllViaTableNameAsExpandoObjectsForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDuckDBConnectionInsertAllViaTableNameAsyncForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionInsertAllAsyncViaTableNameAsDynamicsForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsDynamics(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionInsertAllAsyncViaTableNameAsExpandoObjectsForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionInsertAllViaTableNameAsyncForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionInsertAllAsyncViaTableNameAsDynamicsForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionInsertAllAsyncViaTableNameAsExpandoObjectsForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        #endregion

        #endregion
    }
}
