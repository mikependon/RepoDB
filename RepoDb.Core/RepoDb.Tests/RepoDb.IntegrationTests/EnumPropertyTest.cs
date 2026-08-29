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
using RepoDb.Options;
using System.Threading.Tasks;

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

        public class BooleanValuePropertyHandler : IPropertyHandler<bool?, BooleanValue?>
        {
            public BooleanValue? Get(bool? input, PropertyHandlerGetOptions options)
            {
                if (input == null)
                {
                    return null;
                }
                return input == true ? BooleanValue.True : BooleanValue.False;
            }

            public bool? Set(BooleanValue? input, PropertyHandlerSetOptions options)
            {
                if (input == null)
                {
                    return null;
                }
                return input == BooleanValue.True;
            }
        }

        public class DirectionPropertyHandler : IPropertyHandler<string, Direction?>
        {
            public Direction? Get(string input, PropertyHandlerGetOptions options)
            {
                if (input == null)
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(input))
                {
                    var type = typeof(Direction);
                    if (Enum.IsDefined(type, input))
                    {
                        return (Direction)Enum.Parse(typeof(Direction), input);
                    }
                }
                return null;
            }

            public string Set(Direction? input, PropertyHandlerSetOptions options)
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
            [PropertyHandler(typeof(BooleanValuePropertyHandler))]
            public BooleanValue ColumnBit { get; set; }
            [PropertyHandler(typeof(DirectionPropertyHandler))]
            public Direction ColumnNVarChar { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        public class EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler
        {
            public Guid SessionId { get; set; }
            [PropertyHandler(typeof(BooleanValuePropertyHandler))]
            public BooleanValue? ColumnBit { get; set; }
            [PropertyHandler(typeof(DirectionPropertyHandler))]
            public Direction? ColumnNVarChar { get; set; }
        }

        #endregion

        #region Helpers

        public EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler CreateEnumCompleteTableNullablePropertiesAndWithPropertyHandler()
        {
            return new EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = DateTime.UtcNow.Ticks % 2 == 0 ? BooleanValue.True : BooleanValue.False,
                ColumnNVarChar = (Direction)Enum.ToObject(typeof(Direction), Convert.ToInt32(DateTime.UtcNow.Ticks % 4))
            };
        }

        public EnumCompleteTableWithPropertyHandler CreateEnumCompleteTableWithPropertyHandler()
        {
            return new EnumCompleteTableWithPropertyHandler
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = BooleanValue.True,
                ColumnNVarChar = Direction.West
            };
        }

        public EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler CreateEnumCompleteTableWithNullablePropertiesAndWithPropertyHandler()
        {
            return new EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = null,
                ColumnNVarChar = null
            };
        }

        public IEnumerable<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler> CreateEnumCompleteTableWithPropertyHandlers(int count = 10)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler
                {
                    SessionId = Guid.NewGuid(),
                    ColumnBit = DateTime.UtcNow.Ticks % 2 == 0 ? BooleanValue.True : BooleanValue.False,
                    ColumnNVarChar = (Direction)Enum.ToObject(typeof(Direction), Convert.ToInt32(DateTime.UtcNow.Ticks % 4))
                };
            }
        }

        public EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler CreateEnumCompleteTableWithPropertyHandlerAsNull()
        {
            return new EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = null,
                ColumnNVarChar = null
            };
        }

        public IEnumerable<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler> CreateEnumCompleteTableWithPropertyHandlersAsNull(int count = 10)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler
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
            var entity = CreateEnumCompleteTableWithNullablePropertiesAndWithPropertyHandler();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = connection.ExecuteScalar<Guid>("INSERT INTO [dbo].[CompleteTable] " +
                    "(SessionId, ColumnBit, ColumnNVarChar) " +
                    "VALUES " +
                    "(@SessionId, @ColumnBit, @ColumnNVarChar); " +
                    "SELECT CONVERT(UNIQUEIDENTIFIER, @SessionId);", entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);
            }
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncForEnumWithPropertyHandlerFor()
        {
            // Setup
            var entity = CreateEnumCompleteTableWithNullablePropertiesAndWithPropertyHandler();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.ExecuteScalarAsync<Guid>("INSERT INTO [dbo].[CompleteTable] " +
                    "(SessionId, ColumnBit, ColumnNVarChar) " +
                    "VALUES " +
                    "(@SessionId, @ColumnBit, @ColumnNVarChar); " +
                    "SELECT CONVERT(UNIQUEIDENTIFIER, @SessionId);", entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>());
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
            var entity = CreateEnumCompleteTableWithNullablePropertiesAndWithPropertyHandler();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = connection.Insert<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler, Guid>(entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);
            }
        }

        [TestMethod]
        public async Task TestInsertAsyncForEnumWithPropertyHandler()
        {
            // Setup
            var entity = CreateEnumCompleteTableWithNullablePropertiesAndWithPropertyHandler();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler, Guid>(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);
            }
        }

        [TestMethod]
        public void TestInsertForEnumWithPropertyHandlerAsNull()
        {
            // Setup
            var entity = CreateEnumCompleteTableWithPropertyHandlerAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = connection.Insert<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler, Guid>(entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);
            }
        }

        [TestMethod]
        public async Task TestInsertAsyncForEnumWithPropertyHandlerAsNull()
        {
            // Setup
            var entity = CreateEnumCompleteTableWithPropertyHandlerAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler, Guid>(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>());
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

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>(entities);

                // Assert
                Assert.AreEqual(entities.Count(), connection.CountAll<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>());
            }
        }

        [TestMethod]
        public async Task TestInsertAllAsyncForEnumWithPropertyHandler()
        {
            // Setup
            var entities = CreateEnumCompleteTableWithPropertyHandlers(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>(entities);

                // Assert
                Assert.AreEqual(entities.Count(), await connection.CountAllAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>());
            }
        }

        [TestMethod]
        public void TestInsertAllForEnumWithPropertyHandlerAsNull()
        {
            // Setup
            var entities = CreateEnumCompleteTableWithPropertyHandlersAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>(entities);

                // Assert
                Assert.AreEqual(entities.Count(), connection.CountAll<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>());
            }
        }

        [TestMethod]
        public async Task TestInsertAllAsyncForEnumWithPropertyHandlerAsNull()
        {
            // Setup
            var entities = CreateEnumCompleteTableWithPropertyHandlersAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.InsertAllAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>(entities);

                // Assert
                Assert.AreEqual(entities.Count(), await connection.CountAllAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>());
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestQueryForEnumWithPropertyHandler()
        {
            // Setup
            var entity = CreateEnumCompleteTableWithNullablePropertiesAndWithPropertyHandler();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler, Guid>(entity);
                var queryResult = connection.Query<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>(insertResult).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestQueryAsyncForEnumWithPropertyHandler()
        {
            // Setup
            var entity = CreateEnumCompleteTableWithNullablePropertiesAndWithPropertyHandler();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler, Guid>(entity);
                var queryResult = (await connection.QueryAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>(insertResult)).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryForEnumWithPropertyHandlerAsNull()
        {
            // Setup
            var entity = CreateEnumCompleteTableWithPropertyHandlerAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler, Guid>(entity);
                var queryResult = connection.Query<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>(insertResult).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestQueryAsyncForEnumWithPropertyHandlerAsNull()
        {
            // Setup
            var entity = CreateEnumCompleteTableWithPropertyHandlerAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler, Guid>(entity);
                var queryResult = (await connection.QueryAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>(insertResult)).First();

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

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.InsertAll<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>(entities);
                var queryResult = connection.QueryAll<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>().AsList();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public async Task TestInsertAllAsyncForEnumWithPropertyHandlers()
        {
            // Setup
            var entities = CreateEnumCompleteTableWithPropertyHandlers(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAllAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>(entities);
                var queryResult = (await connection.QueryAllAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>()).AsList();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestInsertAllForEnumWithPropertyHandlersAsNull()
        {
            // Setup
            var entities = CreateEnumCompleteTableWithPropertyHandlersAsNull(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.InsertAll<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>(entities);
                var queryResult = connection.QueryAll<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>().AsList();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public async Task TestInsertAllAsyncForEnumWithPropertyHandlersAsNull()
        {
            // Setup
            var entities = CreateEnumCompleteTableWithPropertyHandlersAsNull(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAllAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>(entities);
                var queryResult = (await connection.QueryAllAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>()).AsList();

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestExecuteQueryAsyncForEnumViaExpression()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);
                var executeResult = await connection.ExecuteQueryAsync<EnumCompleteTable>("SELECT * FROM CompleteTable WHERE ColumnNVarChar = @ColumnNVarChar;",
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestExecuteQueryAsyncForMappedEnumViaExpression()
        {
            // Setup
            var entities = Helper.CreateTypeLevelMappedForStringEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);
                var executeResult = await connection.ExecuteQueryAsync<TypeLevelMappedForStringEnumCompleteTable>("SELECT * FROM CompleteTable WHERE ColumnNVarChar = @ColumnNVarChar;",
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestQueryAsyncGroupForEnumViaDynamic()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

                // Assert
                var queryResult = await connection.QueryAsync<EnumCompleteTable>(new { ColumnNVarChar = Direction.West });

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestQueryAsyncGroupForEnumViaExpression()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);
            var where = (Expression<Func<EnumCompleteTable, bool>>)(e => e.ColumnNVarChar == Direction.West);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);
                var queryResult = await connection.QueryAsync(where);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestQueryAsyncGroupForEnumViaQueryField()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);
                var queryResult = await connection.QueryAsync<EnumCompleteTable>(new QueryField("ColumnNVarChar", Direction.West));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestQueryAsyncGroupForEnumViaQueryFields()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);
                var queryResult = await connection.QueryAsync<EnumCompleteTable>(new QueryField("ColumnNVarChar", Direction.West).AsEnumerable());

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestQueryAsyncGroupForEnumViaQueryGroup()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);
                var queryResult = await connection.QueryAsync<EnumCompleteTable>(new QueryGroup(new QueryField("ColumnNVarChar", Direction.West)));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestQueryAsyncGroupForEnumForTextWithOrConditionViaExpression()
        {
            // Setup
            var entities = CreateEnumCompleteTablesRandomized(10);
            var where = (Expression<Func<EnumCompleteTable, bool>>)(e => e.ColumnNVarChar == Direction.West || e.ColumnNVarChar == Direction.East);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);
                var queryResult = await connection.QueryAsync(where);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestQueryAsyncGroupForEnumForNonTextWithOrConditionViaExpression()
        {
            // Setup
            var entities = CreateEnumCompleteTablesRandomized(10);
            var where = (Expression<Func<EnumCompleteTable, bool>>)(e => e.ColumnInt == Direction.West || e.ColumnInt == Direction.East);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);
                var queryResult = await connection.QueryAsync(where);

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


            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestQueryAsyncGroupForEnumsForTextWithOrConditionViaQueryGroup()
        {
            // Setup
            var entities = CreateEnumCompleteTablesRandomized(10);
            var fields = new[]
            {
                new QueryField("ColumnNVarChar", Direction.West),
                new QueryField("ColumnNVarChar", Direction.East)
            };
            var where = new QueryGroup(fields, RepoDb.Enumerations.Conjunction.Or);


            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);
                var queryResult = await connection.QueryAsync<EnumCompleteTable>(where);

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


            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestQueryAsyncGroupForEnumsForNonTextWithOrConditionViaQueryGroup()
        {
            // Setup
            var entities = CreateEnumCompleteTablesRandomized(10);
            var fields = new[]
            {
                new QueryField("ColumnInt", Direction.West),
                new QueryField("ColumnInt", Direction.East)
            };
            var where = new QueryGroup(fields, RepoDb.Enumerations.Conjunction.Or);


            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);
                var queryResult = await connection.QueryAsync<EnumCompleteTable>(where);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestInsertAsyncForEnum()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<EnumCompleteTable, Guid>(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<EnumCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = await connection.QueryAsync<EnumCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
            }
        }

        [TestMethod]
        public void TestInsertForEnumAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestInsertAsyncForEnumAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<EnumCompleteTable, Guid>(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<EnumCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = await connection.QueryAsync<FlaggedEnumForIntCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
            }
        }

        [TestMethod]
        public void TestInsertForEnumAsIntForString()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestInsertAsyncForEnumAsIntForString()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<EnumAsIntForStringCompleteTable, Guid>(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<EnumAsIntForStringCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = await connection.QueryAsync<EnumAsIntForStringCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
            }
        }

        [TestMethod]
        public void TestInsertForEnumAsIntForStringAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestInsertAsyncForEnumAsIntForStringAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<EnumAsIntForStringCompleteTable, Guid>(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<EnumAsIntForStringCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = await connection.QueryAsync<FlaggedEnumForIntCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
            }
        }

        [TestMethod]
        public void TestInsertForFlaggedEnumForString()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestInsertAsyncForFlaggedEnumForString()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<FlaggedEnumForStringCompleteTable, Guid>(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<FlaggedEnumForStringCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = await connection.QueryAsync<FlaggedEnumForIntCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
            }
        }

        [TestMethod]
        public void TestInsertForFlaggedEnumForStringAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestInsertAsyncForFlaggedEnumForStringAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<FlaggedEnumForStringCompleteTable, Guid>(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<FlaggedEnumForStringCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = await connection.QueryAsync<FlaggedEnumForIntCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
            }
        }

        [TestMethod]
        public void TestInsertForFlaggedEnumForInt()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestInsertAsyncForFlaggedEnumForInt()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<FlaggedEnumForIntCompleteTable, Guid>(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<FlaggedEnumForIntCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = await connection.QueryAsync<FlaggedEnumForIntCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
            }
        }

        [TestMethod]
        public void TestInsertForFlaggedEnumForIntAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestInsertAsyncForFlaggedEnumForIntAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<FlaggedEnumForIntCompleteTable, Guid>(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<FlaggedEnumForIntCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = await connection.QueryAsync<FlaggedEnumForIntCompleteTable>(id);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestInsertAllAsyncForEnum()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

                // Assert
                Assert.AreEqual(insertAllResult, await connection.CountAllAsync<EnumCompleteTable>());
                var queryResult = await connection.QueryAllAsync<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestInsertAllForEnumAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestInsertAllAsyncForEnumAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

                // Assert
                Assert.AreEqual(insertAllResult, await connection.CountAllAsync<EnumCompleteTable>());
                var queryResult = await connection.QueryAllAsync<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestInsertAllForEnumAsIntForString()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestInsertAllAsyncForEnumAsIntForString()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

                // Assert
                Assert.AreEqual(insertAllResult, await connection.CountAllAsync<EnumAsIntForStringCompleteTable>());
                var queryResult = await connection.QueryAllAsync<EnumAsIntForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestInsertAllForEnumAsIntForStringAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestInsertAllAsyncForEnumAsIntForStringAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

                // Assert
                Assert.AreEqual(insertAllResult, await connection.CountAllAsync<EnumAsIntForStringCompleteTable>());
                var queryResult = await connection.QueryAllAsync<EnumAsIntForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestInsertAllForFlaggedEnumForString()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestInsertAllAsyncForFlaggedEnumForString()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync<FlaggedEnumForStringCompleteTable>(entities);

                // Assert
                Assert.AreEqual(insertAllResult, await connection.CountAllAsync<FlaggedEnumForStringCompleteTable>());
                var queryResult = await connection.QueryAllAsync<FlaggedEnumForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestInsertAllForFlaggedEnumForStringAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestInsertAllAsyncForFlaggedEnumForStringAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync<FlaggedEnumForStringCompleteTable>(entities);

                // Assert
                Assert.AreEqual(insertAllResult, await connection.CountAllAsync<FlaggedEnumForStringCompleteTable>());
                var queryResult = await connection.QueryAllAsync<FlaggedEnumForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestInsertAllForFlaggedEnumForInt()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestInsertAllAsyncForFlaggedEnumForInt()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync<FlaggedEnumForIntCompleteTable>(entities);

                // Assert
                Assert.AreEqual(insertAllResult, await connection.CountAllAsync<FlaggedEnumForIntCompleteTable>());
                var queryResult = await connection.QueryAllAsync<FlaggedEnumForIntCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestInsertAllForFlaggedEnumForIntAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestInsertAllAsyncForFlaggedEnumForIntAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync<FlaggedEnumForIntCompleteTable>(entities);

                // Assert
                Assert.AreEqual(insertAllResult, await connection.CountAllAsync<FlaggedEnumForIntCompleteTable>());
                var queryResult = await connection.QueryAllAsync<FlaggedEnumForIntCompleteTable>();

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAsyncForEnum()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.MergeAsync(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<EnumCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = (await connection.QueryAllAsync<EnumCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeForEnumAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAsyncForEnumAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.MergeAsync(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<EnumCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = (await connection.QueryAllAsync<EnumCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeForEnumForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAsyncForEnumForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<EnumCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnBigInt = Direction.East;
                entity.ColumnBit = BooleanValue.False;
                entity.ColumnInt = Direction.East;
                entity.ColumnNVarChar = Direction.East;
                entity.ColumnSmallInt = Direction.East;

                // Act
                var mergeResult = await connection.MergeAsync(entity);

                // Assert
                Assert.AreEqual(entity.SessionId, mergeResult);

                // Act
                var queryResult = (await connection.QueryAllAsync<EnumCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeForEnumForNonEmptyTableAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAsyncForEnumForNonEmptyTableAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<EnumCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnBigInt = null;
                entity.ColumnBit = null;
                entity.ColumnInt = null;
                entity.ColumnNVarChar = null;
                entity.ColumnSmallInt = Direction.None;

                // Act
                var mergeResult = await connection.MergeAsync(entity);

                // Assert
                Assert.AreEqual(entity.SessionId, mergeResult);

                // Act
                var queryResult = (await connection.QueryAllAsync<EnumCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeForEnumAsIntForString()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAsyncForEnumAsIntForString()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.MergeAsync(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<EnumAsIntForStringCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = (await connection.QueryAllAsync<EnumAsIntForStringCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeForEnumAsIntForStringAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAsyncForEnumAsIntForStringAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.MergeAsync(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<EnumAsIntForStringCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = (await connection.QueryAllAsync<EnumAsIntForStringCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeEnumAsIntForStringForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAsyncEnumAsIntForStringForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<EnumAsIntForStringCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnNVarChar = Direction.East;

                // Act
                var mergeResult = await connection.MergeAsync(entity);

                // Assert
                Assert.AreEqual(entity.SessionId, mergeResult);

                // Act
                var queryResult = (await connection.QueryAllAsync<EnumAsIntForStringCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeEnumAsIntForStringForNonEmptyTableAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAsyncEnumAsIntForStringForNonEmptyTableAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<EnumAsIntForStringCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnNVarChar = null;

                // Act
                var mergeResult = await connection.MergeAsync(entity);

                // Assert
                Assert.AreEqual(entity.SessionId, mergeResult);

                // Act
                var queryResult = (await connection.QueryAllAsync<EnumAsIntForStringCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeForFlaggedEnumForStringCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAsyncForFlaggedEnumForStringCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.MergeAsync(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<FlaggedEnumForStringCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = (await connection.QueryAllAsync<FlaggedEnumForStringCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeForFlaggedEnumForStringCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAsyncForFlaggedEnumForStringCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.MergeAsync(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<FlaggedEnumForStringCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = (await connection.QueryAllAsync<FlaggedEnumForStringCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeForFlaggedEnumForIntCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAsyncForFlaggedEnumForIntCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.MergeAsync(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<FlaggedEnumForIntCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = (await connection.QueryAllAsync<FlaggedEnumForIntCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestMergeForFlaggedEnumForIntCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAsyncForFlaggedEnumForIntCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.MergeAsync(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<FlaggedEnumForIntCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = (await connection.QueryAllAsync<FlaggedEnumForIntCompleteTable>()).First();

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAllAsyncForEnum()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllResult = await connection.MergeAllAsync(entities);

                // Assert
                Assert.AreEqual(mergeAllResult, await connection.CountAllAsync<EnumCompleteTable>());
                var queryResult = await connection.QueryAllAsync<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForEnumAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAllAsyncForEnumAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllResult = await connection.MergeAllAsync(entities);

                // Assert
                Assert.AreEqual(mergeAllResult, await connection.CountAllAsync<EnumCompleteTable>());
                var queryResult = await connection.QueryAllAsync<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForEnumForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAllAsyncForEnumForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

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
                var mergeAllResult = await connection.MergeAllAsync(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);

                // Act
                var queryResult = await connection.QueryAllAsync<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForEnumForNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAllAsyncForEnumForNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

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
                var mergeAllResult = await connection.MergeAllAsync(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);

                // Act
                var queryResult = await connection.QueryAllAsync<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForEnumAsIntForString()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAllAsyncForEnumAsIntForString()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllResult = await connection.MergeAllAsync(entities);

                // Assert
                Assert.AreEqual(mergeAllResult, await connection.CountAllAsync<EnumAsIntForStringCompleteTable>());
                var queryResult = await connection.QueryAllAsync<EnumAsIntForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForEnumAsIntForStringAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAllAsyncForEnumAsIntForStringAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllResult = await connection.MergeAllAsync(entities);

                // Assert
                Assert.AreEqual(mergeAllResult, await connection.CountAllAsync<EnumAsIntForStringCompleteTable>());
                var queryResult = await connection.QueryAllAsync<EnumAsIntForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForEnumAsIntForStringForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAllAsyncForEnumAsIntForStringForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnNVarChar = Direction.East;
                });

                // Act
                var mergeAllResult = await connection.MergeAllAsync(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);

                // Act
                var queryResult = await connection.QueryAllAsync<EnumAsIntForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForEnumAsIntForStringForNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAllAsyncForEnumAsIntForStringForNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnNVarChar = null;
                });

                // Act
                var mergeAllResult = await connection.MergeAllAsync(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);

                // Act
                var queryResult = await connection.QueryAllAsync<EnumAsIntForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForFlaggedEnumForStringCompleteTable()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAllAsyncForFlaggedEnumForStringCompleteTable()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllResult = await connection.MergeAllAsync(entities);

                // Assert
                Assert.AreEqual(mergeAllResult, await connection.CountAllAsync<FlaggedEnumForStringCompleteTable>());
                var queryResult = await connection.QueryAllAsync<FlaggedEnumForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForFlaggedEnumForStringCompleteTableAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAllAsyncForFlaggedEnumForStringCompleteTableAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllResult = await connection.MergeAllAsync(entities);

                // Assert
                Assert.AreEqual(mergeAllResult, await connection.CountAllAsync<FlaggedEnumForStringCompleteTable>());
                var queryResult = await connection.QueryAllAsync<FlaggedEnumForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForFlaggedEnumForIntCompleteTable()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAllAsyncForFlaggedEnumForIntCompleteTable()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllResult = await connection.MergeAllAsync(entities);

                // Assert
                Assert.AreEqual(mergeAllResult, await connection.CountAllAsync<FlaggedEnumForIntCompleteTable>());
                var queryResult = await connection.QueryAllAsync<FlaggedEnumForIntCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestMergeAllForFlaggedEnumForIntCompleteTableAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestMergeAllAsyncForFlaggedEnumForIntCompleteTableAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTablesAsNull(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var mergeAllResult = await connection.MergeAllAsync(entities);

                // Assert
                Assert.AreEqual(mergeAllResult, await connection.CountAllAsync<FlaggedEnumForIntCompleteTable>());
                var queryResult = await connection.QueryAllAsync<FlaggedEnumForIntCompleteTable>();

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

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<EnumCompleteTable, Guid>(entity);
                var queryResult = connection.QueryAll<EnumCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestQueryAsyncForEnum()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<EnumCompleteTable, Guid>(entity);
                var queryResult = (await connection.QueryAllAsync<EnumCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryForEnumAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<EnumCompleteTable, Guid>(entity);
                var queryResult = connection.QueryAll<EnumCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestQueryAsyncForEnumAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<EnumCompleteTable, Guid>(entity);
                var queryResult = (await connection.QueryAllAsync<EnumCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryForEnumAsIntForString()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<EnumAsIntForStringCompleteTable, Guid>(entity);
                var queryResult = connection.QueryAll<EnumAsIntForStringCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestQueryAsyncForEnumAsIntForString()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<EnumAsIntForStringCompleteTable, Guid>(entity);
                var queryResult = (await connection.QueryAllAsync<EnumAsIntForStringCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryForEnumAsIntForStringAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<EnumAsIntForStringCompleteTable, Guid>(entity);
                var queryResult = connection.QueryAll<EnumAsIntForStringCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestQueryAsyncForEnumAsIntForStringAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<EnumAsIntForStringCompleteTable, Guid>(entity);
                var queryResult = (await connection.QueryAllAsync<EnumAsIntForStringCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryForFlaggedEnumForStringCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<FlaggedEnumForStringCompleteTable, Guid>(entity);
                var queryResult = connection.QueryAll<FlaggedEnumForStringCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestQueryAsyncForFlaggedEnumForStringCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<FlaggedEnumForStringCompleteTable, Guid>(entity);
                var queryResult = (await connection.QueryAllAsync<FlaggedEnumForStringCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryForFlaggedEnumForStringCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<FlaggedEnumForStringCompleteTable, Guid>(entity);
                var queryResult = connection.QueryAll<FlaggedEnumForStringCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestQueryAsyncForFlaggedEnumForStringCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<FlaggedEnumForStringCompleteTable, Guid>(entity);
                var queryResult = (await connection.QueryAllAsync<FlaggedEnumForStringCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryForFlaggedEnumForIntCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<FlaggedEnumForIntCompleteTable, Guid>(entity);
                var queryResult = connection.QueryAll<FlaggedEnumForIntCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestQueryAsyncForFlaggedEnumForIntCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<FlaggedEnumForIntCompleteTable, Guid>(entity);
                var queryResult = (await connection.QueryAllAsync<FlaggedEnumForIntCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestQueryForFlaggedEnumForIntCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = connection.Insert<FlaggedEnumForIntCompleteTable, Guid>(entity);
                var queryResult = connection.QueryAll<FlaggedEnumForIntCompleteTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestQueryAsyncForFlaggedEnumForIntCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertResult = await connection.InsertAsync<FlaggedEnumForIntCompleteTable, Guid>(entity);
                var queryResult = (await connection.QueryAllAsync<FlaggedEnumForIntCompleteTable>()).First();

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestUpdateAsyncForEnum()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<EnumCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnBigInt = Direction.East;
                entity.ColumnBit = BooleanValue.False;
                entity.ColumnInt = Direction.East;
                entity.ColumnNVarChar = Direction.East;
                entity.ColumnSmallInt = Direction.East;

                // Act
                var updateResult = await connection.UpdateAsync(entity);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = (await connection.QueryAllAsync<EnumCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateForEnumAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestUpdateAsyncForEnumAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<EnumCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnBigInt = null;
                entity.ColumnBit = null;
                entity.ColumnInt = null;
                entity.ColumnNVarChar = null;
                entity.ColumnSmallInt = Direction.None;

                // Act
                var updateResult = await connection.UpdateAsync(entity);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = (await connection.QueryAllAsync<EnumCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateForEnumAsIntForString()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestUpdateAsyncForEnumAsIntForString()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<EnumAsIntForStringCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnNVarChar = Direction.East;

                // Act
                var updateResult = await connection.UpdateAsync(entity);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = (await connection.QueryAllAsync<EnumAsIntForStringCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateForEnumAsIntForStringAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestUpdateAsyncForEnumAsIntForStringAsNull()
        {
            // Setup
            var entity = Helper.CreateEnumAsIntForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<EnumAsIntForStringCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnNVarChar = null;

                // Act
                var updateResult = await connection.UpdateAsync(entity);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = (await connection.QueryAllAsync<EnumAsIntForStringCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateForFlaggedEnumForStringCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestUpdateAsyncForFlaggedEnumForStringCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<FlaggedEnumForStringCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnNVarChar = StorageType.Drive | StorageType.File | StorageType.MemoryStorage;

                // Act
                var updateResult = await connection.UpdateAsync(entity);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = (await connection.QueryAllAsync<FlaggedEnumForStringCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateForFlaggedEnumForStringCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestUpdateAsyncForFlaggedEnumForStringCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForStringCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<FlaggedEnumForStringCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnNVarChar = null;

                // Act
                var updateResult = await connection.UpdateAsync(entity);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = (await connection.QueryAllAsync<FlaggedEnumForStringCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateForFlaggedEnumForIntCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestUpdateAsyncForFlaggedEnumForIntCompleteTable()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<FlaggedEnumForIntCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnNVarChar = StorageType.Drive | StorageType.File | StorageType.MemoryStorage;

                // Act
                var updateResult = await connection.UpdateAsync(entity);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = (await connection.QueryAllAsync<FlaggedEnumForIntCompleteTable>()).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateForFlaggedEnumForIntCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestUpdateAsyncForFlaggedEnumForIntCompleteTableAsNull()
        {
            // Setup
            var entity = Helper.CreateFlaggedEnumForIntCompleteTableAsNull();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<FlaggedEnumForIntCompleteTable, Guid>(entity);

                // Setup
                entity.ColumnNVarChar = null;

                // Act
                var updateResult = await connection.UpdateAsync(entity);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = (await connection.QueryAllAsync<FlaggedEnumForIntCompleteTable>()).First();

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestUpdateAllAsyncForEnumForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

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
                var updateAllResult = await connection.UpdateAllAsync(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = await connection.QueryAllAsync<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestUpdateAllForEnumForNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestUpdateAllAsyncForEnumForNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

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
                var updateAllResult = await connection.UpdateAllAsync(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = await connection.QueryAllAsync<EnumCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestUpdateAllForEnumAsIntForStringForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestUpdateAllAsyncForEnumAsIntForStringForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnNVarChar = Direction.East;
                });

                // Act
                var updateAllResult = await connection.UpdateAllAsync(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = await connection.QueryAllAsync<EnumAsIntForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestUpdateAllForEnumAsIntForStringForNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestUpdateAllAsyncForEnumAsIntForStringForNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateEnumAsIntForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnNVarChar = null;
                });

                // Act
                var updateAllResult = await connection.UpdateAllAsync(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = await connection.QueryAllAsync<EnumAsIntForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestUpdateAllForFlaggedEnumForStringForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestUpdateAllAsyncForFlaggedEnumForStringForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnNVarChar = StorageType.MemoryStorage | StorageType.Folder | StorageType.Drive;
                });

                // Act
                var updateAllResult = await connection.UpdateAllAsync(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = await connection.QueryAllAsync<FlaggedEnumForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestUpdateAllForFlaggedEnumForStringNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestUpdateAllAsyncForFlaggedEnumForStringNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForStringCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnNVarChar = null;
                });

                // Act
                var updateAllResult = await connection.UpdateAllAsync(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = await connection.QueryAllAsync<FlaggedEnumForStringCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestUpdateAllForFlaggedEnumForIntForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestUpdateAllAsyncForFlaggedEnumForIntForNonEmptyTable()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnNVarChar = StorageType.MemoryStorage | StorageType.Folder | StorageType.Drive;
                });

                // Act
                var updateAllResult = await connection.UpdateAllAsync(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = await connection.QueryAllAsync<FlaggedEnumForIntCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        [TestMethod]
        public void TestUpdateAllForFlaggedEnumForIntNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestUpdateAllAsyncForFlaggedEnumForIntNonEmptyTableAsNull()
        {
            // Setup
            var entities = Helper.CreateFlaggedEnumForIntCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var insertAllResult = await connection.InsertAllAsync(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnNVarChar = null;
                });

                // Act
                var updateAllResult = await connection.UpdateAllAsync(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = await connection.QueryAllAsync<FlaggedEnumForIntCompleteTable>();

                // Assert
                entities.ForEach(entity => Helper.AssertPropertiesEquality(entity, queryResult.First(item => item.SessionId == entity.SessionId)));
            }
        }

        #endregion

        #endregion

        #region Custom Enum Mapping

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

            public TEnum Get(TDbType input, PropertyHandlerGetOptions options)
                => input == null || !dbToEnum.TryGetValue(input, out var v) ? default(TEnum) : v;

            public TDbType Set(TEnum input, PropertyHandlerSetOptions options)
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
            EnsureCustomedMappingEnumPropertyHandler<CustomedStringEnum?>(customedStringEnumHandler);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestEnumGetFromStringWithPropertyHandlerAsync()
        {
            EnsureCustomedMappingEnumPropertyHandler<CustomedStringEnum>(customedStringEnumHandler);
            EnsureCustomedMappingEnumPropertyHandler<CustomedStringEnum?>(customedStringEnumHandler);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                var enumValue = (await connection.ExecuteQueryAsync<CustomedStringEnum>("select 'Special-B'")).First();
                Assert.AreEqual(CustomedStringEnum.B, enumValue);

                var nullEnumValue = (await connection.ExecuteQueryAsync<CustomedStringEnum?>("select convert(varchar, null)")).First();
                Assert.IsNull(nullEnumValue);

                var entry = (await connection.ExecuteQueryAsync<CustomedEnumModel<CustomedStringEnum>>("select 'Special-B' Value")).First();
                Assert.AreEqual(CustomedStringEnum.B, entry.Value);

                var nullEntry = (await connection.ExecuteQueryAsync<CustomedEnumModel<CustomedStringEnum>>("select convert(varchar, null) Value")).First();
                Assert.IsNull(nullEntry.Value);
            }
        }

        [TestMethod]
        public void TestEnumSetFromStringWithPropertyHandler()
        {
            EnsureCustomedMappingEnumPropertyHandler<CustomedStringEnum>(customedStringEnumHandler);
            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestEnumSetFromStringWithPropertyHandlerAsync()
        {
            EnsureCustomedMappingEnumPropertyHandler<CustomedStringEnum>(customedStringEnumHandler);
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                var entry = new CustomedEnumModel<CustomedStringEnum> { Value = CustomedStringEnum.B };
                var stringValue = (await connection.ExecuteQueryAsync<string>("select @Value", entry)).First();
                Assert.AreEqual("Special-B", stringValue);

                var nullEntry = new CustomedEnumModel<CustomedStringEnum> { Value = null };
                var nullStringValue = (await connection.ExecuteQueryAsync<string>("select @Value", nullEntry)).First();
                Assert.IsNull(nullStringValue);
            }
        }

        [TestMethod]
        public void TestEnumGetFromDecimalWithPropertyHandler()
        {
            EnsureCustomedMappingEnumPropertyHandler<CustomedDecimalEnum>(customedDecimalEnumHandler);
            EnsureCustomedMappingEnumPropertyHandler<CustomedDecimalEnum?>(customedDecimalEnumHandler);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestEnumGetFromDecimalWithPropertyHandlerAsync()
        {
            EnsureCustomedMappingEnumPropertyHandler<CustomedDecimalEnum>(customedDecimalEnumHandler);
            EnsureCustomedMappingEnumPropertyHandler<CustomedDecimalEnum?>(customedDecimalEnumHandler);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                var enumValue = (await connection.ExecuteQueryAsync<CustomedDecimalEnum>("select convert(decimal(8,3), 6.2)")).First();
                Assert.AreEqual(CustomedDecimalEnum.B, enumValue);

                var nullEnumValue = (await connection.ExecuteQueryAsync<CustomedDecimalEnum?>("select convert(decimal(8,3), null)")).First();
                Assert.IsNull(nullEnumValue);

                var entry = (await connection.ExecuteQueryAsync<CustomedEnumModel<CustomedDecimalEnum>>("select convert(decimal(8,3), 6.2) Value")).First();
                Assert.AreEqual(CustomedDecimalEnum.B, entry.Value);

                var nullEntry = (await connection.ExecuteQueryAsync<CustomedEnumModel<CustomedDecimalEnum>>("select convert(decimal(8,3), null) Value")).First();
                Assert.IsNull(nullEntry.Value);
            }
        }

        [TestMethod]
        public void TestEnumSetFromDecimalWithPropertyHandler()
        {
            EnsureCustomedMappingEnumPropertyHandler<CustomedDecimalEnum>(customedDecimalEnumHandler);
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                var entry = new CustomedEnumModel<CustomedDecimalEnum> { Value = CustomedDecimalEnum.B };
                var decimalValue = connection.ExecuteQuery<decimal>("select @Value", entry).First();
                Assert.AreEqual(6.2m, decimalValue);

                var nullEntry = new CustomedEnumModel<CustomedDecimalEnum> { Value = null };
                var nullDecimalValue = connection.ExecuteQuery<decimal?>("select convert(decimal, @Value)", nullEntry).First();
                Assert.IsNull(nullDecimalValue);
            }
        }

        [TestMethod]
        public async Task TestEnumSetFromDecimalWithPropertyHandlerAsync()
        {
            EnsureCustomedMappingEnumPropertyHandler<CustomedDecimalEnum>(customedDecimalEnumHandler);
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                var entry = new CustomedEnumModel<CustomedDecimalEnum> { Value = CustomedDecimalEnum.B };
                var decimalValue = (await connection.ExecuteQueryAsync<decimal>("select @Value", entry)).First();
                Assert.AreEqual(6.2m, decimalValue);

                var nullEntry = new CustomedEnumModel<CustomedDecimalEnum> { Value = null };
                var nullDecimalValue = (await connection.ExecuteQueryAsync<decimal?>("select convert(decimal, @Value)", nullEntry)).First();
                Assert.IsNull(nullDecimalValue);
            }
        }

        #endregion

        #region InvalidValue/OutOfRangeValue (with PropertyHandlers)

        #region Insert

        [TestMethod]
        public void TestInsertForEnumWithPropertyHandlerForInvalid()
        {
            // Setup
            var entity = CreateEnumCompleteTableWithPropertyHandler();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = connection.Insert<EnumCompleteTableWithPropertyHandler, Guid>(entity);

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
        public async Task TestInsertAsyncForEnumWithPropertyHandlerForInvalid()
        {
            // Setup
            var entity = CreateEnumCompleteTableWithPropertyHandler();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<EnumCompleteTableWithPropertyHandler, Guid>(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<EnumCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = await connection.QueryAsync<EnumCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
            }
        }

        [TestMethod]
        public void TestInsertForEnumWithNullPropertiesAndWithPropertyHandlerForInvalid()
        {
            // Setup
            var entity = CreateEnumCompleteTableNullablePropertiesAndWithPropertyHandler();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = connection.Insert<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler, Guid>(entity);

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
        public async Task TestInsertAsyncForEnumWithNullPropertiesAndWithPropertyHandlerForInvalid()
        {
            // Setup
            var entity = CreateEnumCompleteTableNullablePropertiesAndWithPropertyHandler();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler, Guid>(entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<EnumCompleteTable>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);

                // Act
                var queryResult = await connection.QueryAsync<EnumCompleteTable>(id);

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult.First());
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestQueryForEnumWithNullPropertiesAndWithPropertyHandlerForInvalid()
        {
            // Setup
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = -1,
                ColumnNVarChar = "OutsideOfEnumRange"
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = connection.Insert<Guid>(ClassMappedNameCache.Get<EnumCompleteTable>(), entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<EnumCompleteTable>());

                // Act
                var queryResult = connection.Query<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>(id);

                // Setup
                var expected = new
                {
                    SessionId = entity.SessionId,
                    ColumnBit = BooleanValue.True,
                    ColumnNVarChar = (Direction?)null,
                };

                // Assert
                Helper.AssertPropertiesEquality(expected, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestQueryAsyncForEnumWithNullPropertiesAndWithPropertyHandlerForInvalid()
        {
            // Setup
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = -1,
                ColumnNVarChar = "OutsideOfEnumRange"
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<Guid>(ClassMappedNameCache.Get<EnumCompleteTable>(), entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<EnumCompleteTable>());

                // Act
                var queryResult = await connection.QueryAsync<EnumCompleteTableWithNullablePropertiesAndWithPropertyHandler>(id);

                // Setup
                var expected = new
                {
                    SessionId = entity.SessionId,
                    ColumnBit = BooleanValue.True,
                    ColumnNVarChar = (Direction?)null,
                };

                // Assert
                Helper.AssertPropertiesEquality(expected, queryResult.First());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnQueryForEnumWithPropertyHandlerForInvalid()
        {
            // Setup
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = -1,
                ColumnNVarChar = "OutsideOfEnumRange"
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = connection.Insert<Guid>(ClassMappedNameCache.Get<EnumCompleteTable>(), entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<EnumCompleteTable>());

                // Act
                Assert.Throws<InvalidOperationException>(() => connection.Query<EnumCompleteTableWithPropertyHandler>(id));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnQueryAsyncForEnumWithPropertyHandlerForInvalid()
        {
            // Setup
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = -1,
                ColumnNVarChar = "OutsideOfEnumRange"
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.InsertAsync<Guid>(ClassMappedNameCache.Get<EnumCompleteTable>(), entity);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<EnumCompleteTable>());

                // Act
                await Assert.ThrowsAsync<InvalidOperationException>(async () => await connection.QueryAsync<EnumCompleteTableWithPropertyHandler>(id));
            }
        }

        #endregion

        #endregion
    }
}
