#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using System.Linq;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class BulkDeleteByKeyTest
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

        [TestMethod]
        public void TestBulkDeleteByKey()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var primaryKeys = entities.Select(entity => entity.Id);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteByKey(connection,
                    tableName,
                    primaryKeys: primaryKeys);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteByKeyWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var primaryKeys = entities.Select(entity => entity.Id);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteByKey(connection,
                    tableName,
                    primaryKeys: primaryKeys,
                    batchSize: 3);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteByKeyViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var primaryKeys = entities.Select(entity => entity.Id);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteByKey(connection,
                    tableName,
                    primaryKeys: primaryKeys,
                    pseudoTableType: PostgreSqlBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestBulkDeleteByKeyAsync()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var primaryKeys = entities.Select(entity => entity.Id);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteByKeyAsync(connection,
                    tableName,
                    primaryKeys: primaryKeys).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteByKeyAsyncWithBatchSize()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var primaryKeys = entities.Select(entity => entity.Id);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteByKeyAsync(connection,
                    tableName,
                    primaryKeys: primaryKeys,
                    batchSize: 3).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestBulkDeleteByKeyAsyncViaPhysicalTable()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var primaryKeys = entities.Select(entity => entity.Id);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BulkInsert<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: PostgreSqlBulkImportIdentityBehavior.KeepIdentity);

                // Act
                result = NpgsqlConnectionExtension.BulkDeleteByKeyAsync(connection,
                    tableName,
                    primaryKeys: primaryKeys,
                    pseudoTableType: PostgreSqlBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var countResult = connection.CountAll(tableName);
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion
    }
}
