using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Firebird.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations.Firebird;
using RepoDb.Firebird.BulkOperations;
using RepoDb.Exceptions;

namespace RepoDb.Firebird.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class FirebirdConnectionBulkMergeOperationsTest
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
        public void TestFirebirdConnectionBulkMergeForEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables,
                    pseudoTableType: FirebirdBulkImportPseudoTableType.Physical);

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
        public void TestFirebirdConnectionBulkMergeForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForMappedEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForMappedEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables,
                    pseudoTableType: FirebirdBulkImportPseudoTableType.Physical);

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
        public void TestFirebirdConnectionBulkMergeForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.IdMapped), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.RowGuidMapped), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.ColumnBitMapped), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDecimalMapped), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.ColumnFloatMapped), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.ColumnIntMapped), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add invalid mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<FormatException>(() => connection.BulkMerge(tables, mappings: mappings));
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkMergeForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForEntitiesDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add invalid mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        //public void ThrowExceptionOnFirebirdConnectionBulkMergeForNullEntities()
        //{
        //    using (var connection = new FbConnection(Database.ConnectionString))
        //    {
        //        Assert.Throws<NullReferenceException>(() => connection.BulkMerge((IEnumerable<BulkOperationIdentityTable>)null));
        //    }
        //}

        //[TestMethod, ExpectedException(typeof(EmptyException))]
        //public void ThrowExceptionOnFirebirdConnectionBulkMergeForEmptyEntities()
        //{
        //    using (var connection = new FbConnection(Database.ConnectionString))
        //    {
        //        connection.BulkMerge(Enumerable.Empty<BulkOperationIdentityTable>());
        //    }
        //}

        

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForNullDataTable()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null));
            }
        }

        #endregion

        #region BulkMerge<TEntity>(Extra Fields)

        [TestMethod]
        public void TestFirebirdConnectionBulkMergeForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForTableNameExpandoObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForTableNameExpandoObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForTableNameExpandoObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForTableNameExpandoObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForTableNameAnonymousObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForTableNameAnonymousObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForTableNameAnonymousObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForTableNameAnonymousObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForTableNameDataEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForTableNameDataEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForTableNameDataEntitiesForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForTableNameDataEntitiesForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);
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
        public void TestFirebirdConnectionBulkMergeForTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    pseudoTableType: FirebirdBulkImportPseudoTableType.Physical);

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
        public void TestFirebirdConnectionBulkMergeForTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForTableNameDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add invalid mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add invalid mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    pseudoTableType: FirebirdBulkImportPseudoTableType.Physical).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForMappedEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForMappedEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    pseudoTableType: FirebirdBulkImportPseudoTableType.Physical).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.IdMapped), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.RowGuidMapped), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.ColumnBitMapped), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.ColumnDecimalMapped), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.ColumnFloatMapped), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.ColumnIntMapped), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add invalid mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<AggregateException>(() => connection.BulkMergeAsync(tables,
                    mappings: mappings).Result);
            }
        }

        

        

        

        [TestMethod]
        public void TestFirebirdConnectionBulkMergeAsyncForEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForEntitiesDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add invalid mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        //public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForNullEntities()
        //{
        //    using (var connection = new FbConnection(Database.ConnectionString))
        //    {
        //        Assert.Throws<AggregateException>(() => connection.BulkInsertAsync((IEnumerable<BulkOperationIdentityTable>)null).Wait();)
        //    }
        //}

        //[TestMethod, ExpectedException(typeof(AggregateException))]
        //public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForEmptyEntities()
        //{
        //    using (var connection = new FbConnection(Database.ConnectionString))
        //    {
        //        Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(Enumerable.Empty<BulkOperationIdentityTable>()).Wait();)
        //    }
        //}

        

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForNullDataTable()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        #endregion

        #region BulkMergeAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestFirebirdConnectionBulkMergeAsyncForEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameExpandoObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameExpandoObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameExpandoObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameExpandoObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameAnonymousObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameAnonymousObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameAnonymousObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameAnonymousObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameDataEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameDataEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameDataEntitiesForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameDataEntitiesForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;
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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(),
                    tables,
                    pseudoTableType: FirebirdBulkImportPseudoTableType.Physical).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add invalid mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.Id), nameof(BulkOperationIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add invalid mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnBit), nameof(BulkOperationIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime), nameof(BulkOperationIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDateTime2), nameof(BulkOperationIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnDecimal), nameof(BulkOperationIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnFloat), nameof(BulkOperationIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnInt), nameof(BulkOperationIdentityTable.ColumnNVarChar)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationIdentityTable.ColumnNVarChar), nameof(BulkOperationIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForNonIdentityEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables,
                    pseudoTableType: FirebirdBulkImportPseudoTableType.Physical);

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
        public void TestFirebirdConnectionBulkMergeForNonIdentityEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityMappedEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityMappedEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForNonIdentityMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForNonIdentityMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(tables,
                    pseudoTableType: FirebirdBulkImportPseudoTableType.Physical);

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
        public void TestFirebirdConnectionBulkMergeForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.IdMapped), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.RowGuidMapped), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnBitMapped), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDecimalMapped), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnFloatMapped), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnIntMapped), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForNonIdentityEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add invalid mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<FormatException>(() => connection.BulkMerge(tables, mappings: mappings));
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkMergeForNonIdentityEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityEntitiesDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForNonIdentityEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForNonIdentityEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add invalid mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForNonIdentityNullDataTable()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                Assert.Throws<NullReferenceException>(() => connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null));
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkMergeForNonIdentityEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameExpandoObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameExpandoObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameExpandoObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameExpandoObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameAnonymousObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameAnonymousObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameAnonymousObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameAnonymousObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameDataEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameDataEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameDataEntitiesForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameDataEntitiesForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    tables,
                    pseudoTableType: FirebirdBulkImportPseudoTableType.Physical);

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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMerge(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForNonIdentityTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add invalid mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForNonIdentityTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add invalid mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<MissingFieldsException>(() => destinationConnection.BulkMerge("InvalidTable", table));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkMergeForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    pseudoTableType: FirebirdBulkImportPseudoTableType.Physical).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityMappedEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityMappedEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityMappedEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityMappedEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityMappedEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityMappedEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationMappedNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(tables,
                    pseudoTableType: FirebirdBulkImportPseudoTableType.Physical).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityMappedEntitiesWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationMappedNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.IdMapped), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.RowGuidMapped), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnBitMapped), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTimeMapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDateTime2Mapped), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnDecimalMapped), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnFloatMapped), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnIntMapped), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationMappedNonIdentityTable.ColumnNVarCharMapped), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForNonIdentityEntitiesIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add invalid mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<AggregateException>(() => connection.BulkMergeAsync(tables,
                    mappings: mappings).Result);
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityEntitiesDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityEntitiesDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityEntitiesDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForNonIdentityEntitiesDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add invalid mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForNonIdentityNullDataTable()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                Assert.Throws<AggregateException>(() => connection.BulkInsertAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    (DataTable)null).Wait());
            }
        }

        [TestMethod]
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityEntitiesWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityEntitiesWithExtraFieldsWithMappings()
        {
            // Setup
            var tables = Helper.CreateWithExtraFieldsBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameExpandoObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameExpandoObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameExpandoObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameExpandoObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationExpandoObjectNonIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameAnonymousObjectsForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameAnonymousObjectsForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10, true);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameAnonymousObjectsForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameAnonymousObjectsForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                var entities = Helper.CreateBulkOperationAnonymousObjectNonIdentityTables(10, true);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesForEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesForNonEmptyTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesForNonEmptyTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<BulkOperationNonIdentityTable>(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDataEntities()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), tables, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesWithQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDataEntitiesWithUsePhysicalPseudoTempTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                connection.InsertAll(tables);

                // Setup
                Helper.UpdateBulkOperationNonIdentityTables(tables);

                // Act
                var bulkMergeResult = connection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(),
                    tables,
                    pseudoTableType: FirebirdBulkImportPseudoTableType.Physical).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDataTableWithReturnIdentity()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            var bulkMergeResult = destinationConnection.BulkMergeAsync(ClassMappedNameCache.Get<BulkOperationNonIdentityTable>(), table, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity).Result;

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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add invalid mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
                        {
                            // Act
                            Assert.Throws<AggregateException>(() => destinationConnection.BulkMergeAsync("InvalidTable", table).Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDbDataTable()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDbDataTableWithMappings()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add the mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.Id), nameof(BulkOperationNonIdentityTable.Id)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.RowGuid), nameof(BulkOperationNonIdentityTable.RowGuid)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnInt)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDbDataTableIfTheMappingsAreInvalid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            var mappings = new List<FirebirdCommandBatcherMapItem>();

            // Add invalid mappings
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnBit), nameof(BulkOperationNonIdentityTable.ColumnBit)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime), nameof(BulkOperationNonIdentityTable.ColumnDateTime)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDateTime2), nameof(BulkOperationNonIdentityTable.ColumnDateTime2)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnDecimal), nameof(BulkOperationNonIdentityTable.ColumnDecimal)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnFloat), nameof(BulkOperationNonIdentityTable.ColumnFloat)));

            // Switched
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnInt), nameof(BulkOperationNonIdentityTable.ColumnNVarChar)));
            mappings.Add(new FirebirdCommandBatcherMapItem(nameof(BulkOperationNonIdentityTable.ColumnNVarChar), nameof(BulkOperationNonIdentityTable.ColumnInt)));

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsNotValid()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnFirebirdConnectionBulkMergeAsyncForNonIdentityTableNameDbDataTableIfTheTableNameIsMissing()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            // Insert the records first
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                connection.InsertAll(tables);
            }

            // Open the source connection
            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                // Read the data from source connection
                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                {
                    using (var table = new DataTable())
                    {
                        table.Load(reader);

                        // Open the destination connection
                        using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeForDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                sourceConnection.InsertAll(tables);

                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionBulkMergeAsyncForDbDataReader()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);

            using (var sourceConnection = new FbConnection(Database.ConnectionString))
            {
                sourceConnection.InsertAll(tables);

                using (var reader = sourceConnection.ExecuteReader("SELECT * FROM \"BulkOperationNonIdentityTable\""))
                using (var destinationConnection = new FbConnection(Database.ConnectionString))
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
