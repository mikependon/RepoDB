#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.AuroraDb.MySqlConnector.IntegrationTests.Models;
using RepoDb.AuroraDb.MySqlConnector.IntegrationTests.Setup;
using RepoDb.Connector.AuroraDb.MySqlConnector;
using RepoDb.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace RepoDb.AuroraDb.MySqlConnector.IntegrationTests
{
    /// <summary>
    /// Scenario tests that are not covered by the per-operation test classes: the identity correlation of the
    /// batched operations, the transactions, the character set, the repositories, and the errors raised by the
    /// server through the <see cref="AuroraDbException"/> of the connector.
    /// </summary>
    [TestClass]
    public class ScenarioTests
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

        private class CompleteTableRepository : BaseRepository<CompleteTable, AuroraDbConnection>
        {
            public CompleteTableRepository()
                : base(Database.ConnectionString)
            { }
        }

        #endregion

        #region Helpers

        private static long CountCompleteTable()
        {
            using var connection = new AuroraDbConnection(Database.ConnectionString);
            return connection.CountAll<CompleteTable>();
        }

        private static long CountNonIdentityCompleteTable()
        {
            using var connection = new AuroraDbConnection(Database.ConnectionString);
            return connection.CountAll<NonIdentityCompleteTable>();
        }

        private static void AssertError(int expectedNumber, string expectedSqlState, Exception exception)
        {
            var auroraDbException = exception as AuroraDbException ?? exception.InnerException as AuroraDbException;
            Assert.IsNotNull(auroraDbException, $"Expected an '{nameof(AuroraDbException)}' but was '{exception.GetType().FullName}': {exception.Message}");
            Assert.AreEqual(expectedNumber, auroraDbException.Number, auroraDbException.Message);
            Assert.AreEqual(expectedSqlState, auroraDbException.SqlState, StringComparer.Ordinal, auroraDbException.Message);
        }

        #endregion

        #region Identity

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public void TestAuroraDbConnectionInsertAllReturnsTheCorrelatedIdentities(int batchSize)
        {
            // Setup
            var tables = Helper.CreateCompleteTables(250);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables, batchSize: batchSize);

                // Assert (every returned identity points to the row that was inserted from the same entity)
                var queryResult = connection.QueryAll<CompleteTable>().ToDictionary(t => t.Id.Value);
                Assert.AreEqual(tables.Count, queryResult.Count);
                tables.ForEach(t => Assert.AreEqual(t.ColumnVarchar, queryResult[t.Id.Value].ColumnVarchar, StringComparer.Ordinal));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionMergeAllInsertsAndUpdatesWithoutChangingTheIdentities()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables.Take(4));
                var existingIds = tables.Take(4).Select(t => t.Id.Value).ToList();
                tables.ForEach(t => t.ColumnVarchar = $"Merged:{t.ColumnInt}");

                // Act
                var result = connection.MergeAll(tables);

                // Assert
                Assert.AreEqual(tables.Count, result);
                CollectionAssert.AreEqual(existingIds, tables.Take(4).Select(t => t.Id.Value).ToList());
                var queryResult = connection.QueryAll<CompleteTable>().ToDictionary(t => t.Id.Value);
                Assert.AreEqual(tables.Count, queryResult.Count);
                tables.ForEach(t => Assert.AreEqual(t.ColumnVarchar, queryResult[t.Id.Value].ColumnVarchar, StringComparer.Ordinal));
            }
        }

        #endregion

        #region Transactions

        [TestMethod]
        public void TestAuroraDbConnectionInsertAllWithTransactionRolledBack()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                connection.EnsureOpen();
                using (var transaction = connection.BeginTransaction())
                {
                    // Act
                    var result = connection.InsertAll(tables, transaction: transaction);
                    transaction.Rollback();

                    // Assert
                    Assert.AreEqual(tables.Count, result);
                }
            }

            // Assert
            Assert.AreEqual(0, CountCompleteTable());
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionMergeAllAsyncWithTransactionCommitted()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                await connection.EnsureOpenAsync().ConfigureAwait(false);
                using (var transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    // Act
                    var result = await connection.MergeAllAsync(tables, transaction: transaction).ConfigureAwait(false);
                    await transaction.CommitAsync().ConfigureAwait(false);

                    // Assert
                    Assert.AreEqual(tables.Count, result);
                }
            }

            // Assert
            Assert.AreEqual(tables.Count, CountNonIdentityCompleteTable());
        }

        [TestMethod]
        public void TestAuroraDbConnectionInsideTransactionScopeCompleted()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            // Act
            using (var scope = new TransactionScope())
            {
                using (var connection = new AuroraDbConnection(Database.ConnectionString))
                {
                    connection.InsertAll(tables);
                }
                scope.Complete();
            }

            // Assert
            Assert.AreEqual(tables.Count, CountCompleteTable());
        }

        [TestMethod]
        public void TestAuroraDbConnectionInsideTransactionScopeNotCompleted()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            // Act (the scope is disposed without calling Complete(), hence it is rolled back)
            using (new TransactionScope())
            {
                using (var connection = new AuroraDbConnection(Database.ConnectionString))
                {
                    connection.InsertAll(tables);
                    Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                }
            }

            // Assert
            Assert.AreEqual(0, CountCompleteTable());
        }

        #endregion

        #region Values

        [TestMethod]
        public void TestAuroraDbConnectionInsertAndQueryWithUnicodeText()
        {
            // Setup
            var texts = new[] { "Hello, 世界", "Привет мир", "🐬🚀✨", "tab\tnew\r\nline 'quote' \"double\" \\back `tick`" };

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                foreach (var text in texts)
                {
                    // Act
                    var id = connection.Insert<CompleteTable, long>(new CompleteTable { ColumnVarchar = text });
                    var result = connection.Query<CompleteTable>(id).First();

                    // Assert
                    Assert.AreEqual(text, result.ColumnVarchar, StringComparer.Ordinal);
                }
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionInsertAndQueryWithNullValues()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var id = connection.Insert<CompleteTable, long>(new CompleteTable());
                var result = connection.Query<CompleteTable>(id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnVarchar);
                Assert.IsNull(result.ColumnInt);
                Assert.IsNull(result.ColumnDecimal2);
                Assert.IsNull(result.ColumnDateTime);
            }
        }

        #endregion

        #region Repositories

        [TestMethod]
        public void TestBaseRepositoryInsertAllQueryAndDeleteAll()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var repository = new CompleteTableRepository())
            {
                // Act
                repository.InsertAll(tables);
                var queryResult = repository.QueryAll().ToList();
                var deleteResult = repository.DeleteAll();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count);
                Assert.AreEqual(tables.Count, deleteResult);
                Assert.AreEqual(0, CountCompleteTable());
            }
        }

        [TestMethod]
        public async Task TestDbRepositoryMergeAllAsyncAndCountAll()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var repository = new DbRepository<AuroraDbConnection>(Database.ConnectionString))
            {
                // Act
                var mergeResult = await repository.MergeAllAsync(tables).ConfigureAwait(false);
                var countResult = await repository.CountAllAsync<NonIdentityCompleteTable>().ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count, mergeResult);
                Assert.AreEqual(tables.Count, countResult);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionInsertIfThePrimaryKeyAlreadyExists()
        {
            // Setup
            var table = Helper.CreateNonIdentityCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                connection.Insert(table);

                // Act/Assert (ER_DUP_ENTRY)
                var exception = Assert.Throws<Exception>(() => connection.Insert(table));
                AssertError(1062, "23000", exception);
                Assert.AreEqual(1, CountNonIdentityCompleteTable());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionInsertAllIfThePrimaryKeyIsDuplicatedWithinTheBatch()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);
            tables[9].Id = tables[0].Id;

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                connection.EnsureOpen();
                using (var transaction = connection.BeginTransaction())
                {
                    // Act/Assert (ER_DUP_ENTRY)
                    var exception = Assert.Throws<Exception>(() => connection.InsertAll(tables, transaction: transaction));
                    AssertError(1062, "23000", exception);
                    transaction.Rollback();
                }
            }

            // Assert
            Assert.AreEqual(0, CountNonIdentityCompleteTable());
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionInsertIfTheTextIsTooLong()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act/Assert (ER_DATA_TOO_LONG, as the STRICT_TRANS_TABLES mode is on by default in MySQL 8.0)
                var exception = Assert.Throws<Exception>(() =>
                    connection.Insert(new CompleteTable { ColumnVarchar = new string('x', 257) }));
                AssertError(1406, "22001", exception);
                Assert.AreEqual(0, CountCompleteTable());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionQueryIfTheTableDoesNotExist()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act/Assert (ER_NO_SUCH_TABLE)
                var exception = Assert.Throws<Exception>(() =>
                    connection.ExecuteQuery("SELECT * FROM `NonExistingTable`;").ToList());
                AssertError(1146, "42S02", exception);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionExecuteNonQueryIfTheSyntaxIsInvalid()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act/Assert (ER_PARSE_ERROR)
                var exception = Assert.Throws<Exception>(() => connection.ExecuteNonQuery("SELEC * FROM `CompleteTable`;"));
                AssertError(1064, "42000", exception);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionOpenIfThePasswordIsInvalid()
        {
            // Setup
            var builder = new AuroraDbConnectionStringBuilder(Database.ConnectionString)
            {
                Password = "NotTheRightPassword"
            };

            using (var connection = new AuroraDbConnection(builder.ConnectionString))
            {
                // Act/Assert (ER_ACCESS_DENIED_ERROR). Unlike the command errors, a failure to open the connection is not
                // wrapped into an AuroraDbException by the connector; the underlying MySqlException is thrown as-is.
                var exception = Assert.Throws<System.Data.Common.DbException>(() => connection.Open());
                Assert.IsNotInstanceOfType<AuroraDbException>(exception);
                Assert.AreEqual("28000", exception.SqlState, StringComparer.Ordinal, exception.Message);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnAuroraDbConnectionQueryAllAsyncIfTheTokenIsCancelled()
        {
            // Setup
            using var cancellationTokenSource = new CancellationTokenSource();
            await cancellationTokenSource.CancelAsync().ConfigureAwait(false);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                    await connection.QueryAllAsync<CompleteTable>(cancellationToken: cancellationTokenSource.Token).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionBeginTransactionIfTheConnectionIsClosed()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => connection.BeginTransaction());
            }
        }

        #endregion
    }
}
