#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
{
    [TestClass]
    public class DeleteAllTest
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

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionDeleteAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.DeleteAll<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count(), result);
            Assert.AreEqual(0, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public void TestDb2ConnectionDeleteAllWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = connection.DeleteAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestDb2ConnectionDeleteAllViaPrimaryKeys()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var keysToDelete = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables.Take(5), e => e.Id);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.DeleteAll<CompleteTable>(keysToDelete);

            // Assert
            Assert.AreEqual(5, result);
            Assert.AreEqual(5, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public void TestDb2ConnectionDeleteAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>());

            // Assert
            Assert.AreEqual(tables.Count(), result);
            Assert.AreEqual(0, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public void TestDb2ConnectionDeleteAllViaTableNameViaPrimaryKeys()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var keysToDelete = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables.Take(5), e => e.Id);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.DeleteAll(ClassMappedNameCache.Get<CompleteTable>(), keysToDelete);

            // Assert
            Assert.AreEqual(5, result);
            Assert.AreEqual(5, connection.CountAll<CompleteTable>());
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAllAsync<CompleteTable>();

            // Assert
            Assert.AreEqual(tables.Count(), result);
            Assert.AreEqual(0, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAllAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = await connection.DeleteAllAsync<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAllAsyncViaPrimaryKeys()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var keysToDelete = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables.Take(5), e => e.Id);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAllAsync<CompleteTable>(keysToDelete);

            // Assert
            Assert.AreEqual(5, result);
            Assert.AreEqual(5, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>());

            // Assert
            Assert.AreEqual(tables.Count(), result);
            Assert.AreEqual(0, connection.CountAll<CompleteTable>());
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAllAsyncViaTableNameViaPrimaryKeys()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var keysToDelete = ClassExpression.GetEntitiesPropertyValues<CompleteTable, object>(tables.Take(5), e => e.Id);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAllAsync(ClassMappedNameCache.Get<CompleteTable>(), keysToDelete);

            // Assert
            Assert.AreEqual(5, result);
            Assert.AreEqual(5, connection.CountAll<CompleteTable>());
        }

        #endregion
    }
}
