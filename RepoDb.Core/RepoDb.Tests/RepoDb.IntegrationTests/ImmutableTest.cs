using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
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

        #region Helper

        private ImmutableIdentityTable ToImmutableIdentityTable(IdentityTable entity)
        {
            return new ImmutableIdentityTable(entity.Id,
                entity.RowGuid,
                entity.ColumnBit,
                entity.ColumnDateTime,
                entity.ColumnDateTime2,
                entity.ColumnDecimal,
                entity.ColumnFloat,
                entity.ColumnInt,
                entity.ColumnNVarChar);
        }

        private ImmutableWithWritablePropertiesIdentityTable ToImmutableWithWritablePropertiesIdentityTable(IdentityTable entity)
        {
            return new ImmutableWithWritablePropertiesIdentityTable(entity.Id,
                entity.RowGuid,
                entity.ColumnBit,
                entity.ColumnDateTime,
                entity.ColumnDateTime2,
                entity.ColumnDecimal,
                entity.ColumnFloat,
                entity.ColumnInt,
                entity.ColumnNVarChar);
        }

        private ImmutableWithFewerCtorArgumentsIdentityTable ToImmutableWithFewerCtorArgumentsIdentityTable(IdentityTable entity)
        {
            return new ImmutableWithFewerCtorArgumentsIdentityTable(entity.Id,
                entity.RowGuid,
                entity.ColumnBit,
                entity.ColumnDateTime,
                entity.ColumnDateTime2)
            {
                ColumnDecimal = entity.ColumnDecimal,
                ColumnFloat = entity.ColumnFloat,
                ColumnInt = entity.ColumnInt,
                ColumnNVarChar = entity.ColumnNVarChar
            };
        }

        private MappedPropertiesImmutableIdentityTable ToMappedPropertiesImmutableIdentityTable(IdentityTable entity)
        {
            return new MappedPropertiesImmutableIdentityTable(entity.Id,
                entity.RowGuid,
                entity.ColumnBit,
                entity.ColumnDateTime,
                entity.ColumnDateTime2,
                entity.ColumnDecimal,
                entity.ColumnFloat,
                entity.ColumnInt,
                entity.ColumnNVarChar);
        }

        #endregion

        #region ImmutableIdentityTable

        #region Delete

        [TestMethod]
        public void TestSqlConnectionDeleteForImmutableViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var deleteResult = connection.Delete<ImmutableIdentityTable>(
                    ToImmutableIdentityTable(entity));

                // Assert
                Assert.IsTrue(deleteResult == 1);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncForImmutableViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTable>(entity);

                // Act
                var deleteResult = await connection.DeleteAsync<ImmutableIdentityTable>(
                    ToImmutableIdentityTable(entity));

                // Assert
                Assert.IsTrue(deleteResult == 1);
                Assert.AreEqual(0, await connection.CountAllAsync<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteForImmutableViaPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var deleteResult = connection.Delete<ImmutableIdentityTable>(entity.Id);

                // Assert
                Assert.IsTrue(deleteResult == 1);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncForImmutableViaPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTable>(entity);

                // Act
                var deleteResult = await connection.DeleteAsync<ImmutableIdentityTable>(entity.Id);

                // Assert
                Assert.IsTrue(deleteResult == 1);
                Assert.AreEqual(0, await connection.CountAllAsync<IdentityTable>());
            }
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestSqlConnectionInsertForImmutable()
        {
            // Setup
            var entity = Helper.CreateImmutableIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<ImmutableIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(insertResult > 0);

                // The ID could not be set back to the entity, so it should be 0

                // Assert
                Assert.AreEqual(0, entity.Id);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAsyncForImmutable()
        {
            // Setup
            var entity = Helper.CreateImmutableIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<ImmutableIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(insertResult > 0);

                // The ID could not be set back to the entity, so it should be 0

                // Assert
                Assert.AreEqual(0, entity.Id);
            }
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestSqlConnectionInsertAllForImmutable()
        {
            // Setup
            var entities = Helper.CreateImmutableIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = connection.InsertAll<ImmutableIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, insertAllResult);
                Assert.AreEqual(entities.Count, connection.CountAll<ImmutableIdentityTable>());

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(entities.All(e => e.Id == 0));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForImmutable()
        {
            // Setup
            var entities = Helper.CreateImmutableIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync<ImmutableIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, insertAllResult);
                Assert.AreEqual(entities.Count, await connection.CountAllAsync<ImmutableIdentityTable>());

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(entities.All(e => e.Id == 0));
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestSqlConnectionMergeForImmutable()
        {
            // Setup
            var entity = Helper.CreateImmutableIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeResult = connection.Merge<ImmutableIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(1, connection.CountAll<ImmutableIdentityTable>());

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(entity.Id == 0);

                // Act
                var queryResult = connection.Query<ImmutableIdentityTable>(mergeResult).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForImmutable()
        {
            // Setup
            var entity = Helper.CreateImmutableIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeResult = await connection.MergeAsync<ImmutableIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(1, await connection.CountAllAsync<ImmutableIdentityTable>());

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(entity.Id == 0);

                // Act
                var queryResult = (await connection.QueryAsync<ImmutableIdentityTable>(mergeResult)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForImmutableWithNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<IdentityTable, long>(entity);

                // Setup
                var newEntity = new ImmutableIdentityTable(insertResult,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar);

                // Act
                var mergeResult = connection.Merge<ImmutableIdentityTable, long>(newEntity);

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(insertResult, mergeResult);

                // Act
                var queryResult = connection.Query<ImmutableIdentityTable>(newEntity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForImmutableWithNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<IdentityTable, long>(entity);

                // Setup
                var newEntity = new ImmutableIdentityTable(insertResult,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar);

                // Act
                var mergeResult = await connection.MergeAsync<ImmutableIdentityTable, long>(newEntity);

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(insertResult, mergeResult);

                // Act
                var queryResult = (await connection.QueryAsync<ImmutableIdentityTable>(newEntity.Id)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        #endregion

        #region MergeAll

        [TestMethod]
        public void TestSqlConnectionMergeAllForImmutable()
        {
            // Setup
            var entities = Helper.CreateImmutableIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllRequest = connection.MergeAll<ImmutableIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllRequest);
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityTable>());

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(entities.All(e => e.Id == 0));

                // Act
                var queryResult = connection.QueryAll<ImmutableIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[entities.IndexOf(entity)]));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAllAsyncForImmutable()
        {
            // Setup
            var entities = Helper.CreateImmutableIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllRequest = await connection.MergeAllAsync<ImmutableIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllRequest);
                Assert.AreEqual(entities.Count, await connection.CountAllAsync<IdentityTable>());

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(entities.All(e => e.Id == 0));

                // Act
                var queryResult = (await connection.QueryAllAsync<ImmutableIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[entities.IndexOf(entity)]));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForImmutableWithNonEmptyTables()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = connection.InsertAll<IdentityTable>(entities);

                // Setup
                var newEntities = entities.Select(entity => new ImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar)).AsList();

                // Act
                var mergeAllResult = connection.MergeAll<ImmutableIdentityTable>(newEntities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);
                Assert.AreEqual(entities.Count, connection.CountAll<ImmutableIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<ImmutableIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                newEntities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[newEntities.IndexOf(entity)]));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAllAsyncForImmutableWithNonEmptyTables()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync<IdentityTable>(entities);

                // Setup
                var newEntities = entities.Select(entity => new ImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar)).AsList();

                // Act
                var mergeAllResult = await connection.MergeAllAsync<ImmutableIdentityTable>(newEntities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);
                Assert.AreEqual(entities.Count, await connection.CountAllAsync<ImmutableIdentityTable>());

                // Act
                var queryResult = (await connection.QueryAllAsync<ImmutableIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                newEntities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[newEntities.IndexOf(entity)]));
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestSqlConnectionQueryForImmutable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<IdentityTable, long>(entity);

                // Act
                var queryResult = connection.Query<ImmutableIdentityTable>(insertResult).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForImmutable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<IdentityTable, long>(entity);

                // Act
                var queryResult = (await connection.QueryAsync<ImmutableIdentityTable>(insertResult)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestSqlConnectionUpdateForImmutableViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable, long>(entity);

                // Setup
                var newEntity = new ImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar);

                // Act
                var updateResult = connection.Update<ImmutableIdentityTable>(newEntity);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = connection.Query<IdentityTable>(newEntity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAsyncForImmutableViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTable, long>(entity);

                // Setup
                var newEntity = new ImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar);

                // Act
                var updateResult = await connection.UpdateAsync<ImmutableIdentityTable>(newEntity);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = (await connection.QueryAsync<IdentityTable>(newEntity.Id)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateForImmutableViaPrimaryKey()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Setup
                var newEntity = new ImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar);

                // Act
                var updateResult = connection.Update<ImmutableIdentityTable>(newEntity, newEntity.Id);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = connection.Query<ImmutableIdentityTable>(newEntity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAsyncForImmutableViaPrimaryKey()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTable>(entity);

                // Setup
                var newEntity = new ImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar);

                // Act
                var updateResult = await connection.UpdateAsync<ImmutableIdentityTable>(newEntity, newEntity.Id);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = (await connection.QueryAsync<ImmutableIdentityTable>(newEntity.Id)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        public void TestSqlConnectionUpdateAllForImmutable()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Setup
                var newEntities = entities.Select(entity => new ImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar)).AsList();

                // Act
                var updateAllResult = connection.UpdateAll<ImmutableIdentityTable>(newEntities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<ImmutableIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                newEntities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[newEntities.IndexOf(entity)]));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncForImmutable()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Setup
                var newEntities = entities.Select(entity => new ImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar)).AsList();

                // Act
                var updateAllResult = await connection.UpdateAllAsync<ImmutableIdentityTable>(newEntities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = (await connection.QueryAllAsync<ImmutableIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                newEntities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[newEntities.IndexOf(entity)]));
            }
        }

        #endregion

        #endregion

        #region ImmutableWithFewerCtorArgumentsIdentityTable

        #region Delete

        [TestMethod]
        public void TestSqlConnectionDeleteForImmutableWithFewerCtorArgumentsViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var deleteResult = connection.Delete<ImmutableWithFewerCtorArgumentsIdentityTable>(
                    ToImmutableWithFewerCtorArgumentsIdentityTable(entity));

                // Assert
                Assert.IsTrue(deleteResult == 1);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncForImmutableWithFewerCtorArgumentsViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTable>(entity);

                // Act
                var deleteResult = await connection.DeleteAsync<ImmutableWithFewerCtorArgumentsIdentityTable>(
                    ToImmutableWithFewerCtorArgumentsIdentityTable(entity));

                // Assert
                Assert.IsTrue(deleteResult == 1);
                Assert.AreEqual(0, await connection.CountAllAsync<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteForImmutableWithFewerCtorArgumentsViaPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var deleteResult = connection.Delete<ImmutableWithFewerCtorArgumentsIdentityTable>(entity.Id);

                // Assert
                Assert.IsTrue(deleteResult == 1);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncForImmutableWithFewerCtorArgumentsViaPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTable>(entity);

                // Act
                var deleteResult = await connection.DeleteAsync<ImmutableWithFewerCtorArgumentsIdentityTable>(entity.Id);

                // Assert
                Assert.IsTrue(deleteResult == 1);
                Assert.AreEqual(0, await connection.CountAllAsync<IdentityTable>());
            }
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestSqlConnectionInsertForImmutableWithFewerCtorArguments()
        {
            // Setup
            var entity = Helper.CreateImmutableWithFewerCtorArgumentsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<ImmutableWithFewerCtorArgumentsIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(insertResult > 0);

                // The ID could not be set back to the entity, so it should be 0

                // Assert
                Assert.AreEqual(0, entity.Id);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAsyncForImmutableWithFewerCtorArguments()
        {
            // Setup
            var entity = Helper.CreateImmutableWithFewerCtorArgumentsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<ImmutableWithFewerCtorArgumentsIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(insertResult > 0);

                // The ID could not be set back to the entity, so it should be 0

                // Assert
                Assert.AreEqual(0, entity.Id);
            }
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestSqlConnectionInsertAllForImmutableWithFewerCtorArguments()
        {
            // Setup
            var entities = Helper.CreateImmutableWithFewerCtorArgumentsIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = connection.InsertAll<ImmutableWithFewerCtorArgumentsIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, insertAllResult);
                Assert.AreEqual(entities.Count, connection.CountAll<ImmutableWithFewerCtorArgumentsIdentityTable>());

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(entities.All(e => e.Id == 0));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForImmutableWithFewerCtorArguments()
        {
            // Setup
            var entities = Helper.CreateImmutableWithFewerCtorArgumentsIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync<ImmutableWithFewerCtorArgumentsIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, insertAllResult);
                Assert.AreEqual(entities.Count, await connection.CountAllAsync<ImmutableWithFewerCtorArgumentsIdentityTable>());

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(entities.All(e => e.Id == 0));
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestSqlConnectionMergeForImmutableWithFewerCtorArguments()
        {
            // Setup
            var entity = Helper.CreateImmutableWithFewerCtorArgumentsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeResult = connection.Merge<ImmutableWithFewerCtorArgumentsIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(1, connection.CountAll<ImmutableWithFewerCtorArgumentsIdentityTable>());

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(entity.Id == 0);

                // Act
                var queryResult = connection.Query<ImmutableWithFewerCtorArgumentsIdentityTable>(mergeResult).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForImmutableWithFewerCtorArguments()
        {
            // Setup
            var entity = Helper.CreateImmutableWithFewerCtorArgumentsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeResult = await connection.MergeAsync<ImmutableWithFewerCtorArgumentsIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(1, await connection.CountAllAsync<ImmutableWithFewerCtorArgumentsIdentityTable>());

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(entity.Id == 0);

                // Act
                var queryResult = (await connection.QueryAsync<ImmutableWithFewerCtorArgumentsIdentityTable>(mergeResult)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForImmutableWithFewerCtorArgumentsWithNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<IdentityTable, long>(entity);

                // Setup
                var newEntity = new ImmutableWithFewerCtorArgumentsIdentityTable(insertResult,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow)
                {
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                };

                // Act
                var mergeResult = connection.Merge<ImmutableWithFewerCtorArgumentsIdentityTable, long>(newEntity);

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(insertResult, mergeResult);

                // Act
                var queryResult = connection.Query<ImmutableWithFewerCtorArgumentsIdentityTable>(newEntity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForImmutableWithFewerCtorArgumentsWithNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<IdentityTable, long>(entity);

                // Setup
                var newEntity = new ImmutableWithFewerCtorArgumentsIdentityTable(insertResult,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow)
                {
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                };

                // Act
                var mergeResult = await connection.MergeAsync<ImmutableWithFewerCtorArgumentsIdentityTable, long>(newEntity);

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(insertResult, mergeResult);

                // Act
                var queryResult = (await connection.QueryAsync<ImmutableWithFewerCtorArgumentsIdentityTable>(newEntity.Id)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        #endregion

        #region MergeAll

        [TestMethod]
        public void TestSqlConnectionMergeAllForImmutableWithFewerCtorArguments()
        {
            // Setup
            var entities = Helper.CreateImmutableWithFewerCtorArgumentsIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllRequest = connection.MergeAll<ImmutableWithFewerCtorArgumentsIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllRequest);
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityTable>());

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(entities.All(e => e.Id == 0));

                // Act
                var queryResult = connection.QueryAll<ImmutableWithFewerCtorArgumentsIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[entities.IndexOf(entity)]));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAllAsyncForImmutableWithFewerCtorArguments()
        {
            // Setup
            var entities = Helper.CreateImmutableWithFewerCtorArgumentsIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllRequest = await connection.MergeAllAsync<ImmutableWithFewerCtorArgumentsIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllRequest);
                Assert.AreEqual(entities.Count, await connection.CountAllAsync<IdentityTable>());

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(entities.All(e => e.Id == 0));

                // Act
                var queryResult = (await connection.QueryAllAsync<ImmutableWithFewerCtorArgumentsIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[entities.IndexOf(entity)]));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForImmutableWithFewerCtorArgumentsWithNonEmptyTables()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = connection.InsertAll<IdentityTable>(entities);

                // Setup
                var newEntities = entities.Select(entity => new ImmutableWithFewerCtorArgumentsIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow)
                {
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                }).AsList();

                // Act
                var mergeAllResult = connection.MergeAll<ImmutableWithFewerCtorArgumentsIdentityTable>(newEntities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);
                Assert.AreEqual(entities.Count, connection.CountAll<ImmutableWithFewerCtorArgumentsIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<ImmutableWithFewerCtorArgumentsIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                newEntities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[newEntities.IndexOf(entity)]));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAllAsyncForImmutableWithFewerCtorArgumentsWithNonEmptyTables()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync<IdentityTable>(entities);

                // Setup
                var newEntities = entities.Select(entity => new ImmutableWithFewerCtorArgumentsIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow)
                {
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                }).AsList();

                // Act
                var mergeAllResult = await connection.MergeAllAsync<ImmutableWithFewerCtorArgumentsIdentityTable>(newEntities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);
                Assert.AreEqual(entities.Count, await connection.CountAllAsync<ImmutableWithFewerCtorArgumentsIdentityTable>());

                // Act
                var queryResult = (await connection.QueryAllAsync<ImmutableWithFewerCtorArgumentsIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                newEntities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[newEntities.IndexOf(entity)]));
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestSqlConnectionQueryForImmutableWithFewerCtorArguments()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<IdentityTable, long>(entity);

                // Act
                var queryResult = connection.Query<ImmutableWithFewerCtorArgumentsIdentityTable>(insertResult).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForImmutableWithFewerCtorArguments()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<IdentityTable, long>(entity);

                // Act
                var queryResult = (await connection.QueryAsync<ImmutableWithFewerCtorArgumentsIdentityTable>(insertResult)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestSqlConnectionUpdateForImmutableWithFewerCtorArgumentsViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable, long>(entity);

                // Setup
                var newEntity = new ImmutableWithFewerCtorArgumentsIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow)
                {
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                };

                // Act
                var updateResult = connection.Update<ImmutableWithFewerCtorArgumentsIdentityTable>(newEntity);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = connection.Query<ImmutableWithFewerCtorArgumentsIdentityTable>(newEntity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAsyncForImmutableWithFewerCtorArgumentsViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTable, long>(entity);

                // Setup
                var newEntity = new ImmutableWithFewerCtorArgumentsIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow)
                {
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                };

                // Act
                var updateResult = await connection.UpdateAsync<ImmutableWithFewerCtorArgumentsIdentityTable>(newEntity);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = (await connection.QueryAsync<ImmutableWithFewerCtorArgumentsIdentityTable>(newEntity.Id)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateForImmutableWithFewerCtorArgumentsViaPrimaryKey()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Setup
                var newEntity = new ImmutableWithFewerCtorArgumentsIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow)
                {
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                };

                // Act
                var updateResult = connection.Update<ImmutableWithFewerCtorArgumentsIdentityTable>(newEntity, newEntity.Id);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = connection.Query<ImmutableWithFewerCtorArgumentsIdentityTable>(newEntity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAsyncForImmutableWithFewerCtorArgumentsViaPrimaryKey()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTable>(entity);

                // Setup
                var newEntity = new ImmutableWithFewerCtorArgumentsIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow)
                {
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                };

                // Act
                var updateResult = await connection.UpdateAsync<ImmutableWithFewerCtorArgumentsIdentityTable>(newEntity, newEntity.Id);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = (await connection.QueryAsync<ImmutableWithFewerCtorArgumentsIdentityTable>(newEntity.Id)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        public void TestSqlConnectionUpdateAllForImmutableWithFewerCtorArguments()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Setup
                var newEntities = entities.Select(entity => new ImmutableWithFewerCtorArgumentsIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow)
                {
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                }).AsList();

                // Act
                var updateAllResult = connection.UpdateAll<ImmutableWithFewerCtorArgumentsIdentityTable>(newEntities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<ImmutableWithFewerCtorArgumentsIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                newEntities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[newEntities.IndexOf(entity)]));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncForImmutableWithFewerCtorArguments()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Setup
                var newEntities = entities.Select(entity => new ImmutableWithFewerCtorArgumentsIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow)
                {
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                }).AsList();

                // Act
                var updateAllResult = await connection.UpdateAllAsync<ImmutableWithFewerCtorArgumentsIdentityTable>(newEntities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = (await connection.QueryAllAsync<ImmutableWithFewerCtorArgumentsIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                newEntities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[newEntities.IndexOf(entity)]));
            }
        }

        #endregion

        #endregion

        #region ImmutableWithWritablePropertiesIdentityTable

        #region Delete

        [TestMethod]
        public void TestSqlConnectionDeleteForImmutableWithWritablePropertiesViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var deleteResult = connection.Delete<ImmutableWithWritablePropertiesIdentityTable>(
                    ToImmutableWithWritablePropertiesIdentityTable(entity));

                // Assert
                Assert.IsTrue(deleteResult == 1);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncForImmutableWithWritablePropertiesViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTable>(entity);

                // Act
                var deleteResult = await connection.DeleteAsync<ImmutableWithWritablePropertiesIdentityTable>(
                    ToImmutableWithWritablePropertiesIdentityTable(entity));

                // Assert
                Assert.IsTrue(deleteResult == 1);
                Assert.AreEqual(0, await connection.CountAllAsync<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteForImmutableWithWritablePropertiesViaPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var deleteResult = connection.Delete<ImmutableWithWritablePropertiesIdentityTable>(entity.Id);

                // Assert
                Assert.IsTrue(deleteResult == 1);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncForImmutableWithWritablePropertiesViaPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTable>(entity);

                // Act
                var deleteResult = await connection.DeleteAsync<ImmutableWithWritablePropertiesIdentityTable>(entity.Id);

                // Assert
                Assert.IsTrue(deleteResult == 1);
                Assert.AreEqual(0, await connection.CountAllAsync<IdentityTable>());
            }
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestSqlConnectionInsertForImmutableWithWritableProperties()
        {
            // Setup
            var entity = Helper.CreateImmutableWithWritablePropertiesIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<ImmutableWithWritablePropertiesIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.AreEqual(insertResult, entity.Id);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAsyncForImmutableWithWritableProperties()
        {
            // Setup
            var entity = Helper.CreateImmutableWithWritablePropertiesIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<ImmutableWithWritablePropertiesIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.AreEqual(insertResult, entity.Id);
            }
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestSqlConnectionInsertAllForImmutableWithWritableProperties()
        {
            // Setup
            var entities = Helper.CreateImmutableWithWritablePropertiesIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = connection.InsertAll<ImmutableWithWritablePropertiesIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, insertAllResult);
                Assert.AreEqual(entities.Count, connection.CountAll<ImmutableWithWritablePropertiesIdentityTable>());

                // Assert
                Assert.IsTrue(entities.All(e => e.Id > 0));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForImmutableWithWritableProperties()
        {
            // Setup
            var entities = Helper.CreateImmutableWithWritablePropertiesIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync<ImmutableWithWritablePropertiesIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, insertAllResult);
                Assert.AreEqual(entities.Count, await connection.CountAllAsync<ImmutableWithWritablePropertiesIdentityTable>());

                // Assert
                Assert.IsTrue(entities.All(e => e.Id > 0));
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestSqlConnectionMergeForImmutableWithWritableProperties()
        {
            // Setup
            var entity = Helper.CreateImmutableWithWritablePropertiesIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeResult = connection.Merge<ImmutableWithWritablePropertiesIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(1, connection.CountAll<ImmutableWithWritablePropertiesIdentityTable>());

                // Assert
                Assert.IsTrue(entity.Id == 1);

                // Act
                var queryResult = connection.Query<ImmutableWithWritablePropertiesIdentityTable>(mergeResult).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForImmutableWithWritableProperties()
        {
            // Setup
            var entity = Helper.CreateImmutableWithWritablePropertiesIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeResult = await connection.MergeAsync<ImmutableWithWritablePropertiesIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(1, await connection.CountAllAsync<ImmutableWithWritablePropertiesIdentityTable>());

                // Assert
                Assert.IsTrue(entity.Id == 1);

                // Act
                var queryResult = (await connection.QueryAsync<ImmutableWithWritablePropertiesIdentityTable>(mergeResult)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForImmutableWithWritablePropertiesWithNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<IdentityTable, long>(entity);

                // Setup
                var newEntity = new ImmutableWithWritablePropertiesIdentityTable(0,
                    Guid.NewGuid(),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null)
                {
                    Id = insertResult,
                    RowGuid = entity.RowGuid,
                    ColumnBit = false,
                    ColumnDateTime = entity.ColumnDateTime,
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                };

                // Act
                var mergeResult = connection.Merge<ImmutableWithWritablePropertiesIdentityTable, long>(newEntity);

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(insertResult, mergeResult);

                // Act
                var queryResult = connection.Query<ImmutableWithWritablePropertiesIdentityTable>(newEntity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForImmutableWithWritablePropertiesWithNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<IdentityTable, long>(entity);

                // Setup
                var newEntity = new ImmutableWithWritablePropertiesIdentityTable(0,
                    Guid.NewGuid(),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null)
                {
                    Id = insertResult,
                    RowGuid = entity.RowGuid,
                    ColumnBit = false,
                    ColumnDateTime = entity.ColumnDateTime,
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                };

                // Act
                var mergeResult = await connection.MergeAsync<ImmutableWithWritablePropertiesIdentityTable, long>(newEntity);

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(insertResult, mergeResult);

                // Act
                var queryResult = (await connection.QueryAsync<ImmutableWithWritablePropertiesIdentityTable>(newEntity.Id)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        #endregion

        #region MergeAll

        [TestMethod]
        public void TestSqlConnectionMergeAllForImmutableWithWritableProperties()
        {
            // Setup
            var entities = Helper.CreateImmutableWithWritablePropertiesIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllRequest = connection.MergeAll<ImmutableWithWritablePropertiesIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllRequest);
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityTable>());

                // Assert
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Act
                var queryResult = connection.QueryAll<ImmutableWithWritablePropertiesIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[entities.IndexOf(entity)]));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAllAsyncForImmutableWithWritableProperties()
        {
            // Setup
            var entities = Helper.CreateImmutableWithWritablePropertiesIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllRequest = await connection.MergeAllAsync<ImmutableWithWritablePropertiesIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllRequest);
                Assert.AreEqual(entities.Count, await connection.CountAllAsync<IdentityTable>());

                // Assert
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Act
                var queryResult = (await connection.QueryAllAsync<ImmutableWithWritablePropertiesIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[entities.IndexOf(entity)]));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForImmutableWithWritablePropertiesWithNonEmptyTables()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = connection.InsertAll<IdentityTable>(entities);

                // Setup
                var newEntities = entities.Select(entity => new ImmutableWithWritablePropertiesIdentityTable(0,
                    Guid.NewGuid(),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null)
                {
                    Id = entity.Id,
                    RowGuid = entity.RowGuid,
                    ColumnBit = false,
                    ColumnDateTime = entity.ColumnDateTime,
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                }).AsList();

                // Act
                var mergeAllResult = connection.MergeAll<ImmutableWithWritablePropertiesIdentityTable>(newEntities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);
                Assert.AreEqual(entities.Count, connection.CountAll<ImmutableWithWritablePropertiesIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<ImmutableWithWritablePropertiesIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                newEntities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[newEntities.IndexOf(entity)]));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAllAsyncForImmutableWithWritablePropertiesWithNonEmptyTables()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync<IdentityTable>(entities);

                // Setup
                var newEntities = entities.Select(entity => new ImmutableWithWritablePropertiesIdentityTable(0,
                    Guid.NewGuid(),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null)
                {
                    Id = entity.Id,
                    RowGuid = entity.RowGuid,
                    ColumnBit = false,
                    ColumnDateTime = entity.ColumnDateTime,
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                }).AsList();

                // Act
                var mergeAllResult = await connection.MergeAllAsync<ImmutableWithWritablePropertiesIdentityTable>(newEntities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);
                Assert.AreEqual(entities.Count, await connection.CountAllAsync<ImmutableWithWritablePropertiesIdentityTable>());

                // Act
                var queryResult = (await connection.QueryAllAsync<ImmutableWithWritablePropertiesIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                newEntities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[newEntities.IndexOf(entity)]));
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestSqlConnectionQueryForImmutableWithWritableProperties()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<IdentityTable, long>(entity);

                // Act
                var queryResult = connection.Query<ImmutableWithWritablePropertiesIdentityTable>(insertResult).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForImmutableWithWritableProperties()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<IdentityTable, long>(entity);

                // Act
                var queryResult = (await connection.QueryAsync<ImmutableWithWritablePropertiesIdentityTable>(insertResult)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestSqlConnectionUpdateForImmutableWithWritablePropertiesViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable, long>(entity);

                // Setup
                var newEntity = new ImmutableWithWritablePropertiesIdentityTable(0,
                    Guid.NewGuid(),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null)
                {
                    Id = entity.Id,
                    RowGuid = entity.RowGuid,
                    ColumnBit = false,
                    ColumnDateTime = entity.ColumnDateTime,
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                };

                // Act
                var updateResult = connection.Update<ImmutableWithWritablePropertiesIdentityTable>(newEntity);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = connection.Query<ImmutableWithWritablePropertiesIdentityTable>(newEntity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAsyncForImmutableWithWritablePropertiesViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTable, long>(entity);

                // Setup
                var newEntity = new ImmutableWithWritablePropertiesIdentityTable(0,
                    Guid.NewGuid(),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null)
                {
                    Id = entity.Id,
                    RowGuid = entity.RowGuid,
                    ColumnBit = false,
                    ColumnDateTime = entity.ColumnDateTime,
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                };

                // Act
                var updateResult = await connection.UpdateAsync<ImmutableWithWritablePropertiesIdentityTable>(newEntity);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = (await connection.QueryAsync<ImmutableWithWritablePropertiesIdentityTable>(newEntity.Id)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateForImmutableWithWritablePropertiesViaPrimaryKey()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Setup
                var newEntity = new ImmutableWithWritablePropertiesIdentityTable(0,
                    Guid.NewGuid(),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null)
                {
                    Id = entity.Id,
                    RowGuid = entity.RowGuid,
                    ColumnBit = false,
                    ColumnDateTime = entity.ColumnDateTime,
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                };

                // Act
                var updateResult = connection.Update<ImmutableWithWritablePropertiesIdentityTable>(newEntity, newEntity.Id);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = connection.Query<IdentityTable>(newEntity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAsyncForImmutableWithWritablePropertiesViaPrimaryKey()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTable>(entity);

                // Setup
                var newEntity = new ImmutableWithWritablePropertiesIdentityTable(0,
                    Guid.NewGuid(),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null)
                {
                    Id = entity.Id,
                    RowGuid = entity.RowGuid,
                    ColumnBit = false,
                    ColumnDateTime = entity.ColumnDateTime,
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                };

                // Act
                var updateResult = await connection.UpdateAsync<ImmutableWithWritablePropertiesIdentityTable>(newEntity, newEntity.Id);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = (await connection.QueryAsync<IdentityTable>(newEntity.Id)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        public void TestSqlConnectionUpdateAllForImmutableWithWritableProperties()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Setup
                var newEntities = entities.Select(entity => new ImmutableWithWritablePropertiesIdentityTable(0,
                    Guid.NewGuid(),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null)
                {
                    Id = entity.Id,
                    RowGuid = entity.RowGuid,
                    ColumnBit = false,
                    ColumnDateTime = entity.ColumnDateTime,
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                }).AsList();

                // Act
                var updateAllResult = connection.UpdateAll<ImmutableWithWritablePropertiesIdentityTable>(newEntities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<ImmutableWithWritablePropertiesIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                newEntities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[newEntities.IndexOf(entity)]));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncForImmutableWithWritableProperties()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Setup
                var newEntities = entities.Select(entity => new ImmutableWithWritablePropertiesIdentityTable(0,
                    Guid.NewGuid(),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null)
                {
                    Id = entity.Id,
                    RowGuid = entity.RowGuid,
                    ColumnBit = false,
                    ColumnDateTime = entity.ColumnDateTime,
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = entity.ColumnDecimal,
                    ColumnFloat = entity.ColumnFloat,
                    ColumnInt = entity.ColumnInt,
                    ColumnNVarChar = entity.ColumnNVarChar
                }).AsList();

                // Act
                var updateAllResult = await connection.UpdateAllAsync<ImmutableWithWritablePropertiesIdentityTable>(newEntities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = (await connection.QueryAllAsync<ImmutableWithWritablePropertiesIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                newEntities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[newEntities.IndexOf(entity)]));
            }
        }

        #endregion

        #endregion

        #region MappedPropertiesImmutableIdentityTable

        #region Delete

        [TestMethod]
        public void TestSqlConnectionDeleteForMappedPropertiesImmutableViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var deleteResult = connection.Delete<MappedPropertiesImmutableIdentityTable>(
                    ToMappedPropertiesImmutableIdentityTable(entity));

                // Assert
                Assert.IsTrue(deleteResult == 1);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncForMappedPropertiesImmutableViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTable>(entity);

                // Act
                var deleteResult = await connection.DeleteAsync<MappedPropertiesImmutableIdentityTable>(
                    ToMappedPropertiesImmutableIdentityTable(entity));

                // Assert
                Assert.IsTrue(deleteResult == 1);
                Assert.AreEqual(0, await connection.CountAllAsync<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteForMappedPropertiesImmutableViaPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var deleteResult = connection.Delete<MappedPropertiesImmutableIdentityTable>(entity.Id);

                // Assert
                Assert.IsTrue(deleteResult == 1);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncForMappedPropertiesImmutableViaPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTable>(entity);

                // Act
                var deleteResult = await connection.DeleteAsync<MappedPropertiesImmutableIdentityTable>(entity.Id);

                // Assert
                Assert.IsTrue(deleteResult == 1);
                Assert.AreEqual(0, await connection.CountAllAsync<IdentityTable>());
            }
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestSqlConnectionInsertForMappedPropertiesImmutable()
        {
            // Setup
            var entity = Helper.CreateMappedPropertiesImmutableIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<MappedPropertiesImmutableIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(insertResult > 0);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAsyncForMappedPropertiesImmutable()
        {
            // Setup
            var entity = Helper.CreateMappedPropertiesImmutableIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<MappedPropertiesImmutableIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(insertResult > 0);
            }
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestSqlConnectionInsertAllForMappedPropertiesImmutable()
        {
            // Setup
            var entities = Helper.CreateMappedPropertiesImmutableIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = connection.InsertAll<MappedPropertiesImmutableIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, insertAllResult);
                Assert.AreEqual(entities.Count, connection.CountAll<MappedPropertiesImmutableIdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionInsertAllAsyncForMappedPropertiesImmutable()
        {
            // Setup
            var entities = Helper.CreateMappedPropertiesImmutableIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync<MappedPropertiesImmutableIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, insertAllResult);
                Assert.AreEqual(entities.Count, await connection.CountAllAsync<MappedPropertiesImmutableIdentityTable>());
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestSqlConnectionMergeForMappedPropertiesImmutable()
        {
            // Setup
            var entity = Helper.CreateMappedPropertiesImmutableIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeResult = connection.Merge<MappedPropertiesImmutableIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(1, connection.CountAll<MappedPropertiesImmutableIdentityTable>());

                // Act
                var queryResult = connection.Query<MappedPropertiesImmutableIdentityTable>(mergeResult).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForMappedPropertiesImmutable()
        {
            // Setup
            var entity = Helper.CreateMappedPropertiesImmutableIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeResult = await connection.MergeAsync<MappedPropertiesImmutableIdentityTable, long>(entity);

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(1, await connection.CountAllAsync<MappedPropertiesImmutableIdentityTable>());

                // Act
                var queryResult = (await connection.QueryAsync<MappedPropertiesImmutableIdentityTable>(mergeResult)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForMappedPropertiesImmutableWithNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<IdentityTable, long>(entity);

                // Setup
                var newEntity = new MappedPropertiesImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar);

                // Act
                var mergeResult = connection.Merge<MappedPropertiesImmutableIdentityTable, long>(newEntity);

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(insertResult, mergeResult);

                // Act
                var queryResult = connection.Query<MappedPropertiesImmutableIdentityTable>(newEntity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForMappedPropertiesImmutableWithNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<IdentityTable, long>(entity);

                // Setup
                var newEntity = new MappedPropertiesImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar);

                // Act
                var mergeResult = await connection.MergeAsync<MappedPropertiesImmutableIdentityTable, long>(newEntity);

                // The ID could not be set back to the entities, so it should be 0

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(insertResult, mergeResult);

                // Act
                var queryResult = (await connection.QueryAsync<MappedPropertiesImmutableIdentityTable>(newEntity.Id)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        #endregion

        #region MergeAll

        [TestMethod]
        public void TestSqlConnectionMergeAllForMappedPropertiesImmutable()
        {
            // Setup
            var entities = Helper.CreateMappedPropertiesImmutableIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllRequest = connection.MergeAll<MappedPropertiesImmutableIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllRequest);
                Assert.AreEqual(entities.Count, connection.CountAll<MappedPropertiesImmutableIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<MappedPropertiesImmutableIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[entities.IndexOf(entity)]));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAllAsyncForMappedPropertiesImmutable()
        {
            // Setup
            var entities = Helper.CreateMappedPropertiesImmutableIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllRequest = await connection.MergeAllAsync<MappedPropertiesImmutableIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllRequest);
                Assert.AreEqual(entities.Count, await connection.CountAllAsync<MappedPropertiesImmutableIdentityTable>());

                // Act
                var queryResult = (await connection.QueryAllAsync<MappedPropertiesImmutableIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[entities.IndexOf(entity)]));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForMappedPropertiesImmutableWithNonEmptyTables()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = connection.InsertAll<IdentityTable>(entities);

                // Setup
                var newEntities = entities.Select(entity => new MappedPropertiesImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar)).AsList();

                // Act
                var mergeAllResult = connection.MergeAll<MappedPropertiesImmutableIdentityTable>(newEntities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);
                Assert.AreEqual(entities.Count, connection.CountAll<MappedPropertiesImmutableIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<MappedPropertiesImmutableIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                newEntities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[newEntities.IndexOf(entity)]));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAllAsyncForMappedPropertiesImmutableWithNonEmptyTables()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync<IdentityTable>(entities);

                // Setup
                var newEntities = entities.Select(entity => new MappedPropertiesImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar)).AsList();

                // Act
                var mergeAllResult = await connection.MergeAllAsync<MappedPropertiesImmutableIdentityTable>(newEntities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);
                Assert.AreEqual(entities.Count, await connection.CountAllAsync<MappedPropertiesImmutableIdentityTable>());

                // Act
                var queryResult = (await connection.QueryAllAsync<MappedPropertiesImmutableIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                newEntities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[newEntities.IndexOf(entity)]));
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestSqlConnectionQueryForMappedPropertiesImmutable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<IdentityTable, long>(entity);

                // Act
                var queryResult = connection.Query<MappedPropertiesImmutableIdentityTable>(insertResult).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForMappedPropertiesImmutable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<IdentityTable, long>(entity);

                // Act
                var queryResult = (await connection.QueryAsync<MappedPropertiesImmutableIdentityTable>(insertResult)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestSqlConnectionUpdateForMappedPropertiesImmutableViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable, long>(entity);

                // Setup
                var newEntity = new MappedPropertiesImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar);

                // Act
                var updateResult = connection.Update<MappedPropertiesImmutableIdentityTable>(newEntity);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = connection.Query<MappedPropertiesImmutableIdentityTable>(newEntity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAsyncForMappedPropertiesImmutableViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTable, long>(entity);

                // Setup
                var newEntity = new MappedPropertiesImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar);

                // Act
                var updateResult = await connection.UpdateAsync<MappedPropertiesImmutableIdentityTable>(newEntity);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = (await connection.QueryAsync<MappedPropertiesImmutableIdentityTable>(newEntity.Id)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateForMappedPropertiesImmutableViaPrimaryKey()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Setup
                var newEntity = new MappedPropertiesImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar);

                // Act
                var updateResult = connection.Update<MappedPropertiesImmutableIdentityTable>(newEntity, newEntity.Id);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = connection.Query<MappedPropertiesImmutableIdentityTable>(newEntity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAsyncForMappedPropertiesImmutableViaPrimaryKey()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<IdentityTable>(entity);

                // Setup
                var newEntity = new MappedPropertiesImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar);

                // Act
                var updateResult = await connection.UpdateAsync<MappedPropertiesImmutableIdentityTable>(newEntity, newEntity.Id);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = (await connection.QueryAsync<MappedPropertiesImmutableIdentityTable>(newEntity.Id)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(newEntity, queryResult);
            }
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        public void TestSqlConnectionUpdateAllForMappedPropertiesImmutable()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Setup
                var newEntities = entities.Select(entity => new MappedPropertiesImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar)).AsList();

                // Act
                var updateAllResult = connection.UpdateAll<MappedPropertiesImmutableIdentityTable>(newEntities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<MappedPropertiesImmutableIdentityTable>().AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                newEntities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[newEntities.IndexOf(entity)]));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionUpdateAllAsyncForMappedPropertiesImmutable()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Setup
                var newEntities = entities.Select(entity => new MappedPropertiesImmutableIdentityTable(entity.Id,
                    entity.RowGuid,
                    false,
                    entity.ColumnDateTime,
                    DateTime.UtcNow,
                    entity.ColumnDecimal,
                    entity.ColumnFloat,
                    entity.ColumnInt,
                    entity.ColumnNVarChar)).AsList();

                // Act
                var updateAllResult = await connection.UpdateAllAsync<MappedPropertiesImmutableIdentityTable>(newEntities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = (await connection.QueryAllAsync<MappedPropertiesImmutableIdentityTable>()).AsList();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                newEntities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult[newEntities.IndexOf(entity)]));
            }
        }

        #endregion

        #endregion

        #region Constructor/ExecuteQuery

        #region ExecuteQuery (Matched CTOR Arguments)

        private class ImmutableWithMatchedCtorArguments
        {
            public ImmutableWithMatchedCtorArguments(int id,
                DateTime value)
            {
                Id = id;
                Value = value;
            }

            public int Id { get; set; }
            public DateTime Value { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryForImmutableWithMatchedCtorArguments()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new { Value = DateTime.UtcNow.Date };
                var sql = "SELECT 1 AS [Id], @Value AS [Value];";

                // Act
                var queryResult = connection.ExecuteQuery<ImmutableWithMatchedCtorArguments>(sql, param).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Id);
                Assert.AreEqual(param.Value, queryResult.Value);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncForImmutableWithMatchedCtorArguments()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new { Value = DateTime.UtcNow.Date };
                var sql = "SELECT 1 AS [Id], @Value AS [Value];";

                // Act
                var queryResult = (await connection.ExecuteQueryAsync<ImmutableWithMatchedCtorArguments>(sql, param)).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Id);
                Assert.AreEqual(param.Value, queryResult.Value);
            }
        }

        #endregion

        #region ExecuteQuery (Matched CTOR Arguments From Multiple CTORs)

        private class ImmutableWithMatchedCtorArgumentsFromMultipleCtors
        {
            public ImmutableWithMatchedCtorArgumentsFromMultipleCtors()
            { }

            public ImmutableWithMatchedCtorArgumentsFromMultipleCtors(int id,
                DateTime value)
            {
                Id = id;
                Value = value;
            }

            public int Id { get; set; }
            public DateTime Value { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryForImmutableWithMatchedCtorArgumentsFromMultipleCtors()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new { Value = DateTime.UtcNow.Date };
                var sql = "SELECT 1 AS [Id], @Value AS [Value];";

                // Act
                var queryResult = connection.ExecuteQuery<ImmutableWithMatchedCtorArgumentsFromMultipleCtors>(sql, param).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Id);
                Assert.AreEqual(param.Value, queryResult.Value);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncForImmutableWithMatchedCtorArgumentsFromMultipleCtors()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new { Value = DateTime.UtcNow.Date };
                var sql = "SELECT 1 AS [Id], @Value AS [Value];";

                // Act
                var queryResult = (await connection.ExecuteQueryAsync<ImmutableWithMatchedCtorArgumentsFromMultipleCtors>(sql, param)).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, queryResult.Id);
                Assert.AreEqual(param.Value, queryResult.Value);
            }
        }

        #endregion

        #region ExecuteQuery (Unmatched CTOR Arguments From Multiple CTORs)

        private class ImmutableWithUnmatchedCtorArgumentsFromMultipleCtors
        {
            public ImmutableWithUnmatchedCtorArgumentsFromMultipleCtors()
            { }

            public ImmutableWithUnmatchedCtorArgumentsFromMultipleCtors(int id,
                DateTime value,
                string extra)
            {
                Id = id;
                Value = value;
                Extra = extra;
            }

            public int Id { get; set; }
            public DateTime Value { get; set; }
            public string Extra { get; set; }
        }

        [TestMethod]
        public void ThrowExceptionOnSqlConnectionExecuteQueryForImmutableWithUnmatchedCtorArgumentsFromMultipleCtors()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new { Value = DateTime.UtcNow.Date };
                var sql = "SELECT 1 AS [Id], @Value AS [Value];";

                // Act
                Assert.Throws<MissingMemberException>(() => connection.ExecuteQuery<ImmutableWithUnmatchedCtorArgumentsFromMultipleCtors>(sql, param));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnSqlConnectionExecuteQueryAsyncForImmutableWithUnmatchedCtorArgumentsFromMultipleCtors()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new { Value = DateTime.UtcNow.Date };
                var sql = "SELECT 1 AS [Id], @Value AS [Value];";

                // Act
                await Assert.ThrowsAsync<MissingMemberException>(async () => await connection.ExecuteQueryAsync<ImmutableWithUnmatchedCtorArgumentsFromMultipleCtors>(sql, param));
            }
        }

        #endregion

        #endregion
    }
}
