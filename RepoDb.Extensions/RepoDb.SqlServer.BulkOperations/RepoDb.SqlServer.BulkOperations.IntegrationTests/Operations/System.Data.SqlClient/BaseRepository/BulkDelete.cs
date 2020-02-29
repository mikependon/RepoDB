using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Setup;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class SystemSqlConnectionBaseRepositoryBulkDeleteOperationsTest
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

        #region SubClass

        private class BulkOperationIdentityTableRepository : BaseRepository<BulkOperationIdentityTable, SqlConnection>
        {
            public BulkOperationIdentityTableRepository() :
                base(Database.ConnectionStringForRepoDb, ConnectionPersistency.Instance)
            { }
        }

        private class WithExtraFieldsBulkOperationIdentityTableRepository : BaseRepository<WithExtraFieldsBulkOperationIdentityTable, SqlConnection>
        {
            public WithExtraFieldsBulkOperationIdentityTableRepository() :
                base(Database.ConnectionStringForRepoDb, ConnectionPersistency.Instance)
            { }
        }

        #endregion

        #region BulkDelete

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkDeleteForEntitiesWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => (object)e.Id);

                // Act
                var bulkDeleteResult = repository.BulkDelete(primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkDeleteForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkDeleteForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDelete(tables,
                    qualifiers: Field.From("RowGuid", "ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkDeleteForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDelete(tables,
                    usePhysicalPseudoTempTable: true);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkDeleteForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSystemSqlConnectionBaseRepositoryBulkDeleteForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.BulkDelete(tables, mappings: mappings);
            }
        }

        #endregion

        #region BulkDelete(Extra Fields)

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkDeleteForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var withExtraFieldsRepository = new WithExtraFieldsBulkOperationIdentityTableRepository())
            {
                // Act
                withExtraFieldsRepository.InsertAll(tables);

                // Act
                var bulkDeleteResult = withExtraFieldsRepository.BulkDelete(tables);

                using (var repository = new BulkOperationIdentityTableRepository())
                {
                    // Act
                    var queryResult = repository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, bulkDeleteResult);

                    // Act
                    var countResult = repository.CountAll();

                    // Assert
                    Assert.AreEqual(0, countResult);
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkDeleteForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var withExtraFieldsRepository = new WithExtraFieldsBulkOperationIdentityTableRepository())
            {
                // Act
                withExtraFieldsRepository.InsertAll(tables);

                // Act
                var bulkDeleteResult = withExtraFieldsRepository.BulkDelete(tables);

                using (var repository = new BulkOperationIdentityTableRepository())
                {
                    // Act
                    var queryResult = repository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, bulkDeleteResult);

                    // Act
                    var countResult = repository.CountAll();

                    // Assert
                    Assert.AreEqual(0, countResult);
                }
            }
        }

        #endregion

        #region BulkDelete(TableName)

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForTableNameEntitiesWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => (object)e.Id);

                // Act
                var bulkDeleteResult = repository.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForTableNameEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForTableNameEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    qualifiers: Field.From("RowGuid", "ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteForTableNameEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    usePhysicalPseudoTempTable: true);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region BulkDeleteAsync

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkDeleteAsyncForEntitiesWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => (object)e.Id);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(primaryKeys).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkDeleteAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkDeleteAsyncForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(tables,
                    qualifiers: Field.From("RowGuid", "ColumnInt")).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkDeleteAsyncForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(tables,
                    usePhysicalPseudoTempTable: true).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkDeleteAsyncForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSystemSqlConnectionBaseRepositoryBulkDeleteAsyncForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(tables, mappings: mappings);
                bulkDeleteResult.Wait();

                // Trigger
                var result = bulkDeleteResult.Result;
            }
        }

        #endregion

        #region BulkDeleteAsync(Extra Fields)

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkDeleteAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var withExtraFieldsRepository = new WithExtraFieldsBulkOperationIdentityTableRepository())
            {
                // Act
                withExtraFieldsRepository.InsertAll(tables);

                // Act
                var bulkDeleteResult = withExtraFieldsRepository.BulkDeleteAsync(tables).Result;

                using (var repository = new BulkOperationIdentityTableRepository())
                {
                    // Act
                    var queryResult = repository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, bulkDeleteResult);

                    // Act
                    var countResult = repository.CountAll();

                    // Assert
                    Assert.AreEqual(0, countResult);
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkDeleteAsyncForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var withExtraFieldsRepository = new WithExtraFieldsBulkOperationIdentityTableRepository())
            {
                // Act
                withExtraFieldsRepository.InsertAll(tables);

                // Act
                var bulkDeleteResult = withExtraFieldsRepository.BulkDeleteAsync(tables).Result;

                using (var repository = new BulkOperationIdentityTableRepository())
                {
                    // Act
                    var queryResult = repository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, bulkDeleteResult);

                    // Act
                    var countResult = repository.CountAll();

                    // Assert
                    Assert.AreEqual(0, countResult);
                }
            }
        }

        #endregion

        #region BulkDeleteAsync(TableName)

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForTableNameEntitiesWithPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => (object)e.Id);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForTableNameEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForTableNameEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    qualifiers: Field.From("RowGuid", "ColumnInt")).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkDeleteAsyncForTableNameEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var bulkDeleteResult = repository.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    usePhysicalPseudoTempTable: true).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = repository.CountAll();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion
    }
}
