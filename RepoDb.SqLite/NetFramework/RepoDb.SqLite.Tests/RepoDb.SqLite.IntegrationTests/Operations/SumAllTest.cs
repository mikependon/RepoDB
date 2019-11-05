using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Data.SQLite;
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
        public void TestSumAllWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SumAll<CompleteTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt64(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void TestSumAllWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                connection.SumAll<CompleteTable>(e => e.ColumnInt,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSumAllAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SumAllAsync<CompleteTable>(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt64(result));
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void TestSumAllAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
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
        public void TestSumAllViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SumAll(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt));

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt64(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void TestSumAllViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                connection.SumAll(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt),
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSumAllAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SumAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt)).Result;

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt64(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void TestSumAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                connection.SumAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt),
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
