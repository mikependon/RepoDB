using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class BinaryBulkUpdateTest
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

        #region BinaryBulkUpdate<TEntity>

        [TestMethod]
        public void TestBinaryBulkUpdate()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkUpdateTableNameWithSchema()
        {
            using var connection = GetConnection();
            
            // Prepare
            var createdEntities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "public.BulkOperationIdentityTable";
            
            // Act
            connection.BinaryBulkInsert(tableName,
                entities: createdEntities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Prepare
            var updatedEntities = Helper.UpdateBulkOperationLightIdentityTables(createdEntities);

            // Act
            var result = connection.BinaryBulkUpdate(tableName, updatedEntities);

            // Assert
            Assert.AreEqual(updatedEntities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            Assert.AreEqual(10, queryResult.Count);
            
            var assertCount = Helper.AssertEntitiesEquality(updatedEntities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(expected: updatedEntities.Count, assertCount);
        }

        [TestMethod]
        public void TestBinaryBulkUpdateWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkUpdateWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkUpdateWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateWithMappings()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationMappedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate<BulkOperationMappedIdentityTable>(connection,
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
        public void TestBinaryBulkUpdateWithMappingsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationMappedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateWithMappingsAndWithKeepIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationMappedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true,
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
        public void TestBinaryBulkUpdateWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate<BulkOperationUnmatchedIdentityTable>(connection,
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
        public void TestBinaryBulkUpdateWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true,
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
        public void TestBinaryBulkUpdateOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdate<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BinaryBulkUpdate<Anonymous>

        [TestMethod]
        public void TestBinaryBulkUpdateViaAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationAnonymousLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
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
        public void TestBinaryBulkUpdateViaAnonymousWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationAnonymousLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
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
        public void TestBinaryBulkUpdateViaAnonymousWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationAnonymousLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
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
        public void TestBinaryBulkUpdateViaAnonymousWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationAnonymousLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateViaAnonymousWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true);
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationAnonymousUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
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
        public void TestBinaryBulkUpdateViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationAnonymousUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationAnonymousUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true,
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
        public void TestBinaryBulkUpdateViaAnonymousOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BinaryBulkUpdate<IDictionary<string, object>>

        [TestMethod]
        public void TestBinaryBulkUpdateViaExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationExpandoObjectLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
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
        public void TestBinaryBulkUpdateViaExpandoObjectWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationExpandoObjectLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
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
        public void TestBinaryBulkUpdateViaExpandoObjectWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationExpandoObjectLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
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
        public void TestBinaryBulkUpdateViaExpandoObjectWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationExpandoObjectLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateViaExpandoObjectWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, true);
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationExpandoObjectUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
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
        public void TestBinaryBulkUpdateViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationExpandoObjectUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationExpandoObjectUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true,
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
        public void TestBinaryBulkUpdateViaExpandoObjectOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BinaryBulkUpdate<DataTable>

        [TestMethod]
        public void TestBinaryBulkUpdateViaDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    table,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
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
        public void TestBinaryBulkUpdateViaDataTableWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    table,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
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
        public void TestBinaryBulkUpdateViaDataTableWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    table,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
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
        public void TestBinaryBulkUpdateViaDataTableWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    table,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                    tableName,
                    table: table,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateViaDataTableWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    table,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
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
        public void TestBinaryBulkUpdateViaDataTableWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    table,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                    tableName,
                    table: table,
                    mappings: mappings,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateViaDataTableWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    table,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                    tableName,
                    table: table,
                    mappings: mappings,
                    keepIdentity: true,
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
        public void TestBinaryBulkUpdateViaDataTableOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                    tableName,
                    table);

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BinaryBulkUpdate<DbDataReader>

        [TestMethod]
        public void TestBinaryBulkUpdateViaDbDataReader()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                        tableName,
                        reader,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
                }

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
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
        public void TestBinaryBulkUpdateViaDbDataReaderWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                        tableName,
                        reader,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
                }

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
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
        public void TestBinaryBulkUpdateViaDbDataReaderWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                        tableName,
                        reader,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
                }

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                        tableName,
                        reader,
                        keepIdentity: true);

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
        public void TestBinaryBulkUpdateViaDbDataReaderWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
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
                    NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
                }

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                        tableName,
                        reader,
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
        public void TestBinaryBulkUpdateViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentity()
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
                    NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
                }

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        keepIdentity: true);

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
        public void TestBinaryBulkUpdateViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                    NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
                }

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        keepIdentity: true,
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
        public void TestBinaryBulkUpdateViaDbDataReaderOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkUpdate(connection,
                        tableName,
                        reader);

                    // Assert
                    Assert.AreEqual(0, result);
                }
            }
        }
        #endregion

        #endregion

        #region Async

        #region BinaryBulkUpdate<TEntity>

        [TestMethod]
        public void TestBinaryBulkUpdateAsync()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync<BulkOperationLightIdentityTable>(connection,
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
        public async Task TestBinaryBulkUpdateAsyncTableNameWithSchema()
        {
            await using var connection = GetConnection();
            
            // Prepare
            var createdEntities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "public.BulkOperationIdentityTable";
            
            // Act
            await connection.BinaryBulkInsertAsync(tableName,
                entities: createdEntities,
                identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

            // Prepare
            var updatedEntities = Helper.UpdateBulkOperationLightIdentityTables(createdEntities);

            // Act
            var result = await connection.BinaryBulkUpdateAsync(tableName, updatedEntities);

            // Assert
            Assert.AreEqual(updatedEntities.Count, result);

            // Assert
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
            Assert.AreEqual(10, queryResult.Count);
            
            var assertCount = Helper.AssertEntitiesEquality(updatedEntities, queryResult, (t1, t2) => t1.Id == t2.Id, false);
            Assert.AreEqual(expected: updatedEntities.Count, assertCount);
        }

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkUpdateAsyncWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkUpdateAsyncWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncWithMappings()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationMappedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync<BulkOperationMappedIdentityTable>(connection,
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
        public void TestBinaryBulkUpdateAsyncWithMappingsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationMappedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncWithMappingsAndWithKeepIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationMappedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true,
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
        public void TestBinaryBulkUpdateAsyncWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync<BulkOperationUnmatchedIdentityTable>(connection,
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
        public void TestBinaryBulkUpdateAsyncWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true,
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
        public void TestBinaryBulkUpdateAsyncOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BinaryBulkUpdate<Anonymous>

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncViaAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationAnonymousLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
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
        public void TestBinaryBulkUpdateAsyncViaAnonymousWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationAnonymousLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
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
        public void TestBinaryBulkUpdateAsyncViaAnonymousWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationAnonymousLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
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
        public void TestBinaryBulkUpdateAsyncViaAnonymousWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationAnonymousLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncViaAnonymousWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true);
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationAnonymousUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
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
        public void TestBinaryBulkUpdateAsyncViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationAnonymousUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.IdMapped);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationAnonymousUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true,
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
        public void TestBinaryBulkUpdateAsyncViaAnonymousOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BinaryBulkUpdate<IDictionary<string, object>>

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncViaExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationExpandoObjectLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
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
        public void TestBinaryBulkUpdateAsyncViaExpandoObjectWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationExpandoObjectLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
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
        public void TestBinaryBulkUpdateAsyncViaExpandoObjectWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationExpandoObjectLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
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
        public void TestBinaryBulkUpdateAsyncViaExpandoObjectWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationExpandoObjectLightIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.Id == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncViaExpandoObjectWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, true);
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationExpandoObjectUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
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
        public void TestBinaryBulkUpdateAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationExpandoObjectUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName);
                var assertCount = Helper.AssertExpandoObjectsEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationExpandoObjectUnmatchedIdentityTables(entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true,
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
        public void TestBinaryBulkUpdateAsyncViaExpandoObjectOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BinaryBulkUpdate<DataTable>

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncViaDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    table,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
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
        public void TestBinaryBulkUpdateAsyncViaDataTableWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    table,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
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
        public void TestBinaryBulkUpdateAsyncViaDataTableWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    table,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
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
        public void TestBinaryBulkUpdateAsyncViaDataTableWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    table,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                    tableName,
                    table: table,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => entities.IndexOf(t1) == queryResult.IndexOf(t2));
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncViaDataTableWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    table,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
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
        public void TestBinaryBulkUpdateAsyncViaDataTableWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    table,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                    tableName,
                    table: table,
                    mappings: mappings,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName);
                var assertCount = Helper.AssertEntitiesEquality(entities, queryResult, (t1, t2) => t1.IdMapped == t2.Id);
                Assert.AreEqual(entities.Count(), assertCount);
            }
        }

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncViaDataTableWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                    tableName,
                    table,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);
                table = Helper.ToDataTable(tableName, entities);

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                    tableName,
                    table: table,
                    mappings: mappings,
                    keepIdentity: true,
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
        public void TestBinaryBulkUpdateAsyncViaDataTableOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                    tableName,
                    table).Result;

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BinaryBulkUpdate<DbDataReader>

        [TestMethod]
        public void TestBinaryBulkUpdateAsyncViaDbDataReader()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                        tableName,
                        reader,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
                }

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
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
        public void TestBinaryBulkUpdateAsyncViaDbDataReaderWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                        tableName,
                        reader,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
                }

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
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
        public void TestBinaryBulkUpdateAsyncViaDbDataReaderWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                        tableName,
                        reader,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
                }

                // Prepare
                entities = Helper.UpdateBulkOperationLightIdentityTables(entities);

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                        tableName,
                        reader,
                        keepIdentity: true).Result;

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
        public void TestBinaryBulkUpdateAsyncViaDbDataReaderWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
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
                    NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
                }

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                        tableName,
                        reader,
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
        public void TestBinaryBulkUpdateAsyncViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentity()
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
                    NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
                }

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        keepIdentity: true).Result;

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
        public void TestBinaryBulkUpdateAsyncViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentityViaPhysicalTable()
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
                    NpgsqlConnectionExtension.BinaryBulkInsert(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
                }

                // Prepare
                entities = Helper.UpdateBulkOperationUnmatchedIdentityTables(entities);

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        keepIdentity: true,
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
        public void TestBinaryBulkUpdateAsyncViaDbDataReaderOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkUpdateAsync(connection,
                        tableName,
                        reader).Result;

                    // Assert
                    Assert.AreEqual(0, result);
                }
            }
        }
        #endregion

        #endregion
    }
}
