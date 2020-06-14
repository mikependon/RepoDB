using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.Enumerations;
using RepoDb.MySqlConnector.IntegrationTests.Models;
using RepoDb.MySqlConnector.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.MySqlConnector.IntegrationTests
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

        #region BatchQuery

        #region BatchQuery

        [TestMethod]
        public void TestSqlTransactionForBatchQuery()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForBatchQueryAsync()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForCount()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForCountAsync()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForCountAll()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForCountAllAsync()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForDeleteAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForDeleteAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForDeleteAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForDeleteAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForDeleteAllAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForDeleteAllAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForDeleteAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForDeleteAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForInsertAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForInsertAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForInsertAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForInsertAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForInsertAllAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForInsertAllAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForInsertAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForInsertAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForMergeAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForMergeAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForMergeAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForMergeAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForMergeAllAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForMergeAllAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForMergeAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForMergeAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForQuery()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForQueryAsync()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForQueryAll()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForQueryAllAsync()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForQueryMultipleT2()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForQueryMultipleT3()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForQueryMultipleT4()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForQueryMultipleT5()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForQueryMultipleT6()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForQueryMultipleT7()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForQueryMultipleAsyncT2()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForQueryMultipleAsyncT3()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForQueryMultipleAsyncT4()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForQueryMultipleAsyncT5()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForQueryMultipleAsyncT6()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForQueryMultipleAsyncT7()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForTruncate()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForTruncateAsync()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestSqlTransactionForUpdateAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBit = 0;

                    // Act
                    connection.Update<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.Query<CompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual((ulong)0, queryResult.First().ColumnBit);
            }
        }

        [TestMethod]
        public void TestSqlTransactionForUpdateAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBit = 0;

                    // Act
                    connection.Update<CompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.Query<CompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual((ulong)1, queryResult.First().ColumnBit);
            }
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestSqlTransactionForUpdateAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBit = 0;

                    // Act
                    connection.UpdateAsync<CompleteTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.Query<CompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual((ulong)0, queryResult.First().ColumnBit);
            }
        }

        [TestMethod]
        public void TestSqlTransactionForUpdateAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<CompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBit = 0;

                    // Act
                    connection.UpdateAsync<CompleteTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.Query<CompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual((ulong)1, queryResult.First().ColumnBit);
            }
        }

        #endregion

        #endregion

        #region UpdateAll

        #region UpdateAll

        [TestMethod]
        public void TestSqlTransactionForUpdateAllAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBit = 0);

                    // Act
                    connection.UpdateAll<CompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual((ulong)0, queryResult.First(item => item.Id == entity.Id).ColumnBit));
            }
        }

        [TestMethod]
        public void TestSqlTransactionForUpdateAllAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBit = 0);

                    // Act
                    connection.UpdateAll<CompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual((ulong)1, queryResult.First(item => item.Id == entity.Id).ColumnBit));
            }
        }

        #endregion

        #region UpdateAllAsync

        [TestMethod]
        public void TestSqlTransactionForUpdateAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBit = 0);

                    // Act
                    connection.UpdateAllAsync<CompleteTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual((ulong)0, queryResult.First(item => item.Id == entity.Id).ColumnBit));
            }
        }

        [TestMethod]
        public void TestSqlTransactionForUpdateAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<CompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBit = 0);

                    // Act
                    connection.UpdateAllAsync<CompleteTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual((ulong)1, queryResult.First(item => item.Id == entity.Id).ColumnBit));
            }
        }

        #endregion

        #endregion
    }
}
