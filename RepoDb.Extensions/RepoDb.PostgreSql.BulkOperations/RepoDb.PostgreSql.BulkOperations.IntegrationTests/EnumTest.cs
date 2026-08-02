using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using Npgsql.NameTranslation;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Enumerations;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests
{
    [TestClass]
    public class EnumTest
    {
        private NpgsqlDataSource _enumDataSource;

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
            _enumDataSource = new NpgsqlDataSourceBuilder(Database.ConnectionString)
                .MapEnum<Hands>("hand", new NpgsqlNullNameTranslator())
                .Build();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
            _enumDataSource?.Dispose();
            _enumDataSource = null;
        }

        #region Methods

        private NpgsqlConnection GetConnection() =>
            (NpgsqlConnection)(_enumDataSource.CreateConnection()).EnsureOpen();

        private static IEnumerable<NpgsqlBulkInsertMapItem> GetEnumColumnMappings() =>
            Helper.GetEnumTableMappings().Where(m => m.SourceColumn != nameof(Models.EnumTable.Id));

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
                    ColumnEnumHand = Hands.Right,
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
                    ColumnEnumHand = (Hands?)null,
                    ColumnEnumInt = (int?)null,
                    ColumnEnumText = (string)null
                });
            }
            return tables;
        }

        #endregion

        #region Sync

        #region TEntity

        #region BulkInsert

        [TestMethod]
        public void TestBinaryBulkInsertForEnum()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTables(10, true);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<EnumTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings());

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
                var result = NpgsqlConnectionExtension.BulkInsert<EnumTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkDelete

        [TestMethod]
        public void TestBinaryBulkDeleteForEnum()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTables(10, true);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll<EnumTable>(entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkDelete<EnumTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings());

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
                var result = NpgsqlConnectionExtension.BulkDelete<EnumTable>(connection,
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

        #region BulkMerge

        [TestMethod]
        public void TestBinaryBulkMergeForEnum()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTables(10, true);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<EnumTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings());

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
                var result = NpgsqlConnectionExtension.BulkMerge<EnumTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkUpdate

        [TestMethod]
        public void TestBinaryBulkUpdateForEnum()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTables(10, true);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll<EnumTable>(entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkUpdate<EnumTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings());

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
                var result = NpgsqlConnectionExtension.BulkUpdate<EnumTable>(connection,
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

        #region BulkInsert

        [TestMethod]
        public void TestBinaryBulkInsertForEnumForAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableAnonymousTables(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: GetEnumColumnMappings());

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
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkDelete

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
                var result = NpgsqlConnectionExtension.BulkDelete(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings());

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
                var result = NpgsqlConnectionExtension.BulkDelete(connection,
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

        #region BulkMerge

        [TestMethod]
        public void TestBinaryBulkMergeForEnumForAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableAnonymousTables(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings(),
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity);

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
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkUpdate

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
                var result = NpgsqlConnectionExtension.BulkUpdate(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings());

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
                var result = NpgsqlConnectionExtension.BulkUpdate(connection,
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

        #region BulkInsert

        [TestMethod]
        public void TestBinaryBulkInsertForEnumForExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableExpandoObjectTables(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: GetEnumColumnMappings());

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
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkDelete

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
                var result = NpgsqlConnectionExtension.BulkDelete(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings());

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
                var result = NpgsqlConnectionExtension.BulkDelete(connection,
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

        #region BulkMerge

        [TestMethod]
        public void TestBinaryBulkMergeForEnumForExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableExpandoObjectTables(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings(),
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity);

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
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkUpdate

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
                var result = NpgsqlConnectionExtension.BulkUpdate(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings());

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
                var result = NpgsqlConnectionExtension.BulkUpdate(connection,
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

        #region BulkInsert

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
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    table,
                    mappings: GetEnumColumnMappings());

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
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    table,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkDelete

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
                var result = NpgsqlConnectionExtension.BulkDelete(connection,
                    tableName,
                    table,
                    mappings: Helper.GetEnumTableMappings());

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
                var result = NpgsqlConnectionExtension.BulkDelete(connection,
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

        #region BulkMerge

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
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table,
                    mappings: Helper.GetEnumTableMappings(),
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity);

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
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(entities.Count(), result);
            }
        }

        #endregion

        #region BulkUpdate

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
                var result = NpgsqlConnectionExtension.BulkUpdate(connection,
                    tableName,
                    table,
                    mappings: Helper.GetEnumTableMappings());

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
                var result = NpgsqlConnectionExtension.BulkUpdate(connection,
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

        #region BulkInsertAsync

        [TestMethod]
        public void TestBinaryBulkInsertAsyncForEnum()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTables(10, true);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsertAsync<EnumTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings()).Result;

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
                var result = NpgsqlConnectionExtension.BulkInsertAsync<EnumTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkDeleteAsync

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncForEnum()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTables(10, true);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll<EnumTable>(entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkDeleteAsync<EnumTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings()).Result;

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
                var result = NpgsqlConnectionExtension.BulkDeleteAsync<EnumTable>(connection,
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

        #region BulkMergeAsync

        [TestMethod]
        public void TestBinaryBulkMergeAsyncForEnum()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTables(10, true);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<EnumTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings()).Result;

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
                var result = NpgsqlConnectionExtension.BulkMergeAsync<EnumTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkUpdateAsync

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncForEnum()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTables(10, true);
                var tableName = "EnumTable";

                // Act
                connection.InsertAll<EnumTable>(entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkUpdateAsync<EnumTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings()).Result;

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
                var result = NpgsqlConnectionExtension.BulkUpdateAsync<EnumTable>(connection,
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

        #region BulkInsertAsync

        [TestMethod]
        public void TestBinaryBulkInsertAsyncForEnumForAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableAnonymousTables(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsertAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: GetEnumColumnMappings()).Result;

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
                var result = NpgsqlConnectionExtension.BulkInsertAsync(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkDeleteAsync

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
                var result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings()).Result;

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
                var result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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

        #region BulkMergeAsync

        [TestMethod]
        public void TestBinaryBulkMergeAsyncForEnumForAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableAnonymousTables(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings(),
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity).Result;

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
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkUpdateAsync

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
                var result = NpgsqlConnectionExtension.BulkUpdateAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings()).Result;

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
                var result = NpgsqlConnectionExtension.BulkUpdateAsync(connection,
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

        #region BulkInsertAsync

        [TestMethod]
        public void TestBinaryBulkInsertAsyncForEnumForExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableExpandoObjectTables(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsertAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: GetEnumColumnMappings()).Result;

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
                var result = NpgsqlConnectionExtension.BulkInsertAsync(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkDeleteAsync

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
                var result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings()).Result;

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
                var result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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

        #region BulkMergeAsync

        [TestMethod]
        public void TestBinaryBulkMergeAsyncForEnumForExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateEnumTableExpandoObjectTables(10, false);
                var tableName = "EnumTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings(),
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity).Result;

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
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkUpdateAsync

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
                var result = NpgsqlConnectionExtension.BulkUpdateAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: Helper.GetEnumTableMappings()).Result;

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
                var result = NpgsqlConnectionExtension.BulkUpdateAsync(connection,
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

        #region BulkInsertAsync

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
                var result = NpgsqlConnectionExtension.BulkInsertAsync(connection,
                    tableName,
                    table,
                    mappings: GetEnumColumnMappings()).Result;

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
                var result = NpgsqlConnectionExtension.BulkInsertAsync(connection,
                    tableName,
                    table,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<EnumTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkDeleteAsync

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
                var result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
                    tableName,
                    table,
                    mappings: Helper.GetEnumTableMappings()).Result;

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
                var result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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

        #region BulkMergeAsync

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
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table,
                    mappings: Helper.GetEnumTableMappings(),
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity).Result;

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
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll<EnumTable>();
                Assert.AreEqual(entities.Count(), result);
            }
        }

        #endregion

        #region BulkUpdateAsync

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
                var result = NpgsqlConnectionExtension.BulkUpdateAsync(connection,
                    tableName,
                    table,
                    mappings: Helper.GetEnumTableMappings()).Result;

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
                var result = NpgsqlConnectionExtension.BulkUpdateAsync(connection,
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
