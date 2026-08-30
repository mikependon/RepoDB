#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

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
    public class BulkDeleteTest
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

        #region BulkDelete<TEntity>

        [TestMethod]
        public void TestBulkDelete()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete<BulkOperationLightIdentityTable>(connection,
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
        public void TestBulkDeleteTableNameWithSchema()
        {
            using var connection = GetConnection();
            
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "public.BulkOperationIdentityTable";
            
            // Act
            connection.BulkInsert(tableName,
                entities: entities,
                identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

            // Act
            var result = connection.BulkDelete(tableName, entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = connection.CountAll(tableName);
            Assert.AreEqual(0, countResult);
        }

        [TestMethod]
        public void TestBulkDeleteWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete<BulkOperationLightIdentityTable>(connection,
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
        public void TestBulkDeleteWithKeepIdentityFalse()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete<BulkOperationLightIdentityTable>(connection,
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
        public void TestBulkDeleteWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete<BulkOperationLightIdentityTable>(connection,
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
        public void TestBulkDeleteWithMappings()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete<BulkOperationMappedIdentityTable>(connection,
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
        public void TestBulkDeleteWithMappingsViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    pseudoTableType: PostgreSqlBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete<BulkOperationUnmatchedIdentityTable>(connection,
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
        public void TestBulkDeleteWithBulkInsertMapItemsViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    pseudoTableType: PostgreSqlBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkDelete<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BulkDelete<Anonymous>

        [TestMethod]
        public void TestBulkDeleteViaAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaAnonymousWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaAnonymousWithKeepIdentityFalse()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaAnonymousWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaAnonymousWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaAnonymousWithBulkInsertMapItemsViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    pseudoTableType: PostgreSqlBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteViaAnonymousOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkDelete(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BulkDelete<IDictionary<string, object>>

        [TestMethod]
        public void TestBulkDeleteViaExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaExpandoObjectWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaExpandoObjectWithKeepIdentityFalse()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaExpandoObjectWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaExpandoObjectWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaExpandoObjectWithBulkInsertMapItemsViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    pseudoTableType: PostgreSqlBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteViaExpandoObjectOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkDelete(connection,
                    tableName,
                    entities: entities);

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BulkDelete<DataTable>

        [TestMethod]
        public void TestBulkDeleteViaDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    table,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaDataTableWithKeepIdentityFalse()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    table,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
                    tableName,
                    table: table);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteViaDataTableWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    table,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaDataTableWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    table,
                    mappings: mappings,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaDataTableWithBulkInsertMapItemsViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    table,
                    mappings: mappings,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDelete(connection,
                    tableName,
                    table: table,
                    mappings: mappings,
                    pseudoTableType: PostgreSqlBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteViaDataTableOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkDelete(connection,
                    tableName,
                    table);

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BulkDelete<DbDataReader>

        [TestMethod]
        public void TestBulkDeleteViaDbDataReader()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BulkInsert(connection,
                        tableName,
                        reader,
                        identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);
                }

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaDbDataReaderWithKeepIdentityFalse()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BulkInsert(connection,
                        tableName,
                        reader,
                        identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);
                }

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaDbDataReaderWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BulkInsert(connection,
                        tableName,
                        reader,
                        identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);
                }

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaDbDataReaderWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BulkInsert(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);
                }

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkDelete(connection,
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
        public void TestBulkDeleteViaDbDataReaderWithBulkInsertMapItemsViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BulkInsert(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);
                }

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkDelete(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        pseudoTableType: PostgreSqlBulkImportPseudoTableType.Physical);

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteViaDbDataReaderOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkDelete(connection,
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

        #region BulkDelete<TEntity>

        [TestMethod]
        public void TestBulkDeleteAsync()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync<BulkOperationLightIdentityTable>(connection,
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
        public async Task TestBulkDeleteAsyncTableNameWithSchema()
        {
            await using var connection = GetConnection();
            
            // Prepare
            var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
            var tableName = "public.BulkOperationIdentityTable";
            
            // Act
            await connection.BulkInsertAsync(tableName,
                entities: entities,
                identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

            // Act
            var result = await connection.BulkDeleteAsync(tableName, entities);

            // Assert
            Assert.AreEqual(entities.Count, result);

            // Assert
            var countResult = await connection.CountAllAsync(tableName);
            Assert.AreEqual(0, countResult);
        }

        [TestMethod]
        public void TestBulkDeleteAsyncWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBulkDeleteAsyncWithKeepIdentityFalse()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBulkDeleteAsyncWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync<BulkOperationLightIdentityTable>(connection,
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
        public void TestBulkDeleteAsyncWithMappings()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync<BulkOperationMappedIdentityTable>(connection,
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
        public void TestBulkDeleteAsyncWithMappingsViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationMappedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync<BulkOperationMappedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    pseudoTableType: PostgreSqlBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteAsyncWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync<BulkOperationUnmatchedIdentityTable>(connection,
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
        public void TestBulkDeleteAsyncWithBulkInsertMapItemsViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync<BulkOperationUnmatchedIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    pseudoTableType: PostgreSqlBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteAsyncOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkDeleteAsync<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BulkDelete<Anonymous>

        [TestMethod]
        public void TestBulkDeleteAsyncViaAnonymous()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaAnonymousWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaAnonymousWithKeepIdentityFalse()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaAnonymousWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaAnonymousWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaAnonymousWithBulkInsertMapItemsViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    pseudoTableType: PostgreSqlBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteAsyncViaAnonymousOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationAnonymousLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BulkDelete<IDictionary<string, object>>

        [TestMethod]
        public void TestBulkDeleteAsyncViaExpandoObject()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaExpandoObjectWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaExpandoObjectWithKeepIdentityFalse()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaExpandoObjectWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaExpandoObjectWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaExpandoObjectWithBulkInsertMapItemsViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
                    tableName,
                    entities: entities,
                    mappings: mappings,
                    pseudoTableType: PostgreSqlBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteAsyncViaExpandoObjectOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationExpandoObjectLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
                    tableName,
                    entities: entities).Result;

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BulkDelete<DataTable>

        [TestMethod]
        public void TestBulkDeleteAsyncViaDataTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    table,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaDataTableWithKeepIdentityFalse()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    table,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
                    tableName,
                    table: table).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteAsyncViaDataTableWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    table,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaDataTableWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    table,
                    mappings: mappings,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaDataTableWithBulkInsertMapItemsViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert(connection,
                    tableName,
                    table,
                    mappings: mappings,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
                    tableName,
                    table: table,
                    mappings: mappings,
                    pseudoTableType: PostgreSqlBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteAsyncViaDataTableOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var table = Helper.ToDataTable(tableName, entities);

                // Act
                var result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
                    tableName,
                    table).Result;

                // Assert
                Assert.AreEqual(0, result);
            }
        }

        #endregion

        #region BulkDelete<DbDataReader>

        [TestMethod]
        public void TestBulkDeleteAsyncViaDbDataReader()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BulkInsert(connection,
                        tableName,
                        reader,
                        identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);
                }

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaDbDataReaderWithKeepIdentityFalse()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BulkInsert(connection,
                        tableName,
                        reader,
                        identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);
                }

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaDbDataReaderWithQualifiers()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BulkInsert(connection,
                        tableName,
                        reader,
                        identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);
                }

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaDbDataReaderWithBulkInsertMapItems()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BulkInsert(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);
                }

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
        public void TestBulkDeleteAsyncViaDbDataReaderWithBulkInsertMapItemsViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationUnmatchedIdentityTables(10, true, 100);
                var tableName = "BulkOperationIdentityTable";
                var mappings = new[]
                {
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.IdMapped), "Id"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBigIntMapped), "ColumnBigInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnBooleanMapped), "ColumnBoolean"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnIntegerMapped), "ColumnInteger"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnNumericMapped), "ColumnNumeric"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnRealMapped), "ColumnReal"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnSmallIntMapped), "ColumnSmallInt"),
                    new PostgreSqlBulkInsertMapItem(nameof(BulkOperationUnmatchedIdentityTable.ColumnTextMapped), "ColumnText")
                };

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    NpgsqlConnectionExtension.BulkInsert(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);
                }

                using (var reader = new DataEntityDataReader<BulkOperationUnmatchedIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
                        tableName,
                        reader,
                        mappings: mappings,
                        pseudoTableType: PostgreSqlBulkImportPseudoTableType.Physical).Result;

                    // Assert
                    Assert.AreEqual(entities.Count(), result);
                }

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteAsyncViaDbDataReaderOnEmptyTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                using (var reader = new DataEntityDataReader<BulkOperationLightIdentityTable>(entities))
                {
                    // Act
                    var result = NpgsqlConnectionExtension.BulkDeleteAsync(connection,
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
