#region Copyright Attributions

// Copyright (c) 2019 Bradley Graigner and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuckDB.NET.Data;
using RepoDb.Extensions;
using RepoDb.DuckDb.IntegrationTests.Models;
using RepoDb.DuckDb.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.DuckDb.IntegrationTests.Operations
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
        public void TestDuckDBConnectionBatchQueryFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
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
        public void TestDuckDBConnectionBatchQueryFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
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
        public void TestDuckDBConnectionBatchQueryThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
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
        public void TestDuckDBConnectionBatchQueryThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnDuckDBConnectionBatchQueryWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
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
        public async Task TestDuckDBConnectionBatchQueryAsyncFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync<CompleteTable>(0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null).ConfigureAwait(false);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionBatchQueryAsyncFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync<CompleteTable>(0,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null).ConfigureAwait(false);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionBatchQueryAsyncThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync<CompleteTable>(2,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null).ConfigureAwait(false);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionBatchQueryAsyncThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync<CompleteTable>(2,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null).ConfigureAwait(false);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnDuckDBConnectionBatchQueryAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.BatchQueryAsync<CompleteTable>(0,
                        3,
                        OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                        (object)null,
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestDuckDBConnectionBatchQueryViaTableNameFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
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
        public void TestDuckDBConnectionBatchQueryViaTableNameFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
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
        public void TestDuckDBConnectionBatchQueryViaTableNameThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
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
        public void TestDuckDBConnectionBatchQueryViaTableNameThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnDuckDBConnectionBatchQueryViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
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
        public async Task TestDuckDBConnectionBatchQueryViaTableNameAsyncFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null).ConfigureAwait(false);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionBatchQueryViaTableNameAsyncFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    0,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null).ConfigureAwait(false);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionBatchQueryViaTableNameAsyncThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    2,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null).ConfigureAwait(false);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestDuckDBConnectionBatchQueryViaTableNameAsyncThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    2,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null).ConfigureAwait(false);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnDuckDBConnectionBatchQueryAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.BatchQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        0,
                        3,
                        OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                        (object)null,
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}
