using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS
{
    [TestClass]
    public class CountAllTest
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
        public void TestSqLiteConnectionCountAll()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.CountAll<MdsCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnSqLiteConnectionCountAllWithHints()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                connection.CountAll<MdsCompleteTable>(hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqLiteConnectionCountAllAsync()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = await connection.CountAllAsync<MdsCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public async Task ThrowExceptionOnSqLiteConnectionCountAllAsyncWithHints()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                await connection.CountAllAsync<MdsCompleteTable>(hints: "WhatEver");
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionCountAllViaTableName()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.CountAll(ClassMappedNameCache.Get<MdsCompleteTable>());

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnSqLiteConnectionCountAllViaTableNameWithHints()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                connection.CountAll(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqLiteConnectionCountAllAsyncViaTableName()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = await connection.CountAllAsync(ClassMappedNameCache.Get<MdsCompleteTable>());

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public async Task ThrowExceptionOnSqLiteConnectionCountAllAsyncViaTableNameWithHints()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                await connection.CountAllAsync(ClassMappedNameCache.Get<MdsCompleteTable>(),
                    hints: "WhatEver");
            }
        }

        #endregion

        #endregion
    }
}
