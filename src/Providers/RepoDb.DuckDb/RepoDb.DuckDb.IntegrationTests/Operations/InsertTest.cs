#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuckDB.NET.Data;
using RepoDb.DuckDb.IntegrationTests.Models;
using RepoDb.DuckDb.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.DuckDb.IntegrationTests.Operations
{
    [TestClass]
    public class InsertTest
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
        public void TestDuckDBConnectionInsertForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert<CompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result, System.Globalization.CultureInfo.InvariantCulture) > 0);
                Assert.IsTrue(table.Id > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestDuckDBConnectionInsertForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert<NonIdentityCompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDuckDBConnectionInsertAsyncForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync<CompleteTable>(table).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result, System.Globalization.CultureInfo.InvariantCulture) > 0);
                Assert.IsTrue(table.Id > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionInsertAsyncForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync<NonIdentityCompleteTable>(table).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestDuckDBConnectionInsertViaTableNameForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result, System.Globalization.CultureInfo.InvariantCulture) > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestDuckDBConnectionInsertViaTableNameAsDynamicForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result, System.Globalization.CultureInfo.InvariantCulture) > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestDuckDBConnectionInsertViaTableNameAsExpandoObjectForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result, System.Globalization.CultureInfo.InvariantCulture) > 0);
                Assert.IsTrue(((dynamic)table).Id == Convert.ToInt64(result, System.Globalization.CultureInfo.InvariantCulture));

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestDuckDBConnectionInsertViaTableNameForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestDuckDBConnectionInsertViaTableNameAsDynamicForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTablesAsDynamics(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestDuckDBConnectionInsertViaTableNameAsExpandoObjectForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Insert(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result, System.Globalization.CultureInfo.InvariantCulture) > 0);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDuckDBConnectionInsertViaTableNameAsyncForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result, System.Globalization.CultureInfo.InvariantCulture) > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionInsertAsyncViaTableNameAsDynamicForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)table).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result, System.Globalization.CultureInfo.InvariantCulture) > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionInsertAsyncViaTableNameAsExpandoObjectForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result, System.Globalization.CultureInfo.InvariantCulture) > 0);
                Assert.IsTrue(((dynamic)table).Id == Convert.ToInt64(result, System.Globalization.CultureInfo.InvariantCulture));

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionInsertViaTableNameAsyncForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    table).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result, System.Globalization.CultureInfo.InvariantCulture) > 0);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionInsertAsyncViaTableNameAsDynamicForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTablesAsDynamics(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    (object)table).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionInsertAsyncViaTableNameAsExpandoObjectForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    table).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result, System.Globalization.CultureInfo.InvariantCulture) > 0);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        #endregion

        #endregion
    }
}
