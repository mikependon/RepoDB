using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class SqlTransactionTest
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.BatchQuery<IdentityTable>(0, 10, OrderField.Parse(new { Id = Order.Ascending }), it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public void TestDbTransactionForBatchQueryAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.BatchQueryAsync<IdentityTable>(0, 10, OrderField.Parse(new { Id = Order.Ascending }), it => it.Id != 0, transaction: transaction).Wait();
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Count<IdentityTable>(it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestDbTransactionForCountAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.CountAsync<IdentityTable>(it => it.Id != 0, transaction: transaction).Wait();
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.CountAll<IdentityTable>(transaction: transaction);
                }
            }
        }

        #endregion

        #region CountAllAsync

        [TestMethod]
        public void TestDbTransactionForCountAllAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.CountAllAsync<IdentityTable>(transaction: transaction).Wait();
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
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Delete<IdentityTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForDeleteAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Delete<IdentityTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestDbTransactionForDeleteAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAsync<IdentityTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForDeleteAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAsync<IdentityTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());
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
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAll<IdentityTable>(transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForDeleteAllAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAll<IdentityTable>(transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public void TestDbTransactionForDeleteAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAllAsync<IdentityTable>(transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForDeleteAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAllAsync<IdentityTable>(transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityTable>());
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
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Insert<IdentityTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForInsertAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Insert<IdentityTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestDbTransactionForInsertAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAsync<IdentityTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForInsertAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAsync<IdentityTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
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
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAll<IdentityTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForInsertAllAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAll<IdentityTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region InsertAllAsync

        [TestMethod]
        public void TestDbTransactionForInsertAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAllAsync<IdentityTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForInsertAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAllAsync<IdentityTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
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
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Merge<IdentityTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForMergeAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Merge<IdentityTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public void TestDbTransactionForMergeAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.MergeAsync<IdentityTable>(entity, transaction: transaction).Wait();

                // Act
                transaction.Commit();

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForMergeAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.MergeAsync<IdentityTable>(entity, transaction: transaction).Wait();

                // Act
                transaction.Rollback();

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
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
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.MergeAll<IdentityTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForMergeAllAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.MergeAll<IdentityTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region MergeAllAsync

        [TestMethod]
        public void TestDbTransactionForMergeAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.MergeAllAsync<IdentityTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestDbTransactionForMergeAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.MergeAllAsync<IdentityTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #endregion

        #region Query

        #region Query

        [TestMethod]
        public void TestDbTransactionForQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Query<IdentityTable>(it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestDbTransactionForQueryAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryAsync<IdentityTable>(it => it.Id != 0, transaction: transaction).Wait();
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryAll<IdentityTable>(transaction: transaction);
                }
            }
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public void TestDbTransactionForQueryAllAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryAllAsync<IdentityTable>(transaction: transaction).Wait();
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<IdentityTable, IdentityTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public void TestDbTransactionForQueryMultipleT3()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<IdentityTable, IdentityTable, IdentityTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public void TestDbTransactionForQueryMultipleT4()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<IdentityTable, IdentityTable, IdentityTable, IdentityTable>(it => it.Id != 0,
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(it => it.Id != 0,
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(it => it.Id != 0,
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(it => it.Id != 0,
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<IdentityTable, IdentityTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction).Wait();
                }
            }
        }

        [TestMethod]
        public void TestDbTransactionForQueryMultipleAsyncT3()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction).Wait();
                }
            }
        }

        [TestMethod]
        public void TestDbTransactionForQueryMultipleAsyncT4()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable, IdentityTable>(it => it.Id != 0,
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(it => it.Id != 0,
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(it => it.Id != 0,
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable, IdentityTable>(it => it.Id != 0,
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
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Truncate<IdentityTable>(transaction: transaction);
                }
            }
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public void TestDbTransactionForTruncateAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.TruncateAsync<IdentityTable>(transaction: transaction).Wait();
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
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBit = false;

                    // Act
                    connection.Update<IdentityTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id);

                // Assert
                Assert.AreEqual(false, queryResult.First().ColumnBit);
            }
        }

        [TestMethod]
        public void TestDbTransactionForUpdateAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBit = false;

                    // Act
                    connection.Update<IdentityTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id);

                // Assert
                Assert.AreEqual(true, queryResult.First().ColumnBit);
            }
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestDbTransactionForUpdateAsyncAsCommitted()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBit = false;

                    // Act
                    connection.UpdateAsync<IdentityTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id);

                // Assert
                Assert.AreEqual(false, queryResult.First().ColumnBit);
            }
        }

        [TestMethod]
        public void TestDbTransactionForUpdateAsyncAsRolledBack()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBit = false;

                    // Act
                    connection.UpdateAsync<IdentityTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id);

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
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    connection.UpdateAll<IdentityTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBit));
            }
        }

        [TestMethod]
        public void TestDbTransactionForUpdateAllAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    connection.UpdateAll<IdentityTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(true, queryResult.First(item => item.Id == entity.Id).ColumnBit));
            }
        }

        #endregion

        #region UpdateAllAsync

        [TestMethod]
        public void TestDbTransactionForUpdateAllAsyncAsCommitted()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    connection.UpdateAllAsync<IdentityTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBit));
            }
        }

        [TestMethod]
        public void TestDbTransactionForUpdateAllAsyncAsRolledBack()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    connection.UpdateAllAsync<IdentityTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.QueryAll<IdentityTable>();

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
            var entities = Helper.CreateIdentityTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
                {
                    // Act
                    connection.InsertAll<IdentityTable>(entities);

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<IdentityTable>());
                }

                // Complete
                transaction.Complete();
            }
        }

        [TestMethod]
        public void TestTransactionForInsertAllAsync()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
                {
                    // Act
                    connection.InsertAllAsync<IdentityTable>(entities).Wait();

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<IdentityTable>());
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
            var entities = Helper.CreateIdentityTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
                {
                    // Act
                    connection.MergeAll<IdentityTable>(entities);

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<IdentityTable>());
                }

                // Complete
                transaction.Complete();
            }
        }

        [TestMethod]
        public void TestTransactionScopeForMergeAllAsync()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
                {
                    // Act
                    connection.MergeAllAsync<IdentityTable>(entities).Wait();

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<IdentityTable>());
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
            var entities = Helper.CreateIdentityTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
                {
                    // Act
                    connection.InsertAll<IdentityTable>(entities);

                    // Prepare
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    connection.UpdateAll<IdentityTable>(entities);

                    // Act
                    var queryResult = connection.QueryAll<IdentityTable>();

                    // Assert
                    entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBit));
                }

                // Complete
                transaction.Complete();
            }
        }

        [TestMethod]
        public void TestTransactionScopeForUpdateAllAsync()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
                {
                    // Act
                    connection.InsertAll<IdentityTable>(entities);

                    // Prepare
                    entities.ForEach(entity => entity.ColumnBit = false);

                    // Act
                    connection.UpdateAllAsync<IdentityTable>(entities).Wait();

                    // Act
                    var queryResult = connection.QueryAll<IdentityTable>();

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
