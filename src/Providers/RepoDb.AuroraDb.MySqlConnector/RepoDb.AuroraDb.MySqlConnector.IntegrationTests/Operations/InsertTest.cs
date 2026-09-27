#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.AuroraDb.MySqlConnector;
using RepoDb.AuroraDb.MySqlConnector.IntegrationTests.Models;
using RepoDb.AuroraDb.MySqlConnector.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.AuroraDb.MySqlConnector.IntegrationTests.Operations
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
        public void TestAuroraDbConnectionInsertForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
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
        public void TestAuroraDbConnectionInsertForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
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
        public async Task TestAuroraDbConnectionInsertAsyncForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
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
        public async Task TestAuroraDbConnectionInsertAsyncForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAsync<NonIdentityCompleteTable>(table).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<NonIdentityCompleteTable>(table.Id);

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
        public void TestAuroraDbConnectionInsertViaTableNameForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
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
        public void TestAuroraDbConnectionInsertViaTableNameAsDynamicForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
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
        public void TestAuroraDbConnectionInsertViaTableNameAsExpandoObjectForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
            
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
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
        public void TestAuroraDbConnectionInsertViaTableNameForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
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
        public void TestAuroraDbConnectionInsertViaTableNameAsDynamicForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTablesAsDynamics(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
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
        public void TestAuroraDbConnectionInsertViaTableNameAsExpandoObjectForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
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
        public async Task TestAuroraDbConnectionInsertViaTableNameAsyncForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
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
        public async Task TestAuroraDbConnectionInsertAsyncViaTableNameAsDynamicForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
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
        public async Task TestAuroraDbConnectionInsertAsyncViaTableNameAsExpandoObjectForIdentity()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
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
        public async Task TestAuroraDbConnectionInsertViaTableNameAsyncForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
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
        public async Task TestAuroraDbConnectionInsertAsyncViaTableNameAsDynamicForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTablesAsDynamics(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
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
        public async Task TestAuroraDbConnectionInsertAsyncViaTableNameAsExpandoObjectForNonIdentity()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
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
