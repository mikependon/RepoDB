#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.AuroraDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Enumerations.AuroraDb;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using RepoDb.Extensions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace RepoDb.AuroraDb.PostgreSql.BulkOperations.IntegrationTests.Operations
{
    /// <summary>
    /// Scenario tests for the bulk operations that are not covered by the per-operation test classes:
    /// transactions, the pseudo-table types, the identity behaviors, the repositories, the type coercion,
    /// the cancellation and the errors raised by the server.
    /// </summary>
    [TestClass]
    public class AuroraDbConnectionBulkScenarioTest
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

        #region SubClasses

        /// <summary>
        /// Maps into <c>BulkOperationNonIdentityTable</c> with CLR types that differ from the column types
        /// (<c>int</c> into <c>BIGINT</c>, <c>long</c> into <c>INTEGER</c>, <c>float</c> into <c>DOUBLE PRECISION</c>
        /// and <c>short</c> into <c>SMALLINT</c>), so the values must be coerced before the binary <c>COPY</c>.
        /// </summary>
        [Map("BulkOperationNonIdentityTable")]
        private class CoercedNonIdentityTable
        {
            public int Id { get; set; }
            public Guid RowGuid { get; set; }
            public short? ColumnBit { get; set; }
            public float? ColumnFloat { get; set; }
            public long? ColumnInt { get; set; }
            public string ColumnNVarChar { get; set; }
        }

        private class IdentityTableRepository : BaseRepository<BulkOperationIdentityTable, AuroraDbConnection>
        {
            public IdentityTableRepository()
                : base(Database.ConnectionString)
            { }
        }

        #endregion

        #region Helpers

        private static long CountIdentityTable() =>
            CountAll<BulkOperationIdentityTable>();

        private static long CountNonIdentityTable() =>
            CountAll<BulkOperationNonIdentityTable>();

        private static long CountAll<TEntity>()
            where TEntity : class
        {
            using var connection = new AuroraDbConnection(Database.ConnectionString);
            return connection.CountAll<TEntity>();
        }

        /// <summary>
        /// Asserts the SQLSTATE of a server error. The errors raised by the staged SQL statements are wrapped into an
        /// <see cref="AuroraDbException"/>, but the errors raised by the binary <c>COPY</c> of <see cref="Connector.AuroraDb.Npgsql.Bulk.AuroraDbBulkCopy"/>
        /// (which runs on the unwrapped Npgsql connection) are thrown as-is, hence both are accepted as a <see cref="DbException"/>.
        /// </summary>
        private static void AssertSqlState(string expected, Exception exception)
        {
            var dbException = exception as DbException ?? exception.InnerException as DbException;
            Assert.IsNotNull(dbException, $"Expected a '{nameof(DbException)}' but was '{exception.GetType().FullName}': {exception.Message}");
            Assert.AreEqual(expected, dbException.SqlState, dbException.Message);
        }

        #endregion

        #region Transactions

        [TestMethod]
        public void TestAuroraDbConnectionBulkInsertWithTransactionCommitted()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                connection.EnsureOpen();
                using (var transaction = (AuroraDbTransaction)connection.BeginTransaction())
                {
                    // Act
                    var result = connection.BulkInsert(tables, transaction: transaction);
                    transaction.Commit();

                    // Assert
                    Assert.AreEqual(tables.Count, result);
                }
            }

            // Assert
            Assert.AreEqual(tables.Count, CountIdentityTable());
        }

        [TestMethod]
        public void TestAuroraDbConnectionBulkInsertWithTransactionRolledBack()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                connection.EnsureOpen();
                using (var transaction = (AuroraDbTransaction)connection.BeginTransaction())
                {
                    // Act
                    var result = connection.BulkInsert(tables, transaction: transaction);
                    transaction.Rollback();

                    // Assert
                    Assert.AreEqual(tables.Count, result);
                }
            }

            // Assert
            Assert.AreEqual(0, CountIdentityTable());
        }

        [TestMethod]
        [DataRow(AuroraDbBulkImportPseudoTableType.Memory)]
        [DataRow(AuroraDbBulkImportPseudoTableType.Physical)]
        public void TestAuroraDbConnectionBulkMergeWithTransactionRolledBack(AuroraDbBulkImportPseudoTableType pseudoTableType)
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                connection.BulkInsert(tables);
            }
            var originals = tables.ToDictionary(t => t.Id, t => t.ColumnNVarChar);
            Helper.UpdateBulkOperationNonIdentityTables(tables);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                connection.EnsureOpen();
                using (var transaction = (AuroraDbTransaction)connection.BeginTransaction())
                {
                    // Act
                    var result = connection.BulkMerge(tables, pseudoTableType: pseudoTableType, transaction: transaction);
                    transaction.Rollback();

                    // Assert
                    Assert.AreEqual(tables.Count, result);
                }

                // Assert (the original values are untouched)
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>().ToList();
                Assert.AreEqual(tables.Count, queryResult.Count);
                queryResult.ForEach(item => Assert.AreEqual(originals[item.Id], item.ColumnNVarChar));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionBulkDeleteByKeyWithTransactionRolledBack()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                connection.BulkInsert(tables);
                connection.EnsureOpen();
                using (var transaction = (AuroraDbTransaction)connection.BeginTransaction())
                {
                    // Act
                    var result = connection.BulkDeleteByKey("BulkOperationNonIdentityTable", tables.Select(t => t.Id), transaction: transaction);
                    transaction.Rollback();

                    // Assert
                    Assert.AreEqual(tables.Count, result);
                }
            }

            // Assert
            Assert.AreEqual(tables.Count, CountNonIdentityTable());
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionBulkUpdateAsyncWithTransactionCommitted()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                await connection.BulkInsertAsync(tables).ConfigureAwait(false);
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                await connection.EnsureOpenAsync().ConfigureAwait(false);
                using (var transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    // Act
                    var result = await connection.BulkUpdateAsync(tables, transaction: (AuroraDbTransaction)transaction).ConfigureAwait(false);
                    await transaction.CommitAsync().ConfigureAwait(false);

                    // Assert
                    Assert.AreEqual(tables.Count, result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>().ToDictionary(t => t.Id);
                tables.ForEach(t => Helper.AssertPropertiesEquality(t, queryResult[t.Id]));
            }
        }

        #endregion

        #region PseudoTableType

        [TestMethod]
        [DataRow(AuroraDbBulkImportPseudoTableType.Auto)]
        [DataRow(AuroraDbBulkImportPseudoTableType.Memory)]
        [DataRow(AuroraDbBulkImportPseudoTableType.Physical)]
        public void TestAuroraDbConnectionBulkMergeForEachPseudoTableType(AuroraDbBulkImportPseudoTableType pseudoTableType)
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BulkMerge(tables,
                    identityBehavior: AuroraDbBulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: pseudoTableType);

                // Assert
                Assert.AreEqual(tables.Count, result);
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>().ToDictionary(t => t.Id);
                tables.ForEach(t => Helper.AssertPropertiesEquality(t, queryResult[t.Id]));
            }
        }

        [TestMethod]
        [DataRow(AuroraDbBulkImportPseudoTableType.Memory)]
        [DataRow(AuroraDbBulkImportPseudoTableType.Physical)]
        public void TestAuroraDbConnectionBulkOperationsDoNotLeaveThePseudoTableBehind(AuroraDbBulkImportPseudoTableType pseudoTableType)
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                connection.BulkMerge(tables, pseudoTableType: pseudoTableType);
                connection.BulkUpdate(tables, pseudoTableType: pseudoTableType);
                connection.BulkDelete(tables, pseudoTableType: pseudoTableType);

                // Assert (only the two test tables exist, no staging table is left in the database)
                var tableNames = connection.ExecuteQuery<string>(@"SELECT table_name FROM information_schema.tables
                    WHERE table_schema = 'public' AND table_type = 'BASE TABLE';").ToList();
                CollectionAssert.AreEquivalent(new[] { "BulkOperationIdentityTable", "BulkOperationNonIdentityTable" }, tableNames);
                Assert.AreEqual(0, CountNonIdentityTable());
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionBulkMergeAboveThePhysicalTableThreshold()
        {
            // Setup (the 'Auto' pseudo-table type switches to a physical table at 5,000 rows)
            var tables = Helper.CreateBulkOperationIdentityTables(5000);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BulkMerge(tables, identityBehavior: AuroraDbBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count, tables.Select(t => t.Id).Distinct().Count());
                Assert.AreEqual(tables.Count, CountIdentityTable());
            }
        }

        #endregion

        #region IdentityBehavior

        [TestMethod]
        public void TestAuroraDbConnectionBulkInsertWithReturnIdentityCorrelatesTheRows()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(1000);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                connection.BulkInsert(tables, identityBehavior: AuroraDbBulkImportIdentityBehavior.ReturnIdentity);

                // Assert (every returned identity points to the row that was inserted from the same entity)
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>().ToDictionary(t => t.Id);
                Assert.AreEqual(tables.Count, queryResult.Count);
                tables.ForEach(t => Assert.AreEqual(t.RowGuid, queryResult[t.Id].RowGuid));
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionBulkMergeAsyncWithReturnIdentityCorrelatesTheRows()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(1000);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                await connection.BulkMergeAsync(tables, identityBehavior: AuroraDbBulkImportIdentityBehavior.ReturnIdentity).ConfigureAwait(false);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>().ToDictionary(t => t.Id);
                Assert.AreEqual(tables.Count, queryResult.Count);
                tables.ForEach(t => Assert.AreEqual(t.RowGuid, queryResult[t.Id].RowGuid));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionBulkInsertWithoutReturnIdentityLeavesTheEntityIdentityUntouched()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                connection.BulkInsert(tables);

                // Assert
                tables.ForEach(t => Assert.AreEqual(0L, t.Id));
                Assert.AreEqual(tables.Count, CountIdentityTable());
            }
        }

        #endregion

        #region BatchSize

        [TestMethod]
        [DataRow(1)]
        [DataRow(7)]
        [DataRow(1000)]
        public void TestAuroraDbConnectionBulkInsertWithBatchSize(int batchSize)
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(100);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BulkInsert(tables, batchSize: batchSize);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.AreEqual(tables.Count, CountIdentityTable());
            }
        }

        #endregion

        #region Partial Matches

        [TestMethod]
        public void TestAuroraDbConnectionBulkUpdateOnlyCountsTheMatchedRows()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                connection.BulkInsert(tables.Take(5));
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var result = connection.BulkUpdate(tables);

                // Assert
                Assert.AreEqual(5, result);
                Assert.AreEqual(5, CountNonIdentityTable());
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionBulkDeleteByKeyOnlyCountsTheExistingKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                connection.BulkInsert(tables);

                // Act
                var keys = tables.Take(3).Select(t => t.Id).Concat(new[] { 1000L, 1001L });
                var result = connection.BulkDeleteByKey("BulkOperationNonIdentityTable", keys);

                // Assert
                Assert.AreEqual(3, result);
                Assert.AreEqual(7, CountNonIdentityTable());
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionBulkMergeInsertsAndUpdatesInOneCall()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                connection.BulkInsert(tables.Take(4));
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var result = connection.BulkMerge(tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>().ToDictionary(t => t.Id);
                Assert.AreEqual(tables.Count, queryResult.Count);
                tables.ForEach(t => Helper.AssertPropertiesEquality(t, queryResult[t.Id]));
            }
        }

        #endregion

        #region Values

        [TestMethod]
        public void TestAuroraDbConnectionBulkInsertWithNullValues()
        {
            // Setup
            var tables = Enumerable.Range(1, 5).Select(i => new BulkOperationNonIdentityTable
            {
                Id = i,
                RowGuid = Guid.NewGuid()
            }).ToList();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                connection.BulkInsert(tables);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>().ToList();
                Assert.AreEqual(tables.Count, queryResult.Count);
                queryResult.ForEach(item =>
                {
                    Assert.IsNull(item.ColumnBit);
                    Assert.IsNull(item.ColumnDateTime);
                    Assert.IsNull(item.ColumnDecimal);
                    Assert.IsNull(item.ColumnFloat);
                    Assert.IsNull(item.ColumnInt);
                    Assert.IsNull(item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionBulkInsertWithUnicodeAndLongText()
        {
            // Setup
            var texts = new[] { "Hello, 世界", "Привет мир", "🐘🚀✨", "tab\tnew\r\nline 'quote' \"double\" \\back", new string('x', 2000) };
            var tables = texts.Select((text, i) => new BulkOperationNonIdentityTable
            {
                Id = i + 1,
                RowGuid = Guid.NewGuid(),
                ColumnNVarChar = text
            }).ToList();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                connection.BulkInsert(tables);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>().ToDictionary(t => t.Id);
                tables.ForEach(t => Assert.AreEqual(t.ColumnNVarChar, queryResult[t.Id].ColumnNVarChar));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionBulkInsertCoercesTheValuesToTheColumnTypes()
        {
            // Setup
            var tables = Enumerable.Range(1, 10).Select(i => new CoercedNonIdentityTable
            {
                Id = i,
                RowGuid = Guid.NewGuid(),
                ColumnBit = 1,
                ColumnFloat = i + 0.5f,
                ColumnInt = i * 10L,
                ColumnNVarChar = $"Coerced{i}"
            }).ToList();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>().ToDictionary(t => t.Id);
                tables.ForEach(t =>
                {
                    var item = queryResult[t.Id];
                    Assert.AreEqual(t.RowGuid, item.RowGuid);
                    Assert.AreEqual((double)t.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual((int)t.ColumnInt, item.ColumnInt);
                    Assert.AreEqual(t.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        #endregion

        #region Repositories

        [TestMethod]
        public void TestBaseRepositoryBulkInsertAndBulkDelete()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new IdentityTableRepository())
            {
                // Act
                var insertResult = repository.BulkInsert(tables, identityBehavior: AuroraDbBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, insertResult);
                Assert.IsTrue(tables.All(t => t.Id > 0));

                // Act
                var deleteResult = repository.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, deleteResult);
                Assert.AreEqual(0, CountIdentityTable());
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryBulkMergeAsyncAndBulkDeleteByKey()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var repository = new DbRepository<AuroraDbConnection>(Database.ConnectionString))
            {
                // Act
                var mergeResult = await repository.BulkMergeAsync(tables).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count, mergeResult);

                // Act
                var deleteResult = repository.BulkDeleteByKey("BulkOperationNonIdentityTable", tables.Select(t => t.Id));

                // Assert
                Assert.AreEqual(tables.Count, deleteResult);
                Assert.AreEqual(0, CountNonIdentityTable());
            }
        }

        #endregion

        #region TransactionScope

        [TestMethod]
        public void TestAuroraDbConnectionBulkInsertInsideTransactionScope()
        {
            // Setup
            Database.AssumeAuroraCluster();
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Act
            using (var scope = new TransactionScope())
            {
                using (var connection = new AuroraDbConnection(Database.ConnectionString))
                {
                    connection.BulkInsert(tables);
                }
                scope.Complete();
            }

            // Assert
            Assert.AreEqual(tables.Count, CountIdentityTable());
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionBulkInsertInsideTransactionScopeIfTheServerIsNotAnAuroraCluster()
        {
            // Setup
            Database.AssumeNonAuroraServer();
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Act/Assert (the dialect probe of the AWS wrapper aborts the ambient transaction)
            using (var scope = new TransactionScope())
            {
                using (var connection = new AuroraDbConnection(Database.ConnectionString))
                {
                    var exception = Assert.Throws<Exception>(() => connection.BulkInsert(tables));
                    AssertSqlState("25P02", exception);
                }
            }

            // Assert
            Assert.AreEqual(0, CountIdentityTable());
        }

        #endregion

        #region Negative

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionBulkInsertIfThePrimaryKeyAlreadyExists()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                connection.BulkInsert(tables);

                // Act/Assert
                var exception = Assert.Throws<Exception>(() => connection.BulkInsert(tables));
                AssertSqlState("23505", exception);

                // Assert (nothing from the failed batch was written)
                Assert.AreEqual(tables.Count, CountNonIdentityTable());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionBulkInsertIfThePrimaryKeyIsDuplicatedWithinTheBatch()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            tables[9].Id = tables[0].Id;

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act/Assert
                var exception = Assert.Throws<Exception>(() => connection.BulkInsert(tables));
                AssertSqlState("23505", exception);
                Assert.AreEqual(0, CountNonIdentityTable());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionBulkMergeIfThePrimaryKeyIsDuplicatedWithinTheBatch()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            tables[9].Id = tables[0].Id;

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act/Assert (the rows that do not exist yet are inserted in one statement, which then collide with each other)
                var exception = Assert.Throws<Exception>(() => connection.BulkMerge(tables));
                AssertSqlState("23505", exception);
                Assert.AreEqual(0, CountNonIdentityTable());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionBulkInsertIfTheTextIsTooLong()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(3);
            tables[1].ColumnNVarChar = new string('x', 2001);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act/Assert (VARCHAR(2000))
                var exception = Assert.Throws<Exception>(() => connection.BulkInsert(tables));
                AssertSqlState("22001", exception);
                Assert.AreEqual(0, CountNonIdentityTable());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionBulkInsertIfANotNullColumnIsNull()
        {
            // Setup
            var table = new DataTable("BulkOperationNonIdentityTable");
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("RowGuid", typeof(Guid));
            table.Rows.Add(1L, Guid.NewGuid());
            table.Rows.Add(2L, DBNull.Value);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act/Assert
                var exception = Assert.Throws<Exception>(() => connection.BulkInsert("BulkOperationNonIdentityTable", table));
                AssertSqlState("23502", exception);
                Assert.AreEqual(0, CountNonIdentityTable());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionBulkInsertIfTheTableDoesNotExist()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(3);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act/Assert
                Assert.Throws<Exception>(() => connection.BulkInsert("NonExistingTable", tables));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionBulkMergeIfTheQualifierDoesNotExist()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(3);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act/Assert
                Assert.Throws<Exception>(() => connection.BulkMerge("BulkOperationNonIdentityTable", tables,
                    qualifiers: Field.From("NonExistingColumn")));
                Assert.AreEqual(0, CountNonIdentityTable());
            }
        }

        /*
         * A transaction that has already been committed (or rolled back) is not rejected: the rows are written outside of any
         * transaction (auto-committed). AuroraDbTransaction does not expose its completion state, so the bulk operations cannot
         * detect it. These tests pin that behavior (see the LIMITATIONS page).
         */

        [TestMethod]
        public void TestAuroraDbConnectionBulkInsertWithACompletedTransactionIsAutoCommitted()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(3);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                connection.EnsureOpen();
                var transaction = (AuroraDbTransaction)connection.BeginTransaction();
                transaction.Commit();

                // Act
                var result = connection.BulkInsert(tables, transaction: transaction);

                // Assert
                Assert.AreEqual(tables.Count, result);
                Assert.Throws<InvalidOperationException>(() => transaction.Rollback());
            }

            // Assert
            Assert.AreEqual(tables.Count, CountIdentityTable());
        }

        [TestMethod]
        public void TestAuroraDbConnectionBulkMergeWithACompletedTransactionIsAutoCommitted()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(3);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                connection.EnsureOpen();
                var transaction = (AuroraDbTransaction)connection.BeginTransaction();
                transaction.Rollback();

                // Act
                var result = connection.BulkMerge(tables, transaction: transaction);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }

            // Assert
            Assert.AreEqual(tables.Count, CountNonIdentityTable());
        }

        [TestMethod]
        public async Task ThrowExceptionOnAuroraDbConnectionBulkInsertAsyncIfTheTokenIsCancelled()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            using var cancellationTokenSource = new CancellationTokenSource();
            await cancellationTokenSource.CancelAsync().ConfigureAwait(false);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                    await connection.BulkInsertAsync(tables, cancellationToken: cancellationTokenSource.Token).ConfigureAwait(false)).ConfigureAwait(false);
            }

            // Assert
            Assert.AreEqual(0, CountIdentityTable());
        }

        [TestMethod]
        public async Task ThrowExceptionOnAuroraDbConnectionBulkMergeAsyncIfTheTokenIsCancelled()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            using var cancellationTokenSource = new CancellationTokenSource();
            await cancellationTokenSource.CancelAsync().ConfigureAwait(false);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                    await connection.BulkMergeAsync(tables, cancellationToken: cancellationTokenSource.Token).ConfigureAwait(false)).ConfigureAwait(false);
            }

            // Assert
            Assert.AreEqual(0, CountNonIdentityTable());
        }

        #endregion
    }
}
