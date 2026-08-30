#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.ClickHouse.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using RepoDb.Enumerations.ClickHouse;
using RepoDb.Exceptions;
using ClickHouse.Driver.ADO;

namespace RepoDb.ClickHouse.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class ClickHouseConnectionBulkMergeOperationsTest
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

        #region BulkMerge<TEntity>

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForEntities()
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
                var bulkMergeResult = connection.BulkMerge(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForEntitiesWithReturnIdentity()
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

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForEntitiesWithQualifiers()
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
                var bulkMergeResult = connection.BulkMerge(tables,
                    qualifiers: e => new { e.RowGuid, e.ColumnInt });

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForEntitiesWithUsePhysicalPseudoTempTable()
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
                var bulkMergeResult = connection.BulkMerge(tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForEntitiesWithMappings()
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
                var bulkMergeResult = connection.BulkMerge(tables, mappings: mappings);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForMappedEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForMappedEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForMappedEntities()
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
                var bulkMergeResult = connection.BulkMerge(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForMappedEntitiesWithReturnIdentity()
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

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForMappedEntitiesWithQualifiers()
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
                var bulkMergeResult = connection.BulkMerge(tables,
                    qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped });

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForMappedEntitiesWithUsePhysicalPseudoTempTable()
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
                var bulkMergeResult = connection.BulkMerge(tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.IdMapped), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.RowGuidMapped), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnBitMapped), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDecimalMapped), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnFloatMapped), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnIntMapped), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables, mappings: mappings);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForEntitiesIfTheMappingsAreInvalid()
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
                Assert.Throws<InvalidTypeException>(() => connection.BulkMerge(tables, mappings: mappings));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForEntitiesDataTable()
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
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForEntitiesDataTableWithReturnIdentity()
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

                            // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                            Assert.Throws<NotSupportedException>(() =>
                                destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForEntitiesDataTableWithMappings()
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
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table,
                                mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForEntitiesDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForNullDataTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<NullReferenceException>(() => connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null));
            }
        }

        #endregion

        #region BulkMerge<TEntity>(Extra Fields)

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForEntitiesWithExtraFields()
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
                var bulkMergeResult = connection.BulkMerge(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForEntitiesWithExtraFieldsWithMappings()
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
                var bulkMergeResult = connection.BulkMerge(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        #endregion

        #region BulkMerge(TableName)

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameExpandoObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == ((dynamic)t).Id); Helper.AssertMembersEquality(item, t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameExpandoObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameExpandoObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count + entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == ((dynamic)t).Id); Helper.AssertMembersEquality(item, t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameExpandoObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameAnonymousObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
                    Helper.AssertMembersEquality(item, t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameAnonymousObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameAnonymousObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert - entities' Ids are freshly generated and never overlap tables', so the merge
                // inserts them as new, unmatched rows alongside the pre-existing tables rows.
                Assert.AreEqual(tables.Count + entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
                    Helper.AssertMembersEquality(item, t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameAnonymousObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameDataEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameDataEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameDataEntitiesForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameDataEntitiesForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    qualifiers: Field.Parse<BulkOperationIdentityTable>(e => new { e.RowGuid, e.ColumnInt }));

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        

        

        

        

        

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameDbDataTable()
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
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameDataTableWithReturnIdentity()
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

                            // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                            Assert.Throws<NotSupportedException>(() =>
                                destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameDbDataTableWithMappings()
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
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForTableNameDbDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForTableNameDbDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("InvalidTable",
                                table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForTableNameDbDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("MissingTable",
                                table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameDataTable()
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
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForTableNameDataTableWithMappings()
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
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForTableNameDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForTableNameDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForTableNameDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("MissingTable",
                                table));
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkMergeAsync<TEntity>

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
                    Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForEntitiesWithQualifiers()
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
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    qualifiers: e => new { e.RowGuid, e.ColumnInt }).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
                    Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForEntitiesWithUsePhysicalPseudoTempTable()
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
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForEntitiesWithMappings()
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
                var bulkMergeResult = connection.BulkMergeAsync(tables, mappings: mappings).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
                    Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForMappedEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForMappedEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForMappedEntities()
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
                var bulkMergeResult = connection.BulkMergeAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped);
                    Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForMappedEntitiesWithReturnIdentity()
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

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForMappedEntitiesWithQualifiers()
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
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped }).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped);
                    Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForMappedEntitiesWithUsePhysicalPseudoTempTable()
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
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.IdMapped), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.RowGuidMapped), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnBitMapped), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDecimalMapped), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnFloatMapped), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnIntMapped), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, mappings: mappings).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForEntitiesIfTheMappingsAreInvalid()
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
                Assert.Throws<AggregateException>(() => connection.BulkMergeAsync(tables,
                    mappings: mappings).Result);
            }
        }

        

        

        

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForEntitiesDataTable()
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
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForEntitiesDataTableWithReturnIdentity()
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

                            // Act - must observe the Task synchronously via .Result (not an async lambda, which
                            // MSTest coerces to async void here - an unhandled exception inside that would crash
                            // the process instead of being caught by Assert.Throws). ClickHouse has no
                            // session-wide scope identity/auto-increment mechanism, so ReturnIdentity always
                            // throws (see ClickHouseConnectionExtension.GuardReturnIdentity) - this asserts that
                            // guard, rather than the merge succeeding.
                            Assert.Throws<AggregateException>(() =>
                                destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForEntitiesDataTableWithMappings()
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
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table,
                                mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table,
                                mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForNullEntities()
        //{
        //    using (var connection = new ClickHouseConnection(Database.ConnectionString))
        //    {
        //        Assert.Throws<AggregateException>(() => connection.BulkInsertAsync((IEnumerable<BulkOperationIdentityTable>)null).Wait();)
        //    }
        //}

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForEmptyEntities()
        //{
        //    using (var connection = new ClickHouseConnection(Database.ConnectionString))
        //    {
        //        Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(Enumerable.Empty<BulkOperationIdentityTable>()).Wait();)
        //    }
        //}

        

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForNullDataTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        #endregion

        #region BulkMergeAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForEntitiesWithExtraFields()
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
                var bulkMergeResult = connection.BulkMergeAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForEntitiesWithExtraFieldsWithMappings()
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
                var bulkMergeResult = connection.BulkMergeAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        #endregion

        #region BulkMergeAsync(TableName)

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameExpandoObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == ((dynamic)t).Id); Helper.AssertMembersEquality(item, t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameExpandoObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameExpandoObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count + entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == ((dynamic)t).Id); Helper.AssertMembersEquality(item, t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameExpandoObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameAnonymousObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
                    Helper.AssertMembersEquality(item, t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameAnonymousObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameAnonymousObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert - entities' Ids are freshly generated and never overlap tables', so the merge
                // inserts them as new, unmatched rows alongside the pre-existing tables rows.
                Assert.AreEqual(tables.Count + entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
                    Helper.AssertMembersEquality(item, t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameAnonymousObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameDataEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameDataEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameDataEntitiesForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameDataEntitiesForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    qualifiers: Field.Parse<BulkOperationIdentityTable>(e => new { e.RowGuid, e.ColumnInt })).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        

        

        

        

        

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameDataTable()
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
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameDataTableWithReturnIdentity()
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

                            // Act - must observe the Task synchronously via .Result (not an async lambda, which
                            // MSTest coerces to async void here - an unhandled exception inside that would crash
                            // the process instead of being caught by Assert.Throws).
                            Assert.Throws<AggregateException>(() =>
                                destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameDataTableWithMappings()
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
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForTableNameDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForTableNameDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForTableNameDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync("MissingTable",
                                table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameDbDataTable()
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
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForTableNameDbDataTableWithMappings()
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
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForTableNameDbDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForTableNameDbDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync("InvalidTable",
                                table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForTableNameDbDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync("MissingTable",
                                table).Result);
                        }
                    }
                }
            }
        }

        #endregion

        #region NonIdentityTable Mirrors

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityEntities()
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
                var bulkMergeResult = connection.BulkMerge(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityEntitiesWithReturnIdentity()
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

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityEntitiesWithQualifiers()
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
                var bulkMergeResult = connection.BulkMerge(tables,
                    qualifiers: e => new { e.RowGuid, e.ColumnInt });

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityEntitiesWithUsePhysicalPseudoTempTable()
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
                var bulkMergeResult = connection.BulkMerge(tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityEntitiesWithMappings()
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
                var bulkMergeResult = connection.BulkMerge(tables, mappings: mappings);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityMappedEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityMappedEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityMappedEntities()
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
                var bulkMergeResult = connection.BulkMerge(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityMappedEntitiesWithReturnIdentity()
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

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityMappedEntitiesWithQualifiers()
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
                var bulkMergeResult = connection.BulkMerge(tables,
                    qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped });

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityMappedEntitiesWithUsePhysicalPseudoTempTable()
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
                var bulkMergeResult = connection.BulkMerge(tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.IdMapped), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.RowGuidMapped), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnBitMapped), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDecimalMapped), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnFloatMapped), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnIntMapped), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables, mappings: mappings);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForNonIdentityEntitiesIfTheMappingsAreInvalid()
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
                Assert.Throws<InvalidTypeException>(() => connection.BulkMerge(tables, mappings: mappings));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityEntitiesDataTable()
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
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityEntitiesDataTableWithReturnIdentity()
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

                            // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                            Assert.Throws<NotSupportedException>(() =>
                                destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityEntitiesDataTableWithMappings()
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
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table,
                                mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForNonIdentityEntitiesDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForNonIdentityNullDataTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<NullReferenceException>(() => connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityEntitiesWithExtraFields()
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
                var bulkMergeResult = connection.BulkMerge(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityEntitiesWithExtraFieldsWithMappings()
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
                var bulkMergeResult = connection.BulkMerge(tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameExpandoObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == ((dynamic)t).Id); Helper.AssertMembersEquality(item, t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameExpandoObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameExpandoObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count + entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == ((dynamic)t).Id); Helper.AssertMembersEquality(item, t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameExpandoObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameAnonymousObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
                    Helper.AssertMembersEquality(item, t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameAnonymousObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameAnonymousObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert - entities' Ids are freshly generated and never overlap tables', so the merge
                // inserts them as new, unmatched rows alongside the pre-existing tables rows.
                Assert.AreEqual(tables.Count + entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
                    Helper.AssertMembersEquality(item, t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameAnonymousObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameDataEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameDataEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameDataEntitiesForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameDataEntitiesForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    tables,
                    qualifiers: Field.Parse<BulkOperationNonIdentityTable>(e => new { e.RowGuid, e.ColumnInt }));

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameDbDataTable()
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
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameDataTableWithReturnIdentity()
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

                            // Act - a sync call throws the guard's NotSupportedException directly, not wrapped.
                            Assert.Throws<NotSupportedException>(() =>
                                destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameDbDataTableWithMappings()
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
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForNonIdentityTableNameDbDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("InvalidTable",
                                table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("MissingTable",
                                table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameDataTable()
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
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForNonIdentityTableNameDataTableWithMappings()
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
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForNonIdentityTableNameDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<InvalidTypeException>(() => destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("MissingTable",
                                table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityEntitiesWithQualifiers()
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
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    qualifiers: e => new { e.RowGuid, e.ColumnInt }).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityEntitiesWithUsePhysicalPseudoTempTable()
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
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityEntitiesWithMappings()
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
                var bulkMergeResult = connection.BulkMergeAsync(tables, mappings: mappings).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityMappedEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityMappedEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityMappedEntities()
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
                var bulkMergeResult = connection.BulkMergeAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityMappedEntitiesWithReturnIdentity()
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

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityMappedEntitiesWithQualifiers()
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
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    qualifiers: e => new { e.RowGuidMapped, e.ColumnIntMapped }).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityMappedEntitiesWithUsePhysicalPseudoTempTable()
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
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<ClickHouseBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.IdMapped), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.RowGuidMapped), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnBitMapped), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDecimalMapped), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnFloatMapped), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnIntMapped), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new ClickHouseBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, mappings: mappings).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.IdMapped == t.IdMapped); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForNonIdentityEntitiesIfTheMappingsAreInvalid()
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
                Assert.Throws<AggregateException>(() => connection.BulkMergeAsync(tables,
                    mappings: mappings).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityEntitiesDataTable()
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
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityEntitiesDataTableWithReturnIdentity()
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

                            // Act - must observe the Task synchronously via .Result (not an async lambda, which
                            // MSTest coerces to async void here - an unhandled exception inside that would crash
                            // the process instead of being caught by Assert.Throws).
                            Assert.Throws<AggregateException>(() =>
                                destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityEntitiesDataTableWithMappings()
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
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table,
                                mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForNonIdentityEntitiesDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table,
                                mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForNonIdentityNullDataTable()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityEntitiesWithExtraFields()
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
                var bulkMergeResult = connection.BulkMergeAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityEntitiesWithExtraFieldsWithMappings()
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
                var bulkMergeResult = connection.BulkMergeAsync(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameExpandoObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == ((dynamic)t).Id); Helper.AssertMembersEquality(item, t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameExpandoObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameExpandoObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count + entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == ((dynamic)t).Id); Helper.AssertMembersEquality(item, t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameExpandoObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameAnonymousObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
                    Helper.AssertMembersEquality(item, t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameAnonymousObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameAnonymousObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert - entities' Ids are freshly generated and never overlap tables', so the merge
                // inserts them as new, unmatched rows alongside the pre-existing tables rows.
                Assert.AreEqual(tables.Count + entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id);
                    Helper.AssertMembersEquality(item, t);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameAnonymousObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act - must observe the Task synchronously via .Result (not an async lambda, which MSTest
                // coerces to async void here - an unhandled exception inside that would crash the process
                // instead of being caught by Assert.Throws).
                Assert.Throws<AggregateException>(() =>
                    connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    tables,
                    qualifiers: Field.Parse<BulkOperationNonIdentityTable>(e => new { e.RowGuid, e.ColumnInt })).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                Helper.SetupAsyncInsert(connection);

                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    tables,
                    pseudoTableType: ClickHouseBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    var item = queryResult.FirstOrDefault(e => e.Id == t.Id); Helper.AssertPropertiesEquality(t, item);
                });
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDataTable()
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
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDataTableWithReturnIdentity()
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

                            // Act - must observe the Task synchronously via .Result (not an async lambda, which
                            // MSTest coerces to async void here - an unhandled exception inside that would crash
                            // the process instead of being caught by Assert.Throws).
                            Assert.Throws<AggregateException>(() =>
                                destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: ClickHouseBulkImportIdentityBehavior.ReturnIdentity).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDataTableWithMappings()
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
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync("MissingTable",
                                table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDbDataTable()
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
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDbDataTableWithMappings()
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
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDbDataTableIfTheMappingsAreInvalid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync("InvalidTable",
                                table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionBulkMergeAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
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
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync("MissingTable",
                                table).Result);
                        }
                    }
                }
            }
        }

        #endregion

        #region BulkMerge(DbDataReader)

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeForDbDataReader()
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
                    var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), reader);

                    // Assert
                    Assert.AreEqual(tables.Count, bulkMergeResult);

                    // Act
                    var countResult = destinationConnection.CountAll<BulkOperationNonIdentityTable>();

                    // Assert
                    Assert.AreEqual(tables.Count, countResult);
                }
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionBulkMergeAsyncForDbDataReader()
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
                    var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), reader).Result;

                    // Assert
                    Assert.AreEqual(tables.Count, bulkMergeResult);

                    // Act
                    var countResult = destinationConnection.CountAll<BulkOperationNonIdentityTable>();

                    // Assert
                    Assert.AreEqual(tables.Count, countResult);
                }
            }
        }

        #endregion

    }
}
