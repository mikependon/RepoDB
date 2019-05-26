using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.Delete<IdentityTable>(entity, transaction: transaction);

                // Act
                transaction.Commit();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.Delete<IdentityTable>(entity, transaction: transaction);

                // Act
                transaction.Rollback();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.DeleteAsync<IdentityTable>(entity, transaction: transaction).Wait();

                // Act
                transaction.Commit();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.DeleteAsync<IdentityTable>(entity, transaction: transaction).Wait();

                // Act
                transaction.Rollback();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.DeleteAll<IdentityTable>(transaction: transaction);

                // Act
                transaction.Commit();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.DeleteAll<IdentityTable>(transaction: transaction);

                // Act
                transaction.Rollback();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.DeleteAllAsync<IdentityTable>(transaction: transaction).Wait();

                // Act
                transaction.Commit();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.DeleteAllAsync<IdentityTable>(transaction: transaction).Wait();

                // Act
                transaction.Rollback();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.Insert<IdentityTable>(entity, transaction: transaction);

                // Act
                transaction.Commit();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.Insert<IdentityTable>(entity, transaction: transaction);

                // Act
                transaction.Rollback();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.InsertAsync<IdentityTable>(entity, transaction: transaction).Wait();

                // Act
                transaction.Commit();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.InsertAsync<IdentityTable>(entity, transaction: transaction).Wait();

                // Act
                transaction.Rollback();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.InsertAll<IdentityTable>(entities, transaction: transaction);

                // Act
                transaction.Commit();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.InsertAll<IdentityTable>(entities, transaction: transaction);

                // Act
                transaction.Rollback();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.InsertAllAsync<IdentityTable>(entities, transaction: transaction).Wait();

                // Act
                transaction.Commit();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.InsertAllAsync<IdentityTable>(entities, transaction: transaction).Wait();

                // Act
                transaction.Rollback();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.Merge<IdentityTable>(entity, transaction: transaction);

                // Act
                transaction.Commit();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.Merge<IdentityTable>(entity, transaction: transaction);

                // Act
                transaction.Rollback();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.MergeAll<IdentityTable>(entities, transaction: transaction);

                // Act
                transaction.Commit();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.MergeAll<IdentityTable>(entities, transaction: transaction);

                // Act
                transaction.Rollback();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.MergeAllAsync<IdentityTable>(entities, transaction: transaction).Wait();

                // Act
                transaction.Commit();

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
                var transaction = connection.EnsureOpen().BeginTransaction();

                // Act
                connection.MergeAllAsync<IdentityTable>(entities, transaction: transaction).Wait();

                // Act
                transaction.Rollback();

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
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
                var transaction = connection.EnsureOpen().BeginTransaction();
                entity.ColumnBit = false;

                // Act
                connection.Update<IdentityTable>(entity, transaction: transaction);

                // Act
                transaction.Commit();

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
                var transaction = connection.EnsureOpen().BeginTransaction();
                entity.ColumnBit = false;

                // Act
                connection.Update<IdentityTable>(entity, transaction: transaction);

                // Act
                transaction.Rollback();

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
                var transaction = connection.EnsureOpen().BeginTransaction();
                entity.ColumnBit = false;

                // Act
                connection.UpdateAsync<IdentityTable>(entity, transaction: transaction).Wait();

                // Act
                transaction.Commit();

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
                var transaction = connection.EnsureOpen().BeginTransaction();
                entity.ColumnBit = false;

                // Act
                connection.UpdateAsync<IdentityTable>(entity, transaction: transaction).Wait();

                // Act
                transaction.Rollback();

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
                var transaction = connection.EnsureOpen().BeginTransaction();
                entities.ForEach(entity => entity.ColumnBit = false);

                // Act
                connection.UpdateAll<IdentityTable>(entities, transaction: transaction);

                // Act
                transaction.Commit();

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
                var transaction = connection.EnsureOpen().BeginTransaction();
                entities.ForEach(entity => entity.ColumnBit = false);

                // Act
                connection.UpdateAll<IdentityTable>(entities, transaction: transaction);

                // Act
                transaction.Rollback();

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
                var transaction = connection.EnsureOpen().BeginTransaction();
                entities.ForEach(entity => entity.ColumnBit = false);

                // Act
                connection.UpdateAllAsync<IdentityTable>(entities, transaction: transaction).Wait();

                // Act
                transaction.Commit();

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
                var transaction = connection.EnsureOpen().BeginTransaction();
                entities.ForEach(entity => entity.ColumnBit = false);

                // Act
                connection.UpdateAllAsync<IdentityTable>(entities, transaction: transaction).Wait();

                // Act
                transaction.Rollback();

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
