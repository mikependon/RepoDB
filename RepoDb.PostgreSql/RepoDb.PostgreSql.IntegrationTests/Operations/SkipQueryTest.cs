using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.PostgreSql.IntegrationTests.Operations
{
    [TestClass]
    public class SkipQueryTest
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
        public void TestPostgreSqlConnectionSkipQueryFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery<CompleteTable>(
                    0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionSkipQueryFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery<CompleteTable>(
                    0,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionSkipQueryThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery<CompleteTable>(
                    6,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionSkipQueryThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery<CompleteTable>(
                    6,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlConnectionSkipQueryWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.SkipQuery<CompleteTable>(
                    0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestPostgreSqlConnectionSkipQueryAsyncFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync<CompleteTable>(
                    0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionSkipQueryAsyncFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync<CompleteTable>(
                    0,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionSkipQueryAsyncThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync<CompleteTable>(
                    6,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionSkipQueryAsyncThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync<CompleteTable>(
                    6,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public async Task ThrowExceptionOnPostgreSqlConnectionSkipQueryAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.SkipQueryAsync<CompleteTable>(
                    0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestPostgreSqlConnectionSkipQueryViaTableNameFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery(ClassMappedNameCache.Get<CompleteTable>(),
                    0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionSkipQueryViaTableNameFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery(ClassMappedNameCache.Get<CompleteTable>(),
                    0,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionSkipQueryViaTableNameThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery(ClassMappedNameCache.Get<CompleteTable>(),
                    6,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionSkipQueryViaTableNameThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery(ClassMappedNameCache.Get<CompleteTable>(),
                    6,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlConnectionSkipQueryViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.SkipQuery(ClassMappedNameCache.Get<CompleteTable>(),
                    5,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestPostgreSqlConnectionSkipQueryViaTableNameAsyncFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionSkipQueryViaTableNameAsyncFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    0,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionSkipQueryViaTableNameAsyncThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    6,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionSkipQueryViaTableNameAsyncThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    6,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public async Task ThrowExceptionOnPostgreSqlConnectionSkipQueryAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.SkipQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #endregion
    }
}
