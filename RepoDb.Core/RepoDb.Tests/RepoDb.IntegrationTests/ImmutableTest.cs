using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class ImmutableTest
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

        [TestMethod]
        public void TestSqlConnectionDeleteForImmutableViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateImmutableIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<ImmutableIdentityTable>(entity);

                // Act
                var deleteResult = connection.Delete<ImmutableIdentityTable>(entity);

                // Assert
                Assert.IsTrue(deleteResult > 0);
                Assert.AreEqual(0, connection.CountAll<ImmutableIdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteForImmutableViaPrimary()
        {
            // Setup
            var entity = Helper.CreateImmutableIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<ImmutableIdentityTable>(entity);

                // Act
                var deleteResult = connection.Delete<ImmutableIdentityTable>(entity.Id);

                // Assert
                Assert.IsTrue(deleteResult > 0);
                Assert.AreEqual(0, connection.CountAll<ImmutableIdentityTable>());
            }
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestSqlConnectionInsertForImmutable()
        {
            // Setup
            var entity = Helper.CreateImmutableIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.Insert<ImmutableIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.AreEqual(entity.Id, insertResult);
            }
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestSqlConnectionInsertAllForImmutable()
        {
            // Setup
            var entities = Helper.CreateImmutableIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<ImmutableIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, insertAllResult);
                Assert.AreEqual(entities.Count, connection.CountAll<ImmutableIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<ImmutableIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult.First(e => e.Id == entity.Id)));
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestSqlConnectionMergeForImmutable()
        {
            // Setup
            var entity = Helper.CreateImmutableIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<ImmutableIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<ImmutableIdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForImmutableWithNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateImmutableIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.Merge<ImmutableIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.AreEqual(entity.Id, insertResult);
                Assert.AreEqual(1, connection.CountAll<ImmutableIdentityTable>());

                // Setup
                entity.SetColumnBit(false);
                entity.SetColumnDateTime2(DateTime.UtcNow);

                // Act
                var mergeResult = connection.Merge<ImmutableIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(entity.Id, mergeResult);
            }
        }

        #endregion

        #region MergeAll

        [TestMethod]
        public void TestSqlConnectionMergeAllForImmutable()
        {
            // Setup
            var entities = Helper.CreateImmutableIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllRequest = connection.MergeAll<ImmutableIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllRequest);

                // Act
                var queryResult = connection.QueryAll<ImmutableIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult.First(e => e.Id == entity.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForImmutableWithNonEmptyTables()
        {
            // Setup
            var entities = Helper.CreateImmutableIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<ImmutableIdentityTable>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.SetColumnBit(false);
                    entity.SetColumnDateTime2(DateTime.UtcNow);
                });

                // Act
                var mergeAllResult = connection.MergeAll<ImmutableIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);

                // Act
                var queryResult = connection.QueryAll<ImmutableIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult.First(e => e.Id == entity.Id)));
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestSqlConnectionQueryForImmutable()
        {
            // Setup
            var entity = Helper.CreateImmutableIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<ImmutableIdentityTable, long>(entity);

                // Act
                var queryResult = connection.Query<ImmutableIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestSqlConnectionUpdateForImmutableViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateImmutableIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<ImmutableIdentityTable, long>(entity);

                // Setup
                entity.SetColumnBit(false);
                entity.SetColumnDateTime2(DateTime.UtcNow);

                // Act
                var updateResult = connection.Update<ImmutableIdentityTable>(entity);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var data = connection.Query<ImmutableIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Helper.AssertPropertiesEquality(entity, data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateForImmutableViaPrimaryKey()
        {
            // Setup
            var entity = Helper.CreateImmutableIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<ImmutableIdentityTable, long>(entity);

                // Setup
                entity.SetColumnBit(false);
                entity.SetColumnDateTime2(DateTime.UtcNow);

                // Act
                var updateResult = connection.Update<ImmutableIdentityTable>(entity, entity.Id);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var data = connection.Query<ImmutableIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Helper.AssertPropertiesEquality(entity, data);
            }
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        public void TestSqlConnectionUpdateAllForImmutable()
        {
            // Setup
            var entities = Helper.CreateImmutableIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<ImmutableIdentityTable>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.SetColumnBit(false);
                    entity.SetColumnDateTime2(DateTime.UtcNow);
                });

                // Act
                var updateAllResult = connection.UpdateAll<ImmutableIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<ImmutableIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult.First(e => e.Id == entity.Id)));
            }
        }

        #endregion
    }
}
