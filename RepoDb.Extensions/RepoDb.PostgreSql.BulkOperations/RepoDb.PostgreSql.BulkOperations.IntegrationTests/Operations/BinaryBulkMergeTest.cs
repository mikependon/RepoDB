using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using System;
using System.Data;
using System.Linq;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class BinaryBulkMergeTest
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
            (NpgsqlConnection)(new NpgsqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen());

        #region Sync

        #region BinaryBulkMerge<TEntity>

        [TestMethod]
        public void TestBinaryBulkMerge()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithMappings()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationMappedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithMappingsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationMappedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithMappingsAndWithKeepIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationMappedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithMappingsAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationMappedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithMappingsAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationMappedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithBulkInsertMapItems()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationUnmatchedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationUnmatchedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationUnmatchedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithBulkInsertMapItemsAndWithReturnIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationUnmatchedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationUnmatchedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
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
                NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Act (More)
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
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
                NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Act (More)
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkMerge<BulkOperationLightIdentityTable>(connection,
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

        #region BinaryBulkMerge<Anonymous>

        [TestMethod]
        public void TestBinaryBulkMergeViaAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaAnonymousWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaAnonymousWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaAnonymousWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaAnonymousWithBulkInsertMapItems()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaAnonymousWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
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
                NpgsqlConnectionExtension.BinaryBulkMerge(connection,
                    tableName,
                    entities: entities);

                // Act (More)
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
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
                NpgsqlConnectionExtension.BinaryBulkMerge(connection,
                    tableName,
                    entities: entities);

                // Act (More)
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaAnonymousWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
                    tableName,
                    entities: entities);

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaAnonymousWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaAnonymousWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
                    tableName,
                    entities: entities);

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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

        #region BinaryBulkMerge<IDictionary<string, object>>

        [TestMethod]
        public void TestBinaryBulkMergeViaExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithBulkInsertMapItems()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithBulkInsertMapItemsAndWithReturnIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
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
                NpgsqlConnectionExtension.BinaryBulkMerge(connection,
                    tableName,
                    entities: entities);

                // Act (More)
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
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
                NpgsqlConnectionExtension.BinaryBulkMerge(connection,
                    tableName,
                    entities: entities);

                // Act (More)
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
                    tableName,
                    entities: entities);

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaExpandoObjectWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
                    tableName,
                    entities: entities);

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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

        #region BinaryBulkMerge<DataTable>

        [TestMethod]
        public void TestBinaryBulkMergeViaDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithBulkInsertMapItems()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithBulkInsertMapItemsAndWithReturnIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
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
                NpgsqlConnectionExtension.BinaryBulkMerge(connection,
                    tableName,
                    table: table);

                // Act (More)
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
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
                NpgsqlConnectionExtension.BinaryBulkMerge(connection,
                    tableName,
                    table: table);

                // Act (More)
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
                    tableName,
                    table: table);

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDataTableWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
                    tableName,
                    table: table);

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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

        #region BinaryBulkMerge<DbDataReader>

        [TestMethod]
        public void TestBinaryBulkMergeViaDbDataReader()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDbDataReaderWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDbDataReaderWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDbDataReaderWithBulkInsertMapItems()
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
                    var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentity()
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
                    var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                    var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDbDataReaderWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDbDataReaderWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDbDataReaderWithExistingData()
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
                    var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
                    var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDbDataReaderWithNoIdentityValues()
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
                    var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
        public void TestBinaryBulkMergeViaDbDataReaderWithNoIdentityValuesAndWithExistingData()
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
                    var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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
                    var result = NpgsqlConnectionExtension.BinaryBulkMerge(connection,
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

        #region BinaryBulkMergeAsync<TEntity>

        [TestMethod]
        public void TestBinaryBulkMergeAsync()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithMappings()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationMappedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithMappingsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationMappedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithMappingsAndWithKeepIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationMappedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithMappingsAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationMappedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithMappingsAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationMappedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithBulkInsertMapItems()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationUnmatchedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationUnmatchedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationUnmatchedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithBulkInsertMapItemsAndWithReturnIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationUnmatchedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationUnmatchedIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
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
                NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Wait();

                // Act (More)
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
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
                NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Wait();

                // Act (More)
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkMergeAsyncWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkMergeAsync<BulkOperationLightIdentityTable>(connection,
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

        #region BinaryBulkMergeAsync<Anonymous>

        [TestMethod]
        public void TestBinaryBulkMergeAsyncViaAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaAnonymousWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaAnonymousWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaAnonymousWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaAnonymousWithBulkInsertMapItems()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaAnonymousWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
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
                NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
                    tableName,
                    entities: entities).Wait();

                // Act (More)
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaAnonymousWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
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
                NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
                    tableName,
                    entities: entities).Wait();

                // Act (More)
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaAnonymousWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaAnonymousWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaAnonymousWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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

        #region BinaryBulkMergeAsync<IDictionary<string, object>>

        [TestMethod]
        public void TestBinaryBulkMergeAsyncViaExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithBulkInsertMapItems()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithReturnIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
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
                NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
                    tableName,
                    entities: entities).Wait();

                // Act (More)
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
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
                NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
                    tableName,
                    entities: entities).Wait();

                // Act (More)
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaExpandoObjectWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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

        #region BinaryBulkMergeAsync<DataTable>

        [TestMethod]
        public void TestBinaryBulkMergeAsyncViaDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithBulkInsertMapItems()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithBulkInsertMapItemsAndWithReturnIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithBulkInsertMapItemsAndWithReturnIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentity()
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
                NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
                    tableName,
                    table: table).Wait();

                // Act (More)
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithOnConflictDoUpdateMergeCommandTypeAndWithReturnIdentityViaPhysicalTable()
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
                NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
                    tableName,
                    table: table).Wait();

                // Act (More)
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
                    tableName,
                    table: table).Result;

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithNoIdentityValues()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDataTableWithNoIdentityValuesAndWithExistingData()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
                    tableName,
                    table: table).Result;

                // Prepare (Elimination)
                entities = entities
                    .Where((entity, index) => index % 2 == 0)
                    .ToList();
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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

        #region BinaryBulkMergeAsync<DbDataReader>

        [TestMethod]
        public void TestBinaryBulkMergeAsyncViaDbDataReader()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDbDataReaderWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, false);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDbDataReaderWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDbDataReaderWithBulkInsertMapItems()
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
                    var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentity()
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
                    var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                    var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDbDataReaderWithOnConflictDoUpdateMergeCommandType()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDbDataReaderWithOnConflictDoUpdateMergeCommandTypeAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDbDataReaderWithExistingData()
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
                    var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
                    var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDbDataReaderWithNoIdentityValues()
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
                    var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
        public void TestBinaryBulkMergeAsyncViaDbDataReaderWithNoIdentityValuesAndWithExistingData()
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
                    var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
                    var result = NpgsqlConnectionExtension.BinaryBulkMergeAsync(connection,
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
