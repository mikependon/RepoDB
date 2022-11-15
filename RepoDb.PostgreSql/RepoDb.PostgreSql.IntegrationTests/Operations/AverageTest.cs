using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.PostgreSql.IntegrationTests.Operations
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
        public void TestPostgreSqlConnectionAverageWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average<CompleteTable>(e => e.ColumnInteger,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInteger), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionAverageWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.Average<CompleteTable>(e => e.ColumnInteger,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInteger), Convert.ToDouble(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void TestPostgreSqlConnectionAverageWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Average<CompleteTable>(e => e.ColumnInteger,
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestPostgreSqlConnectionAverageAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync<CompleteTable>(e => e.ColumnInteger,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInteger), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionAverageAsyncWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = await connection.AverageAsync<CompleteTable>(e => e.ColumnInteger,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInteger), Convert.ToDouble(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public async Task TestPostgreSqlConnectionAverageAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.AverageAsync<CompleteTable>(e => e.ColumnInteger,
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestPostgreSqlConnectionAverageViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInteger), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionAverageViaTableNameWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
                    new QueryField("Id", Operation.In, ids));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInteger), Convert.ToDouble(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void TestPostgreSqlConnectionAverageViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestPostgreSqlConnectionAverageAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInteger), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionAverageAsyncViaTableNameWithExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var ids = new[] { tables.First().Id, tables.Last().Id };
                var result = await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
                    new QueryField("Id", Operation.In, ids));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnInteger), Convert.ToDouble(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public async Task TestPostgreSqlConnectionAverageAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnInteger).First(),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #endregion
    }
}
