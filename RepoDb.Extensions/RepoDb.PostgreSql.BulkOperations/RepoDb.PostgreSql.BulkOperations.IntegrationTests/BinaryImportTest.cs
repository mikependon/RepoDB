using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.Enumerations;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using System;
using System.Linq;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests
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

        #region BinaryImport<TEntity/Anonymous/IDictionary<string, object>>

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
                for (var i = 0; i < entities.Count; i++)
                {
                    Helper.AssertPropertiesEquality(entities[i], queryResult[i]);
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
                for (var i = 0; i < entities.Count; i++)
                {
                    Helper.AssertPropertiesEquality(entities[i], queryResult[i]);
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
                for (var i = 0; i < entities.Count; i++)
                {
                    Helper.AssertPropertiesEquality(entities[i], queryResult[i]);
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
                for (var i = 0; i < entities.Count; i++)
                {
                    Helper.AssertPropertiesEquality(entities[i], queryResult[i]);
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
                for (var i = 0; i < entities.Count; i++)
                {
                    Helper.AssertPropertiesEquality(entities[i], queryResult[i]);
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
                    entities: entities);

                // Act (Trigger)
                NpgsqlConnectionExtension.BinaryImport<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
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
                var table = Helper.CreateBulkOperationDataTableLightIdentityTables(10, true);

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    table.TableName,
                    table: table);

                // Assert
                Assert.AreEqual(table.Rows.Count, result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(table.TableName).ToList();
                Assert.AreEqual(table.Rows.Count, queryResult.Count());
            }
        }

        [TestMethod]
        public void TestBinaryImportViaDataTableWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var table = Helper.CreateBulkOperationDataTableLightIdentityTables(99, true);

                // Act
                var result = NpgsqlConnectionExtension.BinaryImport(connection,
                    table.TableName,
                    table: table,
                    batchSize: 10);

                // Assert
                Assert.AreEqual(table.Rows.Count, result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(table.TableName).ToList();
                Assert.AreEqual(table.Rows.Count, queryResult.Count());
            }
        }

        [TestMethod]
        public void TestBinaryImportViaDataTableWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var table = Helper.CreateBulkOperationDataTableUnmatchedIdentityTables(10, true);
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
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(table.TableName).ToList();
                Assert.AreEqual(table.Rows.Count, queryResult.Count());
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
                    table: table);

                // Act (Trigger)
                NpgsqlConnectionExtension.BinaryImport(connection,
                    table.TableName,
                    table: table,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
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
                        reader: reader);
                }

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act (Trigger)
                    NpgsqlConnectionExtension.BinaryImport(connection,
                        tableName,
                        reader: reader,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity);
                }
            }
        }

        #endregion

        #endregion

        #region Async

        #region BinaryImportAsync<TEntity/Anonymous/IDictionary<string, object>>

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
                for (var i = 0; i < entities.Count; i++)
                {
                    Helper.AssertPropertiesEquality(entities[i], queryResult[i]);
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
                for (var i = 0; i < entities.Count; i++)
                {
                    Helper.AssertPropertiesEquality(entities[i], queryResult[i]);
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
                for (var i = 0; i < entities.Count; i++)
                {
                    Helper.AssertPropertiesEquality(entities[i], queryResult[i]);
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
                for (var i = 0; i < entities.Count; i++)
                {
                    Helper.AssertPropertiesEquality(entities[i], queryResult[i]);
                }
            }
        }

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
                for (var i = 0; i < entities.Count; i++)
                {
                    Helper.AssertPropertiesEquality(entities[i], queryResult[i]);
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
                for (var i = 0; i < entities.Count; i++)
                {
                    Helper.AssertPropertiesEquality(entities[i], queryResult[i]);
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
                    entities: entities).Wait();

                // Act (Trigger)
                NpgsqlConnectionExtension.BinaryImportAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity).Wait();
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
                var table = Helper.CreateBulkOperationDataTableLightIdentityTables(10, true);

                // Act
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    table.TableName,
                    table: table).Result;

                // Assert
                Assert.AreEqual(table.Rows.Count, result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(table.TableName).ToList();
                Assert.AreEqual(table.Rows.Count, queryResult.Count());
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaDataTableWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var table = Helper.CreateBulkOperationDataTableLightIdentityTables(99, true);

                // Act
                var result = NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    table.TableName,
                    table: table,
                    batchSize: 10).Result;

                // Assert
                Assert.AreEqual(table.Rows.Count, result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(table.TableName).ToList();
                Assert.AreEqual(table.Rows.Count, queryResult.Count());
            }
        }

        [TestMethod]
        public void TestBinaryImportAsyncViaDataTableWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var table = Helper.CreateBulkOperationDataTableUnmatchedIdentityTables(10, true);
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
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(table.TableName).ToList();
                Assert.AreEqual(table.Rows.Count, queryResult.Count());
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
                    table: table).Wait();

                // Act (Trigger)
                NpgsqlConnectionExtension.BinaryImportAsync(connection,
                    table.TableName,
                    table: table,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity).Wait();
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
                        reader: reader).Wait();
                }

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act (Trigger)
                    NpgsqlConnectionExtension.BinaryImportAsync(connection,
                        tableName,
                        reader: reader,
                        identityBehavior: BulkImportIdentityBehavior.KeepIdentity).Wait();
                }
            }
        }

        #endregion

        #endregion
    }
}
