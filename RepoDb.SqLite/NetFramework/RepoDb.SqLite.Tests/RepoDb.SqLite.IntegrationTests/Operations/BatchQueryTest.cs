using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations
{
    [TestClass]
    public class BatchQueryTest
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
        public void TestBatchQueryFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQuery<CompleteTable>(0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);
                var items = tables
                    .OrderBy(item => item.Id)
                    .Range(0, 2)
                    .AsList();

                // Assert
                Assert.IsTrue(items.All(item => result.FirstOrDefault(e => e.Id == item.Id) != null));
            }
        }

        [TestMethod]
        public void TestBatchQueryFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQuery<CompleteTable>(0,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);
                var items = tables
                    .OrderByDescending(item => item.Id)
                    .Range(0, 2)
                    .AsList();

                // Assert
                Assert.IsTrue(items.All(item => result.FirstOrDefault(e => e.Id == item.Id) != null));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void TestBatchQueryFirstBatchWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQuery<CompleteTable>(0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        #endregion

        #endregion

        #region TableName

        #endregion
    }
}
