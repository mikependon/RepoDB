using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class BaseRepositoryOperationsTest
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

        private class BulkInsertIdentityTableRepository : BaseRepository<BulkInsertIdentityTable, SqlConnection>
        {
            public BulkInsertIdentityTableRepository() :
                base(Database.ConnectionStringForRepoDb, ConnectionPersistency.Instance)
            { }
        }

        private class WithExtraFieldsBulkInsertIdentityTableRepository : BaseRepository<WithExtraFieldsBulkInsertIdentityTable, SqlConnection>
        {
            public WithExtraFieldsBulkInsertIdentityTableRepository() :
                base(Database.ConnectionStringForRepoDb, ConnectionPersistency.Instance)
            { }
        }

        #endregion

        #region BulkInsert

        [TestMethod]
        public void TestBaseRepositoryBulkInsertForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            using (var repository = new BulkInsertIdentityTableRepository())
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
        public void TestBaseRepositoryBulkInsertForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnBit), nameof(BulkInsertIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime), nameof(BulkInsertIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime2), nameof(BulkInsertIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDecimal), nameof(BulkInsertIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnFloat), nameof(BulkInsertIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnInt), nameof(BulkInsertIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnNVarChar), nameof(BulkInsertIdentityTable.ColumnNVarChar)));

            using (var repository = new BulkInsertIdentityTableRepository())
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
        public void ThrowExceptionOnSqlConnectionBulkInsertForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnBit), nameof(BulkInsertIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime), nameof(BulkInsertIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime2), nameof(BulkInsertIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDecimal), nameof(BulkInsertIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnFloat), nameof(BulkInsertIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnInt), nameof(BulkInsertIdentityTable.ColumnNVarChar)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnNVarChar), nameof(BulkInsertIdentityTable.ColumnInt)));

            using (var repository = new BulkInsertIdentityTableRepository())
            {
                // Act
                repository.BulkInsert(tables, mappings);
            }
        }

        #endregion

        #region BulkInsert(Extra Fields)

        [TestMethod]
        public void TestBaseRepositoryBulkInsertForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkInsertIdentityTables(10);

            using (var withExtraFieldsRepository = new WithExtraFieldsBulkInsertIdentityTableRepository())
            {
                // Act
                var bulkInsertResult = withExtraFieldsRepository.BulkInsert(tables);

                using (var repository = new BulkInsertIdentityTableRepository())
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
        public void TestBaseRepositoryBulkInsertForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkInsertIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnBit), nameof(BulkInsertIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime), nameof(BulkInsertIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime2), nameof(BulkInsertIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDecimal), nameof(BulkInsertIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnFloat), nameof(BulkInsertIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnInt), nameof(BulkInsertIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnNVarChar), nameof(BulkInsertIdentityTable.ColumnNVarChar)));

            using (var withExtraFieldsRepository = new WithExtraFieldsBulkInsertIdentityTableRepository())
            {
                // Act
                var bulkInsertResult = withExtraFieldsRepository.BulkInsert(tables);

                using (var repository = new BulkInsertIdentityTableRepository())
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
        public void TestDbRepositoryBulkInsertForTableNameEntities()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            using (var repository = new BulkInsertIdentityTableRepository())
            {
                // Act
                var bulkInsertResult = repository.BulkInsert(ClassMappedNameCache.Get<BulkInsertIdentityTable>(), tables);

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
        public void TestBaseRepositoryBulkInsertAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            using (var repository = new BulkInsertIdentityTableRepository())
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
        public void TestBaseRepositoryBulkInsertAsyncForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnBit), nameof(BulkInsertIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime), nameof(BulkInsertIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime2), nameof(BulkInsertIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDecimal), nameof(BulkInsertIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnFloat), nameof(BulkInsertIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnInt), nameof(BulkInsertIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnNVarChar), nameof(BulkInsertIdentityTable.ColumnNVarChar)));

            using (var repository = new BulkInsertIdentityTableRepository())
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
        public void ThrowExceptionOnSqlConnectionBulkInsertAsyncForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnBit), nameof(BulkInsertIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime), nameof(BulkInsertIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime2), nameof(BulkInsertIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDecimal), nameof(BulkInsertIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnFloat), nameof(BulkInsertIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnInt), nameof(BulkInsertIdentityTable.ColumnNVarChar)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnNVarChar), nameof(BulkInsertIdentityTable.ColumnInt)));

            using (var repository = new BulkInsertIdentityTableRepository())
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
        public void TestBaseRepositoryBulkInsertAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkInsertIdentityTables(10);

            using (var withExtraFieldsRepository = new WithExtraFieldsBulkInsertIdentityTableRepository())
            {
                // Act
                var bulkInsertResult = withExtraFieldsRepository.BulkInsertAsync(tables).Result;

                using (var repository = new BulkInsertIdentityTableRepository())
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
        public void TestBaseRepositoryBulkInsertAsyncForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkInsertIdentityTables(10);
            var mappings = new List<BulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnBit), nameof(BulkInsertIdentityTable.ColumnBit)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime), nameof(BulkInsertIdentityTable.ColumnDateTime)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDateTime2), nameof(BulkInsertIdentityTable.ColumnDateTime2)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnDecimal), nameof(BulkInsertIdentityTable.ColumnDecimal)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnFloat), nameof(BulkInsertIdentityTable.ColumnFloat)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnInt), nameof(BulkInsertIdentityTable.ColumnInt)));
            mappings.Add(new BulkInsertMapItem(nameof(BulkInsertIdentityTable.ColumnNVarChar), nameof(BulkInsertIdentityTable.ColumnNVarChar)));

            using (var withExtraFieldsRepository = new WithExtraFieldsBulkInsertIdentityTableRepository())
            {
                // Act
                var bulkInsertResult = withExtraFieldsRepository.BulkInsertAsync(tables).Result;

                using (var repository = new BulkInsertIdentityTableRepository())
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
        public void TestDbRepositoryBulkInsertAsyncForTableNameEntities()
        {
            // Setup
            var tables = Helper.CreateBulkInsertIdentityTables(10);

            using (var repository = new BulkInsertIdentityTableRepository())
            {
                // Act
                var bulkInsertResult = repository.BulkInsertAsync(ClassMappedNameCache.Get<BulkInsertIdentityTable>(), tables).Result;

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
