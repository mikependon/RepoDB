using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System.Data.SQLite;
using System.Linq;
using System.Transactions;

namespace RepoDb.SqLite.IntegrationTests.SDS
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

        //[TestMethod]
        //public void Test()
        //{
        //    using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
        //    {
        //        using (var reader = connection.ExecuteReader("SELECT [Id], [ColumnBigInt], [ColumnBlob], [ColumnBoolean], [ColumnChar], [ColumnDate], [ColumnDateTime], [ColumnDecimal], [ColumnDouble], [ColumnInteger], [ColumnInt], [ColumnNone], [ColumnNumeric], [ColumnReal], [ColumnString], [ColumnText], [ColumnTime], [ColumnVarChar] FROM [SdsNonIdentityCompleteTable] ;"))
        //        {
        //            var columns = string.Empty;
        //            for(var i = 0; i < reader.FieldCount; i++)
        //            {
        //                //columns += $"{reader.GetName(i)} : {reader.GetFieldType(i)}\n";
        //                columns += $"public {reader.GetFieldType(i)}? {reader.GetName(i)} {{ get; set; }}\n";
        //            }

        //        }
        //    }
        //}

        #region DbTransaction

        #region BatchQuery

        #region BatchQuery

        [TestMethod]
        public void TestSqlTransactionForBatchQuery()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.BatchQuery<SdsCompleteTable>(0, 10, OrderField.Parse(new { Id = Order.Ascending }), it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public void TestSqlTransactionForBatchQueryAsync()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.BatchQueryAsync<SdsCompleteTable>(0, 10, OrderField.Parse(new { Id = Order.Ascending }), it => it.Id != 0, transaction: transaction).Wait();
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
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Count<SdsCompleteTable>(it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestSqlTransactionForCountAsync()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.CountAsync<SdsCompleteTable>(it => it.Id != 0, transaction: transaction).Wait();
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
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.CountAll<SdsCompleteTable>(transaction: transaction);
                }
            }
        }

        #endregion

        #region CountAllAsync

        [TestMethod]
        public void TestSqlTransactionForCountAllAsync()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.CountAllAsync<SdsCompleteTable>(transaction: transaction).Wait();
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
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateSdsCompleteTables(1).First();

                // Act
                connection.Insert<SdsCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Delete<SdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<SdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForDeleteAsRolledBack()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateSdsCompleteTables(1).First();

                // Act
                connection.Insert<SdsCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Delete<SdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
            }
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestSqlTransactionForDeleteAsyncAsCommitted()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateSdsCompleteTables(1).First();

                // Act
                connection.Insert<SdsCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAsync<SdsCompleteTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<SdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForDeleteAsyncAsRolledBack()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateSdsCompleteTables(1).First();

                // Act
                connection.Insert<SdsCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAsync<SdsCompleteTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
            }
        }

        #endregion

        #endregion

        #region DeleteAll

        #region DeleteAll

        [TestMethod]
        public void TestSqlTransactionForDeleteAllAsCommitted()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateSdsCompleteTables(10);

                // Act
                connection.InsertAll<SdsCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAll<SdsCompleteTable>(transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<SdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForDeleteAllAsRolledBack()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateSdsCompleteTables(10);

                // Act
                connection.InsertAll<SdsCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAll<SdsCompleteTable>(transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<SdsCompleteTable>());
            }
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public void TestSqlTransactionForDeleteAllAsyncAsCommitted()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateSdsCompleteTables(10);

                // Act
                connection.InsertAll<SdsCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAllAsync<SdsCompleteTable>(transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<SdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForDeleteAllAsyncAsRolledBack()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateSdsCompleteTables(10);

                // Act
                connection.InsertAll<SdsCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAllAsync<SdsCompleteTable>(transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<SdsCompleteTable>());
            }
        }

        #endregion

        #endregion

        #region Insert

        #region Insert

        [TestMethod]
        public void TestSqlTransactionForInsertAsCommitted()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateSdsCompleteTables(1).First();

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Insert<SdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForInsertAsRolledBack()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateSdsCompleteTables(1).First();

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Insert<SdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<SdsCompleteTable>());
            }
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestSqlTransactionForInsertAsyncAsCommitted()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateSdsCompleteTables(1).First();

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAsync<SdsCompleteTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForInsertAsyncAsRolledBack()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateSdsCompleteTables(1).First();

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAsync<SdsCompleteTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<SdsCompleteTable>());
            }
        }

        #endregion

        #endregion

        #region InsertAll

        #region InsertAll

        [TestMethod]
        public void TestSqlTransactionForInsertAllAsCommitted()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateSdsCompleteTables(10);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAll<SdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<SdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForInsertAllAsRolledBack()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateSdsCompleteTables(10);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAll<SdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<SdsCompleteTable>());
            }
        }

        #endregion

        #region InsertAllAsync

        [TestMethod]
        public void TestSqlTransactionForInsertAllAsyncAsCommitted()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateSdsCompleteTables(10);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAllAsync<SdsCompleteTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<SdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForInsertAllAsyncAsRolledBack()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateSdsCompleteTables(10);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAllAsync<SdsCompleteTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<SdsCompleteTable>());
            }
        }

        #endregion

        #endregion

        #region Merge

        #region Merge

        [TestMethod]
        public void TestSqlTransactionForMergeAsCommitted()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateSdsCompleteTables(1).First();

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Merge<SdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForMergeAsRolledBack()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateSdsCompleteTables(1).First();

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Merge<SdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<SdsCompleteTable>());
            }
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public void TestSqlTransactionForMergeAsyncAsCommitted()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateSdsCompleteTables(1).First();

                // Prepare
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.MergeAsync<SdsCompleteTable>(entity, transaction: transaction).Wait();

                // Act
                transaction.Commit();

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForMergeAsyncAsRolledBack()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateSdsCompleteTables(1).First();

                // Prepare
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.MergeAsync<SdsCompleteTable>(entity, transaction: transaction).Wait();

                // Act
                transaction.Rollback();

                // Assert
                Assert.AreEqual(0, connection.CountAll<SdsCompleteTable>());
            }
        }

        #endregion

        #endregion

        #region MergeAll

        #region MergeAll

        [TestMethod]
        public void TestSqlTransactionForMergeAllAsCommitted()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateSdsCompleteTables(10);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.MergeAll<SdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<SdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForMergeAllAsRolledBack()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateSdsCompleteTables(10);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.MergeAll<SdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<SdsCompleteTable>());
            }
        }

        #endregion

        #region MergeAllAsync

        [TestMethod]
        public void TestSqlTransactionForMergeAllAsyncAsCommitted()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateSdsCompleteTables(10);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.MergeAllAsync<SdsCompleteTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<SdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForMergeAllAsyncAsRolledBack()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateSdsCompleteTables(10);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.MergeAllAsync<SdsCompleteTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<SdsCompleteTable>());
            }
        }

        #endregion

        #endregion

        #region Query

        #region Query

        [TestMethod]
        public void TestSqlTransactionForQuery()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Query<SdsCompleteTable>(it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestSqlTransactionForQueryAsync()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryAsync<SdsCompleteTable>(it => it.Id != 0, transaction: transaction).Wait();
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
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryAll<SdsCompleteTable>(transaction: transaction);
                }
            }
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public void TestSqlTransactionForQueryAllAsync()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryAllAsync<SdsCompleteTable>(transaction: transaction).Wait();
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
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<SdsCompleteTable, SdsCompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public void TestSqlTransactionForQueryMultipleT3()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<SdsCompleteTable, SdsCompleteTable, SdsCompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public void TestSqlTransactionForQueryMultipleT4()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<SdsCompleteTable, SdsCompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction).Wait();
                }
            }
        }

        [TestMethod]
        public void TestSqlTransactionForQueryMultipleAsyncT3()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<SdsCompleteTable, SdsCompleteTable, SdsCompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction).Wait();
                }
            }
        }

        [TestMethod]
        public void TestSqlTransactionForQueryMultipleAsyncT4()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultipleAsync<SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable, SdsCompleteTable>(it => it.Id != 0,
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

        /*
         * Message: Test method RepoDb.SqLite.IntegrationTests.TransactionTests.TestSqlTransactionForTruncateAsync threw exception: 
         * System.AggregateException: One or more errors occurred. (SQL logic error cannot VACUUM from within a transaction) ---> 
         * System.Data.SQLite.SQLiteException: SQL logic error cannot VACUUM from within a transaction
         */

        //#region Truncate

        //[TestMethod]
        //public void TestSqlTransactionForTruncate()
        //{
        //    using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
        //    {
        //        // Create the tables
        //        Database.CreateCompleteTable(connection);
        //        // Prepare
        //        using (var transaction = connection.EnsureOpen().BeginTransaction())
        //        {
        //            // Act
        //            connection.Truncate<CompleteTable>(transaction: transaction);
        //        }
        //    }
        //}

        //#endregion

        //#region TruncateAsync

        //[TestMethod]
        //public void TestSqlTransactionForTruncateAsync()
        //{
        //    using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
        //    {
        //        // Create the tables
        //        Database.CreateCompleteTable(connection);

        //        // Prepare
        //        using (var transaction = connection.EnsureOpen().BeginTransaction())
        //        {
        //            // Act
        //            connection.TruncateAsync<CompleteTable>(transaction: transaction).Wait();
        //        }
        //    }
        //}

        //#endregion

        #endregion

        #region Update

        #region Update

        [TestMethod]
        public void TestSqlTransactionForUpdateAsCommitted()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateSdsCompleteTables(1).First();

                // Act
                connection.Insert<SdsCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBoolean = false;

                    // Act
                    connection.Update<SdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual(false, queryResult.First().ColumnBoolean);
            }
        }

        [TestMethod]
        public void TestSqlTransactionForUpdateAsRolledBack()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateSdsCompleteTables(1).First();

                // Act
                connection.Insert<SdsCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBoolean = false;

                    // Act
                    connection.Update<SdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual(true, queryResult.First().ColumnBoolean);
            }
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestSqlTransactionForUpdateAsyncAsCommitted()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateSdsCompleteTables(1).First();

                // Act
                connection.Insert<SdsCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBoolean = false;

                    // Act
                    connection.UpdateAsync<SdsCompleteTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual(false, queryResult.First().ColumnBoolean);
            }
        }

        [TestMethod]
        public void TestSqlTransactionForUpdateAsyncAsRolledBack()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateSdsCompleteTables(1).First();

                // Act
                connection.Insert<SdsCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBoolean = false;

                    // Act
                    connection.UpdateAsync<SdsCompleteTable>(entity, transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual(true, queryResult.First().ColumnBoolean);
            }
        }

        #endregion

        #endregion

        #region UpdateAll

        #region UpdateAll

        [TestMethod]
        public void TestSqlTransactionForUpdateAllAsCommitted()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateSdsCompleteTables(10);

                // Act
                connection.InsertAll<SdsCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBoolean = false);

                    // Act
                    connection.UpdateAll<SdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
            }
        }

        [TestMethod]
        public void TestSqlTransactionForUpdateAllAsRolledBack()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateSdsCompleteTables(10);

                // Act
                connection.InsertAll<SdsCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBoolean = false);

                    // Act
                    connection.UpdateAll<SdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(true, queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
            }
        }

        #endregion

        #region UpdateAllAsync

        [TestMethod]
        public void TestSqlTransactionForUpdateAllAsyncAsCommitted()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateSdsCompleteTables(10);

                // Act
                connection.InsertAll<SdsCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBoolean = false);

                    // Act
                    connection.UpdateAllAsync<SdsCompleteTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
            }
        }

        [TestMethod]
        public void TestSqlTransactionForUpdateAllAsyncAsRolledBack()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Create the tables
                Database.CreateSdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateSdsCompleteTables(10);

                // Act
                connection.InsertAll<SdsCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBoolean = false);

                    // Act
                    connection.UpdateAllAsync<SdsCompleteTable>(entities, transaction: transaction).Wait();

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.QueryAll<SdsCompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual(true, queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
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
            var entities = Helper.CreateSdsCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
                {
                    // Create the tables
                    Database.CreateSdsCompleteTable(connection);

                    // Act
                    connection.InsertAll<SdsCompleteTable>(entities);

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<SdsCompleteTable>());
                }

                // Complete
                transaction.Complete();
            }
        }

        [TestMethod]
        public void TestTransactionForInsertAllAsync()
        {
            // Setup
            var entities = Helper.CreateSdsCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
                {
                    // Create the tables
                    Database.CreateSdsCompleteTable(connection);

                    // Act
                    connection.InsertAllAsync<SdsCompleteTable>(entities).Wait();

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<SdsCompleteTable>());
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
            var entities = Helper.CreateSdsCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
                {
                    // Create the tables
                    Database.CreateSdsCompleteTable(connection);

                    // Act
                    connection.MergeAll<SdsCompleteTable>(entities);

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<SdsCompleteTable>());
                }

                // Complete
                transaction.Complete();
            }
        }

        [TestMethod]
        public void TestTransactionScopeForMergeAllAsync()
        {
            // Setup
            var entities = Helper.CreateSdsCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
                {
                    // Create the tables
                    Database.CreateSdsCompleteTable(connection);

                    // Act
                    connection.MergeAllAsync<SdsCompleteTable>(entities).Wait();

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<SdsCompleteTable>());
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
            var entities = Helper.CreateSdsCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
                {
                    // Create the tables
                    Database.CreateSdsCompleteTable(connection);

                    // Act
                    connection.InsertAll<SdsCompleteTable>(entities);

                    // Prepare
                    entities.ForEach(entity => entity.ColumnBoolean = false);

                    // Act
                    connection.UpdateAll<SdsCompleteTable>(entities);

                    // Act
                    var queryResult = connection.QueryAll<SdsCompleteTable>();

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
            var entities = Helper.CreateSdsCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
                {
                    // Create the tables
                    Database.CreateSdsCompleteTable(connection);

                    // Act
                    connection.InsertAll<SdsCompleteTable>(entities);

                    // Prepare
                    entities.ForEach(entity => entity.ColumnBoolean = false);

                    // Act
                    connection.UpdateAllAsync<SdsCompleteTable>(entities).Wait();

                    // Act
                    var queryResult = connection.QueryAll<SdsCompleteTable>();

                    // Assert
                    entities.ForEach(entity => Assert.AreEqual(false, queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
                }

                // Complete
                transaction.Complete();
            }
        }

        #endregion

        #endregion
    }
}
