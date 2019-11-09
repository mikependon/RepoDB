using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations
{
    [TestClass]
    public class MinTest
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
        public void TestMinWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Min<CompleteTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.Min<CompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMinWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                connection.Min<CompleteTable>(e => e.ColumnInt,
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMinAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MinAsync<CompleteTable>(e => e.ColumnInt,
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinAsyncWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.MinAsync<CompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMinAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                connection.MinAsync<CompleteTable>(e => e.ColumnInt,
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestMinViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinViaTableNameWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt),
                    new QueryField("Id", Operation.In, ids));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMinViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMinAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinAsyncViaTableNameWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt),
                    new QueryField("Id", Operation.In, ids)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMinAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt),
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
