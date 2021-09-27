using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations.SDS
{
    [TestClass]
    public class AverageTest
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
        public void TestSqLiteConnectionAverageWithoutExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.Average<SdsCompleteTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionAverageWithExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.Average<SdsCompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void TestSqLiteConnectionAverageWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                connection.Average<SdsCompleteTable>(e => e.ColumnInt,
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionAverageAsyncWithoutExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.AverageAsync<SdsCompleteTable>(e => e.ColumnInt,
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionAverageAsyncWithExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.AverageAsync<SdsCompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void TestSqLiteConnectionAverageAsyncWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                connection.AverageAsync<SdsCompleteTable>(e => e.ColumnInt,
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionAverageViaTableNameWithoutExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.Average(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionAverageViaTableNameWithExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.Average(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First(),
                    new QueryField("Id", Operation.In, ids));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void TestSqLiteConnectionAverageViaTableNameWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                connection.Average(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First(),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionAverageAsyncViaTableNameWithoutExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.AverageAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First(),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionAverageAsyncViaTableNameWithExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.AverageAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First(),
                    new QueryField("Id", Operation.In, ids)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void TestSqLiteConnectionAverageAsyncViaTableNameWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                connection.AverageAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    Field.Parse<SdsCompleteTable>(e => e.ColumnInt).First(),
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
