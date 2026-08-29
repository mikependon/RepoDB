using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

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

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var rowsInserted = connection.InsertAll<UnorganizedTable>(entities);
                var deleteResult = connection.Delete<UnorganizedTable>(last.Id);

                // Assert
                Assert.AreEqual(1, deleteResult);
            }
        }

        [TestMethod]
        public async Task TestDeleteAsyncObjectQuotation()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);
            var last = entities.Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var rowsInserted = await connection.InsertAllAsync<UnorganizedTable>(entities);
                var deleteResult = await connection.DeleteAsync<UnorganizedTable>(last.Id);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var rowsInserted = connection.InsertAll<UnorganizedTable>(entities);
                var deleteResult = connection.Delete<UnorganizedTable>(e => e.SessionId == last.SessionId);

                // Assert
                Assert.AreEqual(1, deleteResult);
            }
        }

        [TestMethod]
        public async Task TestDeleteAsyncObjectQuotationViaNonAlphaNumericField()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);
            var last = entities.Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var rowsInserted = await connection.InsertAllAsync<UnorganizedTable>(entities);
                var deleteResult = await connection.DeleteAsync<UnorganizedTable>(e => e.SessionId == last.SessionId);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                long? id = connection.Insert<UnorganizedTable, long>(entity);

                // Assert
                Assert.IsNotNull(id);
                Assert.IsTrue(id > 0);
                Assert.AreEqual(1, connection.CountAll<UnorganizedTable>());
            }
        }

        [TestMethod]
        public async Task TestInsertAsyncObjectQuotation()
        {
            // Setup
            var entity = Helper.CreateUnorganizedTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                long? id = await connection.InsertAsync<UnorganizedTable, long>(entity);

                // Assert
                Assert.IsNotNull(id);
                Assert.IsTrue(id > 0);
                Assert.AreEqual(1, await connection.CountAllAsync<UnorganizedTable>());
            }
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestInsertAllObjectQuotation()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var rowsInserted = connection.InsertAll<UnorganizedTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, rowsInserted);
                Assert.AreEqual(entities.Count, connection.CountAll<UnorganizedTable>());
            }
        }

        [TestMethod]
        public async Task TestInsertAllAsyncObjectQuotation()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var rowsInserted = await connection.InsertAllAsync<UnorganizedTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, rowsInserted);
                Assert.AreEqual(entities.Count, await connection.CountAllAsync<UnorganizedTable>());
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestMergeObjectQuotation()
        {
            // Setup
            var entity = Helper.CreateUnorganizedTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                long? id = connection.Merge<UnorganizedTable, long>(entity);

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

        [TestMethod]
        public async Task TestMergeAsyncObjectQuotation()
        {
            // Setup
            var entity = Helper.CreateUnorganizedTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                long? id = await connection.MergeAsync<UnorganizedTable, long>(entity);

                // Assert
                Assert.IsNotNull(id);
                Assert.IsTrue(id > 0);
                Assert.AreEqual(1, await connection.CountAllAsync<UnorganizedTable>());

                // Setup
                entity.ColumnDateTime2 = DateTime.UtcNow;
                entity.ColumnInt = 2;
                entity.ColumnNVarChar = Guid.NewGuid().ToString();

                // Act
                id = await connection.MergeAsync<UnorganizedTable, long>(entity);
                var queryResult = (await connection.QueryAsync<UnorganizedTable>(id)).First();

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestMergeAllAsyncObjectQuotation()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var rowsAffected = await connection.MergeAllAsync<UnorganizedTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, rowsAffected);
                Assert.AreEqual(entities.Count, await connection.CountAllAsync<UnorganizedTable>());

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnDateTime2 = DateTime.UtcNow;
                    entity.ColumnInt = 2;
                    entity.ColumnNVarChar = Guid.NewGuid().ToString();
                });

                // Act
                rowsAffected = await connection.MergeAllAsync<UnorganizedTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, rowsAffected);

                // Act
                var queryAllResult = await connection.QueryAllAsync<UnorganizedTable>();

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestQueryAsyncObjectQuotation()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);
            var last = entities.Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var rowsInserted = await connection.InsertAllAsync<UnorganizedTable>(entities);
                var queryResult = (await connection.QueryAsync<UnorganizedTable>(last.Id)).FirstOrDefault();

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

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var rowsInserted = connection.InsertAll<UnorganizedTable>(entities);
                var queryResult = connection.Query<UnorganizedTable>(e => e.SessionId == last.SessionId).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(last, queryResult);
            }
        }

        [TestMethod]
        public async Task TestQueryAsyncObjectQuotationViaNonAlphaNumericField()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);
            var last = entities.Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var rowsInserted = await connection.InsertAllAsync<UnorganizedTable>(entities);
                var queryResult = (await connection.QueryAsync<UnorganizedTable>(e => e.SessionId == last.SessionId)).FirstOrDefault();

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

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var rowsInserted = connection.InsertAll<UnorganizedTable>(entities);
                var queryAllResult = connection.QueryAll<UnorganizedTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryAllResult.Count());
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryAllResult.First(item => item.Id == entity.Id)));
            }
        }

        [TestMethod]
        public async Task TestQueryAllAsyncObjectQuotation()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var rowsInserted = await connection.InsertAllAsync<UnorganizedTable>(entities);
                var queryAllResult = await connection.QueryAllAsync<UnorganizedTable>();

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestUpdateAsyncObjectQuotation()
        {
            // Setup
            var entity = Helper.CreateUnorganizedTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<UnorganizedTable, long>(entity);

                // Setup
                entity.ColumnDateTime2 = DateTime.UtcNow;
                entity.ColumnInt = 2;
                entity.ColumnNVarChar = Guid.NewGuid().ToString();

                // Act
                var updateReuslt = await connection.UpdateAsync<UnorganizedTable>(entity);
                var queryResult = (await connection.QueryAsync<UnorganizedTable>(id)).First();

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestUpdateAllAsyncObjectQuotation()
        {
            // Setup
            var entities = Helper.CreateUnorganizedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var rowsAffected = await connection.InsertAllAsync<UnorganizedTable>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnDateTime2 = DateTime.UtcNow;
                    entity.ColumnInt = 2;
                    entity.ColumnNVarChar = Guid.NewGuid().ToString();
                });

                // Act
                rowsAffected = await connection.UpdateAllAsync<UnorganizedTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, rowsAffected);

                // Act
                var queryAllResult = await connection.QueryAllAsync<UnorganizedTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryAllResult.First(item => item.Id == entity.Id)));
            }
        }

        #endregion
    }
}
