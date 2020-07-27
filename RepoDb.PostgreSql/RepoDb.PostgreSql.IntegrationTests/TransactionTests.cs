using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System.Linq;
using System.Transactions;

namespace RepoDb.PostgreSql.IntegrationTests
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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestDbTransactionForBatchQueryAsync()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.BatchQueryAsync<CompleteTable>(0, 10, OrderField.Parse(new { Id = Order.Ascending }), it => it.Id != 0, transaction: transaction).Wait();
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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestDbTransactionForCountAsync()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.CountAsync<CompleteTable>(it => it.Id != 0, transaction: transaction).Wait();
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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestDbTransactionForCountAllAsync()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.CountAllAsync<CompleteTable>(transaction: transaction).Wait();
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

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestDbTransactionForDeleteAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAsync<CompleteTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForDeleteAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAsync<CompleteTable>(entity, transaction: transaction).Wait();

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

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestDbTransactionForDeleteAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAllAsync<CompleteTable>(transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForDeleteAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAllAsync<CompleteTable>(transaction: transaction).Wait();

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

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestDbTransactionForInsertAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAsync<CompleteTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForInsertAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAsync<CompleteTable>(entity, transaction: transaction).Wait();

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

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestDbTransactionForInsertAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAllAsync<CompleteTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForInsertAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAllAsync<CompleteTable>(entities, transaction: transaction).Wait();

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
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestDbTransactionForMergeAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.MergeAsync<CompleteTable>(entity, transaction: transaction).Wait();

                // Act
                transaction.Commit();

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForMergeAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.MergeAsync<CompleteTable>(entity, transaction: transaction).Wait();

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
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestDbTransactionForMergeAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.MergeAllAsync<CompleteTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForMergeAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.MergeAllAsync<CompleteTable>(entities, transaction: transaction).Wait();

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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestDbTransactionForQueryAsync()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryAsync<CompleteTable>(it => it.Id != 0, transaction: transaction).Wait();
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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestDbTransactionForQueryAllAsync()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryAllAsync<CompleteTable>(transaction: transaction).Wait();
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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestDbTransactionForQueryMultipleAsyncT2()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction).Wait();
                }
            }
        }

        [TestMethod]
        public void TestDbTransactionForQueryMultipleAsyncT3()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction).Wait();
                }
            }
        }

        [TestMethod]
        public void TestDbTransactionForQueryMultipleAsyncT4()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction).Wait();
                }
            }
        }

        [TestMethod]
        public void TestDbTransactionForQueryMultipleAsyncT5()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction).Wait();
                }
            }
        }

        [TestMethod]
        public void TestDbTransactionForQueryMultipleAsyncT6()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction).Wait();
                }
            }
        }

        [TestMethod]
        public void TestDbTransactionForQueryMultipleAsyncT7()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction).Wait();
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
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestDbTransactionForTruncateAsync()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.TruncateAsync<CompleteTable>(transaction: transaction).Wait();
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

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBoolean = false;

                    // Act
                    connection.Update<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.Query<CompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual(false, queryResult.First().ColumnBoolean);
            }
        }

        [TestMethod]
        public void TestDbTransactionForUpdateAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBoolean = false;

                    // Act
                    connection.Update<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.Query<CompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual(true, queryResult.First().ColumnBoolean);
            }
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestDbTransactionForUpdateAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBoolean = false;

                    // Act
                    connection.UpdateAsync<CompleteTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.Query<CompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual(false, queryResult.First().ColumnBoolean);
            }
        }

        [TestMethod]
        public void TestDbTransactionForUpdateAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBoolean = false;

                    // Act
                    connection.UpdateAsync<CompleteTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.Query<CompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual(true, queryResult.First().ColumnBoolean);
            }
        }

        #endregion

        #endregion

        #region UpdateAll

        #region UpdateAll

        [TestMethod]
        public void TestDbTransactionForUpdateAllAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBoolean = false);

                    // Act
                    connection.UpdateAll<CompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
            }
        }

        [TestMethod]
        public void TestDbTransactionForUpdateAllAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBoolean = false);

                    // Act
                    connection.UpdateAll<CompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(true, queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
            }
        }

        #endregion

        #region UpdateAllAsync

        [TestMethod]
        public void TestDbTransactionForUpdateAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBoolean = false);

                    // Act
                    connection.UpdateAllAsync<CompleteTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
            }
        }

        [TestMethod]
        public void TestDbTransactionForUpdateAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBoolean = false);

                    // Act
                    connection.UpdateAllAsync<CompleteTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(true, queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
            }
        }

        #endregion

        #endregion

        #endregion

        #region TransactionScope

        #region TransactionScope

        #region InsertAll

        [TestMethod]
        public void TestTransactionForInsertAll()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestTransactionForInsertAllAsync()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new NpgsqlConnection(Database.ConnectionString))
                {
                    // Act
                    connection.InsertAllAsync<CompleteTable>(entities).Wait();

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
        public void TestTransactionScopeForMergeAll()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestTransactionScopeForMergeAllAsync()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new NpgsqlConnection(Database.ConnectionString))
                {
                    // Act
                    connection.MergeAllAsync<CompleteTable>(entities).Wait();

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
        public void TestTransactionScopeForUpdateAll()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new NpgsqlConnection(Database.ConnectionString))
                {
                    // Act
                    connection.InsertAll<CompleteTable>(entities);

                    // Prepare
                    entities.ForEach(entity => entity.ColumnBoolean = false);

                    // Act
                    connection.UpdateAll<CompleteTable>(entities);

                    // Act
                    var queryResult = connection.QueryAll<CompleteTable>();

                    // Assert
                    entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
                }

                // Complete
                transaction.Complete();
            }
        }

        [TestMethod]
        public void TestTransactionScopeForUpdateAllAsync()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new NpgsqlConnection(Database.ConnectionString))
                {
                    // Act
                    connection.InsertAll<CompleteTable>(entities);

                    // Prepare
                    entities.ForEach(entity => entity.ColumnBoolean = false);

                    // Act
                    connection.UpdateAllAsync<CompleteTable>(entities).Wait();

                    // Act
                    var queryResult = connection.QueryAll<CompleteTable>();

                    // Assert
                    entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
                }

                // Complete
                transaction.Complete();
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
