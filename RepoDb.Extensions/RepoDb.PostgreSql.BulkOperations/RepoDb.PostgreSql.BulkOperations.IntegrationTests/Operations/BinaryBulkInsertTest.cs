using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using System.Linq;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class BinaryBulkInsertTest
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

        #region BinaryBulkInsert<TEntity>

        [TestMethod]
        public void TestBinaryBulkInsert()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkInsertWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var hasId = true;
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, hasId);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var target = queryResult.First(item => item.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, target);
                }
            }
        }

        [TestMethod]
        public void TestBinaryBulkInsertWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var hasId = false;
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, hasId);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var target = queryResult.First(item => item.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, target);
                }
            }
        }

        [TestMethod]
        public void TestBinaryBulkInsertWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var hasId = false;
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, hasId);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var target = queryResult.First(item => item.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, target);
                }
            }
        }

        [TestMethod]
        public void TestBinaryBulkInsertWithMappings()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationLightIdentityTable.Id), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationLightIdentityTable.ColumnBigInt), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationLightIdentityTable.ColumnBoolean), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationLightIdentityTable.ColumnInteger), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationLightIdentityTable.ColumnNumeric), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationLightIdentityTable.ColumnReal), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationLightIdentityTable.ColumnSmallInt), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationLightIdentityTable.ColumnText), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsert<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkInsertWithMappingsAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var hasId = false;
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, hasId);
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
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestBinaryBulkInsertWithMappingsAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var hasId = false;
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, hasId);
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
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

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

        #endregion

        #endregion

        #region Async

        #region BinaryBulkInsertAsync<TEntity>

        [TestMethod]
        public void TestBinaryBulkInsertAsync()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsertAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBinaryBulkInsertAsyncWithKeepIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var hasId = true;
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, hasId);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsertAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.KeepIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var target = queryResult.First(item => item.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, target);
                }
            }
        }

        [TestMethod]
        public void TestBinaryBulkInsertAsyncWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var hasId = false;
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, hasId);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsertAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var target = queryResult.First(item => item.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, target);
                }
            }
        }

        [TestMethod]
        public void TestBinaryBulkInsertAsyncWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var hasId = false;
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, hasId);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsertAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                foreach (var entity in entities)
                {
                    var target = queryResult.First(item => item.Id == entity.Id);
                    Helper.AssertPropertiesEquality(entity, target);
                }
            }
        }

        [TestMethod]
        public void TestBinaryBulkInsertAsyncWithMappings()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationLightIdentityTable.Id), "Id"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationLightIdentityTable.ColumnBigInt), "ColumnBigInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationLightIdentityTable.ColumnBoolean), "ColumnBoolean"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationLightIdentityTable.ColumnInteger), "ColumnInteger"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationLightIdentityTable.ColumnNumeric), "ColumnNumeric"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationLightIdentityTable.ColumnReal), "ColumnReal"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationLightIdentityTable.ColumnSmallInt), "ColumnSmallInt"),
                    new NpgsqlBulkInsertMapItem(nameof(BulkOperationLightIdentityTable.ColumnText), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsertAsync<BulkOperationLightIdentityTable>(connection,
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

        [TestMethod]
        public void TestBinaryBulkInsertAsyncWithMappingsAndWithReturnIdentity()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var hasId = false;
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, hasId);
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsertAsync<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestBinaryBulkInsertAsyncWithMappingsAndWithReturnIdentityViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var hasId = false;
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, hasId);
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
                var result = NpgsqlConnectionExtension.BinaryBulkInsertAsync<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical).Result;

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

        #endregion

        #endregion
    }
}
