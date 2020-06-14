using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.Enumerations;
using RepoDb.MySqlConnector.IntegrationTests.Models;
using RepoDb.MySqlConnector.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.MySqlConnector.IntegrationTests.Operations
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
        public void TestMySqlConnectionAverageWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average<CompleteTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionAverageWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.Average<CompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void TestMySqlConnectionAverageWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Average<CompleteTable>(e => e.ColumnInt,
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMySqlConnectionAverageAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.AverageAsync<CompleteTable>(e => e.ColumnInt,
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionAverageAsyncWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.AverageAsync<CompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void TestMySqlConnectionAverageAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.AverageAsync<CompleteTable>(e => e.ColumnInt,
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestMySqlConnectionAverageViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionAverageViaTableNameWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                    new QueryField("Id", Operation.In, ids));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void TestMySqlConnectionAverageViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMySqlConnectionAverageAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionAverageAsyncViaTableNameWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                    new QueryField("Id", Operation.In, ids)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void TestMySqlConnectionAverageAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInt).First(),
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
