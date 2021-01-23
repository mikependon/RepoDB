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
using System.Linq.Expressions;

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
        public static List<EnumCompleteTable> CreateEnumCompleteTablesRandomized(int count)
        {
            var tables = new List<EnumCompleteTable>();
            for (var i = 0; i < count; i++)
            {
                var direction = i % 2 == 0 ? Direction.West : Direction.East;
                var index = i + 1;
                tables.Add(new EnumCompleteTable
                {
                    SessionId = Guid.NewGuid(),
                    ColumnBit = BooleanValue.True,
                    ColumnNVarChar = direction,
                    ColumnInt = direction,
                    ColumnBigInt = direction,
                    ColumnSmallInt = direction
                });
            }
            return tables;
        }

        #endregion

        #region EnumProperties (PropertyHandler)

        #region ExecuteScalar

        [TestMethod]
        public void TestExecuteScalarForEnumWithPropertyHandlerFor()
        {
            // Setup
            var entity = CreateEnumCompleteTableWithPropertyHandler();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.ExecuteScalar<Guid>("INSERT INTO [dbo].[CompleteTable] " +
                    "(SessionId, ColumnBit, ColumnNVarChar) " +
                    "VALUES " +
                    "(@SessionId, @ColumnBit, @ColumnNVarChar); " +
                    "SELECT CONVERT(UNIQUEIDENTIFIER, @SessionId);", entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<EnumCompleteTableWithPropertyHandler>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);
            }
        }

        #endregion

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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                Assert.AreEqual(entities.Where(e => e.ColumnNVarChar == Continent.Asia).Count(), executeResult.Count());

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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestQueryGroupForEnumViaExpression()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);
            var where = (Expression<Func<EnumCompleteTable, bool>>)(e => e.ColumnNVarChar == Direction.West);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(entities);
                var queryResult = connection.Query(where);

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        #endregion

        #region EnumAsParam in QueryGroup (OR)

        [TestMethod]
        public void TestQueryGroupForEnumForTextWithOrConditionViaExpression()
        {
            // Setup
            var entities = CreateEnumCompleteTablesRandomized(10);
            var where = (Expression<Func<EnumCompleteTable, bool>>)(e => e.ColumnNVarChar == Direction.West || e.ColumnNVarChar == Direction.East);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(entities);
                var queryResult = connection.Query(where);

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestQueryGroupForEnumForNonTextWithOrConditionViaExpression()
        {
            // Setup
            var entities = CreateEnumCompleteTablesRandomized(10);
            var where = (Expression<Func<EnumCompleteTable, bool>>)(e => e.ColumnInt == Direction.West || e.ColumnInt == Direction.East);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(entities);
                var queryResult = connection.Query(where);

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestQueryGroupForEnumsForTextWithOrConditionViaQueryGroup()
        {
            // Setup
            var entities = CreateEnumCompleteTablesRandomized(10);
            var fields = new[]
            {
                new QueryField("ColumnNVarChar", Direction.West),
                new QueryField("ColumnNVarChar", Direction.East)
            };
            var where = new QueryGroup(fields, RepoDb.Enumerations.Conjunction.Or);


            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(entities);
                var queryResult = connection.Query<EnumCompleteTable>(where);

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestQueryGroupForEnumsForNonTextWithOrConditionViaQueryGroup()
        {
            // Setup
            var entities = CreateEnumCompleteTablesRandomized(10);
            var fields = new[]
            {
                new QueryField("ColumnInt", Direction.West),
                new QueryField("ColumnInt", Direction.East)
            };
            var where = new QueryGroup(fields, RepoDb.Enumerations.Conjunction.Or);


            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(entities);
                var queryResult = connection.Query<EnumCompleteTable>(where);

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        #endregion

        #region CRUD

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

                // Act
                var queryResult = connection.Query<EnumCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
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

                // Act
                var queryResult = connection.Query<FlaggedEnumForIntCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
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

                // Act
                var queryResult = connection.Query<EnumAsIntForStringCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
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

                // Act
                var queryResult = connection.Query<FlaggedEnumForIntCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
            }
        }

        [TestMethod]
        public void TestInsertForFlaggedEnumForString()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<FlaggedEnumForStringCompleteTable, Guid>(entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<FlaggedEnumForStringCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = connection.Query<FlaggedEnumForIntCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
            }
        }

        [TestMethod]
        public void TestInsertForFlaggedEnumForStringAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<FlaggedEnumForStringCompleteTable, Guid>(entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<FlaggedEnumForStringCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = connection.Query<FlaggedEnumForIntCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
            }
        }

        [TestMethod]
        public void TestInsertForFlaggedEnumForInt()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<FlaggedEnumForIntCompleteTable, Guid>(entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<FlaggedEnumForIntCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = connection.Query<FlaggedEnumForIntCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
            }
        }

        [TestMethod]
        public void TestInsertForFlaggedEnumForIntAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<FlaggedEnumForIntCompleteTable, Guid>(entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<FlaggedEnumForIntCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = connection.Query<FlaggedEnumForIntCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestInsertAllForFlaggedEnumForString()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<FlaggedEnumForStringCompleteTable>(entities);

                // Assert
                Assert.AreEqual(insertAllResult, connection.CountAll<FlaggedEnumForStringCompleteTable>());
                var queryResult = connection.QueryAll<FlaggedEnumForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestInsertAllForFlaggedEnumForStringAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<FlaggedEnumForStringCompleteTable>(entities);

                // Assert
                Assert.AreEqual(insertAllResult, connection.CountAll<FlaggedEnumForStringCompleteTable>());
                var queryResult = connection.QueryAll<FlaggedEnumForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestInsertAllForFlaggedEnumForInt()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<FlaggedEnumForIntCompleteTable>(entities);

                // Assert
                Assert.AreEqual(insertAllResult, connection.CountAll<FlaggedEnumForIntCompleteTable>());
                var queryResult = connection.QueryAll<FlaggedEnumForIntCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestInsertAllForFlaggedEnumForIntAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<FlaggedEnumForIntCompleteTable>(entities);

                // Assert
                Assert.AreEqual(insertAllResult, connection.CountAll<FlaggedEnumForIntCompleteTable>());
                var queryResult = connection.QueryAll<FlaggedEnumForIntCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                entity.ColumnSmallInt = Direction.None;

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

        [TestMethod]
        public void TestMergeForFlaggedEnumForStringCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Merge(entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<FlaggedEnumForStringCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = connection.QueryAll<FlaggedEnumForStringCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeForFlaggedEnumForStringCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Merge(entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<FlaggedEnumForStringCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = connection.QueryAll<FlaggedEnumForStringCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeForFlaggedEnumForIntCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Merge(entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<FlaggedEnumForIntCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = connection.QueryAll<FlaggedEnumForIntCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeForFlaggedEnumForIntCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Merge(entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<FlaggedEnumForIntCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = connection.QueryAll<FlaggedEnumForIntCompleteTable>().First();

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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                    entity.ColumnSmallInt = Direction.None;
                });

                // Act
                var mergeAllResult = connection.MergeAll(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);

                // Act
                var queryResult = connection.QueryAll<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForFlaggedEnumForStringCompleteTable()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll(entities);

                // Assert
                Assert.AreEqual(mergeAllResult, connection.CountAll<FlaggedEnumForStringCompleteTable>());
                var queryResult = connection.QueryAll<FlaggedEnumForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForFlaggedEnumForStringCompleteTableAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll(entities);

                // Assert
                Assert.AreEqual(mergeAllResult, connection.CountAll<FlaggedEnumForStringCompleteTable>());
                var queryResult = connection.QueryAll<FlaggedEnumForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForFlaggedEnumForIntCompleteTable()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll(entities);

                // Assert
                Assert.AreEqual(mergeAllResult, connection.CountAll<FlaggedEnumForIntCompleteTable>());
                var queryResult = connection.QueryAll<FlaggedEnumForIntCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForFlaggedEnumForIntCompleteTableAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllResult = connection.MergeAll(entities);

                // Assert
                Assert.AreEqual(mergeAllResult, connection.CountAll<FlaggedEnumForIntCompleteTable>());
                var queryResult = connection.QueryAll<FlaggedEnumForIntCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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

        [TestMethod]
        public void TestQueryForFlaggedEnumForStringCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.Insert<FlaggedEnumForStringCompleteTable, Guid>(entity);
                var queryResult = connection.QueryAll<FlaggedEnumForStringCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryForFlaggedEnumForStringCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.Insert<FlaggedEnumForStringCompleteTable, Guid>(entity);
                var queryResult = connection.QueryAll<FlaggedEnumForStringCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryForFlaggedEnumForIntCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.Insert<FlaggedEnumForIntCompleteTable, Guid>(entity);
                var queryResult = connection.QueryAll<FlaggedEnumForIntCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryForFlaggedEnumForIntCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.Insert<FlaggedEnumForIntCompleteTable, Guid>(entity);
                var queryResult = connection.QueryAll<FlaggedEnumForIntCompleteTable>().First();

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
                entity.ColumnSmallInt = Direction.None;

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

        [TestMethod]
        public void TestUpdateForFlaggedEnumForStringCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<FlaggedEnumForStringCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnNVarChar = StorageType.Drive | StorageType.File | StorageType.MemoryStorage;

                // Act
                var updateResult = connection.Update(entity);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = connection.QueryAll<FlaggedEnumForStringCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateForFlaggedEnumForStringCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<FlaggedEnumForStringCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnNVarChar = null;

                // Act
                var updateResult = connection.Update(entity);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = connection.QueryAll<FlaggedEnumForStringCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateForFlaggedEnumForIntCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<FlaggedEnumForIntCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnNVarChar = StorageType.Drive | StorageType.File | StorageType.MemoryStorage;

                // Act
                var updateResult = connection.Update(entity);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = connection.QueryAll<FlaggedEnumForIntCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateForFlaggedEnumForIntCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<FlaggedEnumForIntCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnNVarChar = null;

                // Act
                var updateResult = connection.Update(entity);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = connection.QueryAll<FlaggedEnumForIntCompleteTable>().First();

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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                    entity.ColumnSmallInt = Direction.None;
                });

                // Act
                var updateAllResult = connection.UpdateAll(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
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
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestUpdateAllForFlaggedEnumForStringForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnNVarChar = StorageType.MemoryStorage | StorageType.Folder | StorageType.Drive;
                });

                // Act
                var updateAllResult = connection.UpdateAll(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<FlaggedEnumForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestUpdateAllForFlaggedEnumForStringNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTables(10);

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
                var queryResult = connection.QueryAll<FlaggedEnumForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestUpdateAllForFlaggedEnumForIntForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnNVarChar = StorageType.MemoryStorage | StorageType.Folder | StorageType.Drive;
                });

                // Act
                var updateAllResult = connection.UpdateAll(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<FlaggedEnumForIntCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestUpdateAllForFlaggedEnumForIntNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTables(10);

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
                var queryResult = connection.QueryAll<FlaggedEnumForIntCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        #endregion

        #endregion

        #region custom enum mapping
        private static CustomedMappingEnumPropertyHandler<TDbType, TEnum> CreateCustomedMappingEnumPropertyHandler<TEnum, TDbType>(
            Dictionary<TEnum, TDbType> mapping)
            => new CustomedMappingEnumPropertyHandler<TDbType, TEnum>(mapping);

        public class CustomedMappingEnumPropertyHandler<TDbType, TEnum> : IPropertyHandler<TDbType, TEnum>
        {
            private readonly Dictionary<TDbType, TEnum> dbToEnum;
            private readonly Dictionary<TEnum, TDbType> enumToDb;

            public CustomedMappingEnumPropertyHandler(Dictionary<TEnum, TDbType> mapping)
            {
                enumToDb = mapping;
                dbToEnum = mapping.ToDictionary(n => n.Value, n => n.Key);
            }

            public TEnum Get(TDbType input, ClassProperty property)
                => input == null || !dbToEnum.TryGetValue(input, out var v) ? default(TEnum) : v;

            public TDbType Set(TEnum input, ClassProperty property)
                => input == null || !enumToDb.TryGetValue(input, out var v) ? default(TDbType) : v;
        }

        public class CustomedEnumModel<TEnum> where TEnum : struct
        {
            public TEnum? Value { get; set; }
        }


        public enum CustomedStringEnum { A, B }
        private CustomedMappingEnumPropertyHandler<string, CustomedStringEnum?> customedStringEnumHandler =
            CreateCustomedMappingEnumPropertyHandler(new Dictionary<CustomedStringEnum?, string>
            {
                [CustomedStringEnum.A] = "Special-A",
                [CustomedStringEnum.B] = "Special-B"
            });

        public enum CustomedDecimalEnum { A, B }
        private CustomedMappingEnumPropertyHandler<decimal?, CustomedDecimalEnum?> customedDecimalEnumHandler =
            CreateCustomedMappingEnumPropertyHandler(new Dictionary<CustomedDecimalEnum?, decimal?>
            {
                [CustomedDecimalEnum.A] = 5.1m,
                [CustomedDecimalEnum.B] = 6.2m
            });

        public enum CustomedFloatEnum { A, B }
        private CustomedMappingEnumPropertyHandler<float?, CustomedFloatEnum?> customedFloatEnumHandler =
            CreateCustomedMappingEnumPropertyHandler(new Dictionary<CustomedFloatEnum?, float?>
            {
                [CustomedFloatEnum.A] = 3.1f,
                [CustomedFloatEnum.B] = 4.2f
            });

        private void EnsureCustomedMappingEnumPropertyHandler<TEnum>(object propertyHandler)
        {
            if (PropertyHandlerMapper.Get<object>(typeof(TEnum)) == null)
            {
                PropertyHandlerMapper.Add(typeof(TEnum), propertyHandler);
            }
        }

        [TestMethod]
        public void TestEnumGetFromStringWithPropertyHandler()
        {
            EnsureCustomedMappingEnumPropertyHandler<CustomedStringEnum>(customedStringEnumHandler);
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                var enumValue = connection.ExecuteQuery<CustomedStringEnum>("select 'Special-B'").First();
                Assert.AreEqual(CustomedStringEnum.B, enumValue);

                var nullEnumValue = connection.ExecuteQuery<CustomedStringEnum?>("select convert(varchar, null)").First();
                Assert.IsNull(nullEnumValue);

                var entry = connection.ExecuteQuery<CustomedEnumModel<CustomedStringEnum>>("select 'Special-B' Value").First();
                Assert.AreEqual(CustomedStringEnum.B, entry.Value);

                var nullEntry = connection.ExecuteQuery<CustomedEnumModel<CustomedStringEnum>>("select convert(varchar, null) Value").First();
                Assert.IsNull(nullEntry.Value);
            }
        }

        [TestMethod]
        public void TestEnumSetFromStringWithPropertyHandler()
        {
            EnsureCustomedMappingEnumPropertyHandler<CustomedStringEnum>(customedStringEnumHandler);
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                var entry = new CustomedEnumModel<CustomedStringEnum> { Value = CustomedStringEnum.B };
                var stringValue = connection.ExecuteQuery<string>("select @Value", entry).First();
                Assert.AreEqual("Special-B", stringValue);

                var nullEntry = new CustomedEnumModel<CustomedStringEnum> { Value = null };
                var nullStringValue = connection.ExecuteQuery<string>("select @Value", nullEntry).First();
                Assert.IsNull(nullStringValue);
            }
        }

        [TestMethod]
        public void TestEnumGetFromDecimalWithPropertyHandler()
        {
            EnsureCustomedMappingEnumPropertyHandler<CustomedDecimalEnum>(customedDecimalEnumHandler);
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                var enumValue = connection.ExecuteQuery<CustomedDecimalEnum>("select convert(decimal(8,3), 6.2)").First();
                Assert.AreEqual(CustomedDecimalEnum.B, enumValue);

                var nullEnumValue = connection.ExecuteQuery<CustomedDecimalEnum?>("select convert(decimal(8,3), null)").First();
                Assert.IsNull(nullEnumValue);

                var entry = connection.ExecuteQuery<CustomedEnumModel<CustomedDecimalEnum>>("select convert(decimal(8,3), 6.2) Value").First();
                Assert.AreEqual(CustomedDecimalEnum.B, entry.Value);

                var nullEntry = connection.ExecuteQuery<CustomedEnumModel<CustomedDecimalEnum>>("select convert(decimal(8,3), null) Value").First();
                Assert.IsNull(nullEntry.Value);
            }
        }

        [TestMethod]
        public void TestEnumSetFromDecimalWithPropertyHandler()
        {
            EnsureCustomedMappingEnumPropertyHandler<CustomedDecimalEnum>(customedDecimalEnumHandler);
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                var entry = new CustomedEnumModel<CustomedDecimalEnum> { Value = CustomedDecimalEnum.B };
                var decimalValue = connection.ExecuteQuery<decimal>("select @Value", entry).First();
                Assert.AreEqual(6.2m, decimalValue);

                var nullEntry = new CustomedEnumModel<CustomedDecimalEnum> { Value = null };
                var nullDecimalValue = connection.ExecuteQuery<decimal?>("select convert(decimal, @Value)", nullEntry).First();
                Assert.IsNull(nullDecimalValue);
            }
        }

        #endregion
    }
}
