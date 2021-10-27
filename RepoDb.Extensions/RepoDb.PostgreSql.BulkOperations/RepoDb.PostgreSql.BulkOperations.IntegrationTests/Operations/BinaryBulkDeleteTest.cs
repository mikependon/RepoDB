using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using System.Linq;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class BinaryBulkDeleteTest
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

        #region BinaryBulkDelete<TEntity>

        [TestMethod]
        public void TestBinaryBulkDelete()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteWithBatchSize()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    batchSize: 3);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteWithKeepIdentityFalse()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: false);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteWithQualifiers()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger)));

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteWithMappings()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteWithMappingsViaPhysicalTable()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteWithBulkInsertMapItems()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteWithBulkInsertMapItemsViaPhysicalTable()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDelete<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BinaryBulkDelete<Anonymous>

        [TestMethod]
        public void TestBinaryBulkDeleteViaAnonymous()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaAnonymousWithBatchSize()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    entities: entities,
                    batchSize: 3);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaAnonymousWithKeepIdentityFalse()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: false);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaAnonymousWithQualifiers()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    entities: entities,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger)));

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaAnonymousWithBulkInsertMapItems()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaAnonymousWithBulkInsertMapItemsViaPhysicalTable()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaAnonymousOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BinaryBulkDelete<IDictionary<string, object>>

        [TestMethod]
        public void TestBinaryBulkDeleteViaExpandoObject()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaExpandoObjectWithBatchSize()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    entities: entities,
                    batchSize: 3);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaExpandoObjectWithKeepIdentityFalse()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: false);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaExpandoObjectWithQualifiers()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    entities: entities,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger)));

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaExpandoObjectWithBulkInsertMapItems()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaExpandoObjectWithBulkInsertMapItemsViaPhysicalTable()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaExpandoObjectOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BinaryBulkDelete<DataTable>

        [TestMethod]
        public void TestBinaryBulkDeleteViaDataTable()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    table);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaDataTableWithKeepIdentityFalse()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    table: table,
                    keepIdentity: false);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaDataTableWithQualifiers()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    table: table,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger)));

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaDataTableWithBulkInsertMapItems()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    table: table,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaDataTableWithBulkInsertMapItemsViaPhysicalTable()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    table: table,
                    mappings: mappings,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaDataTableOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                    tableName,
                    table);

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BinaryBulkDelete<DbDataReader>

        [TestMethod]
        public void TestBinaryBulkDeleteViaDbDataReader()
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

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                        tableName,
                        reader);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaDbDataReaderWithKeepIdentityFalse()
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

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                        tableName,
                        reader,
                        keepIdentity: false);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaDbDataReaderWithQualifiers()
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

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                        tableName,
                        reader,
                        qualifiers: Field.From(
                            nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                            nameof(BulkOperationLightIdentityTable.ColumnInteger)));

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaDbDataReaderWithBulkInsertMapItems()
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

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                        tableName,
                        reader,
                        mappings: mappings);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaDbDataReaderWithBulkInsertMapItemsViaPhysicalTable()
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

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        pseudoTableType: BulkImportPseudoTableType.Physical);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteViaDbDataReaderOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkDelete(connection,
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

        #region BinaryBulkDelete<TEntity>

        [TestMethod]
        public void TestBinaryBulkDeleteAsync()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncWithBatchSize()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    batchSize: 3).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncWithKeepIdentityFalse()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: false).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncWithQualifiers()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger))).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncWithMappings()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncWithMappingsViaPhysicalTable()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncWithBulkInsertMapItems()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncWithBulkInsertMapItemsViaPhysicalTable()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BinaryBulkDelete<Anonymous>

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaAnonymous()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaAnonymousWithBatchSize()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    entities: entities,
                    batchSize: 3).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaAnonymousWithKeepIdentityFalse()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: false).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaAnonymousWithQualifiers()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    entities: entities,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger))).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaAnonymousWithBulkInsertMapItems()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaAnonymousWithBulkInsertMapItemsViaPhysicalTable()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaAnonymousOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BinaryBulkDelete<IDictionary<string, object>>

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaExpandoObject()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaExpandoObjectWithBatchSize()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    entities: entities,
                    batchSize: 3).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaExpandoObjectWithKeepIdentityFalse()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: false).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaExpandoObjectWithQualifiers()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    entities: entities,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger))).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaExpandoObjectWithBulkInsertMapItems()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaExpandoObjectWithBulkInsertMapItemsViaPhysicalTable()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaExpandoObjectOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BinaryBulkDelete<DataTable>

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaDataTable()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    table).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaDataTableWithKeepIdentityFalse()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    table: table,
                    keepIdentity: false).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaDataTableWithQualifiers()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    table: table,
                    qualifiers: Field.From(
                        nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                        nameof(BulkOperationLightIdentityTable.ColumnInteger))).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaDataTableWithBulkInsertMapItems()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    table: table,
                    mappings: mappings).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaDataTableWithBulkInsertMapItemsViaPhysicalTable()
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

                // Act
                result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    table: table,
                    mappings: mappings,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaDataTableOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                    tableName,
                    table).Result;

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BinaryBulkDelete<DbDataReader>

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaDbDataReader()
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

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                        tableName,
                        reader).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaDbDataReaderWithKeepIdentityFalse()
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

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                        tableName,
                        reader,
                        keepIdentity: false).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaDbDataReaderWithQualifiers()
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

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                        tableName,
                        reader,
                        qualifiers: Field.From(
                            nameof(BulkOperationLightIdentityTable.ColumnBigInt),
                            nameof(BulkOperationLightIdentityTable.ColumnInteger))).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaDbDataReaderWithBulkInsertMapItems()
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

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                        tableName,
                        reader,
                        mappings: mappings).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaDbDataReaderWithBulkInsertMapItemsViaPhysicalTable()
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

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBinaryBulkDeleteAsyncViaDbDataReaderOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryBulkDeleteAsync(connection,
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
