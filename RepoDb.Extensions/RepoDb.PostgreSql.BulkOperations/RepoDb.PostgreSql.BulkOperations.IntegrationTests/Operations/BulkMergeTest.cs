using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class BulkMergeTest
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

        private NpgsqlConnection GetConnection() =>
            (NpgsqlConnection)(new NpgsqlConnection(Database.ConnectionString).EnsureOpen());

        #region Sync

        #region BulkMerge<TEntity>

        [TestMethod]
        public void TestBulkMerge()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }
        
        [TestMethod]
        public void TestBulkMergeTableNameWithSchema()
        {
            using var connection = GetConnection();
            
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "public.BulkOperationIdentityTable";
            
            // Act
            var result = connection.BulkMerge(tableName, entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }

        [TestMethod]
        public void TestBulkMergeWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    batchSize: 3);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger)));

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var target = queryResult.First(item => item.Id == entity.Id);
                    Helper.AssertEntityEquality(entity, target);
                }
            }
        }

        [TestMethod]
        public void TestBulkMergeWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithMappings()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithMappingsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithMappingsAndWithKeepIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithMappingsAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.IdMapped > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithMappingsAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.IdMapped > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithBulkInsertMapItemsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithBulkInsertMapItemsAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.IdMapped > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.IdMapped > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Since the Id column is Primary/Identity, we are require to pass a value into it.
                // In order to ensure that the assertion that the identity value is returned, we need
                // create a series of records and validate the entities

                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Act (More)
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                   tableName,
                   entities: entities,
                   identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                   mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Since the Id column is Primary/Identity, we are require to pass a value into it.
                // In order to ensure that the assertion that the identity value is returned, we need
                // create a series of records and validate the entities

                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Act (More)
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                   tableName,
                   entities: entities,
                   identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                   mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate,
                   pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(10, queryResult.Count());
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkMerge<Anonymous>

        [TestMethod]
        public void TestBulkMergeViaAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaAnonymousWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    batchSize: 3);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaAnonymousWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger)));

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaAnonymousWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaAnonymousWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaAnonymousWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Since the Id column is Primary/Identity, we are require to pass a value into it.
                // In order to ensure that the assertion that the identity value is returned, we need
                // create a series of records and validate the entities

                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Act (More)
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                   tableName,
                   entities: entities,
                   identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                   mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Since the Id column is Primary/Identity, we are require to pass a value into it.
                // In order to ensure that the assertion that the identity value is returned, we need
                // create a series of records and validate the entities

                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Act (More)
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                   tableName,
                   entities: entities,
                   identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                   mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate,
                   pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaAnonymousWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(10, queryResult.Count());
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaAnonymousWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaAnonymousWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkMerge<IDictionary<string, object>>

        [TestMethod]
        public void TestBulkMergeViaExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    batchSize: 3);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger)));

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithBulkInsertMapItemsAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                //Assert.IsTrue(entities.All(e => e.IdMapped > 0));
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                //Assert.IsTrue(entities.All(e => e.IdMapped > 0));
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Since the Id column is Primary/Identity, we are require to pass a value into it.
                // In order to ensure that the assertion that the identity value is returned, we need
                // create a series of records and validate the entities

                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Act (More)
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                   tableName,
                   entities: entities,
                   identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                   mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Since the Id column is Primary/Identity, we are require to pass a value into it.
                // In order to ensure that the assertion that the identity value is returned, we need
                // create a series of records and validate the entities

                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Act (More)
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                   tableName,
                   entities: entities,
                   identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                   mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate,
                   pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                Assert.AreEqual(10, queryResult.Count());
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaExpandoObjectWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkMerge<DataTable>

        [TestMethod]
        public void TestBulkMergeViaDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table,
                    batchSize: 3);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger)));

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                foreach (DataRow row in table.Rows)
                {
                    Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                foreach (DataRow row in table.Rows)
                {
                    Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithBulkInsertMapItemsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithBulkInsertMapItemsAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                foreach (DataRow row in table.Rows)
                {
                    Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                foreach (DataRow row in table.Rows)
                {
                    Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Since the Id column is Primary/Identity, we are require to pass a value into it.
                // In order to ensure that the assertion that the identity value is returned, we need
                // create a series of records and validate the entities

                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table);

                // Act (More)
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                   tableName,
                   table: table,
                   identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                   mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                foreach (DataRow row in table.Rows)
                {
                    Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Since the Id column is Primary/Identity, we are require to pass a value into it.
                // In order to ensure that the assertion that the identity value is returned, we need
                // create a series of records and validate the entities

                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table);

                // Act (More)
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                   tableName,
                   table: table,
                   identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                   mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate,
                   pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);
                foreach (DataRow row in table.Rows)
                {
                    Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table);

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(10, queryResult.Count());
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDataTableWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table);

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BulkMerge(connection,
                    tableName,
                    table: table);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkMerge<DbDataReader>

        [TestMethod]
        public void TestBulkMergeViaDbDataReader()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMerge(connection,
                        tableName,
                        reader);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDbDataReaderWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMerge(connection,
                        tableName,
                        reader,
                        qualifiers: Field.From(
                            nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                            nameof(BulkOperationLightIdentityTable.ColumnInteger)));

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDbDataReaderWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMerge(connection,
                        tableName,
                        reader: reader,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDbDataReaderWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMerge(connection,
                        tableName,
                        reader: reader,
                        mappings: mappings);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };


                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMerge(connection,
                        tableName,
                        reader: reader,
                        mappings: mappings,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };


                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMerge(connection,
                        tableName,
                        reader: reader,
                        mappings: mappings,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                        pseudoTableType: BulkImportPseudoTableType.Physical);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDbDataReaderWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMerge(connection,
                        tableName,
                        reader,
                        mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDbDataReaderWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMerge(connection,
                        tableName,
                        reader,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                        mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDbDataReaderWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMerge(connection,
                        tableName,
                        reader);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMerge(connection,
                        tableName,
                        reader);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(10, queryResult.Count());
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDbDataReaderWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMerge(connection,
                        tableName,
                        reader);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeViaDbDataReaderWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMerge(connection,
                        tableName,
                        reader);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMerge(connection,
                        tableName,
                        reader);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #endregion

        #region Async

        #region BulkMergeAsync<TEntity>

        [TestMethod]
        public void TestBulkMergeAsync()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public async Task TestBulkMergeAsyncTableNameWithSchema()
        {
            await using var connection = GetConnection();
            
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
            var tableName = "public.BulkOperationIdentityTable";
            
            // Act
            var result = await connection.BulkMergeAsync(tableName, entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
            Assert.AreEqual(entities.Count, assertCount);
        }
        
        [TestMethod]
        public void TestBulkMergeAsyncWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    batchSize: 3).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger))).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var target = queryResult.First(item => item.Id == entity.Id);
                    Helper.AssertEntityEquality(entity, target);
                }
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithMappings()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithMappingsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithMappingsAndWithKeepIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithMappingsAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.IdMapped > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithMappingsAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.IdMapped > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithBulkInsertMapItemsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithBulkInsertMapItemsAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.IdMapped > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.IdMapped > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Since the Id column is Primary/Identity, we are require to pass a value into it.
                // In order to ensure that the assertion that the identity value is returned, we need
                // create a series of records and validate the entities

                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Wait();

                // Act (More)
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                   tableName,
                   entities: entities,
                   identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                   mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Since the Id column is Primary/Identity, we are require to pass a value into it.
                // In order to ensure that the assertion that the identity value is returned, we need
                // create a series of records and validate the entities

                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Wait();

                // Act (More)
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                   tableName,
                   entities: entities,
                   identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                   mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate,
                   pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(10, queryResult.Count());
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkMergeAsync<Anonymous>

        [TestMethod]
        public void TestBulkMergeAsyncViaAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaAnonymousWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    batchSize: 3).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaAnonymousWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger))).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaAnonymousWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaAnonymousWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaAnonymousWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Since the Id column is Primary/Identity, we are require to pass a value into it.
                // In order to ensure that the assertion that the identity value is returned, we need
                // create a series of records and validate the entities

                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Wait();

                // Act (More)
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                   tableName,
                   entities: entities,
                   identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                   mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Since the Id column is Primary/Identity, we are require to pass a value into it.
                // In order to ensure that the assertion that the identity value is returned, we need
                // create a series of records and validate the entities

                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Wait();

                // Act (More)
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                   tableName,
                   entities: entities,
                   identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                   mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate,
                   pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaAnonymousWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(10, queryResult.Count());
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaAnonymousWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaAnonymousWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkMergeAsync<IDictionary<string, object>>

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    batchSize: 3).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger))).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                //Assert.IsTrue(entities.All(e => e.IdMapped > 0));
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                //Assert.IsTrue(entities.All(e => e.IdMapped > 0));
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Since the Id column is Primary/Identity, we are require to pass a value into it.
                // In order to ensure that the assertion that the identity value is returned, we need
                // create a series of records and validate the entities

                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Wait();

                // Act (More)
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                   tableName,
                   entities: entities,
                   identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                   mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Since the Id column is Primary/Identity, we are require to pass a value into it.
                // In order to ensure that the assertion that the identity value is returned, we need
                // create a series of records and validate the entities

                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Wait();

                // Act (More)
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                   tableName,
                   entities: entities,
                   identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                   mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate,
                   pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                Assert.IsTrue(entities.All(e => e.Id > 0));

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                Assert.AreEqual(10, queryResult.Count());
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaExpandoObjectWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkMergeAsync<DataTable>

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table,
                    batchSize: 3).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger))).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                foreach (DataRow row in table.Rows)
                {
                    Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                foreach (DataRow row in table.Rows)
                {
                    Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table,
                    mappings: mappings).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithBulkInsertMapItemsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithBulkInsertMapItemsAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                foreach (DataRow row in table.Rows)
                {
                    Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                foreach (DataRow row in table.Rows)
                {
                    Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                    mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Since the Id column is Primary/Identity, we are require to pass a value into it.
                // In order to ensure that the assertion that the identity value is returned, we need
                // create a series of records and validate the entities

                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table).Wait();

                // Act (More)
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                   tableName,
                   table: table,
                   identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                   mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                foreach (DataRow row in table.Rows)
                {
                    Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Since the Id column is Primary/Identity, we are require to pass a value into it.
                // In order to ensure that the assertion that the identity value is returned, we need
                // create a series of records and validate the entities

                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table).Wait();

                // Act (More)
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                   tableName,
                   table: table,
                   identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                   mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate,
                   pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);
                foreach (DataRow row in table.Rows)
                {
                    Assert.IsTrue(Convert.ToInt32(row["Id"]) > 0);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table).Result;

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(10, queryResult.Count());
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDataTableWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table).Result;

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                    tableName,
                    table: table).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #region BulkMergeAsync<DbDataReader>

        [TestMethod]
        public void TestBulkMergeAsyncViaDbDataReader()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                        tableName,
                        reader).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDbDataReaderWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                        tableName,
                        reader,
                        qualifiers: Field.From(
                            nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                            nameof(BulkOperationLightIdentityTable.ColumnInteger))).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDbDataReaderWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                        tableName,
                        reader: reader,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDbDataReaderWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                        tableName,
                        reader: reader,
                        mappings: mappings).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };


                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                        tableName,
                        reader: reader,
                        mappings: mappings,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };


                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                        tableName,
                        reader: reader,
                        mappings: mappings,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                        pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDbDataReaderWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                        tableName,
                        reader,
                        mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDbDataReaderWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                        tableName,
                        reader,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity,
                        mergeCommandType: BulkImportMergeCommandType.OnConflictDoUpdate).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDbDataReaderWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                        tableName,
                        reader).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                        tableName,
                        reader).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(10, queryResult.Count());
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDbDataReaderWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                        tableName,
                        reader).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBulkMergeAsyncViaDbDataReaderWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                        tableName,
                        reader).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkMergeAsync(connection,
                        tableName,
                        reader).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2) - 10, false);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        #endregion

        #endregion
    }
}
