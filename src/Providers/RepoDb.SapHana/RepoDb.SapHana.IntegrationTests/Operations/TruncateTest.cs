#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.SapHana.IntegrationTests.Models;
using RepoDb.SapHana.IntegrationTests.Setup;

namespace RepoDb.SapHana.IntegrationTests.Operations
{
    [TestClass]
    public class TruncateTest
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
        public void TestHanaConnectionTruncate()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Truncate<CompleteTable>();
                var countResult = connection.CountAll<CompleteTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestHanaConnectionTruncateAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.TruncateAsync<CompleteTable>();
                var countResult = connection.CountAll<CompleteTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestHanaConnectionTruncateViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Truncate(ClassMappedNameCache.Get<CompleteTable>());
                var countResult = connection.CountAll<CompleteTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestHanaConnectionTruncateAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.TruncateAsync(ClassMappedNameCache.Get<CompleteTable>());
                var countResult = connection.CountAll<CompleteTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #endregion
    }
}
