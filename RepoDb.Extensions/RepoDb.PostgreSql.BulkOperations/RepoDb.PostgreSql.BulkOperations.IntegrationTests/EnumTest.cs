using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Enumerations;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using System.Collections.Generic;
using System.Dynamic;
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

        public static List<dynamic> CreateEnumTablesForExpandoObjectWithNullValues(int count,
            bool hasId = false,
            long addToKey = 0)
        {
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                var expandoObject = new ExpandoObject() as IDictionary<string, object>;
                var index = i + 1;
                expandoObject["Id"] = (long)(hasId ? index + addToKey : 0);
                expandoObject["ColumnEnumHand"] = (Hands?)null;
                expandoObject["ColumnEnumInt"] = (Hands?)null;
                expandoObject["ColumnEnumText"] = (Hands?)null;
                tables.Add((ExpandoObject)expandoObject);
            }
            return tables;
        }

        public static List<dynamic> CreateEnumTablesForDataTable(int count,
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
                    ColumnEnumHand = Hands.Right.ToString(),
                    ColumnEnumInt = (int?)Hands.Left,
                    ColumnEnumText = Hands.Unidentified.ToString()
                });
            }
            return tables;
        }

        public static List<dynamic> CreateEnumTablesForDataTableWithNullValues(int count,
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
                    ColumnEnumHand = (string)null,
                    ColumnEnumInt = (int?)null,
                    ColumnEnumText = (string)null
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

        #region IDictionary<string, object>

        #region BinaryBulkInsert

        [TestMethod]
        public void TestBinaryBulkInsertForEnumForExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableExpandoObjectTables(10, false);
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
        public void TestBinaryBulkInsertForEnumForExpandoObjectWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForExpandoObjectWithNullValues(10, false);
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
        public void TestBinaryBulkDeleteForEnumForExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableExpandoObjectTables(10, true);
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
        public void TestBinaryBulkDeleteForEnumForExpandoObjectWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForExpandoObjectWithNullValues(10, true);
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
        public void TestBinaryBulkMergeForEnumForExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableExpandoObjectTables(10, false);
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
        public void TestBinaryBulkMergeForEnumForExpandoObjectWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForExpandoObjectWithNullValues(10, false);
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
        public void TestBinaryBulkUpdateForEnumForExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableExpandoObjectTables(10, true);
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
        public void TestBinaryBulkUpdateForEnumForExpandoObjectWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForExpandoObjectWithNullValues(10, true);
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

        #region DataTable

        #region BinaryBulkInsert

        [TestMethod]
        public void TestBinaryBulkInsertForEnumForDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForDataTable(10, false);
                var tableName = "EnumTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    table);

                // Assert
                Assert.AreEqual(entities.Count(), result);
            }
        }

        [TestMethod]
        public void TestBinaryBulkInsertForEnumForDataTableWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForDataTableWithNullValues(10, false);
                var tableName = "EnumTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    table);

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
        public void TestBinaryBulkDeleteForEnumForDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForDataTable(10, true);
                var tableName = "EnumTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                connection.InsertAll(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    table);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteForEnumForDataTableWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForDataTableWithNullValues(10, true);
                var tableName = "EnumTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                connection.InsertAll(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    table);

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
        public void TestBinaryBulkMergeForEnumForDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForDataTable(10, false);
                var tableName = "EnumTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
                    tableName,
                    table);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(entities.Count(), result);
            }
        }

        [TestMethod]
        public void TestBinaryBulkMergeForEnumForDataTableWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForDataTableWithNullValues(10, false);
                var tableName = "EnumTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
                    tableName,
                    table);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(entities.Count(), result);
            }
        }

        #endregion

        #region BinaryBulkUpdate

        [TestMethod]
        public void TestBinaryBulkUpdateForEnumForDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForDataTable(10, true);
                var tableName = "EnumTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                connection.InsertAll(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                    tableName,
                    table);

                // Assert
                Assert.AreEqual(entities.Count(), result);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateForEnumForDataTableWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForDataTableWithNullValues(10, true);
                var tableName = "EnumTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                connection.InsertAll(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                    tableName,
                    table);

                // Assert
                Assert.AreEqual(entities.Count(), result);
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

        #region BinaryBulkDeleteAsync

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

        #region BinaryBulkMergeAsync

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

        #region BinaryBulkUpdateAsync

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

        #region BinaryBulkInsertAsync

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

        #region BinaryBulkDeleteAsync

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

        #region BinaryBulkMergeAsync

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

        #region BinaryBulkUpdateAsync

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

        #region IDictionary<string, object>

        #region BinaryBulkInsertAsync

        [TestMethod]
        public void TestBinaryBulkInsertAsyncForEnumForExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableExpandoObjectTables(10, false);
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
        public void TestBinaryBulkInsertAsyncForEnumForExpandoObjectWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForExpandoObjectWithNullValues(10, false);
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

        #region BinaryBulkDeleteAsync

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncForEnumForExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableExpandoObjectTables(10, true);
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
        public void TestBinaryBulkDeleteAsyncForEnumForExpandoObjectWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForExpandoObjectWithNullValues(10, true);
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

        #region BinaryBulkMergeAsync

        [TestMethod]
        public void TestBinaryBulkMergeAsyncForEnumForExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableExpandoObjectTables(10, false);
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
        public void TestBinaryBulkMergeAsyncForEnumForExpandoObjectWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForExpandoObjectWithNullValues(10, false);
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

        #region BinaryBulkUpdateAsync

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncForEnumForExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableExpandoObjectTables(10, true);
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
        public void TestBinaryBulkUpdateAsyncForEnumForExpandoObjectWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForExpandoObjectWithNullValues(10, true);
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

        #region DataTable

        #region BinaryBulkInsertAsync

        [TestMethod]
        public void TestBinaryBulkInsertAsyncForEnumForDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForDataTable(10, false);
                var tableName = "EnumTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsertAsync(connection,
                    tableName,
                    table).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
            }
        }

        [TestMethod]
        public void TestBinaryBulkInsertAsyncForEnumForDataTableWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForDataTableWithNullValues(10, false);
                var tableName = "EnumTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsertAsync(connection,
                    tableName,
                    table).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BinaryBulkDeleteAsync

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncForEnumForDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForDataTable(10, true);
                var tableName = "EnumTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                connection.InsertAll(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    table).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncForEnumForDataTableWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForDataTableWithNullValues(10, true);
                var tableName = "EnumTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                connection.InsertAll(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    table).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region BinaryBulkMergeAsync

        [TestMethod]
        public void TestBinaryBulkMergeAsyncForEnumForDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForDataTable(10, false);
                var tableName = "EnumTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
                    tableName,
                    table).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(entities.Count(), result);
            }
        }

        [TestMethod]
        public void TestBinaryBulkMergeAsyncForEnumForDataTableWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForDataTableWithNullValues(10, false);
                var tableName = "EnumTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
                    tableName,
                    table).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(entities.Count(), result);
            }
        }

        #endregion

        #region BinaryBulkUpdateAsync

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncForEnumForDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForDataTable(10, true);
                var tableName = "EnumTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                connection.InsertAll(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                    tableName,
                    table).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncForEnumForDataTableWithNullValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = CreateEnumTablesForDataTableWithNullValues(10, true);
                var tableName = "EnumTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                connection.InsertAll(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                    tableName,
                    table).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
