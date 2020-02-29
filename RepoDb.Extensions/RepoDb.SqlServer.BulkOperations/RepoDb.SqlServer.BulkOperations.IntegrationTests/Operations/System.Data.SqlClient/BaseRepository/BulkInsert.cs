using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class SystemSqlConnectionBaseRepositoryBulkInsertOperationsTest
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

        #region BulkInsert

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkInsertForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                var bulkInsertResult = repository.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkInsertForEntitiesWithMappings()
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
                var bulkInsertResult = repository.BulkInsert(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSystemSqlConnectionBaseRepositoryBulkInsertForEntitiesIfTheMappingsAreInvalid()
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
                repository.BulkInsert(tables, mappings);
            }
        }

        #endregion

        #region BulkInsert(Extra Fields)

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkInsertForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var withExtraFieldsRepository = new WithExtraFieldsBulkOperationIdentityTableRepository())
            {
                // Act
                var bulkInsertResult = withExtraFieldsRepository.BulkInsert(tables);

                using (var repository = new BulkOperationIdentityTableRepository())
                {
                    // Act
                    var queryResult = repository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, bulkInsertResult);
                    Assert.AreEqual(tables.Count, queryResult.Count());
                    tables.AsList().ForEach(t =>
                    {
                        Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                    });
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkInsertForEntitiesWithExtraFieldsWithMappings()
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
                var bulkInsertResult = withExtraFieldsRepository.BulkInsert(tables);

                using (var repository = new BulkOperationIdentityTableRepository())
                {
                    // Act
                    var queryResult = repository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, bulkInsertResult);
                    Assert.AreEqual(tables.Count, queryResult.Count());
                    tables.AsList().ForEach(t =>
                    {
                        Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                    });
                }
            }
        }

        #endregion

        #region BulkInsert(TableName)

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkInsertForTableNameEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                var bulkInsertResult = repository.BulkInsert(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        #endregion

        #region BulkInsertAsync

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkInsertAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                var bulkInsertResult = repository.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkInsertAsyncForEntitiesWithMappings()
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
                var bulkInsertResult = repository.BulkInsertAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSystemSqlConnectionBaseRepositoryBulkInsertAsyncForEntitiesIfTheMappingsAreInvalid()
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
                var bulkInsertResult = repository.BulkInsertAsync(tables, mappings);
                bulkInsertResult.Wait();

                // Trigger
                var result = bulkInsertResult.Result;
            }
        }

        #endregion

        #region BulkInsertAsync(Extra Fields)

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkInsertAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var withExtraFieldsRepository = new WithExtraFieldsBulkOperationIdentityTableRepository())
            {
                // Act
                var bulkInsertResult = withExtraFieldsRepository.BulkInsertAsync(tables).Result;

                using (var repository = new BulkOperationIdentityTableRepository())
                {
                    // Act
                    var queryResult = repository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, bulkInsertResult);
                    Assert.AreEqual(tables.Count, queryResult.Count());
                    tables.AsList().ForEach(t =>
                    {
                        Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                    });
                }
            }
        }

        [TestMethod]
        public void TestSystemSqlConnectionBaseRepositoryBulkInsertAsyncForEntitiesWithExtraFieldsWithMappings()
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
                var bulkInsertResult = withExtraFieldsRepository.BulkInsertAsync(tables).Result;

                using (var repository = new BulkOperationIdentityTableRepository())
                {
                    // Act
                    var queryResult = repository.QueryAll();

                    // Assert
                    Assert.AreEqual(tables.Count, bulkInsertResult);
                    Assert.AreEqual(tables.Count, queryResult.Count());
                    tables.AsList().ForEach(t =>
                    {
                        Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                    });
                }
            }
        }

        #endregion

        #region BulkInsertAsync(TableName)

        [TestMethod]
        public void TestSystemSqlConnectionDbRepositoryBulkInsertAsyncForTableNameEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var repository = new BulkOperationIdentityTableRepository())
            {
                // Act
                var bulkInsertResult = repository.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkInsertResult);

                // Act
                var queryResult = repository.QueryAll();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        #endregion
    }
}
