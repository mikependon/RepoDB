using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations
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
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.CountAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnSqLiteConnectionCountAllWithHints()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                connection.CountAll<CompleteTable>(hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionCountAllAsync()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.CountAllAsync<CompleteTable>().Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqLiteConnectionCountAllAsyncWithHints()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                connection.CountAllAsync<CompleteTable>(hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionCountAllViaTableName()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.CountAll(ClassMappedNameCache.Get<CompleteTable>());

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnSqLiteConnectionCountAllViaTableNameWithHints()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                connection.CountAll(ClassMappedNameCache.Get<CompleteTable>(),
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionCountAllAsyncViaTableName()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.CountAllAsync(ClassMappedNameCache.Get<CompleteTable>()).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqLiteConnectionCountAllAsyncViaTableNameWithHints()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                connection.CountAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
