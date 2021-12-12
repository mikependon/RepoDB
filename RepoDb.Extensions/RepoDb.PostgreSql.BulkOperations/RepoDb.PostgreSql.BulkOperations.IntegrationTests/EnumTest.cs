using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Enumerations;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests
{
    [TestClass]
    public class EnumTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();

            //FluentMapper
                //.Entity<EnumTable>()
                //.PropertyValueAttributes(e => e.ColumnEnumHand,
                    //new[] { new NpgsqlDbTypeAttribute(NpgsqlTypes.NpgsqlDbType.Unknown) }, true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region Methods

        private NpgsqlConnection GetConnection() =>
            (NpgsqlConnection)(new NpgsqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen());

        public static List<EnumTable> CreateEnumTablesWithNullValues(int count,
            bool hasId = false,
            long addToKey = 0)
        {
            var tables = new List<EnumTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new EnumTable
                {
                    Id = (long)(hasId ? index + addToKey : 0),
                    ColumnEnumHand = null,
                    ColumnEnumInt = null,
                    ColumnEnumText = null
                });
            }
            return tables;
        }

        public static List<dynamic> CreateEnumTablesForAnonymousWithNullValues(int count,
            bool hasId = false,
            long addToKey = 0)
        {
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new
                {
                    Id = (long)(hasId ? index + addToKey : 0),
                    ColumnEnumHand = (Hands?)null,
                    ColumnEnumInt = (Hands?)null,
                    ColumnEnumText = (Hands?)null
                });
            }
            return tables;
        }

        #endregion

        #region Sync

        #region TEntity

        #region BinaryBulkInsert

        [TestMethod]
        public void TestBinaryBulkInsertForEnum()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTables(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<EnumTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkInsertForEnumWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesWithNullValues(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<EnumTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BinaryBulkDelete

        [TestMethod]
        public void TestBinaryBulkDeleteForEnum()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTables(10, false);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll<EnumTable>(entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDelete<EnumTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteForEnumWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesWithNullValues(10, false);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll<EnumTable>(entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDelete<EnumTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region BinaryBulkMerge

        [TestMethod]
        public void TestBinaryBulkMergeForEnum()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTables(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<EnumTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkMergeForEnumWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesWithNullValues(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<EnumTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BinaryBulkUpdate

        [TestMethod]
        public void TestBinaryBulkUpdateForEnum()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTables(10, false);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll<EnumTable>(entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdate<EnumTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateForEnumWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesWithNullValues(10, false);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll<EnumTable>(entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdate<EnumTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #endregion

        #region Anonymous

        #region BinaryBulkInsert

        [TestMethod]
        public void TestBinaryBulkInsertForEnumForAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableAnonymousTables(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkInsertForEnumForAnonymousWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForAnonymousWithNullValues(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BinaryBulkDelete

        [TestMethod]
        public void TestBinaryBulkDeleteForEnumForAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableAnonymousTables(10, true);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteForEnumForAnonymousWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForAnonymousWithNullValues(10, true);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region BinaryBulkMerge

        [TestMethod]
        public void TestBinaryBulkMergeForEnumForAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableAnonymousTables(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkMergeForEnumForAnonymousWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForAnonymousWithNullValues(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BinaryBulkUpdate

        [TestMethod]
        public void TestBinaryBulkUpdateForEnumForAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableAnonymousTables(10, true);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateForEnumForAnonymousWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForAnonymousWithNullValues(10, true);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #endregion

        #endregion

        #region Async

        #region TEntity

        #region BinaryBulkInsertAsync

        [TestMethod]
        public void TestBinaryBulkInsertAsyncForEnum()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTables(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsertAsync<EnumTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkInsertAsyncForEnumWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesWithNullValues(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsertAsync<EnumTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BinaryBulkDelete

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncForEnum()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTables(10, false);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll<EnumTable>(entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync<EnumTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncForEnumWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesWithNullValues(10, false);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll<EnumTable>(entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync<EnumTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region BinaryBulkMerge

        [TestMethod]
        public void TestBinaryBulkMergeAsyncForEnum()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTables(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<EnumTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkMergeAsyncForEnumWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesWithNullValues(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<EnumTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BinaryBulkUpdate

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncForEnum()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTables(10, false);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll<EnumTable>(entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync<EnumTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncForEnumWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesWithNullValues(10, false);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll<EnumTable>(entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync<EnumTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #endregion

        #region Anonymous

        #region BinaryBulkInsert

        [TestMethod]
        public void TestBinaryBulkInsertAsyncForEnumForAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableAnonymousTables(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsertAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkInsertAsyncForEnumForAnonymousWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForAnonymousWithNullValues(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsertAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BinaryBulkDelete

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncForEnumForAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableAnonymousTables(10, true);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncForEnumForAnonymousWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForAnonymousWithNullValues(10, true);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region BinaryBulkMerge

        [TestMethod]
        public void TestBinaryBulkMergeAsyncForEnumForAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableAnonymousTables(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkMergeAsyncForEnumForAnonymousWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForAnonymousWithNullValues(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BinaryBulkUpdate

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncForEnumForAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableAnonymousTables(10, true);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncForEnumForAnonymousWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForAnonymousWithNullValues(10, true);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
