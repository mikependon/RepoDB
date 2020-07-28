using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations
{
    [TestClass]
    public class SumAllTest
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
        public void TestSqLiteConnectionSumAll()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.SumAll<CompleteTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnSqLiteConnectionSumAllWithHints()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                connection.SumAll<CompleteTable>(e => e.ColumnInt,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionSumAllAsync()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.SumAllAsync<CompleteTable>(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqLiteConnectionSumAllAsyncWithHints()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                connection.SumAllAsync<CompleteTable>(e => e.ColumnInt,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionSumAllViaTableName()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.SumAll(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First());

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnSqLiteConnectionSumAllViaTableNameWithHints()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                connection.SumAll(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionSumAllAsyncViaTableName()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.SumAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First()).Result;

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqLiteConnectionSumAllAsyncViaTableNameWithHints()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                connection.SumAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
