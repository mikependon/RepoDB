using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.MySqlConnector.IntegrationTests.Models;
using RepoDb.MySqlConnector.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.MySqlConnector.IntegrationTests.Operations
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
        public void TestMySqlConnectionCountAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlConnectionCountAllWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.CountAll<CompleteTable>(hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMySqlConnectionCountAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAllAsync<CompleteTable>().Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMySqlConnectionCountAllAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.CountAllAsync<CompleteTable>(hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestMySqlConnectionCountAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAll(ClassMappedNameCache.Get<CompleteTable>());

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlConnectionCountAllViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.CountAll(ClassMappedNameCache.Get<CompleteTable>(),
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMySqlConnectionCountAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAllAsync(ClassMappedNameCache.Get<CompleteTable>()).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMySqlConnectionCountAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.CountAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
