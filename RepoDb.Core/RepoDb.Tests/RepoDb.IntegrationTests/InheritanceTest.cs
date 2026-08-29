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
    public class InheritedTest
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
        public void TestSqlConnectionDeleteForInheritedViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<InheritedIdentityTable>(entity);

                // Act
                var deleteResult = connection.Delete<InheritedIdentityTable>(entity);

                // Assert
                Assert.IsTrue(deleteResult > 0);
                Assert.AreEqual(0, connection.CountAll<InheritedIdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncForInheritedViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<InheritedIdentityTable>(entity);

                // Act
                var deleteResult = await connection.DeleteAsync<InheritedIdentityTable>(entity);

                // Assert
                Assert.IsTrue(deleteResult > 0);
                Assert.AreEqual(0, await connection.CountAllAsync<InheritedIdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteForInheritedViaPrimary()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<InheritedIdentityTable>(entity);

                // Act
                var deleteResult = connection.Delete<InheritedIdentityTable>(entity.Id);

                // Assert
                Assert.IsTrue(deleteResult > 0);
                Assert.AreEqual(0, connection.CountAll<InheritedIdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncForInheritedViaPrimary()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<InheritedIdentityTable>(entity);

                // Act
                var deleteResult = await connection.DeleteAsync<InheritedIdentityTable>(entity.Id);

                // Assert
                Assert.IsTrue(deleteResult > 0);
                Assert.AreEqual(0, await connection.CountAllAsync<InheritedIdentityTable>());
            }
        }

        #endregion

        #region DeleteAll

        [TestMethod]
        public void TestSqlConnectionDeleteAllForInheritedViaDataEntity()
        {
            // Setup
            var entities = Helper.CreateInheritedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<InheritedIdentityTable>(entities);

                // Act
                var deleteResult = connection.DeleteAll<InheritedIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count(), deleteResult);
                Assert.AreEqual(0, connection.CountAll<InheritedIdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAllAsyncForInheritedViaDataEntity()
        {
            // Setup
            var entities = Helper.CreateInheritedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<InheritedIdentityTable>(entities);

                // Act
                var deleteResult = await connection.DeleteAllAsync<InheritedIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count(), deleteResult);
                Assert.AreEqual(0, await connection.CountAllAsync<InheritedIdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAllForInheritedViaPrimary()
        {
            // Setup
            var entities = Helper.CreateInheritedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<InheritedIdentityTable>(entities);

                // Act
                var deleteResult = connection.DeleteAll<InheritedIdentityTable>(
                    ClassExpression.GetEntitiesPropertyValues<InheritedIdentityTable, object>(entities, "Id"));

                // Assert
                Assert.AreEqual(entities.Count(), deleteResult);
                Assert.AreEqual(0, connection.CountAll<InheritedIdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAllAsyncForInheritedViaPrimary()
        {
            // Setup
            var entities = Helper.CreateInheritedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<InheritedIdentityTable>(entities);

                // Act
                var deleteResult = await connection.DeleteAllAsync<InheritedIdentityTable>(
                    ClassExpression.GetEntitiesPropertyValues<InheritedIdentityTable, object>(entities, "Id"));

                // Assert
                Assert.AreEqual(entities.Count(), deleteResult);
                Assert.AreEqual(0, await connection.CountAllAsync<InheritedIdentityTable>());
            }
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestSqlConnectionInsertForInherited()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<InheritedIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.AreEqual(entity.Id, insertResult);

                // Act
                var queryResult = connection.Query<InheritedIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAsyncForInherited()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<InheritedIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.AreEqual(entity.Id, insertResult);

                // Act
                var queryResult = (await connection.QueryAsync<InheritedIdentityTable>(entity.Id)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestSqlConnectionInsertAllForInherited()
        {
            // Setup
            var entities = Helper.CreateInheritedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = connection.InsertAll<InheritedIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, insertAllResult);
                Assert.AreEqual(entities.Count, connection.CountAll<InheritedIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<InheritedIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult.First(e => e.Id == entity.Id)));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForInherited()
        {
            // Setup
            var entities = Helper.CreateInheritedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync<InheritedIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, insertAllResult);
                Assert.AreEqual(entities.Count, await connection.CountAllAsync<InheritedIdentityTable>());

                // Act
                var queryResult = await connection.QueryAllAsync<InheritedIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult.First(e => e.Id == entity.Id)));
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestSqlConnectionMergeForInherited()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeResult = connection.Merge<InheritedIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(entity.Id, mergeResult);

                // Act
                var queryResult = connection.Query<InheritedIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForInherited()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeResult = await connection.MergeAsync<InheritedIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(entity.Id, mergeResult);

                // Act
                var queryResult = (await connection.QueryAsync<InheritedIdentityTable>(entity.Id)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForInheritedWithNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Merge<InheritedIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.AreEqual(entity.Id, insertResult);
                Assert.AreEqual(1, connection.CountAll<InheritedIdentityTable>());

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var mergeResult = connection.Merge<InheritedIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(entity.Id, mergeResult);

                // Act
                var queryResult = connection.Query<InheritedIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForInheritedWithNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.MergeAsync<InheritedIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.AreEqual(entity.Id, insertResult);
                Assert.AreEqual(1, await connection.CountAllAsync<InheritedIdentityTable>());

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var mergeResult = await connection.MergeAsync<InheritedIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(entity.Id, mergeResult);

                // Act
                var queryResult = (await connection.QueryAsync<InheritedIdentityTable>(entity.Id)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region MergeAll

        [TestMethod]
        public void TestSqlConnectionMergeAllForInherited()
        {
            // Setup
            var entities = Helper.CreateInheritedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllRequest = connection.MergeAll<InheritedIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllRequest);

                // Act
                var queryResult = connection.QueryAll<InheritedIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult.First(e => e.Id == entity.Id)));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAllAsyncForInherited()
        {
            // Setup
            var entities = Helper.CreateInheritedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllRequest = await connection.MergeAllAsync<InheritedIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllRequest);

                // Act
                var queryResult = await connection.QueryAllAsync<InheritedIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult.First(e => e.Id == entity.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForInheritedWithNonEmptyTables()
        {
            // Setup
            var entities = Helper.CreateInheritedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = connection.InsertAll<InheritedIdentityTable>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnBit = false;
                    entity.ColumnDateTime2 = DateTime.UtcNow;
                });

                // Act
                var mergeAllResult = connection.MergeAll<InheritedIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);

                // Act
                var queryResult = connection.QueryAll<InheritedIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult.First(e => e.Id == entity.Id)));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAllAsyncForInheritedWithNonEmptyTables()
        {
            // Setup
            var entities = Helper.CreateInheritedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync<InheritedIdentityTable>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnBit = false;
                    entity.ColumnDateTime2 = DateTime.UtcNow;
                });

                // Act
                var mergeAllResult = await connection.MergeAllAsync<InheritedIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);

                // Act
                var queryResult = await connection.QueryAllAsync<InheritedIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult.First(e => e.Id == entity.Id)));
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestSqlConnectionQueryForInherited()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<InheritedIdentityTable, long>(entity);

                // Act
                var queryResult = connection.Query<InheritedIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForInherited()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<InheritedIdentityTable, long>(entity);

                // Act
                var queryResult = (await connection.QueryAsync<InheritedIdentityTable>(entity.Id)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestSqlConnectionUpdateForInheritedViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<InheritedIdentityTable, long>(entity);

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var updateResult = connection.Update<InheritedIdentityTable>(entity);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = connection.Query<InheritedIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAsyncForInheritedViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<InheritedIdentityTable, long>(entity);

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var updateResult = await connection.UpdateAsync<InheritedIdentityTable>(entity);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = (await connection.QueryAsync<InheritedIdentityTable>(entity.Id)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateForInheritedViaPrimaryKey()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<InheritedIdentityTable, long>(entity);

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var updateResult = connection.Update<InheritedIdentityTable>(entity, entity.Id);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = connection.Query<InheritedIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAsyncForInheritedViaPrimaryKey()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<InheritedIdentityTable, long>(entity);

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var updateResult = await connection.UpdateAsync<InheritedIdentityTable>(entity, entity.Id);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = (await connection.QueryAsync<InheritedIdentityTable>(entity.Id)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        public void TestSqlConnectionUpdateAllForInherited()
        {
            // Setup
            var entities = Helper.CreateInheritedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<InheritedIdentityTable>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnBit = false;
                    entity.ColumnDateTime2 = DateTime.UtcNow;
                });

                // Act
                var updateAllResult = connection.UpdateAll<InheritedIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<InheritedIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult.First(e => e.Id == entity.Id)));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncForInherited()
        {
            // Setup
            var entities = Helper.CreateInheritedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<InheritedIdentityTable>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnBit = false;
                    entity.ColumnDateTime2 = DateTime.UtcNow;
                });

                // Act
                var updateAllResult = await connection.UpdateAllAsync<InheritedIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = await connection.QueryAllAsync<InheritedIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult.First(e => e.Id == entity.Id)));
            }
        }

        #endregion
    }
}
