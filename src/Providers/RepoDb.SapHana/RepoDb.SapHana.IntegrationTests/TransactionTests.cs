#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.Enumerations;
using RepoDb.SapHana.IntegrationTests.Models;
using RepoDb.SapHana.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace RepoDb.SapHana.IntegrationTests
{
    [TestClass]
    public class TransactionTests
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

        /*
         * Some tests here are only triggers (ie: BatchQuery, Count, CountAll, Query, QueryAll, Truncate)
         */

        #region DbTransaction

        #region BatchQuery

        #region BatchQuery

        [TestMethod]
        public void TestDbTransactionForBatchQuery()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.BatchQuery<CompleteTable>(0, 10, OrderField.Parse(new { Id = Order.Ascending }), it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public async Task TestDbTransactionForBatchQueryAsync()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.BatchQueryAsync<CompleteTable>(0, 10, OrderField.Parse(new { Id = Order.Ascending }), it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #endregion  

        #region Count

        #region Count

        [TestMethod]
        public void TestDbTransactionForCount()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Count<CompleteTable>(it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public async Task TestDbTransactionForCountAsync()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.CountAsync<CompleteTable>(it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #endregion  

        #region CountAll

        #region CountAll

        [TestMethod]
        public void TestDbTransactionForCountAll()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.CountAll<CompleteTable>(transaction: transaction);
                }
            }
        }

        #endregion

        #region CountAllAsync

        [TestMethod]
        public async Task TestDbTransactionForCountAllAsync()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.CountAllAsync<CompleteTable>(transaction: transaction);
                }
            }
        }

        #endregion

        #endregion

        #region Delete

        #region Delete

        [TestMethod]
        public void TestDbTransactionForDeleteAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Delete<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForDeleteAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Delete<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task TestDbTransactionForDeleteAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.DeleteAsync<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForDeleteAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.DeleteAsync<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #endregion

        #region DeleteAll

        #region DeleteAll

        [TestMethod]
        public void TestDbTransactionForDeleteAllAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAll<CompleteTable>(transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForDeleteAllAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAll<CompleteTable>(transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public async Task TestDbTransactionForDeleteAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.DeleteAllAsync<CompleteTable>(transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForDeleteAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.DeleteAllAsync<CompleteTable>(transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #endregion

        #region Insert

        #region Insert

        [TestMethod]
        public void TestDbTransactionForInsertAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Insert<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForInsertAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Insert<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public async Task TestDbTransactionForInsertAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.InsertAsync<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForInsertAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.InsertAsync<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #endregion

        #region InsertAll

        #region InsertAll

        [TestMethod]
        public void TestDbTransactionForInsertAllAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAll<CompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForInsertAllAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAll<CompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #region InsertAllAsync

        [TestMethod]
        public async Task TestDbTransactionForInsertAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.InsertAllAsync<CompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForInsertAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.InsertAllAsync<CompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #endregion

        #region Merge

        #region Merge

        [TestMethod]
        public void TestDbTransactionForMergeAsCommitted()
        {
            // Setup - HANA's UPSERT ... WITH PRIMARY KEY needs a concrete key to match/insert against,
            // unlike an INSERT where the identity column is simply omitted and auto-generated.
            var entity = Helper.CreateCompleteTables(1).First();
            entity.Id = 1;

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Merge<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForMergeAsRolledBack()
        {
            // Setup - see the remark on TestDbTransactionForMergeAsCommitted re: the explicit Id.
            var entity = Helper.CreateCompleteTables(1).First();
            entity.Id = 1;

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Merge<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public async Task TestDbTransactionForMergeAsyncAsCommitted()
        {
            // Setup - see the remark on TestDbTransactionForMergeAsCommitted re: the explicit Id.
            var entity = Helper.CreateCompleteTables(1).First();
            entity.Id = 1;

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                await connection.MergeAsync<CompleteTable>(entity, transaction: transaction);

                // Act
                transaction.Commit();

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForMergeAsyncAsRolledBack()
        {
            // Setup - see the remark on TestDbTransactionForMergeAsCommitted re: the explicit Id.
            var entity = Helper.CreateCompleteTables(1).First();
            entity.Id = 1;

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                await connection.MergeAsync<CompleteTable>(entity, transaction: transaction);

                // Act
                transaction.Rollback();

                // Assert
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #endregion

        #region MergeAll

        #region MergeAll

        [TestMethod]
        public void TestDbTransactionForMergeAllAsCommitted()
        {
            // Setup - see the remark on TestDbTransactionForMergeAsCommitted re: the explicit Ids.
            var entities = Helper.CreateCompleteTables(10);
            for (var i = 0; i < entities.Count; i++) entities[i].Id = i + 1;

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.MergeAll<CompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForMergeAllAsRolledBack()
        {
            // Setup - see the remark on TestDbTransactionForMergeAsCommitted re: the explicit Ids.
            var entities = Helper.CreateCompleteTables(10);
            for (var i = 0; i < entities.Count; i++) entities[i].Id = i + 1;

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.MergeAll<CompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #region MergeAllAsync

        [TestMethod]
        public async Task TestDbTransactionForMergeAllAsyncAsCommitted()
        {
            // Setup - see the remark on TestDbTransactionForMergeAsCommitted re: the explicit Ids.
            var entities = Helper.CreateCompleteTables(10);
            for (var i = 0; i < entities.Count; i++) entities[i].Id = i + 1;

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.MergeAllAsync<CompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForMergeAllAsyncAsRolledBack()
        {
            // Setup - see the remark on TestDbTransactionForMergeAsCommitted re: the explicit Ids.
            var entities = Helper.CreateCompleteTables(10);
            for (var i = 0; i < entities.Count; i++) entities[i].Id = i + 1;

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.MergeAllAsync<CompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
        }

        #endregion

        #endregion

        #region Query

        #region Query

        [TestMethod]
        public void TestDbTransactionForQuery()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Query<CompleteTable>(it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public async Task TestDbTransactionForQueryAsync()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryAsync<CompleteTable>(it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #endregion

        #region QueryAll

        #region QueryAll

        [TestMethod]
        public void TestDbTransactionForQueryAll()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryAll<CompleteTable>(transaction: transaction);
                }
            }
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public async Task TestDbTransactionForQueryAllAsync()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryAllAsync<CompleteTable>(transaction: transaction);
                }
            }
        }

        #endregion

        #endregion

        #region QueryMultiple

        #region QueryMultiple

        [TestMethod]
        public void TestDbTransactionForQueryMultipleT2()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public void TestDbTransactionForQueryMultipleT3()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public void TestDbTransactionForQueryMultipleT4()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public void TestDbTransactionForQueryMultipleT5()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public void TestDbTransactionForQueryMultipleT6()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public void TestDbTransactionForQueryMultipleT7()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        #endregion

        #region QueryMultipleAsync

        [TestMethod]
        public async Task TestDbTransactionForQueryMultipleAsyncT2()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForQueryMultipleAsyncT3()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForQueryMultipleAsyncT4()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForQueryMultipleAsyncT5()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForQueryMultipleAsyncT6()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForQueryMultipleAsyncT7()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        #endregion

        #endregion

        #region Truncate

        #region Truncate

        [TestMethod]
        public void TestDbTransactionForTruncate()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Truncate<CompleteTable>(transaction: transaction);
                }
            }
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public async Task TestDbTransactionForTruncateAsync()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.TruncateAsync<CompleteTable>(transaction: transaction);
                }
            }
        }

        #endregion

        #endregion

        #region Update

        #region Update

        [TestMethod]
        public void TestDbTransactionForUpdateAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBit = false;

                    // Act
                    connection.Update<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.Query<CompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual(false, queryResult.First().ColumnBit);
            }
        }

        [TestMethod]
        public void TestDbTransactionForUpdateAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBit = false;

                    // Act
                    connection.Update<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.Query<CompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual(true, queryResult.First().ColumnBit);
            }
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public async Task TestDbTransactionForUpdateAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBit = false;

                    // Act
                    await connection.UpdateAsync<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.Query<CompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual(false, queryResult.First().ColumnBit);
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForUpdateAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBit = false;

                    // Act
                    await connection.UpdateAsync<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.Query<CompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual(true, queryResult.First().ColumnBit);
            }
        }

        #endregion

        #endregion

        #region UpdateAll

        #region UpdateAll

        [TestMethod]
        [Ignore("HANA binds command parameters by position, not by name. UpdateAll (with no explicit fields/qualifiers) bundles the primary-key parameter into the same physically-ordered list as " +
            "the SET-column parameters (RepoDb.Core's UpdateAllExecutionContextProvider), but HANA's UPDATE " +
            "syntax forces WHERE - and so the key's placeholder - to appear textually last. No SQL " +
            "restructuring (CTE prefix, updatable subquery) HANA accepts can reorder around that.")]
        public void TestDbTransactionForUpdateAllAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    connection.UpdateAll<CompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBit));
            }
        }

        [TestMethod]
        [Ignore("See the remark on TestDbTransactionForUpdateAllAsCommitted.")]
        public void TestDbTransactionForUpdateAllAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    connection.UpdateAll<CompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(true, queryResult.First(item => item.Id == entity.Id).ColumnBit));
            }
        }

        #endregion

        #region UpdateAllAsync

        [TestMethod]
        [Ignore("See the remark on TestDbTransactionForUpdateAllAsCommitted.")]
        public async Task TestDbTransactionForUpdateAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    await connection.UpdateAllAsync<CompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBit));
            }
        }

        [TestMethod]
        [Ignore("See the remark on TestDbTransactionForUpdateAllAsCommitted.")]
        public async Task TestDbTransactionForUpdateAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    await connection.UpdateAllAsync<CompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(true, queryResult.First(item => item.Id == entity.Id).ColumnBit));
            }
        }

        #endregion

        #endregion

        #endregion

        #region TransactionScope

        #region InsertAll

        [TestMethod]
        [Ignore("HANA's ADO.NET client enlists in System.Transactions.Transaction.Current as a " +
            "durable/distributed participant rather than a lightweight promotable one, so any use of " +
            "TransactionScope with a HanaConnection - even a single connection - immediately requires " +
            "MSDTC. That's a Windows-only, COM-based coordinator with no bridge into a Linux Docker " +
            "container, so it cannot work here regardless of local MSDTC configuration.")]
        public void TestTransactionForInsertAll()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new HanaConnection(Database.ConnectionString))
                {
                    // Act
                    connection.InsertAll<CompleteTable>(entities);

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
                }

                // Complete
                transaction.Complete();
            }
        }

        [TestMethod]
        [Ignore("See the remark on TestTransactionForInsertAll.")]
        public async Task TestTransactionForInsertAllAsync()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var connection = new HanaConnection(Database.ConnectionString))
                {
                    // Act
                    await connection.InsertAllAsync<CompleteTable>(entities);

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
                }

                // Complete
                transaction.Complete();
            }
        }

        #endregion

        #region MergeAll

        [TestMethod]
        [Ignore("See the remark on TestTransactionForInsertAll.")]
        public void TestTransactionScopeForMergeAll()
        {
            // Setup - see the remark on TestDbTransactionForMergeAsCommitted re: the explicit Ids.
            var entities = Helper.CreateCompleteTables(10);
            for (var i = 0; i < entities.Count; i++) entities[i].Id = i + 1;

            using (var transaction = new TransactionScope())
            {
                using (var connection = new HanaConnection(Database.ConnectionString))
                {
                    // Act
                    connection.MergeAll<CompleteTable>(entities);

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
                }

                // Complete
                transaction.Complete();
            }
        }

        [TestMethod]
        [Ignore("See the remark on TestTransactionForInsertAll.")]
        public async Task TestTransactionScopeForMergeAllAsync()
        {
            // Setup - see the remark on TestDbTransactionForMergeAsCommitted re: the explicit Ids.
            var entities = Helper.CreateCompleteTables(10);
            for (var i = 0; i < entities.Count; i++) entities[i].Id = i + 1;

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var connection = new HanaConnection(Database.ConnectionString))
                {
                    // Act
                    await connection.MergeAllAsync<CompleteTable>(entities);

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
                }

                // Complete
                transaction.Complete();
            }
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        [Ignore("See the remark on TestDbTransactionForUpdateAllAsCommitted.")]
        public void TestTransactionScopeForUpdateAll()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new HanaConnection(Database.ConnectionString))
                {
                    // Act
                    connection.InsertAll<CompleteTable>(entities);

                    // Prepare
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    connection.UpdateAll<CompleteTable>(entities);

                    // Act
                    var queryResult = connection.QueryAll<CompleteTable>();

                    // Assert
                    entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBit));
                }

                // Complete
                transaction.Complete();
            }
        }

        [TestMethod]
        [Ignore("See the remark on TestDbTransactionForUpdateAllAsCommitted.")]
        public async Task TestTransactionScopeForUpdateAllAsync()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var connection = new HanaConnection(Database.ConnectionString))
                {
                    // Act
                    connection.InsertAll<CompleteTable>(entities);

                    // Prepare
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    await connection.UpdateAllAsync<CompleteTable>(entities);

                    // Act
                    var queryResult = connection.QueryAll<CompleteTable>();

                    // Assert
                    entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBit));
                }

                // Complete
                transaction.Complete();
            }
        }

        #endregion

        #endregion
    }
}
