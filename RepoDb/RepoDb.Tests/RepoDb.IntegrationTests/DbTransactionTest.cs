using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System.Data.SqlClient;
using System.Linq;

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

        #region BatchQuery

        #region BatchQuery

        [TestMethod]
        public void TestSqlTransactionForBatchQuery()
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
        public void TestSqlTransactionForBatchQueryAsync()
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
        public void TestSqlTransactionForCount()
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
        public void TestSqlTransactionForCountAsync()
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
        public void TestSqlTransactionForCountAll()
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
        public void TestSqlTransactionForCountAllAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.CountAllAsync<IdentityTable>(transaction: transaction);
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
        public void TestSqlTransactionForDeleteAsRolledBack()
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
        public void TestSqlTransactionForDeleteAsyncAsCommitted()
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
        public void TestSqlTransactionForDeleteAsyncAsRolledBack()
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
        public void TestSqlTransactionForDeleteAllAsCommitted()
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
        public void TestSqlTransactionForDeleteAllAsRolledBack()
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
        public void TestSqlTransactionForDeleteAllAsyncAsCommitted()
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
        public void TestSqlTransactionForDeleteAllAsyncAsRolledBack()
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
        public void TestSqlTransactionForInsertAsCommitted()
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
        public void TestSqlTransactionForInsertAsRolledBack()
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
        public void TestSqlTransactionForInsertAsyncAsCommitted()
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
        public void TestSqlTransactionForInsertAsyncAsRolledBack()
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
        public void TestSqlTransactionForInsertAllAsCommitted()
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
        public void TestSqlTransactionForInsertAllAsRolledBack()
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
        public void TestSqlTransactionForInsertAllAsyncAsCommitted()
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
        public void TestSqlTransactionForInsertAllAsyncAsRolledBack()
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
        public void TestSqlTransactionForMergeAsCommitted()
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
        public void TestSqlTransactionForMergeAsRolledBack()
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
        public void TestSqlTransactionForMergeAsyncAsCommitted()
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
        public void TestSqlTransactionForMergeAsyncAsRolledBack()
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
        public void TestSqlTransactionForMergeAllAsCommitted()
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
        public void TestSqlTransactionForMergeAllAsRolledBack()
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
        public void TestSqlTransactionForMergeAllAsyncAsCommitted()
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
        public void TestSqlTransactionForMergeAllAsyncAsRolledBack()
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
        public void TestSqlTransactionForQuery()
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
        public void TestSqlTransactionForQueryAsync()
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
        public void TestSqlTransactionForQueryAll()
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
        public void TestSqlTransactionForQueryAllAsync()
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
        public void TestSqlTransactionForQueryMultipleT2()
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
        public void TestSqlTransactionForQueryMultipleT3()
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
        public void TestSqlTransactionForQueryMultipleT4()
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
        public void TestSqlTransactionForQueryMultipleT5()
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
        public void TestSqlTransactionForQueryMultipleT6()
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
        public void TestSqlTransactionForQueryMultipleT7()
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
        public void TestSqlTransactionForQueryMultipleAsyncT2()
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
        public void TestSqlTransactionForQueryMultipleAsyncT3()
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
        public void TestSqlTransactionForQueryMultipleAsyncT4()
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
        public void TestSqlTransactionForQueryMultipleAsyncT5()
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
        public void TestSqlTransactionForQueryMultipleAsyncT6()
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
        public void TestSqlTransactionForQueryMultipleAsyncT7()
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
        public void TestSqlTransactionForTruncate()
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
        public void TestSqlTransactionForTruncateAsync()
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
        public void TestSqlTransactionForUpdateAsCommitted()
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
        public void TestSqlTransactionForUpdateAsRolledBack()
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
        public void TestSqlTransactionForUpdateAsyncAsCommitted()
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
        public void TestSqlTransactionForUpdateAsyncAsRolledBack()
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
        public void TestSqlTransactionForUpdateAllAsCommitted()
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
        public void TestSqlTransactionForUpdateAllAsRolledBack()
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
        public void TestSqlTransactionForUpdateAllAsyncAsCommitted()
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
        public void TestSqlTransactionForUpdateAllAsyncAsRolledBack()
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
    }
}
