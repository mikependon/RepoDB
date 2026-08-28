using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Vertica.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Vertica.Data.VerticaClient;
using RepoDb.Enumerations.Vertica;
using RepoDb.Vertica.BulkOperations;
using RepoDb.Exceptions;

namespace RepoDb.Vertica.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class VerticaConnectionBulkMergeOperationsTest
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
        public void TestVerticaConnectionBulkMergeForEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

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
        public void TestVerticaConnectionBulkMergeForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

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
        public void TestVerticaConnectionBulkMergeForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables,
                    pseudoTableType: VerticaBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

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
        public void TestVerticaConnectionBulkMergeForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForMappedEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForMappedEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.IdMapped <= 0));

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
        public void TestVerticaConnectionBulkMergeForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.IdMapped <= 0));

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
        public void TestVerticaConnectionBulkMergeForMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables,
                    pseudoTableType: VerticaBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

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
        public void TestVerticaConnectionBulkMergeForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.IdMapped), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.RowGuidMapped), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnBitMapped), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDecimalMapped), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnFloatMapped), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnIntMapped), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<FormatException>(() => connection.BulkMerge(tables, mappings: mappings));
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void TestVerticaConnectionBulkMergeForEntitiesDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            var rows = table.Rows.OfType<DataRow>();
                            queryResult.AsList().ForEach(item =>
                            {
                                var row = rows.Where(r => Equals(item.Id, r["Id"]));
                                Assert.IsNotNull(row);
                            });
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnVerticaConnectionBulkMergeForEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<FormatException>(() => destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        //[TestMethod, ExpectedException(typeof(NullReferenceException))]
        //public void ThrowExceptionOnVerticaConnectionBulkMergeForNullEntities()
        //{
        //    using (var connection = new VerticaConnection(Database.ConnectionString))
        //    {
        //        Assert.Throws<NullReferenceException>(() => connection.BulkMerge((IEnumerable<BulkOperationIdentityTable>)null));
        //    }
        //}

        //[TestMethod, ExpectedException(typeof(EmptyException))]
        //public void ThrowExceptionOnVerticaConnectionBulkMergeForEmptyEntities()
        //{
        //    using (var connection = new VerticaConnection(Database.ConnectionString))
        //    {
        //        connection.BulkMerge(Enumerable.Empty<BulkOperationIdentityTable>());
        //    }
        //}

        

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeForNullDataTable()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null));
            }
        }

        #endregion

        #region BulkMerge<TEntity>(Extra Fields)

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        #endregion

        #region BulkMerge(TableName)

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForTableNameExpandoObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForTableNameExpandoObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForTableNameExpandoObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(entities.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForTableNameExpandoObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForTableNameAnonymousObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForTableNameAnonymousObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForTableNameAnonymousObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)entities.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForTableNameAnonymousObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForTableNameDataEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForTableNameDataEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

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
        public void TestVerticaConnectionBulkMergeForTableNameDataEntitiesForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForTableNameDataEntitiesForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

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
        public void TestVerticaConnectionBulkMergeForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

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
        public void TestVerticaConnectionBulkMergeForTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    pseudoTableType: VerticaBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

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
        public void TestVerticaConnectionBulkMergeForTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void TestVerticaConnectionBulkMergeForTableNameDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            var rows = table.Rows.OfType<DataRow>();
                            queryResult.AsList().ForEach(item =>
                            {
                                var row = rows.Where(r => Equals(item.Id, r["Id"]));
                                Assert.IsNotNull(row);
                            });
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnVerticaConnectionBulkMergeForTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<FormatException>(() => destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeForTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("InvalidTable",
                                table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeForTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("MissingTable",
                                table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void TestVerticaConnectionBulkMergeForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnVerticaConnectionBulkMergeForTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<FormatException>(() => destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                                table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeForTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeForTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void TestVerticaConnectionBulkMergeAsyncForEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

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
        public void TestVerticaConnectionBulkMergeAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

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
        public void TestVerticaConnectionBulkMergeAsyncForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    pseudoTableType: VerticaBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

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
        public void TestVerticaConnectionBulkMergeAsyncForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForMappedEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForMappedEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.IdMapped <= 0));

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
        public void TestVerticaConnectionBulkMergeAsyncForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.IdMapped <= 0));

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
        public void TestVerticaConnectionBulkMergeAsyncForMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    pseudoTableType: VerticaBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

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
        public void TestVerticaConnectionBulkMergeAsyncForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.IdMapped), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.RowGuidMapped), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnBitMapped), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDecimalMapped), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnFloatMapped), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnIntMapped), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<AggregateException>(() => connection.BulkMergeAsync(tables,
                    mappings: mappings).Result);
            }
        }

        

        

        

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void TestVerticaConnectionBulkMergeAsyncForEntitiesDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            var rows = table.Rows.OfType<DataRow>();
                            queryResult.AsList().ForEach(item =>
                            {
                                var row = rows.Where(r => Equals(item.Id, r["Id"]));
                                Assert.IsNotNull(row);
                            });
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table,
                                mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForNullEntities()
        //{
        //    using (var connection = new VerticaConnection(Database.ConnectionString))
        //    {
        //        Assert.Throws<AggregateException>(() => connection.BulkInsertAsync((IEnumerable<BulkOperationIdentityTable>)null).Wait();)
        //    }
        //}

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForEmptyEntities()
        //{
        //    using (var connection = new VerticaConnection(Database.ConnectionString))
        //    {
        //        Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(Enumerable.Empty<BulkOperationIdentityTable>()).Wait();)
        //    }
        //}

        

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForNullDataTable()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        #endregion

        #region BulkMergeAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        #endregion

        #region BulkMergeAsync(TableName)

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForTableNameExpandoObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForTableNameExpandoObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForTableNameExpandoObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(entities.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForTableNameExpandoObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForTableNameAnonymousObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForTableNameAnonymousObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForTableNameAnonymousObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)entities.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForTableNameAnonymousObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForTableNameDataEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForTableNameDataEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

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
        public void TestVerticaConnectionBulkMergeAsyncForTableNameDataEntitiesForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForTableNameDataEntitiesForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

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
        public void TestVerticaConnectionBulkMergeAsyncForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

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
        public void TestVerticaConnectionBulkMergeAsyncForTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    pseudoTableType: VerticaBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

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
        public void TestVerticaConnectionBulkMergeAsyncForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void TestVerticaConnectionBulkMergeAsyncForTableNameDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationIdentityTable>();

                            // Assert
                            var rows = table.Rows.OfType<DataRow>();
                            queryResult.AsList().ForEach(item =>
                            {
                                var row = rows.Where(r => Equals(item.Id, r["Id"]));
                                Assert.IsNotNull(row);
                            });
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync("MissingTable",
                                table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void TestVerticaConnectionBulkMergeAsyncForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync("InvalidTable",
                                table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void TestVerticaConnectionBulkMergeForNonIdentityEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

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
        public void TestVerticaConnectionBulkMergeForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

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
        public void TestVerticaConnectionBulkMergeForNonIdentityEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables,
                    pseudoTableType: VerticaBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

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
        public void TestVerticaConnectionBulkMergeForNonIdentityEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityMappedEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityMappedEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.IdMapped <= 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

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
        public void TestVerticaConnectionBulkMergeForNonIdentityMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.IdMapped <= 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

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
        public void TestVerticaConnectionBulkMergeForNonIdentityMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables,
                    pseudoTableType: VerticaBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

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
        public void TestVerticaConnectionBulkMergeForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.IdMapped), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.RowGuidMapped), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnBitMapped), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDecimalMapped), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnFloatMapped), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnIntMapped), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeForNonIdentityEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<FormatException>(() => connection.BulkMerge(tables, mappings: mappings));
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void TestVerticaConnectionBulkMergeForNonIdentityEntitiesDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            var rows = table.Rows.OfType<DataRow>();
                            queryResult.AsList().ForEach(item =>
                            {
                                var row = rows.Where(r => Equals(item.Id, r["Id"]));
                                Assert.IsNotNull(row);
                            });
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnVerticaConnectionBulkMergeForNonIdentityEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<FormatException>(() => destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeForNonIdentityNullDataTable()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null));
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameExpandoObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameExpandoObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameExpandoObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(entities.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameExpandoObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameAnonymousObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameAnonymousObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameAnonymousObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)entities.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameAnonymousObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameDataEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameDataEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

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
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameDataEntitiesForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameDataEntitiesForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

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
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

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
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    tables,
                    pseudoTableType: VerticaBulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

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
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            var rows = table.Rows.OfType<DataRow>();
                            queryResult.AsList().ForEach(item =>
                            {
                                var row = rows.Where(r => Equals(item.Id, r["Id"]));
                                Assert.IsNotNull(row);
                            });
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnVerticaConnectionBulkMergeForNonIdentityTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<FormatException>(() => destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("InvalidTable",
                                table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("MissingTable",
                                table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void TestVerticaConnectionBulkMergeForNonIdentityTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnVerticaConnectionBulkMergeForNonIdentityTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<FormatException>(() => destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                                table,
                                mappings: mappings));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("MissingTable",
                                table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

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
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

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
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    pseudoTableType: VerticaBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

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
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityMappedEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityMappedEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.IdMapped <= 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

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
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.IdMapped <= 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationMappedNonIdentityTable>();

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
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    pseudoTableType: VerticaBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

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
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.IdMapped), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.RowGuidMapped), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnBitMapped), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDecimalMapped), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnFloatMapped), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnIntMapped), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForNonIdentityEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<AggregateException>(() => connection.BulkMergeAsync(tables,
                    mappings: mappings).Result);
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityEntitiesDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            var rows = table.Rows.OfType<DataRow>();
                            queryResult.AsList().ForEach(item =>
                            {
                                var row = rows.Where(r => Equals(item.Id, r["Id"]));
                                Assert.IsNotNull(row);
                            });
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForNonIdentityEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table,
                                mappings: mappings).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForNonIdentityNullDataTable()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameExpandoObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameExpandoObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameExpandoObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(entities.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameExpandoObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt(tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameAnonymousObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameAnonymousObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10, true);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameAnonymousObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), entities).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)entities.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameAnonymousObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsTrue(tables.All(e => ((dynamic)e).Id > 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.AsList().ForEach(t =>
                {
                    Helper.AssertMembersEquality(queryResult.ElementAt((int)tables.IndexOf(t)), t);
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

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
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

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
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;
                Assert.IsFalse(tables.Any(e => e.Id <= 0));

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

                // Act
                var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

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
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
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
                    Helper.AssertPropertiesEquality(t, queryResult.ElementAt(tables.IndexOf(t)));
                });
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    tables,
                    pseudoTableType: VerticaBulkImportPseudoTableType.Physical).Result;

                // Assert
                Assert.AreEqual(tables.Count, bulkMergeResult);

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
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity).Result;

                            // Assert
                            Assert.AreEqual(tables.Count, bulkMergeResult);

                            // Act
                            var queryResult = destinationConnection.QueryAll<BulkOperationNonIdentityTable>();

                            // Assert
                            var rows = table.Rows.OfType<DataRow>();
                            queryResult.AsList().ForEach(item =>
                            {
                                var row = rows.Where(r => Equals(item.Id, r["Id"]));
                                Assert.IsNotNull(row);
                            });
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync("MissingTable",
                                table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void TestVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add the mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<VerticaBulkInsertMapItem>();

            // Add invalid mappings
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new VerticaBulkInsertMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync("InvalidTable",
                                table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaConnectionBulkMergeAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                        {
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
        public void TestVerticaConnectionBulkMergeForDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                sourceConnection.InsertAll(tables);

                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                {
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
        public void TestVerticaConnectionBulkMergeAsyncForDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var sourceConnection = new VerticaConnection(Database.ConnectionString))
            {
                sourceConnection.InsertAll(tables);

                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                using (var destinationConnection = new VerticaConnection(Database.ConnectionString))
                {
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
