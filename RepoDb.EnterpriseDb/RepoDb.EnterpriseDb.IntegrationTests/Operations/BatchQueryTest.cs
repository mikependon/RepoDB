using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnterpriseDB.EDBClient;
using RepoDb.Extensions;
using RepoDb.EnterpriseDb.IntegrationTests.Models;
using RepoDb.EnterpriseDb.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.EnterpriseDb.IntegrationTests.Operations
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
        public void TestEnterpriseDbConnectionBatchQueryFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionBatchQueryFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionBatchQueryThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionBatchQueryThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbConnectionBatchQueryWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.BatchQuery<CompleteTable>(0,
                        3,
                        OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestEnterpriseDbConnectionBatchQueryAsyncFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync<CompleteTable>(0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionBatchQueryAsyncFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync<CompleteTable>(0,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionBatchQueryAsyncThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync<CompleteTable>(2,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionBatchQueryAsyncThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync<CompleteTable>(2,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnEnterpriseDbConnectionBatchQueryAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.BatchQueryAsync<CompleteTable>(0,
                        3,
                        OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestEnterpriseDbConnectionBatchQueryViaTableNameFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionBatchQueryViaTableNameFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionBatchQueryViaTableNameThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionBatchQueryViaTableNameThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbConnectionBatchQueryViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.BatchQuery(ClassMappedNameCache.Get<CompleteTable>(),
                        0,
                        3,
                        OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestEnterpriseDbConnectionBatchQueryViaTableNameAsyncFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
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
        public async Task TestEnterpriseDbConnectionBatchQueryViaTableNameAsyncFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
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
        public async Task TestEnterpriseDbConnectionBatchQueryViaTableNameAsyncThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
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
        public async Task TestEnterpriseDbConnectionBatchQueryViaTableNameAsyncThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    2,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnEnterpriseDbConnectionBatchQueryAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        0,
                        3,
                        OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion
    }
}
