using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace RepoDb.SqlServer.IntegrationTests
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
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.BatchQuery<IdentityCompleteTable>(0, 10, OrderField.Parse(new { Id = Order.Ascending }), it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public async Task TestDbTransactionForBatchQueryAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.BatchQueryAsync<IdentityCompleteTable>(0, 10, OrderField.Parse(new { Id = Order.Ascending }), it => it.Id != 0, transaction: transaction);
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
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Count<IdentityCompleteTable>(it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public async Task TestDbTransactionForCountAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.CountAsync<IdentityCompleteTable>(it => it.Id != 0, transaction: transaction);
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
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.CountAll<IdentityCompleteTable>(transaction: transaction);
                }
            }
        }

        #endregion

        #region CountAllAsync

        [TestMethod]
        public async Task TestDbTransactionForCountAllAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.CountAllAsync<IdentityCompleteTable>(transaction: transaction);
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
            var entity = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Delete<IdentityCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityCompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForDeleteAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Delete<IdentityCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
            }
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task TestDbTransactionForDeleteAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.DeleteAsync<IdentityCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityCompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForDeleteAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.DeleteAsync<IdentityCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
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
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<IdentityCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAll<IdentityCompleteTable>(transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityCompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForDeleteAllAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<IdentityCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAll<IdentityCompleteTable>(transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityCompleteTable>());
            }
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public async Task TestDbTransactionForDeleteAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<IdentityCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.DeleteAllAsync<IdentityCompleteTable>(transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityCompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForDeleteAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<IdentityCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.DeleteAllAsync<IdentityCompleteTable>(transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityCompleteTable>());
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
            var entity = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Insert<IdentityCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForInsertAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Insert<IdentityCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityCompleteTable>());
            }
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public async Task TestDbTransactionForInsertAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.InsertAsync<IdentityCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForInsertAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.InsertAsync<IdentityCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityCompleteTable>());
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
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAll<IdentityCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityCompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForInsertAllAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAll<IdentityCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityCompleteTable>());
            }
        }

        #endregion

        #region InsertAllAsync

        [TestMethod]
        public async Task TestDbTransactionForInsertAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.InsertAllAsync<IdentityCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityCompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForInsertAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.InsertAllAsync<IdentityCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityCompleteTable>());
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
            var entity = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Merge<IdentityCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForMergeAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Merge<IdentityCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityCompleteTable>());
            }
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public async Task TestDbTransactionForMergeAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                await connection.MergeAsync<IdentityCompleteTable>(entity, transaction: transaction);

                // Act
                transaction.Commit();

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForMergeAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                await connection.MergeAsync<IdentityCompleteTable>(entity, transaction: transaction);

                // Act
                transaction.Rollback();

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityCompleteTable>());
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
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.MergeAll<IdentityCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityCompleteTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForMergeAllAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.MergeAll<IdentityCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityCompleteTable>());
            }
        }

        #endregion

        #region MergeAllAsync

        [TestMethod]
        public async Task TestDbTransactionForMergeAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.MergeAllAsync<IdentityCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityCompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForMergeAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.MergeAllAsync<IdentityCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityCompleteTable>());
            }
        }

        #endregion

        #endregion

        #region Query

        #region Query

        [TestMethod]
        public void TestDbTransactionForQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Query<IdentityCompleteTable>(it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public async Task TestDbTransactionForQueryAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryAsync<IdentityCompleteTable>(it => it.Id != 0, transaction: transaction);
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
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryAll<IdentityCompleteTable>(transaction: transaction);
                }
            }
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public async Task TestDbTransactionForQueryAllAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryAllAsync<IdentityCompleteTable>(transaction: transaction);
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
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<IdentityCompleteTable, IdentityCompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public void TestDbTransactionForQueryMultipleT3()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public void TestDbTransactionForQueryMultipleT4()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<IdentityCompleteTable, IdentityCompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForQueryMultipleAsyncT3()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForQueryMultipleAsyncT4()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable, IdentityCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Truncate<IdentityCompleteTable>(transaction: transaction);
                }
            }
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public async Task TestDbTransactionForTruncateAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.TruncateAsync<IdentityCompleteTable>(transaction: transaction);
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
            var entity = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBit = false;

                    // Act
                    connection.Update<IdentityCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual(false, queryResult.First().ColumnBit);
            }
        }

        [TestMethod]
        public void TestDbTransactionForUpdateAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBit = false;

                    // Act
                    connection.Update<IdentityCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(entity.Id);

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
            var entity = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBit = false;

                    // Act
                    await connection.UpdateAsync<IdentityCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual(false, queryResult.First().ColumnBit);
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForUpdateAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBit = false;

                    // Act
                    await connection.UpdateAsync<IdentityCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual(true, queryResult.First().ColumnBit);
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
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<IdentityCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    connection.UpdateAll<IdentityCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.QueryAll<IdentityCompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBit));
            }
        }

        [TestMethod]
        public void TestDbTransactionForUpdateAllAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<IdentityCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    connection.UpdateAll<IdentityCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.QueryAll<IdentityCompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(true, queryResult.First(item => item.Id == entity.Id).ColumnBit));
            }
        }

        #endregion

        #region UpdateAllAsync

        [TestMethod]
        public async Task TestDbTransactionForUpdateAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<IdentityCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    await connection.UpdateAllAsync<IdentityCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.QueryAll<IdentityCompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBit));
            }
        }

        [TestMethod]
        public async Task TestDbTransactionForUpdateAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<IdentityCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    await connection.UpdateAllAsync<IdentityCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.QueryAll<IdentityCompleteTable>();

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
        public void TestTransactionForInsertAll()
        {
            // Setup
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqlConnection(Database.ConnectionString))
                {
                    // Act
                    connection.InsertAll<IdentityCompleteTable>(entities);

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<IdentityCompleteTable>());
                }

                // Complete
                transaction.Complete();
            }
        }

        [TestMethod]
        public async Task TestTransactionForInsertAllAsync()
        {
            // Setup
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var connection = new SqlConnection(Database.ConnectionString))
                {
                    // Act
                    await connection.InsertAllAsync<IdentityCompleteTable>(entities);

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<IdentityCompleteTable>());
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
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqlConnection(Database.ConnectionString))
                {
                    // Act
                    connection.MergeAll<IdentityCompleteTable>(entities);

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<IdentityCompleteTable>());
                }

                // Complete
                transaction.Complete();
            }
        }

        [TestMethod]
        public async Task TestTransactionScopeForMergeAllAsync()
        {
            // Setup
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var connection = new SqlConnection(Database.ConnectionString))
                {
                    // Act
                    await connection.MergeAllAsync<IdentityCompleteTable>(entities);

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<IdentityCompleteTable>());
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
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqlConnection(Database.ConnectionString))
                {
                    // Act
                    connection.InsertAll<IdentityCompleteTable>(entities);

                    // Prepare
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    connection.UpdateAll<IdentityCompleteTable>(entities);

                    // Act
                    var queryResult = connection.QueryAll<IdentityCompleteTable>();

                    // Assert
                    entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBit));
                }

                // Complete
                transaction.Complete();
            }
        }

        [TestMethod]
        public async Task TestTransactionScopeForUpdateAllAsync()
        {
            // Setup
            var entities = Helper.CreateIdentityCompleteTables(10);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var connection = new SqlConnection(Database.ConnectionString))
                {
                    // Act
                    connection.InsertAll<IdentityCompleteTable>(entities);

                    // Prepare
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    await connection.UpdateAllAsync<IdentityCompleteTable>(entities);

                    // Act
                    var queryResult = connection.QueryAll<IdentityCompleteTable>();

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
