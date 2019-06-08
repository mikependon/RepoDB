using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class ObjectQuotationTest
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
        public void TestDeleteObjectQuotation()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);
            var last = entities.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var rowsInserted = connection.InsertAll<UnorganizedTable>(entities);
                var deleteResult = connection.Delete<UnorganizedTable>(last.Id);

                // Assert
                Assert.AreEqual(1, deleteResult);
            }
        }

        [TestMethod]
        public void TestDeleteObjectQuotationViaNonAlphaNumericField()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);
            var last = entities.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var rowsInserted = connection.InsertAll<UnorganizedTable>(entities);
                var deleteResult = connection.Delete<UnorganizedTable>(e => e.SessionId == last.SessionId);

                // Assert
                Assert.AreEqual(1, deleteResult);
            }
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestInsertObjectQuotation()
        {
            // Setup
            var entity = Helper.CreateUnorganizedTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<UnorganizedTable, long>(entity);

                // Assert
                Assert.IsNotNull(id);
                Assert.IsTrue(id > 0);
                Assert.AreEqual(1, connection.CountAll<UnorganizedTable>());
            }
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestInsertAllObjectQuotation()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var rowsInserted = connection.InsertAll<UnorganizedTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, rowsInserted);
                Assert.AreEqual(entities.Count, connection.CountAll<UnorganizedTable>());
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestMergeObjectQuotation()
        {
            // Setup
            var entity = Helper.CreateUnorganizedTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Merge<UnorganizedTable, long>(entity);

                // Assert
                Assert.IsNotNull(id);
                Assert.IsTrue(id > 0);
                Assert.AreEqual(1, connection.CountAll<UnorganizedTable>());

                // Setup
                entity.ColumnDateTime2 = DateTime.UtcNow;
                entity.ColumnInt = 2;
                entity.ColumnNVarChar = Guid.NewGuid().ToString();

                // Act
                id = connection.Merge<UnorganizedTable, long>(entity);
                var queryResult = connection.Query<UnorganizedTable>(id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region MergeAll

        [TestMethod]
        public void TestMergeAllObjectQuotation()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var rowsAffected = connection.MergeAll<UnorganizedTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, rowsAffected);
                Assert.AreEqual(entities.Count, connection.CountAll<UnorganizedTable>());

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnDateTime2 = DateTime.UtcNow;
                    entity.ColumnInt = 2;
                    entity.ColumnNVarChar = Guid.NewGuid().ToString();
                });

                // Act
                rowsAffected = connection.MergeAll<UnorganizedTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, rowsAffected);

                // Act
                var queryAllResult = connection.QueryAll<UnorganizedTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryAllResult.First(item => item.Id == entity.Id)));
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestQueryObjectQuotation()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);
            var last = entities.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var rowsInserted = connection.InsertAll<UnorganizedTable>(entities);
                var queryResult = connection.Query<UnorganizedTable>(last.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(last, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryObjectQuotationViaNonAlphaNumericField()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);
            var last = entities.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var rowsInserted = connection.InsertAll<UnorganizedTable>(entities);
                var queryResult = connection.Query<UnorganizedTable>(e => e.SessionId == last.SessionId).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(last, queryResult);
            }
        }

        #endregion

        #region QueryAll

        [TestMethod]
        public void TestQueryAllObjectQuotation()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var rowsInserted = connection.InsertAll<UnorganizedTable>(entities);
                var queryAllResult = connection.QueryAll<UnorganizedTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryAllResult.Count());
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryAllResult.First(item => item.Id == entity.Id)));
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestUpdateObjectQuotation()
        {
            // Setup
            var entity = Helper.CreateUnorganizedTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<UnorganizedTable, long>(entity);

                // Setup
                entity.ColumnDateTime2 = DateTime.UtcNow;
                entity.ColumnInt = 2;
                entity.ColumnNVarChar = Guid.NewGuid().ToString();

                // Act
                var updateReuslt = connection.Update<UnorganizedTable>(entity);
                var queryResult = connection.Query<UnorganizedTable>(id).First();

                // Assert
                Assert.AreEqual(1, updateReuslt);
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        public void TestUpdateAllObjectQuotation()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var rowsAffected = connection.InsertAll<UnorganizedTable>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnDateTime2 = DateTime.UtcNow;
                    entity.ColumnInt = 2;
                    entity.ColumnNVarChar = Guid.NewGuid().ToString();
                });

                // Act
                rowsAffected = connection.UpdateAll<UnorganizedTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, rowsAffected);

                // Act
                var queryAllResult = connection.QueryAll<UnorganizedTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryAllResult.First(item => item.Id == entity.Id)));
            }
        }

        #endregion
    }
}
