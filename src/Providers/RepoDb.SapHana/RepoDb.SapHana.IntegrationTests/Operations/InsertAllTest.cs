#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.SapHana.IntegrationTests.Models;
using RepoDb.SapHana.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SapHana.IntegrationTests.Operations
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
        public void TestHanaConnectionInsertAllForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
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
        public void TestHanaConnectionInsertAllForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
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
        public async Task TestHanaConnectionInsertAllAsyncForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync<CompleteTable>(tables);

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
        public async Task TestHanaConnectionInsertAllAsyncForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync<NonIdentityCompleteTable>(tables);

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
        public void TestHanaConnectionInsertAllViaTableNameForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
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
        [Ignore("For an anonymous-typed (or otherwise class-like) entity, RepoDb always excludes the " +
            "identity column from the INSERT regardless of any value the caller supplied, so this test's " +
            "pre-assigned Id=1..10 only comes back from the DB if the table's identity counter happens to " +
            "still be sitting at 0. Unlike SQL Server, HANA's TRUNCATE TABLE does not reset a GENERATED BY " +
            "DEFAULT AS IDENTITY counter (confirmed directly against the driver), and there is no ALTER to " +
            "restart it once created ('cannot modify identity column'), so across a real test run the " +
            "counter keeps climbing and the assumption doesn't hold.")]
        public void TestHanaConnectionInsertAllViaTableNameAsDynamicsForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsDynamics(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
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
        public void TestHanaConnectionInsertAllViaTableNameAsExpandoObjectsForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
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
        public void TestHanaConnectionInsertAllViaTableNameForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
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
        public void TestHanaConnectionInsertAllViaTableNameAsDynamicsForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
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
        public void TestHanaConnectionInsertAllViaTableNameAsExpandoObjectsForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
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
        public async Task TestHanaConnectionInsertAllViaTableNameAsyncForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
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
        [Ignore("See the remark on TestHanaConnectionInsertAllViaTableNameAsDynamicsForIdentity.")]
        public async Task TestHanaConnectionInsertAllAsyncViaTableNameAsDynamicsForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsDynamics(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
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
        public async Task TestHanaConnectionInsertAllAsyncViaTableNameAsExpandoObjectsForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
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
        public async Task TestHanaConnectionInsertAllViaTableNameAsyncForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
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
        public async Task TestHanaConnectionInsertAllAsyncViaTableNameAsDynamicsForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
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
        public async Task TestHanaConnectionInsertAllAsyncViaTableNameAsExpandoObjectsForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
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

        #endregion
    }
}
