using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using System;
using System.Linq;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Base
{
    [TestClass]
    public class BinaryImportTest
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

        #region BinaryImport<TEntity>

        [TestMethod]
        public void TestBinaryImport()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(99, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    batchSize: 10);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportWithMappings()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.IdMapped == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportWithMappingsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.IdMapped == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportWithBulkInsertMapItems()
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
                var result = NpgsqlConnectionExtension.BinaryImport<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                for (var i = 0; i < entities.Count; i++)
                {
                    Helper.AssertPropertiesEquality(entities[i], queryResult[i]);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportWithBulkInsertMapItemsWithKeepIdentity()
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
                entities.ForEach(entity => entity.IdMapped += 100);

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.IdMapped == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod, ExpectedException(typeof(PostgresException))]
        public void ThrowExceptionOnBinaryImportWithDuplicateIdentityOnKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BinaryImport<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true);

                // Act (Trigger)
                NpgsqlConnectionExtension.BinaryImport<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true);
            }
        }

        #endregion

        // Use AssertMembersEqualities

        #region BinaryImport<Anonymous>

        [TestMethod]
        public void TestBinaryImportViaAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportViaAnoynymousWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(99, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    tableName,
                    entities: entities,
                    batchSize: 10);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportViaAnonymousWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportViaAnonymousWithBulkInsertMapItems()
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
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true, 100);

                // Prepare
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
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod, ExpectedException(typeof(PostgresException))]
        public void ThrowExceptionOnBinaryImportViaAnonymousWithDuplicateIdentityOnKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BinaryImport(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true);

                // Act (Trigger)
                NpgsqlConnectionExtension.BinaryImport(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true);
            }
        }

        #endregion

        // Use AssertMembersEqualities

        #region BinaryImport<IDictionary<string,object>>

        [TestMethod]
        public void TestBinaryImportViaExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == ((dynamic)entity).Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportViaExpandoObjectWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(99, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    tableName,
                    entities: entities,
                    batchSize: 10);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                for (var i = 0; i < entities.Count; i++)
                {
                    Helper.AssertPropertiesEquality(entities[i], queryResult[i]);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportViaExpandoObjectWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == ((dynamic)entity).Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportViaExpandoObjectWithBulkInsertMapItems()
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
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                for (var i = 0; i < entities.Count; i++)
                {
                    Helper.AssertPropertiesEquality(entities[i], queryResult[i]);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                for (var i = 0; i < entities.Count; i++)
                {
                    Helper.AssertPropertiesEquality(entities[i], queryResult[i]);
                }
            }
        }

        [TestMethod, ExpectedException(typeof(PostgresException))]
        public void ThrowExceptionOnBinaryImportViaExpandoObjectWithDuplicateIdentityOnKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BinaryImport(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true);

                // Act (Trigger)
                NpgsqlConnectionExtension.BinaryImport(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true);
            }
        }

        #endregion

        #region BinaryImport<DataTable>

        [TestMethod]
        public void TestBinaryImportViaDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    table.TableName,
                    table: table);

                // Assert
                Assert.AreEqual(table.Rows.Count, result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportViaDataTableWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(99, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    table.TableName,
                    table: table,
                    batchSize: 10);

                // Assert
                Assert.AreEqual(table.Rows.Count, result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportViaDataTableWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);

                // Prepare
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    table.TableName,
                    table: table,
                    keepIdentity: true);

                // Assert
                Assert.AreEqual(table.Rows.Count, result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportViaDataTableWithBulkInsertMapItems()
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
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    table.TableName,
                    table: table,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(table.Rows.Count, result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportViaDataTableWithBulkInsertMapItemsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);

                // Prepare
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
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    table.TableName,
                    table: table,
                    mappings: mappings);

                // Assert
                Assert.AreEqual(table.Rows.Count, result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod, ExpectedException(typeof(PostgresException))]
        public void ThrowExceptionOnBinaryImportViaDataTableWithDuplicateIdentityOnKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var table = Helper.CreateBulkOperationDataTableLightIdentityTables(10, true);

                // Act
                NpgsqlConnectionExtension.BinaryImport(connection,
                    table.TableName,
                    table: table,
                    keepIdentity: true);

                // Act (Trigger)
                NpgsqlConnectionExtension.BinaryImport(connection,
                    table.TableName,
                    table: table,
                    keepIdentity: true);
            }
        }

        #endregion

        #region BinaryImport<DbDataReader>

        [TestMethod]
        public void TestBinaryImportViaDbDataReader()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryImport(connection,
                        tableName,
                        reader: reader);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);

                    // Assert
                    var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                    Assert.AreEqual(entities.Count(), queryResult.Count());
                    foreach (var entity in entities)
                    {
                        var item = queryResult.First(e => e.Id == entity.Id);
                        Helper.AssertPropertiesEquality(entity, item);
                    }
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportViaDbDataReaderWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryImport(connection,
                        tableName,
                        reader: reader,
                        keepIdentity: true);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);

                    // Assert
                    var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                    Assert.AreEqual(entities.Count(), queryResult.Count());
                    foreach (var entity in entities)
                    {
                        var item = queryResult.First(e => e.Id == entity.Id);
                        Helper.AssertPropertiesEquality(entity, item);
                    }
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportViaDbDataReaderWithBulkInsertMapItems()
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
                    var result = NpgsqlConnectionExtension.BinaryImport(connection,
                        tableName,
                        reader: reader,
                        mappings: mappings);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);

                    // Assert
                    var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                    Assert.AreEqual(entities.Count(), queryResult.Count());
                    foreach (var entity in entities)
                    {
                        var item = queryResult.First(e => e.Id == entity.IdMapped);
                        Helper.AssertPropertiesEquality(entity, item);
                    }
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportViaDbDataReaderWithBulkInsertMapItemsWithKeepIdentity()
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
                    var result = NpgsqlConnectionExtension.BinaryImport(connection,
                        tableName,
                        reader: reader,
                        mappings: mappings,
                        keepIdentity: true);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);

                    // Assert
                    var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                    Assert.AreEqual(entities.Count(), queryResult.Count());
                    foreach (var entity in entities)
                    {
                        var item = queryResult.First(e => e.Id == entity.IdMapped);
                        Helper.AssertPropertiesEquality(entity, item);
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(PostgresException))]
        public void ThrowExceptionOnBinaryImportViaDbDataReaderWithDuplicateIdentityOnKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BinaryImport(connection,
                        tableName,
                        reader: reader,
                        keepIdentity: true);
                }

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act (Trigger)
                    NpgsqlConnectionExtension.BinaryImport(connection,
                        tableName,
                        reader: reader,
                        keepIdentity: true);
                }
            }
        }

        #endregion

        #endregion

        #region Async

        #region BinaryImportAsync<TEntity>

        [TestMethod]
        public void TestBinaryImportAsync()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImportAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(99, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImportAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    batchSize: 10).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImportAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncWithMappings()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImportAsync<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.IdMapped == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncWithMappingsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImportAsync<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.IdMapped == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncWithBulkInsertMapItems()
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
                var result = NpgsqlConnectionExtension.BinaryImportAsync<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.IdMapped == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryImportAsync<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.IdMapped == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnBinaryImportAsyncWithDuplicateIdentityOnKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BinaryImportAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true).Wait();

                // Act (Trigger)
                NpgsqlConnectionExtension.BinaryImportAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true).Wait();
            }
        }

        #endregion

        // Use AssertMembersEqualities

        #region BinaryImportAsync<Anonymous>

        [TestMethod]
        public void TestBinaryImportAsyncViaAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaAnoynymousWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(99, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    tableName,
                    entities: entities,
                    batchSize: 10).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaAnonymousAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaAnonymousWithBulkInsertMapItems()
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
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaAnonymousWithBulkInsertMapItemsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true, 100);

                // Prepare
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
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnBinaryImportAsyncViaAnonymousWithDuplicateIdentityOnKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true).Wait();

                // Act (Trigger)
                NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true).Wait();
            }
        }

        #endregion

        // Use AssertMembersEqualities

        #region BinaryImportAsync<IDictionary<string,object>>

        [TestMethod]
        public void TestBinaryImportAsyncViaExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (dynamic entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaExpandoObjectWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(99, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    tableName,
                    entities: entities,
                    batchSize: 10).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (dynamic entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaExpandoObjectAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (dynamic entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaExpandoObjectWithBulkInsertMapItems()
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
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (dynamic entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaExpandoObjectWithBulkInsertMapItemsAndWithKeepIdentity()
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
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (dynamic entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnBinaryImportAsyncViaExpandoObjectWithDuplicateIdentityOnKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true).Wait();

                // Act (Trigger)
                NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    tableName,
                    entities: entities,
                    keepIdentity: true).Wait();
            }
        }

        #endregion

        #region BinaryImportAsync<DataTable>

        [TestMethod]
        public void TestBinaryImportAsyncViaDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    table.TableName,
                    table: table).Result;

                // Assert
                Assert.AreEqual(table.Rows.Count, result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaDataTableWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(99, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    table.TableName,
                    table: table,
                    batchSize: 10).Result;

                // Assert
                Assert.AreEqual(table.Rows.Count, result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaDataTableAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);

                // Prepare
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    table.TableName,
                    table: table,
                    keepIdentity: true).Result;

                // Assert
                Assert.AreEqual(table.Rows.Count, result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaDataTableWithBulkInsertMapItems()
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
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    table.TableName,
                    table: table,
                    mappings: mappings).Result;

                // Assert
                Assert.AreEqual(table.Rows.Count, result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaDataTableWithBulkInsertMapItemsAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);

                // Prepare
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
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    table.TableName,
                    table: table,
                    mappings: mappings).Result;

                // Assert
                Assert.AreEqual(table.Rows.Count, result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var item = queryResult.First(e => e.Id == entity.IdMapped);
                    Helper.AssertPropertiesEquality(entity, item);
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnBinaryImportAsyncViaDataTableWithDuplicateIdentityOnKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var table = Helper.CreateBulkOperationDataTableLightIdentityTables(10, true);

                // Act
                NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    table.TableName,
                    table: table,
                    keepIdentity: true).Wait();

                // Act (Trigger)
                NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    table.TableName,
                    table: table,
                    keepIdentity: true).Wait();
            }
        }

        #endregion

        #region BinaryImportAsync<DbDataReader>

        [TestMethod]
        public void TestBinaryImportAsyncViaDbDataReader()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                        tableName,
                        reader: reader).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);

                    // Assert
                    var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                    Assert.AreEqual(entities.Count(), queryResult.Count());
                    foreach (var entity in entities)
                    {
                        var item = queryResult.First(e => e.Id == entity.Id);
                        Helper.AssertPropertiesEquality(entity, item);
                    }
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaDbDataReaderAndWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                        tableName,
                        reader: reader,
                        keepIdentity: true).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);

                    // Assert
                    var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                    Assert.AreEqual(entities.Count(), queryResult.Count());
                    foreach (var entity in entities)
                    {
                        var item = queryResult.First(e => e.Id == entity.Id);
                        Helper.AssertPropertiesEquality(entity, item);
                    }
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaDbDataReaderWithBulkInsertMapItems()
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
                    var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                        tableName,
                        reader: reader,
                        mappings: mappings).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);

                    // Assert
                    var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                    Assert.AreEqual(entities.Count(), queryResult.Count());
                    foreach (var entity in entities)
                    {
                        var item = queryResult.First(e => e.Id == entity.IdMapped);
                        Helper.AssertPropertiesEquality(entity, item);
                    }
                }
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaDbDataReaderWithBulkInsertMapItemsAndWithKeepIdentity()
        {

            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);

                // Prepare
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
                    var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                        tableName,
                        reader: reader,
                        mappings: mappings,
                        keepIdentity: true).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);

                    // Assert
                    var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                    Assert.AreEqual(entities.Count(), queryResult.Count());
                    foreach (var entity in entities)
                    {
                        var item = queryResult.First(e => e.Id == entity.IdMapped);
                        Helper.AssertPropertiesEquality(entity, item);
                    }
                }
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnBinaryImportAsyncViaDbDataReaderWithDuplicateIdentityOnKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BinaryImportAsync(connection,
                        tableName,
                        reader: reader,
                        keepIdentity: true).Wait();
                }

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act (Trigger)
                    NpgsqlConnectionExtension.BinaryImportAsync(connection,
                        tableName,
                        reader: reader,
                        keepIdentity: true).Wait();
                }
            }
        }

        #endregion

        #endregion
    }
}
