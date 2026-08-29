using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class DifferentPrimaryTest
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

        #region Insert

        [TestMethod]
        public void TestSqlConnectionInsertForIdentityTableWithDifferentPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<IdentityTableWithDifferentPrimary, long>(entity);

                // Assert
                Assert.AreEqual(entity.Id, insertResult);
                Assert.IsTrue(insertResult > 0);
                Assert.IsTrue(entity.Id > 0);
                Assert.AreEqual(1, connection.CountAll<IdentityTableWithDifferentPrimary>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAsyncForIdentityTableWithDifferentPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<IdentityTableWithDifferentPrimary, long>(entity);

                // Assert
                Assert.AreEqual(entity.Id, insertResult);
                Assert.IsTrue(insertResult > 0);
                Assert.IsTrue(entity.Id > 0);
                Assert.AreEqual(1, await connection.CountAllAsync<IdentityTableWithDifferentPrimary>());
            }
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestSqlConnectionInsertAllForIdentityTableWithDifferentPrimary()
        {
            // Setup
            var entities = Helper.CreateIdentityTableWithDifferentPrimaries(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = connection.InsertAll<IdentityTableWithDifferentPrimary>(entities);

                // Assert
                Assert.AreEqual(entities.Count, insertAllResult);
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityTableWithDifferentPrimary>());

                // Act
                var queryResult = connection.QueryAll<IdentityTableWithDifferentPrimary>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity,
                        queryResult.ElementAt(entities.IndexOf(entity))));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForIdentityTableWithDifferentPrimary()
        {
            // Setup
            var entities = Helper.CreateIdentityTableWithDifferentPrimaries(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync<IdentityTableWithDifferentPrimary>(entities);

                // Assert
                Assert.AreEqual(entities.Count, insertAllResult);
                Assert.AreEqual(entities.Count, await connection.CountAllAsync<IdentityTableWithDifferentPrimary>());

                // Act
                var queryResult = await connection.QueryAllAsync<IdentityTableWithDifferentPrimary>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity,
                        queryResult.ElementAt(entities.IndexOf(entity))));
            }
        }

        #endregion

        #region Delete

        [TestMethod]
        public void TestSqlConnectionDeleteForIdentityTableWithDifferentPrimaryViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTableWithDifferentPrimary>(entity);

                // Act
                var deleteResult = connection.Delete<IdentityTableWithDifferentPrimary>(entity);

                // Assert
                Assert.IsTrue(deleteResult > 0);
                Assert.AreEqual(0, connection.CountAll<IdentityTableWithDifferentPrimary>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncForIdentityTableWithDifferentPrimaryViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTableWithDifferentPrimary>(entity);

                // Act
                var deleteResult = await connection.DeleteAsync<IdentityTableWithDifferentPrimary>(entity);

                // Assert
                Assert.IsTrue(deleteResult > 0);
                Assert.AreEqual(0, await connection.CountAllAsync<IdentityTableWithDifferentPrimary>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteForIdentityTableWithDifferentPrimaryViaPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTableWithDifferentPrimary>(entity);

                // Act
                var deleteResult = connection.Delete<IdentityTableWithDifferentPrimary>(entity.RowGuid);

                // Assert
                Assert.IsTrue(deleteResult > 0);
                Assert.AreEqual(0, connection.CountAll<IdentityTableWithDifferentPrimary>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncForIdentityTableWithDifferentPrimaryViaPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTableWithDifferentPrimary>(entity);

                // Act
                var deleteResult = await connection.DeleteAsync<IdentityTableWithDifferentPrimary>(entity.RowGuid);

                // Assert
                Assert.IsTrue(deleteResult > 0);
                Assert.AreEqual(0, await connection.CountAllAsync<IdentityTableWithDifferentPrimary>());
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestSqlConnectionQueryForIdentityTableWithDifferentPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTableWithDifferentPrimary, long>(entity);

                // Act
                var queryResult = connection.Query<IdentityTableWithDifferentPrimary>(entity.RowGuid).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForIdentityTableWithDifferentPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTableWithDifferentPrimary, long>(entity);

                // Act
                var queryResult = (await connection.QueryAsync<IdentityTableWithDifferentPrimary>(entity.RowGuid)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestSqlConnectionUpdateForIdentityTableWithDifferentPrimaryViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTableWithDifferentPrimary, long>(entity);

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var updateResult = connection.Update<IdentityTableWithDifferentPrimary>(entity);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var data = connection.Query<IdentityTableWithDifferentPrimary>(entity.RowGuid).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Helper.AssertPropertiesEquality(entity, data);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAsyncForIdentityTableWithDifferentPrimaryViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTableWithDifferentPrimary, long>(entity);

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var updateResult = await connection.UpdateAsync<IdentityTableWithDifferentPrimary>(entity);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var data = (await connection.QueryAsync<IdentityTableWithDifferentPrimary>(entity.RowGuid)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Helper.AssertPropertiesEquality(entity, data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateForIdentityTableWithDifferentPrimaryViaPrimaryKey()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTableWithDifferentPrimary, long>(entity);

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var updateResult = connection.Update<IdentityTableWithDifferentPrimary>(entity, entity.RowGuid);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var data = connection.Query<IdentityTableWithDifferentPrimary>(entity.RowGuid).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Helper.AssertPropertiesEquality(entity, data);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAsyncForIdentityTableWithDifferentPrimaryViaPrimaryKey()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTableWithDifferentPrimary, long>(entity);

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var updateResult = await connection.UpdateAsync<IdentityTableWithDifferentPrimary>(entity, entity.RowGuid);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var data = (await connection.QueryAsync<IdentityTableWithDifferentPrimary>(entity.RowGuid)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Helper.AssertPropertiesEquality(entity, data);
            }
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        public void TestSqlConnectionUpdateAllForIdentityTableWithDifferentPrimaries()
        {
            // Setup
            var entities = Helper.CreateIdentityTableWithDifferentPrimaries(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<IdentityTableWithDifferentPrimary>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnBit = false;
                    entity.ColumnDateTime2 = DateTime.UtcNow;
                });

                // Act
                var updateAllResult = connection.UpdateAll<IdentityTableWithDifferentPrimary>(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<IdentityTableWithDifferentPrimary>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity,
                        queryResult.ElementAt(entities.IndexOf(entity))));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncForIdentityTableWithDifferentPrimaries()
        {
            // Setup
            var entities = Helper.CreateIdentityTableWithDifferentPrimaries(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTableWithDifferentPrimary>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnBit = false;
                    entity.ColumnDateTime2 = DateTime.UtcNow;
                });

                // Act
                var updateAllResult = await connection.UpdateAllAsync<IdentityTableWithDifferentPrimary>(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = await connection.QueryAllAsync<IdentityTableWithDifferentPrimary>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity,
                        queryResult.ElementAt(entities.IndexOf(entity))));
            }
        }

        #endregion
    }
}
