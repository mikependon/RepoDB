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

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
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

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestBatchQueryThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQuery<CompleteTable>(2,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestBatchQueryThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQuery<CompleteTable>(2,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnBatchQueryWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                connection.BatchQuery<CompleteTable>(0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestBatchQueryAsyncFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQueryAsync<CompleteTable>(0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestBatchQueryAsyncFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQueryAsync<CompleteTable>(0,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestBatchQueryAsyncThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQueryAsync<CompleteTable>(2,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestBatchQueryAsyncThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQueryAsync<CompleteTable>(2,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null).Result;

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnBatchQueryAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                connection.BatchQueryAsync<CompleteTable>(0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestBatchQueryViaTableNameFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQuery(ClassMappedNameCache.Get<CompleteTable>(),
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
        public void TestBatchQueryViaTableNameFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQuery(ClassMappedNameCache.Get<CompleteTable>(),
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
        public void TestBatchQueryViaTableNameThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQuery(ClassMappedNameCache.Get<CompleteTable>(),
                    2,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestBatchQueryViaTableNameThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQuery(ClassMappedNameCache.Get<CompleteTable>(),
                    2,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnBatchQueryViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                connection.BatchQuery(ClassMappedNameCache.Get<CompleteTable>(),
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
        public void TestBatchQueryViaTableNameAsyncFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null).Result;

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestBatchQueryViaTableNameAsyncFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    0,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null).Result;

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestBatchQueryViaTableNameAsyncThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    2,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null).Result;

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestBatchQueryViaTableNameAsyncThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    2,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null).Result;

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnBatchQueryAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Act
                connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
