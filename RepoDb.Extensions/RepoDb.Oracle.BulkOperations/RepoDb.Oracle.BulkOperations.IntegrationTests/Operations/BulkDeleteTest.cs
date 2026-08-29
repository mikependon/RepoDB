using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Oracle.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations.Oracle;
using RepoDb.Oracle.BulkOperations;
using System.Linq;
using RepoDb.Exceptions;

namespace RepoDb.Oracle.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class OracleConnectionBulkDeleteOperationsTest
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
        public void TestOracleConnectionBulkDeleteForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables,
                    pseudoTableType: OracleBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteForEntitiesWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForEntitiesOnEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var bulkDeleteResult = connection.BulkDelete(tables);

                // Assert
                Assert.AreEqual(0, bulkDeleteResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables,
                    pseudoTableType: OracleBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.IdMapped), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnBitMapped), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDecimalMapped), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnFloatMapped), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnIntMapped), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnOracleConnectionBulkDeleteForNullEntities()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkDelete((IEnumerable<BulkOperationIdentityTable>)null));
            }
        }

        //[TestMethod, ExpectedException(typeof(EmptyException))]
        //public void ThrowExceptionOnOracleConnectionBulkDeleteForEmptyEntities()
        //{
        //    using (var connection = new OracleConnection(Database.ConnectionString))
        //    {
        //        connection.BulkDelete(Enumerable.Empty<BulkOperationIdentityTable>());
        //    }
        //}

        [TestMethod]
        public void ThrowExceptionOnOracleConnectionBulkDeleteForNullDataTable()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null));
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteForEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForTableNameEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForTableNameExpandoObjects()
        {
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForTableNameExpandoObjectsOnEmptyTable()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForTableNameAnonymousObjects()
        {
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForTableNameAnonymousObjectsOnEmptyTable()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    entities: tables,
                    pseudoTableType: OracleBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteForTableNameDataEntitiesOnEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(0, bulkDeleteResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteForTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnOracleConnectionBulkDeleteForTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkDelete("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnOracleConnectionBulkDeleteForTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnOracleConnectionBulkDeleteForTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkDelete("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnOracleConnectionBulkDeleteForTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables,
                    pseudoTableType: OracleBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteAsyncForEntitiesWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForEntitiesOnEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(0, bulkDeleteResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteAsyncForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables,
                    pseudoTableType: OracleBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteAsyncForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.IdMapped), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnBitMapped), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDecimalMapped), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnFloatMapped), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnIntMapped), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnOracleConnectionBulkDeleteAsyncForNullEntities()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkDeleteAsync((IEnumerable<BulkOperationIdentityTable>)null).Wait());
            }
        }

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnOracleConnectionBulkDeleteAsyncForEmptyEntities()
        //{
        //    using (var connection = new OracleConnection(Database.ConnectionString))
        //    {
        //        connection.BulkDeleteAsync(Enumerable.Empty<BulkOperationIdentityTable>()).Wait();
        //    }
        //}

        

        [TestMethod]
        public void ThrowExceptionOnOracleConnectionBulkDeleteAsyncForNullDataTable()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        #endregion

        #region BulkDeleteAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestOracleConnectionBulkDeleteAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForTableNameEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForTableNameExpandoObjects()
        {
            // Setup - see TestOracleConnectionBulkDeleteForTableNameAnonymousObjects for why
            // BulkOperationNonIdentityTable is used here instead of the IDENTITY table.
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForTableNameExpandoObjectsOnEmptyTable()
        {
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForTableNameAnonymousObjects()
        {
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForTableNameAnonymousObjectsOnEmptyTable()
        {
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    entities: tables,
                    pseudoTableType: OracleBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        

        

        

        

        

        [TestMethod]
        public void TestOracleConnectionBulkDeleteAsyncForTableNameDataEntitiesOnEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(0, bulkDeleteResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteAsyncForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnOracleConnectionBulkDeleteAsyncForTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkDeleteAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnOracleConnectionBulkDeleteAsyncForTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnOracleConnectionBulkDeleteAsyncForTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkDeleteAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnOracleConnectionBulkDeleteAsyncForTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables,
                    pseudoTableType: OracleBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteForNonIdentityEntitiesWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityEntitiesOnEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var bulkDeleteResult = connection.BulkDelete(tables);

                // Assert
                Assert.AreEqual(0, bulkDeleteResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteForNonIdentityEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(tables,
                    pseudoTableType: OracleBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.IdMapped), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnBitMapped), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDecimalMapped), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnFloatMapped), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnIntMapped), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnOracleConnectionBulkDeleteForNonIdentityNullEntities()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkDelete((IEnumerable<BulkOperationNonIdentityTable>)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnOracleConnectionBulkDeleteForNonIdentityNullDataTable()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null));
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteForNonIdentityEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityTableNameEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    entities: tables,
                    pseudoTableType: OracleBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteForNonIdentityTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnOracleConnectionBulkDeleteForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkDelete("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnOracleConnectionBulkDeleteForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForNonIdentityTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnOracleConnectionBulkDeleteForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkDelete("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnOracleConnectionBulkDeleteForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables,
                    pseudoTableType: OracleBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityEntitiesWithBatchSize()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityEntitiesOnEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables).Result;

                // Assert
                Assert.AreEqual(0, bulkDeleteResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10).AsList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(tables,
                    pseudoTableType: OracleBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.IdMapped), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnBitMapped), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDecimalMapped), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnFloatMapped), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnIntMapped), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnOracleConnectionBulkDeleteAsyncForNonIdentityNullEntities()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkDeleteAsync((IEnumerable<BulkOperationNonIdentityTable>)null).Wait());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnOracleConnectionBulkDeleteAsyncForNonIdentityNullDataTable()
        {
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityTableNameEntitiesViaPrimaryKeys()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var bulkDeleteResult = connection.BulkDeleteAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    entities: tables,
                    pseudoTableType: OracleBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkDeleteResult);

                // Act
                var countResult = connection.CountAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(0, countResult);
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnOracleConnectionBulkDeleteAsyncForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkDeleteAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnOracleConnectionBulkDeleteAsyncForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForNonIdentityTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<OracleBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new OracleBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnOracleConnectionBulkDeleteAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkDeleteAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnOracleConnectionBulkDeleteAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                sourceConnection.InsertAll(tables);

                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteAsyncForDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                sourceConnection.InsertAll(tables);

                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
        public void TestOracleConnectionBulkDeleteForDbDataReaderOnEmptyTable()
        {
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                using (var destinationConnection = new OracleConnection(Database.ConnectionString))
                {
                    // Act
                    var bulkDeleteResult = destinationConnection.BulkDelete(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), reader);

                    // Assert
                    Assert.AreEqual(0, bulkDeleteResult);
                }
            }
        }

        [TestMethod]
        public void TestOracleConnectionBulkDeleteAsyncForDbDataReaderOnEmptyTable()
        {
            using (var sourceConnection = new OracleConnection(Database.ConnectionString))
            {
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                using (var destinationConnection = new OracleConnection(Database.ConnectionString))
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
