using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class DottedTableTest
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
        public void TestDeleteDottedTable()
        {
            // Setup
            var entities = Helper.CreateDottedTables(10);
            var last = entities.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var rowsInserted = connection.InsertAll<DottedTable>(entities);
                var deleteResult = connection.Delete<DottedTable>(last.Id);

                // Assert
                Assert.AreEqual(1, deleteResult);
            }
        }

        [TestMethod]
        public void TestDeleteDottedTableViaNonAlphaNumericField()
        {
            // Setup
            var entities = Helper.CreateDottedTables(10);
            var last = entities.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var rowsInserted = connection.InsertAll<DottedTable>(entities);
                var deleteResult = connection.Delete<DottedTable>(e => e.SessionId == last.SessionId);

                // Assert
                Assert.AreEqual(1, deleteResult);
            }
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestInsertDottedTable()
        {
            // Setup
            var entity = Helper.CreateDottedTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<DottedTable, long>(entity);

                // Assert
                Assert.IsNotNull(id);
                Assert.IsTrue(id > 0);
                Assert.AreEqual(1, connection.CountAll<DottedTable>());
            }
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestInsertAllDottedTable()
        {
            // Setup
            var entities = Helper.CreateDottedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var rowsInserted = connection.InsertAll<DottedTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, rowsInserted);
                Assert.AreEqual(entities.Count, connection.CountAll<DottedTable>());
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestMergeDottedTable()
        {
            // Setup
            var entity = Helper.CreateDottedTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Merge<DottedTable, long>(entity);

                // Assert
                Assert.IsNotNull(id);
                Assert.IsTrue(id > 0);
                Assert.AreEqual(1, connection.CountAll<DottedTable>());

                // Setup
                entity.ColumnDateTime2 = DateTime.UtcNow;
                entity.ColumnInt = 2;
                entity.ColumnNVarChar = Guid.NewGuid().ToString();

                // Act
                id = connection.Merge<DottedTable, long>(entity);
                var queryResult = connection.Query<DottedTable>(id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region MergeAll

        [TestMethod]
        public void TestMergeAllDottedTable()
        {
            // Setup
            var entities = Helper.CreateDottedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var rowsAffected = connection.MergeAll<DottedTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, rowsAffected);
                Assert.AreEqual(entities.Count, connection.CountAll<DottedTable>());

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnDateTime2 = DateTime.UtcNow;
                    entity.ColumnInt = 2;
                    entity.ColumnNVarChar = Guid.NewGuid().ToString();
                });

                // Act
                rowsAffected = connection.MergeAll<DottedTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, rowsAffected);

                // Act
                var queryAllResult = connection.QueryAll<DottedTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryAllResult.First(item => item.Id == entity.Id)));
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestQueryDottedTable()
        {
            // Setup
            var entities = Helper.CreateDottedTables(10);
            var last = entities.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var rowsInserted = connection.InsertAll<DottedTable>(entities);
                var queryResult = connection.Query<DottedTable>(last.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(last, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryDottedTableViaNonAlphaNumericField()
        {
            // Setup
            var entities = Helper.CreateDottedTables(10);
            var last = entities.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var rowsInserted = connection.InsertAll<DottedTable>(entities);
                var queryResult = connection.Query<DottedTable>(e => e.SessionId == last.SessionId).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(last, queryResult);
            }
        }

        #endregion

        #region QueryAll

        [TestMethod]
        public void TestQueryAllDottedTable()
        {
            // Setup
            var entities = Helper.CreateDottedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var rowsInserted = connection.InsertAll<DottedTable>(entities);
                var queryAllResult = connection.QueryAll<DottedTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryAllResult.Count());
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryAllResult.First(item => item.Id == entity.Id)));
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestUpdateDottedTable()
        {
            // Setup
            var entity = Helper.CreateDottedTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<DottedTable, long>(entity);

                // Setup
                entity.ColumnDateTime2 = DateTime.UtcNow;
                entity.ColumnInt = 2;
                entity.ColumnNVarChar = Guid.NewGuid().ToString();

                // Act
                var updateReuslt = connection.Update<DottedTable>(entity);
                var queryResult = connection.Query<DottedTable>(id).First();

                // Assert
                Assert.AreEqual(1, updateReuslt);
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        public void TestUpdateAllDottedTable()
        {
            // Setup
            var entities = Helper.CreateDottedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var rowsAffected = connection.InsertAll<DottedTable>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnDateTime2 = DateTime.UtcNow;
                    entity.ColumnInt = 2;
                    entity.ColumnNVarChar = Guid.NewGuid().ToString();
                });

                // Act
                rowsAffected = connection.UpdateAll<DottedTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, rowsAffected);

                // Act
                var queryAllResult = connection.QueryAll<DottedTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryAllResult.First(item => item.Id == entity.Id)));
            }
        }

        #endregion
    }
}
