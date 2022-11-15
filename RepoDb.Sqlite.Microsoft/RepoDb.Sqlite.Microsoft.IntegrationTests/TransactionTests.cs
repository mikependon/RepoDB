using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests
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
        //    using (var connection = new SqliteConnection(Database.ConnectionString))
        //    {
        //        using (var reader = connection.ExecuteReader("SELECT [Id], [ColumnBigInt], [ColumnBlob], [ColumnBoolean], [ColumnChar], [ColumnDate], [ColumnDateTime], [ColumnDecimal], [ColumnDouble], [ColumnInteger], [ColumnInt], [ColumnNone], [ColumnNumeric], [ColumnReal], [ColumnString], [ColumnText], [ColumnTime], [ColumnVarChar] FROM [MdsNonIdentityCompleteTable] ;"))
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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.BatchQuery<MdsCompleteTable>(0, 10, OrderField.Parse(new { Id = Order.Ascending }), it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public async Task TestSqlTransactionForBatchQueryAsync()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.BatchQueryAsync<MdsCompleteTable>(0, 10, OrderField.Parse(new { Id = Order.Ascending }), it => it.Id != 0, transaction: transaction);
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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Count<MdsCompleteTable>(it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public async Task TestSqlTransactionForCountAsync()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.CountAsync<MdsCompleteTable>(it => it.Id != 0, transaction: transaction);
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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.CountAll<MdsCompleteTable>(transaction: transaction);
                }
            }
        }

        #endregion

        #region CountAllAsync

        [TestMethod]
        public async Task TestSqlTransactionForCountAllAsync()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.CountAllAsync<MdsCompleteTable>(transaction: transaction);
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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateMdsCompleteTables(1).First();

                // Act
                connection.Insert<MdsCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Delete<MdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<MdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForDeleteAsRolledBack()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateMdsCompleteTables(1).First();

                // Act
                connection.Insert<MdsCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Delete<MdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
            }
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task TestSqlTransactionForDeleteAsyncAsCommitted()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateMdsCompleteTables(1).First();

                // Act
                connection.Insert<MdsCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.DeleteAsync<MdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<MdsCompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlTransactionForDeleteAsyncAsRolledBack()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateMdsCompleteTables(1).First();

                // Act
                connection.Insert<MdsCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.DeleteAsync<MdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
            }
        }

        #endregion

        #endregion

        #region DeleteAll

        #region DeleteAll

        [TestMethod]
        public void TestSqlTransactionForDeleteAllAsCommitted()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateMdsCompleteTables(10);

                // Act
                connection.InsertAll<MdsCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAll<MdsCompleteTable>(transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<MdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForDeleteAllAsRolledBack()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateMdsCompleteTables(10);

                // Act
                connection.InsertAll<MdsCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.DeleteAll<MdsCompleteTable>(transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<MdsCompleteTable>());
            }
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public async Task TestSqlTransactionForDeleteAllAsyncAsCommitted()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateMdsCompleteTables(10);

                // Act
                connection.InsertAll<MdsCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.DeleteAllAsync<MdsCompleteTable>(transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<MdsCompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlTransactionForDeleteAllAsyncAsRolledBack()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateMdsCompleteTables(10);

                // Act
                connection.InsertAll<MdsCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.DeleteAllAsync<MdsCompleteTable>(transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<MdsCompleteTable>());
            }
        }

        #endregion

        #endregion

        #region Insert

        #region Insert

        [TestMethod]
        public void TestSqlTransactionForInsertAsCommitted()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateMdsCompleteTables(1).First();

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Insert<MdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForInsertAsRolledBack()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateMdsCompleteTables(1).First();

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Insert<MdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<MdsCompleteTable>());
            }
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public async Task TestSqlTransactionForInsertAsyncAsCommitted()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateMdsCompleteTables(1).First();

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.InsertAsync<MdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlTransactionForInsertAsyncAsRolledBack()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateMdsCompleteTables(1).First();

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.InsertAsync<MdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<MdsCompleteTable>());
            }
        }

        #endregion

        #endregion

        #region InsertAll

        #region InsertAll

        [TestMethod]
        public void TestSqlTransactionForInsertAllAsCommitted()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateMdsCompleteTables(10);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAll<MdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<MdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForInsertAllAsRolledBack()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateMdsCompleteTables(10);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.InsertAll<MdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<MdsCompleteTable>());
            }
        }

        #endregion

        #region InsertAllAsync

        [TestMethod]
        public async Task TestSqlTransactionForInsertAllAsyncAsCommitted()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateMdsCompleteTables(10);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.InsertAllAsync<MdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(entities.Count, connection.CountAll<MdsCompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlTransactionForInsertAllAsyncAsRolledBack()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateMdsCompleteTables(10);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.InsertAllAsync<MdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<MdsCompleteTable>());
            }
        }

        #endregion

        #endregion

        #region Merge

        #region Merge

        [TestMethod]
        public void TestSqlTransactionForMergeAsCommitted()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateMdsCompleteTables(1).First();

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Merge<MdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForMergeAsRolledBack()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateMdsCompleteTables(1).First();

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Merge<MdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<MdsCompleteTable>());
            }
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public async Task TestSqlTransactionForMergeAsyncAsCommitted()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateMdsCompleteTables(1).First();

                // Prepare
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                await connection.MergeAsync<MdsCompleteTable>(entity, transaction: transaction);

                // Act
                transaction.Commit();

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsCompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlTransactionForMergeAsyncAsRolledBack()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateMdsCompleteTables(1).First();

                // Prepare
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                await connection.MergeAsync<MdsCompleteTable>(entity, transaction: transaction);

                // Act
                transaction.Rollback();

                // Assert
                Assert.AreEqual(0, connection.CountAll<MdsCompleteTable>());
            }
        }

        #endregion

        #endregion

        #region MergeAll

        #region MergeAll

        [TestMethod]
        public void TestSqlTransactionForMergeAllAsCommitted()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateMdsCompleteTables(10);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.MergeAll<MdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(10, connection.CountAll<MdsCompleteTable>());
            }
        }

        [TestMethod]
        public void TestSqlTransactionForMergeAllAsRolledBack()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateMdsCompleteTables(10);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.MergeAll<MdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<MdsCompleteTable>());
            }
        }

        #endregion

        #region MergeAllAsync

        [TestMethod]
        public async Task TestSqlTransactionForMergeAllAsyncAsCommitted()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateMdsCompleteTables(10);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.MergeAllAsync<MdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(10, connection.CountAll<MdsCompleteTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlTransactionForMergeAllAsyncAsRolledBack()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateMdsCompleteTables(10);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.MergeAllAsync<MdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<MdsCompleteTable>());
            }
        }

        #endregion

        #endregion

        #region Query

        #region Query

        [TestMethod]
        public void TestSqlTransactionForQuery()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.Query<MdsCompleteTable>(it => it.Id != 0, transaction: transaction);
                }
            }
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public async Task TestSqlTransactionForQueryAsync()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryAsync<MdsCompleteTable>(it => it.Id != 0, transaction: transaction);
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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryAll<MdsCompleteTable>(transaction: transaction);
                }
            }
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public async Task TestSqlTransactionForQueryAllAsync()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryAllAsync<MdsCompleteTable>(transaction: transaction);
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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public void TestSqlTransactionForQueryMultipleT3()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public void TestSqlTransactionForQueryMultipleT4()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(it => it.Id != 0,
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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(it => it.Id != 0,
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
        public async Task TestSqlTransactionForQueryMultipleAsyncT2()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public async Task TestSqlTransactionForQueryMultipleAsyncT3()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public async Task TestSqlTransactionForQueryMultipleAsyncT4()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public async Task TestSqlTransactionForQueryMultipleAsyncT5()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        it => it.Id != 0,
                        transaction: transaction);
                }
            }
        }

        [TestMethod]
        public async Task TestSqlTransactionForQueryMultipleAsyncT6()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(it => it.Id != 0,
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
        public async Task TestSqlTransactionForQueryMultipleAsyncT7()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    // Act
                    await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(it => it.Id != 0,
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

        /*
         * Message: Test method RepoDb.Sqlite.Microsoft.IntegrationTests.TransactionTests.TestSqlTransactionForTruncateAsync threw exception: 
         * System.AggregateException: One or more errors occurred. (SQL logic error cannot VACUUM from within a transaction) ---> 
         * System.Data.SQLite.SQLiteException: SQL logic error cannot VACUUM from within a transaction
         */

        //#region Truncate

        //[TestMethod]
        //public void TestSqlTransactionForTruncate()
        //{
        //    using (var connection = new SqliteConnection(Database.ConnectionString))
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
        //public async Task TestSqlTransactionForTruncateAsync()
        //{
        //    using (var connection = new SqliteConnection(Database.ConnectionString))
        //    {
        //        // Create the tables
        //        Database.CreateCompleteTable(connection);

        //        // Prepare
        //        using (var transaction = connection.EnsureOpen().BeginTransaction())
        //        {
        //            // Act
        //            connection.TruncateAsync<CompleteTable>(transaction: transaction);
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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateMdsCompleteTables(1).First();

                // Act
                connection.Insert<MdsCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBoolean = "false";

                    // Act
                    connection.Update<MdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual("false", queryResult.First().ColumnBoolean);
            }
        }

        [TestMethod]
        public void TestSqlTransactionForUpdateAsRolledBack()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateMdsCompleteTables(1).First();

                // Act
                connection.Insert<MdsCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBoolean = "false";

                    // Act
                    connection.Update<MdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual("true", queryResult.First().ColumnBoolean);
            }
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public async Task TestSqlTransactionForUpdateAsyncAsCommitted()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateMdsCompleteTables(1).First();

                // Act
                connection.Insert<MdsCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBoolean = "false";

                    // Act
                    await connection.UpdateAsync<MdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual("false", queryResult.First().ColumnBoolean);
            }
        }

        [TestMethod]
        public async Task TestSqlTransactionForUpdateAsyncAsRolledBack()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entity = Helper.CreateMdsCompleteTables(1).First();

                // Act
                connection.Insert<MdsCompleteTable>(entity);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entity.ColumnBoolean = "false";

                    // Act
                    await connection.UpdateAsync<MdsCompleteTable>(entity, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.Query<MdsCompleteTable>(entity.Id);

                // Assert
                Assert.AreEqual("true", queryResult.First().ColumnBoolean);
            }
        }

        #endregion

        #endregion

        #region UpdateAll

        #region UpdateAll

        [TestMethod]
        public void TestSqlTransactionForUpdateAllAsCommitted()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateMdsCompleteTables(10);

                // Act
                connection.InsertAll<MdsCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBoolean = "false");

                    // Act
                    connection.UpdateAll<MdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual("false", queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
            }
        }

        [TestMethod]
        public void TestSqlTransactionForUpdateAllAsRolledBack()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateMdsCompleteTables(10);

                // Act
                connection.InsertAll<MdsCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBoolean = "false");

                    // Act
                    connection.UpdateAll<MdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual("true", queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
            }
        }

        #endregion

        #region UpdateAllAsync

        [TestMethod]
        public async Task TestSqlTransactionForUpdateAllAsyncAsCommitted()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateMdsCompleteTables(10);

                // Act
                connection.InsertAll<MdsCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBoolean = "false");

                    // Act
                    await connection.UpdateAllAsync<MdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Commit();
                }

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual("false", queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
            }
        }

        [TestMethod]
        public async Task TestSqlTransactionForUpdateAllAsyncAsRolledBack()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Create the tables
                Database.CreateMdsCompleteTable(connection);

                // Setup
                var entities = Helper.CreateMdsCompleteTables(10);

                // Act
                connection.InsertAll<MdsCompleteTable>(entities);

                // Prepare
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    entities.ForEach(entity => entity.ColumnBoolean = "false");

                    // Act
                    await connection.UpdateAllAsync<MdsCompleteTable>(entities, transaction: transaction);

                    // Act
                    transaction.Rollback();
                }

                // Act
                var queryResult = connection.QueryAll<MdsCompleteTable>();

                // Assert
                entities.ForEach(entity => Assert.AreEqual("true", queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
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
            var entities = Helper.CreateMdsCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
                {
                    // Create the tables
                    Database.CreateMdsCompleteTable(connection);

                    // Act
                    connection.InsertAll<MdsCompleteTable>(entities);

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<MdsCompleteTable>());
                }

                // Complete
                transaction.Complete();
            }
        }

        [TestMethod]
        public async Task TestTransactionForInsertAllAsync()
        {
            // Setup
            var entities = Helper.CreateMdsCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
                {
                    // Create the tables
                    Database.CreateMdsCompleteTable(connection);

                    // Act
                    await connection.InsertAllAsync<MdsCompleteTable>(entities);

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<MdsCompleteTable>());
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
            var entities = Helper.CreateMdsCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
                {
                    // Create the tables
                    Database.CreateMdsCompleteTable(connection);

                    // Act
                    connection.MergeAll<MdsCompleteTable>(entities);

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<MdsCompleteTable>());
                }

                // Complete
                transaction.Complete();
            }
        }

        [TestMethod]
        public async Task TestTransactionScopeForMergeAllAsync()
        {
            // Setup
            var entities = Helper.CreateMdsCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
                {
                    // Create the tables
                    Database.CreateMdsCompleteTable(connection);

                    // Act
                    await connection.MergeAllAsync<MdsCompleteTable>(entities);

                    // Assert
                    Assert.AreEqual(entities.Count, connection.CountAll<MdsCompleteTable>());
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
            var entities = Helper.CreateMdsCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
                {
                    // Create the tables
                    Database.CreateMdsCompleteTable(connection);

                    // Act
                    connection.InsertAll<MdsCompleteTable>(entities);

                    // Prepare
                    entities.ForEach(entity => entity.ColumnBoolean = "false");

                    // Act
                    connection.UpdateAll<MdsCompleteTable>(entities);

                    // Act
                    var queryResult = connection.QueryAll<MdsCompleteTable>();

                    // Assert
                    entities.ForEach(entity => Assert.AreEqual("false", queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
                }

                // Complete
                transaction.Complete();
            }
        }

        [TestMethod]
        public async Task TestTransactionScopeForUpdateAllAsync()
        {
            // Setup
            var entities = Helper.CreateMdsCompleteTables(10);

            using (var transaction = new TransactionScope())
            {
                using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
                {
                    // Create the tables
                    Database.CreateMdsCompleteTable(connection);

                    // Act
                    connection.InsertAll<MdsCompleteTable>(entities);

                    // Prepare
                    entities.ForEach(entity => entity.ColumnBoolean = "false");

                    // Act
                    await connection.UpdateAllAsync<MdsCompleteTable>(entities);

                    // Act
                    var queryResult = connection.QueryAll<MdsCompleteTable>();

                    // Assert
                    entities.ForEach(entity => Assert.AreEqual("false", queryResult.First(item => item.Id == entity.Id).ColumnBoolean));
                }

                // Complete
                transaction.Complete();
            }
        }

        #endregion

        #endregion
    }
}
