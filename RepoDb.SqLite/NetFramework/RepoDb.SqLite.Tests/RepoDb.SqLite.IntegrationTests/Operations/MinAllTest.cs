using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations
{
    [TestClass]
    public class MinAllTest
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
        public void TestMinAllWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MinAll<CompleteTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void TestMinAllWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                connection.MinAll<CompleteTable>(e => e.ColumnInt,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMinAllAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MinAllAsync<CompleteTable>(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void TestMinAllAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                connection.MinAllAsync<CompleteTable>(e => e.ColumnInt,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestMinAllViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MinAll(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt));

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void TestMinAllViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                connection.MinAll(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt),
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMinAllAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MinAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt)).Result;

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void TestMinAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                connection.MinAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt),
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
