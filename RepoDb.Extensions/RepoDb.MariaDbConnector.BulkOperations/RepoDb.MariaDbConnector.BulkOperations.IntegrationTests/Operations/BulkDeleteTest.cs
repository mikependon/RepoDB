using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.MariaDbConnector;
using RepoDb.Enumerations.MariaDb;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.MariaDbConnector.BulkOperations.IntegrationTests.Models;

namespace RepoDb.MariaDbConnector.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class MariaDbConnectionBulkDeleteOperationsTest
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

        #region BulkDelete<TEntity>

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables,
                    qualifiers: e => new { e.RowGuid, e.ColumnInt });

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables,
                    pseudoTableType: MariaDbBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForEntitiesWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables,
                    batchSize: 3);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForEntitiesOnEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var bulkDeleteResult = connection.BulkDelete(tables);

                // Assert
                Assert.AreEqual(0, bulkDeleteResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables,
                    qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped });

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables,
                    pseudoTableType: MariaDbBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.IdMapped), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnBitMapped), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDecimalMapped), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnFloatMapped), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnIntMapped), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, null, DataRowState.Unchanged);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteForNullEntities()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkDelete((IEnumerable<BulkOperationIdentityTable>)null));
            }
        }

        //[TestMethod, ExpectedException(typeof(EmptyException))]
        //public void ThrowExceptionOnMariaDbConnectionBulkDeleteForEmptyEntities()
        //{
        //    using (var connection = new MariaDbConnection(Database.ConnectionString))
        //    {
        //        connection.BulkDelete(Enumerable.Empty<BulkOperationIdentityTable>());
        //    }
        //}

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteForNullDataTable()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null));
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => (object)e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKey(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region BulkDelete<TEntity>(Extra Fields)

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region BulkDelete(TableName)

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForTableNameEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => (object)e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKey(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForTableNameExpandoObjects()
        {
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

                // Act
                var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForTableNameExpandoObjectsOnEmptyTable()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

                // Act
                var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(0, bulkDeleteResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForTableNameAnonymousObjects()
        {
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

                // Act
                var bulkDeleteResult = connection.BulkDelete<object>(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForTableNameAnonymousObjectsOnEmptyTable()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

                // Act
                var bulkDeleteResult = connection.BulkDelete<object>(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(0, bulkDeleteResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    entities: tables,
                    qualifiers: Field.Parse<BulkOperationIdentityTable>(e => new { e.RowGuid, e.ColumnInt }));

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    entities: tables,
                    pseudoTableType: MariaDbBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForTableNameDataEntitiesOnEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(0, bulkDeleteResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                null,
                                DataRowState.Unchanged);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteForTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkDelete("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteForTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkDelete("MissingTable",
                                table,
                                null,
                                DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                null,
                                DataRowState.Unchanged);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteForTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkDelete("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteForTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkDelete("MissingTable",
                                table,
                                null,
                                DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkDeleteAsync<TEntity>

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => (object)e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKeyAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), primaryKeys).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables,
                    qualifiers: e => new { e.RowGuid, e.ColumnInt }).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables,
                    pseudoTableType: MariaDbBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForEntitiesWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables,
                    batchSize: 3).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForEntitiesOnEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(0, bulkDeleteResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables,
                    qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped }).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables,
                    pseudoTableType: MariaDbBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.IdMapped), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnBitMapped), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDecimalMapped), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnFloatMapped), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnIntMapped), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                null,
                                DataRowState.Unchanged).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteAsyncForNullEntities()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkDeleteAsync((IEnumerable<BulkOperationIdentityTable>)null).Wait());
            }
        }

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnMariaDbConnectionBulkDeleteAsyncForEmptyEntities()
        //{
        //    using (var connection = new MariaDbConnection(Database.ConnectionString))
        //    {
        //        connection.BulkDeleteAsync(Enumerable.Empty<BulkOperationIdentityTable>()).Wait();
        //    }
        //}

        

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteAsyncForNullDataTable()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        #endregion

        #region BulkDeleteAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        #endregion

        #region BulkDeleteAsync(TableName)

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForTableNameEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => (object)e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKeyAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), primaryKeys).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForTableNameExpandoObjects()
        {
            // Setup - see TestMariaDbConnectionBulkDeleteForTableNameAnonymousObjects for why
            // BulkOperationNonIdentityTable is used here instead of the IDENTITY table.
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForTableNameExpandoObjectsOnEmptyTable()
        {
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(0, bulkDeleteResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForTableNameAnonymousObjects()
        {
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync<object>(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForTableNameAnonymousObjectsOnEmptyTable()
        {
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync<object>(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(0, bulkDeleteResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    entities: tables,
                    qualifiers: Field.Parse<BulkOperationIdentityTable>(e => new { e.RowGuid, e.ColumnInt })).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    entities: tables,
                    pseudoTableType: MariaDbBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        

        

        

        

        

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForTableNameDataEntitiesOnEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(0, bulkDeleteResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                null,
                                DataRowState.Unchanged).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteAsyncForTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkDeleteAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteAsyncForTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkDeleteAsync("MissingTable",
                                table,
                                null,
                                DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                null,
                                DataRowState.Unchanged).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteAsyncForTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkDeleteAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteAsyncForTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkDeleteAsync("MissingTable",
                                table,
                                null,
                                DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        #endregion

        #region NonIdentityTable Mirrors

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => (object)e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKey(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables,
                    qualifiers: e => new { e.RowGuid, e.ColumnInt });

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables,
                    pseudoTableType: MariaDbBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityEntitiesWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables,
                    batchSize: 3);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityEntitiesOnEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var bulkDeleteResult = connection.BulkDelete(tables);

                // Assert
                Assert.AreEqual(0, bulkDeleteResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables,
                    qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped });

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables,
                    pseudoTableType: MariaDbBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.IdMapped), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnBitMapped), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDecimalMapped), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnFloatMapped), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnIntMapped), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, null, DataRowState.Unchanged);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteForNonIdentityNullEntities()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkDelete((IEnumerable<BulkOperationNonIdentityTable>)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteForNonIdentityNullDataTable()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null));
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityTableNameEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => (object)e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKey(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), primaryKeys);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    entities: tables,
                    qualifiers: Field.Parse<BulkOperationNonIdentityTable>(e => new { e.RowGuid, e.ColumnInt }));

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    entities: tables,
                    pseudoTableType: MariaDbBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                null,
                                DataRowState.Unchanged);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkDelete("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkDelete("MissingTable",
                                table,
                                null,
                                DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForNonIdentityTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                null,
                                DataRowState.Unchanged);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkDelete("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkDelete("MissingTable",
                                table,
                                null,
                                DataRowState.Unchanged));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => (object)e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKeyAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), primaryKeys).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables,
                    qualifiers: e => new { e.RowGuid, e.ColumnInt }).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables,
                    pseudoTableType: MariaDbBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityEntitiesWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables,
                    batchSize: 3).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityEntitiesOnEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(0, bulkDeleteResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables,
                    qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped }).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10).AsList();

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables,
                    pseudoTableType: MariaDbBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.IdMapped), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnBitMapped), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDecimalMapped), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnFloatMapped), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnIntMapped), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                null,
                                DataRowState.Unchanged).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteAsyncForNonIdentityNullEntities()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkDeleteAsync((IEnumerable<BulkOperationNonIdentityTable>)null).Wait());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteAsyncForNonIdentityNullDataTable()
        {
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityTableNameEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var primaryKeys = tables.Select(e => (object)e.Id);

                // Act
                var bulkDeleteResult = connection.BulkDeleteByKeyAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), primaryKeys).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    entities: tables,
                    qualifiers: Field.Parse<BulkOperationNonIdentityTable>(e => new { e.RowGuid, e.ColumnInt })).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    entities: tables,
                    pseudoTableType: MariaDbBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                null,
                                DataRowState.Unchanged).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteAsyncForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkDeleteAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteAsyncForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkDeleteAsync("MissingTable",
                                table,
                                null,
                                DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForNonIdentityTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<MariaDbBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new MariaDbBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkDeleteResult = destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                null,
                                DataRowState.Unchanged).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkDeleteResult);

                            // Act
                            var countResult = destinationConnection.CountAll<BulkOperationNonIdentityTable>();

                            // Assert
                            Assert.AreEqual(0, countResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkDeleteAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionBulkDeleteAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkDeleteAsync("MissingTable",
                                table,
                                null,
                                DataRowState.Unchanged).Result);
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkDelete(DbDataReader)

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                sourceConnection.InsertAll(tables);

                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                {
                    // Act
                    var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), reader);

                    // Assert
                    Assert.AreEqual(tables.Count, bulkDeleteResult);

                    // Act
                    var countResult = destinationConnection.CountAll<BulkOperationNonIdentityTable>();

                    // Assert
                    Assert.AreEqual(0, countResult);
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                sourceConnection.InsertAll(tables);

                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                {
                    // Act
                    var bulkDeleteResult = destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), reader).Result;

                    // Assert
                    Assert.AreEqual(tables.Count, bulkDeleteResult);

                    // Act
                    var countResult = destinationConnection.CountAll<BulkOperationNonIdentityTable>();

                    // Assert
                    Assert.AreEqual(0, countResult);
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteForDbDataReaderOnEmptyTable()
        {
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                {
                    // Act
                    var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), reader);

                    // Assert
                    Assert.AreEqual(0, bulkDeleteResult);
                }
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionBulkDeleteAsyncForDbDataReaderOnEmptyTable()
        {
            using (var sourceConnection = new MariaDbConnection(Database.ConnectionString))
            {
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                using (var destinationConnection = new MariaDbConnection(Database.ConnectionString))
                {
                    // Act
                    var bulkDeleteResult = destinationConnection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), reader).Result;

                    // Assert
                    Assert.AreEqual(0, bulkDeleteResult);
                }
            }
        }

        #endregion

    }
}
