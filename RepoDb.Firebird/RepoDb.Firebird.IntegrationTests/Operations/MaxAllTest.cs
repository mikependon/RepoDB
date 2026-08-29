using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Firebird.IntegrationTests.Models;
using RepoDb.Firebird.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Firebird.IntegrationTests.Operations
{
    [TestClass]
    public class MaxAllTest
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
        public void TestFirebirdConnectionMaxAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MaxAll<CompleteTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionMaxAllWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.MaxAll<CompleteTable>(e => e.ColumnInt,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestFirebirdConnectionMaxAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAllAsync<CompleteTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnFirebirdConnectionMaxAllAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.MaxAllAsync<CompleteTable>(e => e.ColumnInt,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestFirebirdConnectionMaxAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MaxAll(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First());

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionMaxAllViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.MaxAll(ClassMappedNameCache.Get<CompleteTable>(),
                        Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestFirebirdConnectionMaxAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First());

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnFirebirdConnectionMaxAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.MaxAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion
    }
}
