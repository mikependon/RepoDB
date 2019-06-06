using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class EnumPropertyTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            TypeMapper.Map(typeof(Continent), System.Data.DbType.Int16, true);
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region EnumAsParam in ExecuteMethods

        [TestMethod]
        public void TestExecuteQueryForEnumViaExpression()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<EnumCompleteTable>(entities);
                var executeResult = connection.ExecuteQuery<EnumCompleteTable>("SELECT * FROM CompleteTable WHERE ColumnNVarChar = @ColumnNVarChar;",
                    new { ColumnNVarChar = Direction.West });

                // Assert
                Assert.AreEqual(entities.Count, executeResult.Count());

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, executeResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestExecuteQueryForMappedEnumViaExpression()
        {
            // Setup
            var entities = Helper.CreateTypeLevelMappedForStringEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<TypeLevelMappedForStringEnumCompleteTable>(entities);
                var executeResult = connection.ExecuteQuery<TypeLevelMappedForStringEnumCompleteTable>("SELECT * FROM CompleteTable WHERE ColumnNVarChar = @ColumnNVarChar;",
                    new { ColumnNVarChar = Continent.Asia });

                // Assert
                Assert.AreEqual(entities.Count, executeResult.Count());

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, executeResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        #endregion

        #region EnumAsParam in QueryGroup

        [TestMethod]
        public void TestQueryGroupForEnumViaDynamic()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<EnumCompleteTable>(entities);

                // Assert
                var queryResult = connection.Query<EnumCompleteTable>(new { ColumnNVarChar = Direction.West });

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        //[TestMethod]
        //public void TestQueryGroupForEnumViaExpression()
        //{
        //    // Setup
        //    var entities = Helper.CreateEnumCompleteTables(10);

        //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        //    {
        //        // Act
        //        var insertAllResult = connection.InsertAll<EnumCompleteTable>(entities);
        //        var queryResult = connection.Query<EnumCompleteTable>(e => e.ColumnNVarChar == Direction.West);

        //        // Assert
        //        Assert.AreEqual(entities.Count, queryResult.Count());

        //        // Assert
        //        entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
        //    }
        //}

        [TestMethod]
        public void TestQueryGroupForEnumViaQueryField()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<EnumCompleteTable>(entities);
                var queryResult = connection.Query<EnumCompleteTable>(new QueryField("ColumnNVarChar", Direction.West));

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestQueryGroupForEnumViaQueryFields()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<EnumCompleteTable>(entities);
                var queryResult = connection.Query<EnumCompleteTable>(new QueryField("ColumnNVarChar", Direction.West).AsEnumerable());

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestQueryGroupForEnumViaQueryGroup()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<EnumCompleteTable>(entities);
                var queryResult = connection.Query<EnumCompleteTable>(new QueryGroup(new QueryField("ColumnNVarChar", Direction.West)));

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestInsertForEnum()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<EnumCompleteTable, Guid>(entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<EnumCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);
            }
        }

        [TestMethod]
        public void TestInsertForEnumAsIntForString()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<EnumAsIntForStringCompleteTable, Guid>(entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<EnumAsIntForStringCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);
            }
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestInsertAllForEnum()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<EnumCompleteTable>(entities);

                // Assert
                Assert.AreEqual(insertAllResult, connection.CountAll<EnumCompleteTable>());
                var queryResult = connection.QueryAll<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestInsertAllForEnumAsIntForString()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<EnumAsIntForStringCompleteTable>(entities);

                // Assert
                Assert.AreEqual(insertAllResult, connection.CountAll<EnumAsIntForStringCompleteTable>());
                var queryResult = connection.QueryAll<EnumAsIntForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestMergeForEnum()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Merge<EnumCompleteTable>(entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<EnumCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = connection.QueryAll<EnumCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeForEnumForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<EnumCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnBigInt = Direction.East;
                entity.ColumnBit = BooleanValue.False;
                entity.ColumnInt = Direction.East;
                entity.ColumnNVarChar = Direction.East;
                entity.ColumnSmallInt = Direction.East;

                // Act
                var mergeResult = connection.Merge<EnumCompleteTable>(entity);

                // Assert
                Assert.AreEqual(entity.SessionId, mergeResult);

                // Act
                var queryResult = connection.QueryAll<EnumCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeForEnumAsIntForString()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Merge<EnumAsIntForStringCompleteTable>(entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<EnumAsIntForStringCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = connection.QueryAll<EnumAsIntForStringCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeEnumAsIntForStringForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<EnumAsIntForStringCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnNVarChar = Direction.East;

                // Act
                var mergeResult = connection.Merge<EnumAsIntForStringCompleteTable>(entity);

                // Assert
                Assert.AreEqual(entity.SessionId, mergeResult);

                // Act
                var queryResult = connection.QueryAll<EnumAsIntForStringCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region MergeAll

        [TestMethod]
        public void TestMergeAllForEnum()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<EnumCompleteTable>(entities);

                // Assert
                Assert.AreEqual(mergeAllResult, connection.CountAll<EnumCompleteTable>());
                var queryResult = connection.QueryAll<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForEnumForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<EnumCompleteTable>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnBigInt = Direction.East;
                    entity.ColumnBit = BooleanValue.False;
                    entity.ColumnInt = Direction.East;
                    entity.ColumnNVarChar = Direction.East;
                    entity.ColumnSmallInt = Direction.East;
                });

                // Act
                var mergeAllResult = connection.MergeAll<EnumCompleteTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);

                // Act
                var queryResult = connection.QueryAll<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForEnumAsIntForString()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll<EnumAsIntForStringCompleteTable>(entities);

                // Assert
                Assert.AreEqual(mergeAllResult, connection.CountAll<EnumAsIntForStringCompleteTable>());
                var queryResult = connection.QueryAll<EnumAsIntForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForEnumAsIntForStringForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<EnumAsIntForStringCompleteTable>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnNVarChar = Direction.East;
                });

                // Act
                var mergeAllResult = connection.MergeAll<EnumAsIntForStringCompleteTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);

                // Act
                var queryResult = connection.QueryAll<EnumAsIntForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestQueryForEnum()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.Insert<EnumCompleteTable, Guid>(entity);
                var queryResult = connection.QueryAll<EnumCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryForEnumAsIntForString()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.Insert<EnumAsIntForStringCompleteTable, Guid>(entity);
                var queryResult = connection.QueryAll<EnumAsIntForStringCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestUpdateForEnum()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<EnumCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnBigInt = Direction.East;
                entity.ColumnBit = BooleanValue.False;
                entity.ColumnInt = Direction.East;
                entity.ColumnNVarChar = Direction.East;
                entity.ColumnSmallInt = Direction.East;

                // Act
                var updateResult = connection.Update<EnumCompleteTable>(entity);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = connection.QueryAll<EnumCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateForEnumAsIntForString()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<EnumAsIntForStringCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnNVarChar = Direction.East;

                // Act
                var updateResult = connection.Update<EnumAsIntForStringCompleteTable>(entity);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = connection.QueryAll<EnumAsIntForStringCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        public void TestUpdateAllForEnumForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<EnumCompleteTable>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnBigInt = Direction.East;
                    entity.ColumnBit = BooleanValue.False;
                    entity.ColumnInt = Direction.East;
                    entity.ColumnNVarChar = Direction.East;
                    entity.ColumnSmallInt = Direction.East;
                });

                // Act
                var updateAllResult = connection.UpdateAll<EnumCompleteTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestUpdateAllForEnumAsIntForStringForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<EnumAsIntForStringCompleteTable>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnNVarChar = Direction.East;
                });

                // Act
                var updateAllResult = connection.UpdateAll<EnumAsIntForStringCompleteTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<EnumAsIntForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        #endregion
    }
}
