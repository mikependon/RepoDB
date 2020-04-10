using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using RepoDb;
using System.Collections.Generic;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class EnumPropertyTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            TypeMapper.Add(typeof(Continent), System.Data.DbType.Int16, true);
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region PropertyHandlers

        public class DirectionPropertyHandlerForBit : IPropertyHandler<bool?, BooleanValue?>
        {
            public BooleanValue? Get(bool? input, ClassProperty property)
            {
                if (input == null)
                {
                    return null;
                }
                return input == true ? BooleanValue.True : BooleanValue.False;
            }

            public bool? Set(BooleanValue? input, ClassProperty property)
            {
                if (input == null)
                {
                    return null;
                }
                return input == BooleanValue.True;
            }
        }

        public class DirectionPropertyHandlerForString : IPropertyHandler<string, Direction?>
        {
            public Direction? Get(string input, ClassProperty property)
            {
                if (input == null)
                {
                    return null;
                }
                return (Direction)Enum.Parse(typeof(Direction), input);
            }

            public string Set(Direction? input, ClassProperty property)
            {
                if (input == null)
                {
                    return null;
                }
                return input?.ToString();
            }
        }

        #endregion

        #region SubClasses

        [Map("[dbo].[CompleteTable]")]
        public class EnumCompleteTableWithPropertyHandler
        {
            public Guid SessionId { get; set; }
            [PropertyHandler(typeof(DirectionPropertyHandlerForBit))]
            public BooleanValue? ColumnBit { get; set; }
            [PropertyHandler(typeof(DirectionPropertyHandlerForString))]
            public Direction? ColumnNVarChar { get; set; }
        }

        #endregion

        #region Helpers

        public EnumCompleteTableWithPropertyHandler CreateEnumCompleteTableWithPropertyHandler()
        {
            return new EnumCompleteTableWithPropertyHandler
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = DateTime.UtcNow.Ticks % 2 == 0 ? BooleanValue.True : BooleanValue.False,
                ColumnNVarChar = (Direction)Enum.ToObject(typeof(Direction), Convert.ToInt32(DateTime.UtcNow.Ticks % 4))
            };
        }

        public IEnumerable<EnumCompleteTableWithPropertyHandler> CreateEnumCompleteTableWithPropertyHandlers(int count = 10)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new EnumCompleteTableWithPropertyHandler
                {
                    SessionId = Guid.NewGuid(),
                    ColumnBit = DateTime.UtcNow.Ticks % 2 == 0 ? BooleanValue.True : BooleanValue.False,
                    ColumnNVarChar = (Direction)Enum.ToObject(typeof(Direction), Convert.ToInt32(DateTime.UtcNow.Ticks % 4))
                };
            }
        }

        public EnumCompleteTableWithPropertyHandler CreateEnumCompleteTableWithPropertyHandlerAsNull()
        {
            return new EnumCompleteTableWithPropertyHandler
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = null,
                ColumnNVarChar = null
            };
        }

        public IEnumerable<EnumCompleteTableWithPropertyHandler> CreateEnumCompleteTableWithPropertyHandlersAsNull(int count = 10)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new EnumCompleteTableWithPropertyHandler
                {
                    SessionId = Guid.NewGuid(),
                    ColumnBit = null,
                    ColumnNVarChar = null
                };
            }
        }

        #endregion

        #region EnumProperties (PropertyHandler)

        #region Insert

        [TestMethod]
        public void TestInsertForEnumWithPropertyHandler()
        {
            // Setup
            var entity = CreateEnumCompleteTableWithPropertyHandler();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<EnumCompleteTableWithPropertyHandler, Guid>(entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<EnumCompleteTableWithPropertyHandler>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);
            }
        }

        [TestMethod]
        public void TestInsertForEnumWithPropertyHandlerAsNull()
        {
            // Setup
            var entity = CreateEnumCompleteTableWithPropertyHandlerAsNull();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<EnumCompleteTableWithPropertyHandler, Guid>(entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<EnumCompleteTableWithPropertyHandler>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);
            }
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestInsertAllForEnumWithPropertyHandler()
        {
            // Setup
            var entities = CreateEnumCompleteTableWithPropertyHandlers(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.InsertAll<EnumCompleteTableWithPropertyHandler>(entities);

                // Assert
                Assert.AreEqual(entities.Count(), connection.CountAll<EnumCompleteTableWithPropertyHandler>());
            }
        }

        [TestMethod]
        public void TestInsertAllForEnumWithPropertyHandlerAsNull()
        {
            // Setup
            var entities = CreateEnumCompleteTableWithPropertyHandlersAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.InsertAll<EnumCompleteTableWithPropertyHandler>(entities);

                // Assert
                Assert.AreEqual(entities.Count(), connection.CountAll<EnumCompleteTableWithPropertyHandler>());
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestQueryForEnumWithPropertyHandler()
        {
            // Setup
            var entity = CreateEnumCompleteTableWithPropertyHandler();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.Insert<EnumCompleteTableWithPropertyHandler, Guid>(entity);
                var queryResult = connection.Query<EnumCompleteTableWithPropertyHandler>(insertResult).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryForEnumWithPropertyHandlerAsNull()
        {
            // Setup
            var entity = CreateEnumCompleteTableWithPropertyHandlerAsNull();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.Insert<EnumCompleteTableWithPropertyHandler, Guid>(entity);
                var queryResult = connection.Query<EnumCompleteTableWithPropertyHandler>(insertResult).First();

                // Assert

                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region QueryAll

        [TestMethod]
        public void TestInsertAllForEnumWithPropertyHandlers()
        {
            // Setup
            var entities = CreateEnumCompleteTableWithPropertyHandlers(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.InsertAll<EnumCompleteTableWithPropertyHandler>(entities);
                var queryResult = connection.QueryAll<EnumCompleteTableWithPropertyHandler>().AsList();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestInsertAllForEnumWithPropertyHandlersAsNull()
        {
            // Setup
            var entities = CreateEnumCompleteTableWithPropertyHandlersAsNull(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.InsertAll<EnumCompleteTableWithPropertyHandler>(entities);
                var queryResult = connection.QueryAll<EnumCompleteTableWithPropertyHandler>().AsList();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        #endregion

        #endregion

        #region EnumAsParam in ExecuteMethods

        [TestMethod]
        public void TestExecuteQueryForEnumViaExpression()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(entities);
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
                var insertAllResult = connection.InsertAll(entities);
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
                var insertAllResult = connection.InsertAll(entities);

                // Assert
                var queryResult = connection.Query<EnumCompleteTable>(new { ColumnNVarChar = Direction.West });

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestQueryGroupForEnumViaExpression()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(entities);
                var queryResult = connection.Query((System.Linq.Expressions.Expression<Func<EnumCompleteTable, bool>>)(e => e.ColumnNVarChar == Direction.West));

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestQueryGroupForEnumViaQueryField()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(entities);
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
                var insertAllResult = connection.InsertAll(entities);
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
                var insertAllResult = connection.InsertAll(entities);
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
        public void TestInsertForEnumAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTableAsNull();

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

        [TestMethod]
        public void TestInsertForEnumAsIntForStringAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTableAsNull();

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
                var insertAllResult = connection.InsertAll(entities);

                // Assert
                Assert.AreEqual(insertAllResult, connection.CountAll<EnumCompleteTable>());
                var queryResult = connection.QueryAll<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestInsertAllForEnumAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(entities);

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
                var insertAllResult = connection.InsertAll(entities);

                // Assert
                Assert.AreEqual(insertAllResult, connection.CountAll<EnumAsIntForStringCompleteTable>());
                var queryResult = connection.QueryAll<EnumAsIntForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestInsertAllForEnumAsIntForStringAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(entities);

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
                var id = connection.Merge(entity);

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
        public void TestMergeForEnumAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Merge(entity);

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
                var mergeResult = connection.Merge(entity);

                // Assert
                Assert.AreEqual(entity.SessionId, mergeResult);

                // Act
                var queryResult = connection.QueryAll<EnumCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeForEnumForNonEmptyTableAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<EnumCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnBigInt = null;
                entity.ColumnBit = null;
                entity.ColumnInt = null;
                entity.ColumnNVarChar = null;
                entity.ColumnSmallInt = null;

                // Act
                var mergeResult = connection.Merge(entity);

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
                var id = connection.Merge(entity);

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
        public void TestMergeForEnumAsIntForStringAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Merge(entity);

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
                var mergeResult = connection.Merge(entity);

                // Assert
                Assert.AreEqual(entity.SessionId, mergeResult);

                // Act
                var queryResult = connection.QueryAll<EnumAsIntForStringCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeEnumAsIntForStringForNonEmptyTableAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<EnumAsIntForStringCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnNVarChar = null;

                // Act
                var mergeResult = connection.Merge(entity);

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
                var mergeAllResult = connection.MergeAll(entities);

                // Assert
                Assert.AreEqual(mergeAllResult, connection.CountAll<EnumCompleteTable>());
                var queryResult = connection.QueryAll<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForEnumAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll(entities);

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
                var insertAllResult = connection.InsertAll(entities);

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
                var mergeAllResult = connection.MergeAll(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);

                // Act
                var queryResult = connection.QueryAll<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForEnumForNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnBigInt = null;
                    entity.ColumnBit = null;
                    entity.ColumnInt = null;
                    entity.ColumnNVarChar = null;
                    entity.ColumnSmallInt = null;
                });

                // Act
                var mergeAllResult = connection.MergeAll(entities);

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
                var mergeAllResult = connection.MergeAll(entities);

                // Assert
                Assert.AreEqual(mergeAllResult, connection.CountAll<EnumAsIntForStringCompleteTable>());
                var queryResult = connection.QueryAll<EnumAsIntForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForEnumAsIntForStringAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll(entities);

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
                var insertAllResult = connection.InsertAll(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnNVarChar = Direction.East;
                });

                // Act
                var mergeAllResult = connection.MergeAll(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);

                // Act
                var queryResult = connection.QueryAll<EnumAsIntForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForEnumAsIntForStringForNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnNVarChar = null;
                });

                // Act
                var mergeAllResult = connection.MergeAll(entities);

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
        public void TestQueryForEnumAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTableAsNull();

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

        [TestMethod]
        public void TestQueryForEnumAsIntForStringAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTableAsNull();

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
                var updateResult = connection.Update(entity);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = connection.QueryAll<EnumCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateForEnumAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<EnumCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnBigInt = null;
                entity.ColumnBit = null;
                entity.ColumnInt = null;
                entity.ColumnNVarChar = null;
                entity.ColumnSmallInt = null;

                // Act
                var updateResult = connection.Update(entity);

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
                var updateResult = connection.Update(entity);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = connection.QueryAll<EnumAsIntForStringCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateForEnumAsIntForStringAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<EnumAsIntForStringCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnNVarChar = null;

                // Act
                var updateResult = connection.Update(entity);

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
                var insertAllResult = connection.InsertAll(entities);

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
                var updateAllResult = connection.UpdateAll(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestUpdateAllForEnumForNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnBigInt = null;
                    entity.ColumnBit = null;
                    entity.ColumnInt = null;
                    entity.ColumnNVarChar = null;
                    entity.ColumnSmallInt = null;
                });

                // Act
                var updateAllResult = connection.UpdateAll(entities);

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
                var insertAllResult = connection.InsertAll(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnNVarChar = Direction.East;
                });

                // Act
                var updateAllResult = connection.UpdateAll(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<EnumAsIntForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.Where(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestUpdateAllForEnumAsIntForStringForNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnNVarChar = null;
                });

                // Act
                var updateAllResult = connection.UpdateAll(entities);

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
