using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.ClickHouse.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Data;
using RepoDb.Enumerations.ClickHouse;
using System.Linq;
using ClickHouse.Driver.ADO;

namespace RepoDb.ClickHouse.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class ClickHouseConnectionBulkUpdateOperationsTest
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

        #region BulkUpdate<TEntity>

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables,
                    qualifiers: e => new { e.RowGuid, e.ColumnInt });

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables, mappings: mappings);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables,
                    qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped });

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables, mappings: mappings);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                Assert.Throws<InvalidTypeException>(() => connection.BulkUpdate(tables, mappings: mappings));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table,
                                mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForNullEntities()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<NullReferenceException>(() => connection.BulkUpdate((IEnumerable<BulkOperationIdentityTable>)null));
            }
        }

        //[TestMethod, ExpectedException(typeof(EmptyException))]
        //public void ThrowExceptionOnClickHouseConnectionBulkUpdateForEmptyEntities()
        //{
        //    using (var connection = new ClickHouseConnection(Database.ConnectionString))
        //    {
        //        connection.BulkUpdate(Enumerable.Empty<BulkOperationIdentityTable>());
        //    }
        //}

        

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForNullDataTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<NullReferenceException>(() => connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null));
            }
        }

        #endregion

        #region BulkUpdate<TEntity>(Extra Fields)

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateWithExtraFieldsBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateWithExtraFieldsBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        #endregion

        #region BulkUpdate(TableName)

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(t, queryResult.ElementAt(entities.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(t, queryResult.ElementAt((int)entities.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    qualifiers: Field.Parse<BulkOperationIdentityTable>(e => new { e.RowGuid, e.ColumnInt }));

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        

        

        

        

        

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkUpdate("InvalidTable",
                                table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkUpdate("MissingTable",
                                table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkUpdate("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkUpdate("MissingTable",
                                table));
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkUpdateAsync<TEntity>

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables,
                    qualifiers: e => new { e.RowGuid, e.ColumnInt }).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables, mappings: mappings).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables,
                    qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped }).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables, mappings: mappings).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                Assert.Throws<AggregateException>(() => connection.BulkUpdateAsync(tables,
                    mappings: mappings).Result);
            }
        }

        

        

        

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table,
                                mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table,
                                mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForNullEntities()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<AggregateException>(() => connection.BulkUpdateAsync((IEnumerable<BulkOperationIdentityTable>)null).Wait());
            }
        }

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForEmptyEntities()
        //{
        //    using (var connection = new ClickHouseConnection(Database.ConnectionString))
        //    {
        //        Assert.Throws<AggregateException>(() => connection.BulkUpdateAsync(Enumerable.Empty<BulkOperationIdentityTable>()).Wait();)
        //    }
        //}

        

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForNullDataTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<AggregateException>(() => connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        #endregion

        #region BulkUpdateAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateWithExtraFieldsBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateWithExtraFieldsBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        #endregion

        #region BulkUpdateAsync(TableName)

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(t, queryResult.ElementAt(entities.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(t, queryResult.ElementAt((int)entities.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    qualifiers: Field.Parse<BulkOperationIdentityTable>(e => new { e.RowGuid, e.ColumnInt })).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        

        

        

        

        

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync("MissingTable",
                                table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync("InvalidTable",
                                table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync("MissingTable",
                                table).Result);
                        }
                    }
                }
            }
        }

        #endregion

        #region NonIdentityTable Mirrors

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables,
                    qualifiers: e => new { e.RowGuid, e.ColumnInt });

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables, mappings: mappings);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables,
                    qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped });

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables, mappings: mappings);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForNonIdentityEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                Assert.Throws<InvalidTypeException>(() => connection.BulkUpdate(tables, mappings: mappings));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table,
                                mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForNonIdentityEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForNonIdentityNullEntities()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<NullReferenceException>(() => connection.BulkUpdate((IEnumerable<BulkOperationNonIdentityTable>)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForNonIdentityNullDataTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<NullReferenceException>(() => connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateWithExtraFieldsBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateWithExtraFieldsBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(t, queryResult.ElementAt(entities.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(t, queryResult.ElementAt((int)entities.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    tables,
                    qualifiers: Field.Parse<BulkOperationNonIdentityTable>(e => new { e.RowGuid, e.ColumnInt }));

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForNonIdentityTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkUpdate("InvalidTable",
                                table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkUpdate("MissingTable",
                                table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForNonIdentityTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForNonIdentityTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkUpdate("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkUpdate("MissingTable",
                                table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables,
                    qualifiers: e => new { e.RowGuid, e.ColumnInt }).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables, mappings: mappings).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables,
                    qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped }).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables, mappings: mappings).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForNonIdentityEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                Assert.Throws<AggregateException>(() => connection.BulkUpdateAsync(tables,
                    mappings: mappings).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table,
                                mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForNonIdentityEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table,
                                mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForNonIdentityNullEntities()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<AggregateException>(() => connection.BulkUpdateAsync((IEnumerable<BulkOperationNonIdentityTable>)null).Wait());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForNonIdentityNullDataTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<AggregateException>(() => connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateWithExtraFieldsBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateWithExtraFieldsBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityTableNameExpandoObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(t, queryResult.ElementAt(entities.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityTableNameAnonymousObjects()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(t, queryResult.ElementAt((int)entities.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    tables,
                    qualifiers: Field.Parse<BulkOperationNonIdentityTable>(e => new { e.RowGuid, e.ColumnInt })).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkUpdateResult = connection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkUpdateResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForNonIdentityTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync("MissingTable",
                                table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForNonIdentityTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkUpdateResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForNonIdentityTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync("InvalidTable",
                                table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkUpdateAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                        {
                            // Setup
                            Helper.SetupAsyncInsert(destinationConnection);

                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkUpdateAsync("MissingTable",
                                table).Result);
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkUpdate(DbDataReader)

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateForDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                sourceConnection.InsertAll(tables);

                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                {
                    // Setup
                    Helper.SetupAsyncInsert(destinationConnection);

                    // Act
                    var bulkUpdateResult = destinationConnection.BulkUpdate(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), reader);

                    // Assert
                    Assert.AreEqual(tables.Count, bulkUpdateResult);
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkUpdateAsyncForDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var sourceConnection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(sourceConnection);

                sourceConnection.InsertAll(tables);

                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM `BulkOperationNonIdentityTable`"))
                using (var destinationConnection = new ClickHouseConnection(Database.ConnectionString))
                {
                    // Setup
                    Helper.SetupAsyncInsert(destinationConnection);

                    // Act
                    var bulkUpdateResult = destinationConnection.BulkUpdateAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), reader).Result;

                    // Assert
                    Assert.AreEqual(tables.Count, bulkUpdateResult);
                }
            }
        }

        #endregion

    }
}
